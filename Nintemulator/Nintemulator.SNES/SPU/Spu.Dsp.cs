using System;

namespace Nintemulator.SFC.SPU
{
    public partial class Spu
    {
        public sealed class Dsp : SuperFamicom.Processor
        {
            private const int MVOLL  = 0x0c;
            private const int MVOLR  = 0x1c;
            private const int EVOLL  = 0x2c;
            private const int EVOLR  = 0x3c;
            private const int KON    = 0x4c;
            private const int KOFF   = 0x5c;
            private const int FLG    = 0x6c;
            private const int ENDX   = 0x7c;
            private const int EFB    = 0x0d;
            private const int PMON   = 0x2d;
            private const int NON    = 0x3d;
            private const int EON    = 0x4d;
            private const int DIR    = 0x5d;
            private const int ESA    = 0x6d;
            private const int EDL    = 0x7d;
            private const int FIR    = 0x0f;
            private const int VOLL   = 0x00;
            private const int VOLR   = 0x01;
            private const int PITCHL = 0x02;
            private const int PITCHH = 0x03;
            private const int SRCN   = 0x04;
            private const int ADSR0  = 0x05;
            private const int ADSR1  = 0x06;
            private const int GAIN   = 0x07;
            private const int ENVX   = 0x08;
            private const int OUTX   = 0x09;

            private const int ENVELOPE_A = 1;
            private const int ENVELOPE_D = 2;
            private const int ENVELOPE_S = 3;
            private const int ENVELOPE_R = 0;

            // internal constants
            private const int echo_hist_size = 8;
            private const int brr_buffer_size = 12;
            private const int brr_block_size = 9;
            private const int counter_range = 2048 * 5 * 3; // 30720 (0x7800)

            // gaussian
            private static readonly short[] gaussian_table = new short[ 512 ]
            {
                   0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
                   1,    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,    2,    2,    2,    2,    2,
                   2,    2,    3,    3,    3,    3,    3,    4,    4,    4,    4,    4,    5,    5,    5,    5,
                   6,    6,    6,    6,    7,    7,    7,    8,    8,    8,    9,    9,    9,   10,   10,   10,
                  11,   11,   11,   12,   12,   13,   13,   14,   14,   15,   15,   15,   16,   16,   17,   17,
                  18,   19,   19,   20,   20,   21,   21,   22,   23,   23,   24,   24,   25,   26,   27,   27,
                  28,   29,   29,   30,   31,   32,   32,   33,   34,   35,   36,   36,   37,   38,   39,   40,
                  41,   42,   43,   44,   45,   46,   47,   48,   49,   50,   51,   52,   53,   54,   55,   56,
                  58,   59,   60,   61,   62,   64,   65,   66,   67,   69,   70,   71,   73,   74,   76,   77,
                  78,   80,   81,   83,   84,   86,   87,   89,   90,   92,   94,   95,   97,   99,  100,  102,
                 104,  106,  107,  109,  111,  113,  115,  117,  118,  120,  122,  124,  126,  128,  130,  132,
                 134,  137,  139,  141,  143,  145,  147,  150,  152,  154,  156,  159,  161,  163,  166,  168,
                 171,  173,  175,  178,  180,  183,  186,  188,  191,  193,  196,  199,  201,  204,  207,  210,
                 212,  215,  218,  221,  224,  227,  230,  233,  236,  239,  242,  245,  248,  251,  254,  257,
                 260,  263,  267,  270,  273,  276,  280,  283,  286,  290,  293,  297,  300,  304,  307,  311,
                 314,  318,  321,  325,  328,  332,  336,  339,  343,  347,  351,  354,  358,  362,  366,  370,
                 374,  378,  381,  385,  389,  393,  397,  401,  405,  410,  414,  418,  422,  426,  430,  434,
                 439,  443,  447,  451,  456,  460,  464,  469,  473,  477,  482,  486,  491,  495,  499,  504,
                 508,  513,  517,  522,  527,  531,  536,  540,  545,  550,  554,  559,  563,  568,  573,  577,
                 582,  587,  592,  596,  601,  606,  611,  615,  620,  625,  630,  635,  640,  644,  649,  654,
                 659,  664,  669,  674,  678,  683,  688,  693,  698,  703,  708,  713,  718,  723,  728,  732,
                 737,  742,  747,  752,  757,  762,  767,  772,  777,  782,  787,  792,  797,  802,  806,  811,
                 816,  821,  826,  831,  836,  841,  846,  851,  855,  860,  865,  870,  875,  880,  884,  889,
                 894,  899,  904,  908,  913,  918,  923,  927,  932,  937,  941,  946,  951,  955,  960,  965,
                 969,  974,  978,  983,  988,  992,  997, 1001, 1005, 1010, 1014, 1019, 1023, 1027, 1032, 1036,
                1040, 1045, 1049, 1053, 1057, 1061, 1066, 1070, 1074, 1078, 1082, 1086, 1090, 1094, 1098, 1102,
                1106, 1109, 1113, 1117, 1121, 1125, 1128, 1132, 1136, 1139, 1143, 1146, 1150, 1153, 1157, 1160,
                1164, 1167, 1170, 1174, 1177, 1180, 1183, 1186, 1190, 1193, 1196, 1199, 1202, 1205, 1207, 1210,
                1213, 1216, 1219, 1221, 1224, 1227, 1229, 1232, 1234, 1237, 1239, 1241, 1244, 1246, 1248, 1251,
                1253, 1255, 1257, 1259, 1261, 1263, 1265, 1267, 1269, 1270, 1272, 1274, 1275, 1277, 1279, 1280,
                1282, 1283, 1284, 1286, 1287, 1288, 1290, 1291, 1292, 1293, 1294, 1295, 1296, 1297, 1297, 1298,
                1299, 1300, 1300, 1301, 1302, 1302, 1303, 1303, 1303, 1304, 1304, 1304, 1304, 1304, 1305, 1305,
            };

            private static readonly ushort[] counter_rate = new ushort[ 32 ]
            {
                0x000, 0x800, 0x600,
                0x500, 0x400, 0x300,
                0x280, 0x200, 0x180,
                0x140, 0x100, 0x0c0,
                0x0a0, 0x080, 0x060,
                0x050, 0x040, 0x030,
                0x028, 0x020, 0x018,
                0x014, 0x010, 0x00c,
                0x00a, 0x008, 0x006,
                0x005, 0x004, 0x003,
                       0x002,
                       0x001
            };

            private static readonly ushort[] counter_offset = new ushort[ 32 ]
            {
                0x000, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                0x218, 0x000, 0x410,
                       0x000,
                       0x000
            };

            private State state = new State( );
            private Voice[] voice = new Voice[ 8 ];

            public byte[] ram;
            public int step;

            public Dsp( SuperFamicom console, Timing.System system, byte[] ram )
                : base( console, system )
            {
                Single = 1;

                this.ram = ram;
            }

            protected override void OnInitialize( )
            {
                state.regs[ FLG ] = 0xe0;

                state.noise = 0x4000;
                state.every_other_sample = true;

                for ( int i = 0; i < 8; i++ )
                {
                    voice[ i ] = new Voice( );
                    voice[ i ].brr_offset = 1;
                    voice[ i ].vbit = ( 1 << i );
                    voice[ i ].vidx = ( i << 4 );
                    voice[ i ].env_mode = ENVELOPE_R;
                }
            }

            public byte Peek( )
            {
                int addr = ram[ 0x00f2 ];
                return state.regs[ addr & 0x7f ];
            }
            public void Poke( byte data )
            {
                int addr = ram[ 0x00f2 ];

                if ( ( addr & 0x80 ) != 0 ) return;

                state.regs[ addr ] = data;

                if ( ( addr & 0x0f ) == ENVX )
                {
                    state.envx_buf = data;
                }
                else if ( ( addr & 0x0f ) == OUTX )
                {
                    state.outx_buf = data;
                }
                else if ( addr == KON )
                {
                    state.new_kon = data;
                }
                else if ( addr == ENDX )
                {
                    //always cleared, regardless of data written
                    state.endx_buf = 0;
                    state.regs[ ENDX ] = 0;
                }
            }

            public override void Update( )
            {
                switch ( step )
                {
                case 0x00: v5( voice[ 0 ] ); v2( voice[ 1 ] ); break;
                case 0x01: v6( voice[ 0 ] ); v3( voice[ 1 ] ); break;
                case 0x02: v7( voice[ 0 ] ); v4( voice[ 1 ] ); v1( voice[ 3 ] ); break;
                case 0x03: v8( voice[ 0 ] ); v5( voice[ 1 ] ); v2( voice[ 2 ] ); break;
                case 0x04: v9( voice[ 0 ] ); v6( voice[ 1 ] ); v3( voice[ 2 ] ); break;
                case 0x05: v7( voice[ 1 ] ); v4( voice[ 2 ] ); v1( voice[ 4 ] ); break;
                case 0x06: v8( voice[ 1 ] ); v5( voice[ 2 ] ); v2( voice[ 3 ] ); break;
                case 0x07: v9( voice[ 1 ] ); v6( voice[ 2 ] ); v3( voice[ 3 ] ); break;
                case 0x08: v7( voice[ 2 ] ); v4( voice[ 3 ] ); v1( voice[ 5 ] ); break;
                case 0x09: v8( voice[ 2 ] ); v5( voice[ 3 ] ); v2( voice[ 4 ] ); break;
                case 0x0a: v9( voice[ 2 ] ); v6( voice[ 3 ] ); v3( voice[ 4 ] ); break;
                case 0x0b: v7( voice[ 3 ] ); v4( voice[ 4 ] ); v1( voice[ 6 ] ); break;
                case 0x0c: v8( voice[ 3 ] ); v5( voice[ 4 ] ); v2( voice[ 5 ] ); break;
                case 0x0d: v9( voice[ 3 ] ); v6( voice[ 4 ] ); v3( voice[ 5 ] ); break;
                case 0x0e: v7( voice[ 4 ] ); v4( voice[ 5 ] ); v1( voice[ 7 ] ); break;
                case 0x0f: v8( voice[ 4 ] ); v5( voice[ 5 ] ); v2( voice[ 6 ] ); break;
                case 0x10: v9( voice[ 4 ] ); v6( voice[ 5 ] ); v3( voice[ 6 ] ); break;
                case 0x11: v1( voice[ 0 ] ); v7( voice[ 5 ] ); v4( voice[ 6 ] ); break;
                case 0x12: v8( voice[ 5 ] ); v5( voice[ 6 ] ); v2( voice[ 7 ] ); break;
                case 0x13: v9( voice[ 5 ] ); v6( voice[ 6 ] ); v3( voice[ 7 ] ); break;
                case 0x14: v1( voice[ 1 ] ); v7( voice[ 6 ] ); v4( voice[ 7 ] ); break;
                case 0x15: v8( voice[ 6 ] ); v5( voice[ 7 ] ); v2( voice[ 0 ] ); break;
                case 0x16: v3a( voice[ 0 ] ); v9( voice[ 6 ] ); v6( voice[ 7 ] ); echo_22( ); break;
                case 0x17: v7( voice[ 7 ] ); echo_23( ); break;
                case 0x18: v8( voice[ 7 ] ); echo_24( ); break;
                case 0x19: v3b( voice[ 0 ] ); v9( voice[ 7 ] ); echo_25( ); break;
                case 0x1a: echo_26( ); break;
                case 0x1b: misc_27( ); echo_27( ); break;
                case 0x1c: misc_28( ); echo_28( ); break;
                case 0x1d: misc_29( ); echo_29( ); break;
                case 0x1e: misc_30( ); v3c( voice[ 0 ] ); echo_30( ); break;
                case 0x1f: v4( voice[ 0 ] ); v1( voice[ 2 ] ); break;
                }

                step = ( step + 1 ) & 0x1f;
            }

            private int gaussian_interpolate( Voice v )
            {   //make pointers into gaussian table based on fractional position between samples
                int offset1 = ( v.interp_pos >> 4 ) & 0xff;
                int offset2 = ( v.interp_pos >> 12 ) + v.buf_pos;

                int output;
                output  = ( gaussian_table[ ( 255 - offset1 ) +   0 ] * v.buffer.read( offset2 + 0 ) ) >> 11;
                output += ( gaussian_table[ ( 255 - offset1 ) + 256 ] * v.buffer.read( offset2 + 1 ) ) >> 11;
                output += ( gaussian_table[ (   0 + offset1 ) + 256 ] * v.buffer.read( offset2 + 2 ) ) >> 11;
                output  = ( short )output;
                output += ( gaussian_table[ (   0 + offset1 ) +   0 ] * v.buffer.read( offset2 + 3 ) ) >> 11;

                return sclamp( output ) & ~1;
            }

            public static int sclamp( int x )
            {
                if ( x > +0x7fff ) return +0x7fff;
                if ( x < -0x8000 ) return -0x8000;

                return x;
            }

            //counter
            private void counter_tick( )
            {
                state.counter--;
                if ( state.counter < 0 )
                {
                    state.counter = counter_range - 1;
                }
            }
            private bool counter_poll( uint rate )
            {
                if ( rate == 0 )
                {
                    return false;
                }
                return ( ( ( uint )state.counter + counter_offset[ rate ] ) % counter_rate[ rate ] ) == 0;
            }

            //envelope
            private void envelope_run( Voice v )
            {
                int env = v.env;

                if ( v.env_mode == ENVELOPE_R )
                { //60%
                    env -= 0x8;
                    if ( env < 0 )
                    {
                        env = 0;
                    }
                    v.env = env;
                    return;
                }

                int rate;
                int env_data = state.regs[ v.vidx + ADSR1 ];
                if ( ( state.t_adsr0 & 0x80 ) != 0 )
                { //99% ADSR
                    if ( v.env_mode >= ENVELOPE_D )
                    { //99%
                        env--;
                        env -= env >> 8;
                        rate = env_data & 0x1f;
                        if ( v.env_mode == ENVELOPE_D )
                        { //1%
                            rate = ( ( state.t_adsr0 >> 3 ) & 0x0e ) + 0x10;
                        }
                    }
                    else
                    { //env_attack
                        rate = ( ( state.t_adsr0 & 0x0f ) << 1 ) + 1;
                        env += rate < 31 ? 0x20 : 0x400;
                    }
                }
                else
                { //GAIN
                    env_data = state.regs[ v.vidx + GAIN ];
                    int mode = env_data >> 5;
                    if ( mode < 4 )
                    { //direct
                        env = env_data << 4;
                        rate = 31;
                    }
                    else
                    {
                        rate = env_data & 0x1f;
                        if ( mode == 4 )
                        { //4: linear decrease
                            env -= 0x20;
                        }
                        else if ( mode < 6 )
                        { //5: exponential decrease
                            env--;
                            env -= env >> 8;
                        }
                        else
                        { //6, 7: linear increase
                            env += 0x20;
                            if ( mode > 6 && ( uint )v.hidden_env >= 0x600 )
                            {
                                env += 0x8 - 0x20; //7: two-slope linear increase
                            }
                        }
                    }
                }

                //sustain level
                if ( ( env >> 8 ) == ( env_data >> 5 ) && v.env_mode == ENVELOPE_D )
                {
                    v.env_mode = ENVELOPE_S;
                }
                v.hidden_env = env;

                //unsigned cast because linear decrease underflowing also triggers this
                if ( ( uint )env > 0x7ff )
                {
                    env = ( env < 0 ? 0 : 0x7ff );
                    if ( v.env_mode == ENVELOPE_A )
                    {
                        v.env_mode = ENVELOPE_D;
                    }
                }

                if ( counter_poll( ( uint )rate ) == true )
                {
                    v.env = env;
                }
            }

            //brr
            private void brr_decode( Voice v )
            {   //state.t_brr_byte = ram[v.brr_addr + v.brr_offset] cached from previous clock cycle
                int nybbles = ( state.t_brr_byte << 8 ) + ram[ ( ushort )( v.brr_addr + v.brr_offset + 1 ) ];

                int filter = ( state.t_brr_header >> 2 ) & 3;
                int scale = ( state.t_brr_header >> 4 );

                //decode four samples
                for ( uint i = 0; i < 4; i++ )
                {
                    //bits 12-15 = current nybble; sign extend, then shift right to 4-bit precision
                    //result: s = 4-bit sign-extended sample value
                    int s = ( short )nybbles >> 12;
                    nybbles <<= 4; //slide nybble so that on next loop iteration, bits 12-15 = current nybble

                    if ( scale <= 12 )
                    {
                        s <<= scale;
                        s >>= 1;
                    }
                    else
                    {
                        s &= ~0x7ff;
                    }

                    //apply IIR filter (2 is the most commonly used)
                    int p1 = v.buffer.read( v.buf_pos - 1 );
                    int p2 = v.buffer.read( v.buf_pos - 2 ) >> 1;

                    switch ( filter )
                    {
                    case 0: break; //no filter
                    case 1: //s += p1 * 0.46875
                        s += p1 >> 1;
                        s += ( -p1 ) >> 5;
                        break;

                    case 2: //s += p1 * 0.953125 - p2 * 0.46875
                        s += p1;
                        s -= p2;
                        s += p2 >> 4;
                        s += ( p1 * -3 ) >> 6;
                        break;

                    case 3: //s += p1 * 0.8984375 - p2 * 0.40625
                        s += p1;
                        s -= p2;
                        s += ( p1 * -13 ) >> 7;
                        s += ( p2 * 3 ) >> 4;
                        break;
                    }

                    //adjust and write sample
                    s = sclamp( s );
                    s = ( short )( s << 1 );
                    v.buffer.write( ( uint )v.buf_pos++, s );
                    if ( v.buf_pos >= brr_buffer_size )
                    {
                        v.buf_pos = 0;
                    }
                }
            }

            //misc
            private void misc_27( )
            {
                state.t_pmon = state.regs[ PMON ] & ~1; //voice 0 doesn't support PMON
            }
            private void misc_28( )
            {
                state.t_non = state.regs[ NON ];
                state.t_eon = state.regs[ EON ];
                state.t_dir = state.regs[ DIR ];
            }
            private void misc_29( )
            {
                state.every_other_sample = !state.every_other_sample;

                if ( state.every_other_sample )
                {
                    state.new_kon &= ~state.kon; //clears KON 63 clocks after it was last read
                }
            }
            private void misc_30( )
            {
                if ( state.every_other_sample )
                {
                    state.kon = state.new_kon;
                    state.t_koff = state.regs[ KOFF ];
                }

                counter_tick( );

                //noise
                if ( counter_poll( ( uint )( state.regs[ FLG ] & 0x1f ) ) == true )
                {
                    int feedback = ( state.noise << 13 ) ^ ( state.noise << 14 );
                    state.noise = ( feedback & 0x4000 ) ^ ( state.noise >> 1 );
                }
            }

            //voice
            private void voice_output( Voice v, int channel )
            {   //apply left/right volume
                int amp = ( state.t_output * ( sbyte )( state.regs[ v.vidx + VOLL + channel ] ) ) >> 7;

                //add to output total
                state.t_main_out[ channel ] += amp;
                state.t_main_out[ channel ] = sclamp( state.t_main_out[ channel ] );

                //optionally add to echo total
                if ( ( state.t_eon & v.vbit ) != 0 )
                {
                    state.t_echo_out[ channel ] += amp;
                    state.t_echo_out[ channel ] = sclamp( state.t_echo_out[ channel ] );
                }
            }
            private void v1 ( Voice v )
            {
                state.t_dir_addr = ( state.t_dir << 8 ) + ( state.t_srcn << 2 );
                state.t_srcn = state.regs[ v.vidx + SRCN ];
            }
            private void v2 ( Voice v )
            {   //read sample pointer (ignored if not needed)
                ushort addr = ( ushort )state.t_dir_addr;
                if ( v.kon_delay == 0 )
                {
                    addr += 2;
                }
                byte lo = ram[ ( ushort )( addr + 0 ) ];
                byte hi = ram[ ( ushort )( addr + 1 ) ];
                state.t_brr_next_addr = ( ( hi << 8 ) + lo );

                state.t_adsr0 = state.regs[ v.vidx + ADSR0 ];

                //read pitch, spread over two clocks
                state.t_pitch = state.regs[ v.vidx + PITCHL ];
            }
            private void v3 ( Voice v )
            {
                v3a( v );
                v3b( v );
                v3c( v );
            }
            private void v3a( Voice v )
            {
                state.t_pitch += ( state.regs[ v.vidx + PITCHH ] & 0x3f ) << 8;
            }
            private void v3b( Voice v )
            {
                state.t_brr_byte = ram[ ( ushort )( v.brr_addr + v.brr_offset ) ];
                state.t_brr_header = ram[ ( ushort )( v.brr_addr ) ];
            }
            private void v3c( Voice v )
            {   //pitch modulation using previous voice's output

                if ( ( state.t_pmon & v.vbit ) != 0 )
                {
                    state.t_pitch += ( ( state.t_output >> 5 ) * state.t_pitch ) >> 10;
                }

                if ( v.kon_delay != 0 )
                {
                    //get ready to start BRR decoding on next sample
                    if ( v.kon_delay == 5 )
                    {
                        v.brr_addr = state.t_brr_next_addr;
                        v.brr_offset = 1;
                        v.buf_pos = 0;
                        state.t_brr_header = 0; //header is ignored on this sample
                    }

                    //envelope is never run during KON
                    v.env = 0;
                    v.hidden_env = 0;

                    //disable BRR decoding until last three samples
                    v.interp_pos = 0;
                    v.kon_delay--;

                    if ( ( v.kon_delay & 3 ) != 0 )
                    {
                        v.interp_pos = 0x4000;
                    }

                    //pitch is never added during KON
                    state.t_pitch = 0;
                }

                //gaussian interpolation
                int output = gaussian_interpolate( v );

                //noise
                if ( ( state.t_non & v.vbit ) != 0 )
                { 
                    output = ( short )( state.noise << 1 );
                }

                //apply envelope
                state.t_output = ( ( output * v.env ) >> 11 ) & ~1;
                v.t_envx_out = v.env >> 4;

                //immediate silence due to end of sample or soft reset
                if ( ( state.regs[ FLG ] & 0x80 ) != 0 || ( state.t_brr_header & 3 ) == 1 )
                {
                    v.env_mode = ENVELOPE_R;
                    v.env = 0;
                }

                if ( state.every_other_sample )
                {
                    //KOFF
                    if ( ( state.t_koff & v.vbit ) != 0 )
                    {
                        v.env_mode = ENVELOPE_R;
                    }

                    //KON
                    if ( ( state.kon & v.vbit ) != 0 )
                    {
                        v.kon_delay = 5;
                        v.env_mode = ENVELOPE_A;
                    }
                }

                //run envelope for next sample
                if ( v.kon_delay == 0 )
                {
                    envelope_run( v );
                }
            }
            private void v4 ( Voice v )
            {   //decode BRR
                state.t_looped = 0;
                if ( v.interp_pos >= 0x4000 )
                {
                    brr_decode( v );
                    v.brr_offset += 2;
                    if ( v.brr_offset >= 9 )
                    {
                        //start decoding next BRR block
                        v.brr_addr = ( ushort )( v.brr_addr + 9 );
                        if ( ( state.t_brr_header & 1 ) != 0 )
                        {
                            v.brr_addr = state.t_brr_next_addr;
                            state.t_looped = v.vbit;
                        }
                        v.brr_offset = 1;
                    }
                }

                //apply pitch
                v.interp_pos = ( v.interp_pos & 0x3fff ) + state.t_pitch;

                //keep from getting too far ahead (when using pitch modulation)
                if ( v.interp_pos > 0x7fff )
                {
                    v.interp_pos = 0x7fff;
                }

                //output left
                voice_output( v, 0 );
            }
            private void v5 ( Voice v )
            {   //output right
                voice_output( v, 1 );

                //ENDX, OUTX and ENVX won't update if you wrote to them 1-2 clocks earlier
                state.endx_buf = state.regs[ ENDX ] | state.t_looped;

                //clear bit in ENDX if KON just began
                if ( v.kon_delay == 5 )
                {
                    state.endx_buf &= ~v.vbit;
                }
            }
            private void v6 ( Voice v )
            {
                state.outx_buf = state.t_output >> 8;
            }
            private void v7 ( Voice v )
            {   //update ENDX
                state.regs[ ENDX ] = ( byte )state.endx_buf;
                state.envx_buf = v.t_envx_out;
            }
            private void v8 ( Voice v )
            {   //update OUTX
                state.regs[ v.vidx + OUTX ] = ( byte )state.outx_buf;
            }
            private void v9 ( Voice v )
            {   //update ENVX
                state.regs[ v.vidx + ENVX ] = ( byte )state.envx_buf;
            }

            //echo
            private int calc_fir( int i, int channel )
            {
                int s = state.echo_hist[ channel ].read( state.echo_hist_pos + i + 1 );
                return ( s * ( sbyte )( state.regs[ FIR + i * 0x10 ] ) ) >> 6;
            }
            private int echo_output( int channel )
            {
                int output =
                    ( short )( ( state.t_main_out[ channel ] * ( sbyte )( state.regs[ MVOLL + channel * 0x10 ] ) ) >> 7 ) +
                    ( short )( ( state.t_echo_in [ channel ] * ( sbyte )( state.regs[ EVOLL + channel * 0x10 ] ) ) >> 7 );

                return sclamp( output );
            }
            private void echo_read( int channel )
            {
                uint addr = ( uint )( state.t_echo_ptr + channel * 2 );
                byte lo = ram[ ( ushort )( addr + 0 ) ];
                byte hi = ram[ ( ushort )( addr + 1 ) ];
                int s = ( short )( ( hi << 8 ) + lo );
                state.echo_hist[ channel ].write( ( uint )state.echo_hist_pos, s >> 1 );
            }
            private void echo_write( int channel )
            {
                if ( ( state.t_echo_disabled & 0x20 ) == 0 )
                {
                    uint addr = ( uint )( state.t_echo_ptr + channel * 2 );
                    int s = state.t_echo_out[ channel ];
                    ram[ ( ushort )( addr + 0 ) ] = ( byte )s;
                    ram[ ( ushort )( addr + 1 ) ] = ( byte )( s >> 8 );
                }

                state.t_echo_out[ channel ] = 0;
            }
            private void echo_22( )
            {   //history
                state.echo_hist_pos++;
                if ( state.echo_hist_pos >= echo_hist_size )
                {
                    state.echo_hist_pos = 0;
                }

                state.t_echo_ptr = ( ushort )( ( state.t_esa << 8 ) + state.echo_offset );
                echo_read( 0 );

                //FIR
                int l = calc_fir( 0, 0 );
                int r = calc_fir( 0, 1 );

                state.t_echo_in[ 0 ] = l;
                state.t_echo_in[ 1 ] = r;
            }
            private void echo_23( )
            {
                int l = calc_fir( 1, 0 ) + calc_fir( 2, 0 );
                int r = calc_fir( 1, 1 ) + calc_fir( 2, 1 );

                state.t_echo_in[ 0 ] += l;
                state.t_echo_in[ 1 ] += r;

                echo_read( 1 );
            }
            private void echo_24( )
            {
                int l = calc_fir( 3, 0 ) + calc_fir( 4, 0 ) + calc_fir( 5, 0 );
                int r = calc_fir( 3, 1 ) + calc_fir( 4, 1 ) + calc_fir( 5, 1 );

                state.t_echo_in[ 0 ] += l;
                state.t_echo_in[ 1 ] += r;
            }
            private void echo_25( )
            {
                int l = state.t_echo_in[ 0 ] + calc_fir( 6, 0 );
                int r = state.t_echo_in[ 1 ] + calc_fir( 6, 1 );

                l = ( short )l;
                r = ( short )r;

                l += ( short )calc_fir( 7, 0 );
                r += ( short )calc_fir( 7, 1 );

                state.t_echo_in[ 0 ] = sclamp( l ) & ~1;
                state.t_echo_in[ 1 ] = sclamp( r ) & ~1;
            }
            private void echo_26( )
            {   //left output volumes
                //(save sample for next clock so we can output both together)
                state.t_main_out[ 0 ] = echo_output( 0 );

                //echo feedback
                int l = state.t_echo_out[ 0 ] + ( short )( ( state.t_echo_in[ 0 ] * ( sbyte )state.regs[ EFB ] ) >> 7 );
                int r = state.t_echo_out[ 1 ] + ( short )( ( state.t_echo_in[ 1 ] * ( sbyte )state.regs[ EFB ] ) >> 7 );

                state.t_echo_out[ 0 ] = sclamp( l ) & ~1;
                state.t_echo_out[ 1 ] = sclamp( r ) & ~1;
            }
            private void echo_27( )
            {   //output
                int outl = state.t_main_out[ 0 ];
                int outr = echo_output( 1 );
                state.t_main_out[ 0 ] = 0;
                state.t_main_out[ 1 ] = 0;

                //global muting isn't this simple
                //(turns DAC on and off or something, causing small ~37-sample pulse when first muted)
                if ( ( state.regs[ FLG ] & 0x40 ) != 0 )
                {
                    outl = 0;
                    outr = 0;
                }

                //output sample to DAC
                console.Audio.Sample( ( short )outl );
                console.Audio.Sample( ( short )outr );
            }
            private void echo_28( )
            {
                state.t_echo_disabled = state.regs[ FLG ];
            }
            private void echo_29( )
            {
                state.t_esa = state.regs[ ESA ];

                if ( state.echo_offset == 0 )
                {
                    state.echo_length = ( state.regs[ EDL ] & 0x0f ) << 11;
                }

                state.echo_offset += 4;
                if ( state.echo_offset >= state.echo_length )
                {
                    state.echo_offset = 0;
                }

                //write left echo
                echo_write( 0 );

                state.t_echo_disabled = state.regs[ FLG ];
            }
            private void echo_30( )
            {   //write right echo
                echo_write( 1 );
            }

            class State
            {
                public byte[] regs = new byte[ 128 ];

                public ModuloArray[] echo_hist = new ModuloArray[ 2 ]//echo history keeps most recent 8 samples
                {
                    new ModuloArray( echo_hist_size ),
                    new ModuloArray( echo_hist_size )
                };
                public int echo_hist_pos;

                public bool every_other_sample;  //toggles every sample
                public int kon;                  //KON value when last checked
                public int noise;
                public int counter;
                public int echo_offset;          //offset from ESA in echo buffer
                public int echo_length;          //number of bytes that echo_offset will stop at

                //hidden registers also written to when main register is written to
                public int new_kon;
                public int endx_buf;
                public int envx_buf;
                public int outx_buf;

                //temporary state between clocks

                //read once per sample
                public int t_pmon;
                public int t_non;
                public int t_eon;
                public int t_dir;
                public int t_koff;

                //read a few clocks ahead before used
                public int t_brr_next_addr;
                public int t_adsr0;
                public int t_brr_header;
                public int t_brr_byte;
                public int t_srcn;
                public int t_esa;
                public int t_echo_disabled;

                //internal state that is recalculated every sample
                public int t_dir_addr;
                public int t_pitch;
                public int t_output;
                public int t_looped;
                public int t_echo_ptr;

                //left/right sums
                public int[] t_main_out = new int[ 2 ];
                public int[] t_echo_out = new int[ 2 ];
                public int[] t_echo_in  = new int[ 2 ];
            }
            class Voice
            {
                public ModuloArray buffer = new ModuloArray( brr_buffer_size ); //decoded samples
                public int buf_pos;     //place in buffer where next samples will be decoded
                public int interp_pos;  //relative fractional position in sample (0x1000 = 1.0)
                public int brr_addr;    //address of current BRR block
                public int brr_offset;  //current decoding offset in BRR block
                public int vbit;        //bitmask for voice: 0x01 for voice 0, 0x02 for voice 1, etc
                public int vidx;        //voice channel register index: 0x00 for voice 0, 0x10 for voice 1, etc
                public int kon_delay;   //KON delay/current setup phase
                public int env_mode;
                public int env;         //current envelope level
                public int t_envx_out;
                public int hidden_env;  //used by GAIN mode 7, very obscure quirk
            }
        }

        public class ModuloArray
        {
            private int size;
            private int[] buffer;

            public ModuloArray( int size )
            {
                this.size = size;
                this.buffer = new int[ size * 3 ];
            }

            public int read( int index )
            {
                return buffer[ size + index ];
            }
            public void write( uint index, int value )
            {
                buffer[ index ] = buffer[ index + size ] = buffer[ index + size + size ] = value;
            }
        }
    }
}