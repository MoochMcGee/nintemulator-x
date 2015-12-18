using Nintemulator.Shared;

namespace Nintemulator.GBA.SPU
{
    public partial class Spu : GameboyAdvance.Processor
    {
        private Timing course_timing;
        private Timing sample_timing;
        private byte[] registers = new byte[ 16 ];
        private int bias;
        private int course;
        private int psg_shift;
        private int lvolume;
        private int rvolume;

        public ChannelDsd dsa;
        public ChannelDsd dsb;
        public ChannelNoi noi;
        public ChannelRam wav;
        public ChannelSq1 sq1;
        public ChannelSq2 sq2;

        public Spu( GameboyAdvance console, Timing.System system )
            : base( console, system )
        {
            base.Single = system.Apu;

            this.course_timing.Period = system.Period / 512;
            this.course_timing.Single = system.Apu;

            this.sample_timing.Period = system.Period;
            this.sample_timing.Single = 48000;

            MathHelper.Reduce( ref sample_timing.Period, ref sample_timing.Single );

            this.dsa = new ChannelDsd( console, sample_timing );
            this.dsb = new ChannelDsd( console, sample_timing );
            this.noi = new ChannelNoi( console, sample_timing );
            this.wav = new ChannelRam( console, sample_timing );
            this.sq1 = new ChannelSq1( console, sample_timing );
            this.sq2 = new ChannelSq2( console, sample_timing );
        }

        #region Registers

        //4000080h - SOUNDCNT_L (NR50, NR51) - Channel L/R Volume/Enable (R/W)
        //  Bit   Expl.
        //  0-2   Sound 1-4 Master Volume RIGHT (0-7)
        //  3     Not used
        //  4-6   Sound 1-4 Master Volume LEFT (0-7)
        //  7     Not used
        //  8-11  Sound 1-4 Enable Flags RIGHT (each Bit 8-11, 0=Disable, 1=Enable)
        //  12-15 Sound 1-4 Enable Flags LEFT (each Bit 12-15, 0=Disable, 1=Enable)

        //4000084h - SOUNDCNT_X (NR52) - Sound on/off (R/W)
        // Bits 0-3 are automatically set when starting sound output, and are automatically cleared when a sound ends. (Ie. when the length expires, as far as length is enabled. The bits are NOT reset when an volume envelope ends.)
        //  Bit   Expl.
        //  0     Sound 1 ON flag (Read Only)
        //  1     Sound 2 ON flag (Read Only)
        //  2     Sound 3 ON flag (Read Only)
        //  3     Sound 4 ON flag (Read Only)
        //  4-6   Not used
        //  7     PSG/FIFO Master Enable (0=Disable, 1=Enable) (Read/Write)
        //  8-31  Not used

        //4000088h - SOUNDBIAS - Sound PWM Control (R/W, see below)
        // This register controls the final sound output. The default setting is 0200h, it is normally not required to change this value.
        //  Bit    Expl.
        //  0-9    Bias Level     (Default=200h, converting signed samples into unsigned)
        //  10-13  Not used
        //  14-15  Amplitude Resolution/Sampling Cycle (Default=0, see below)
        //  16-31  Not used

        private byte PeekReg( uint address ) { return registers[ address & 0xF ]; }
        private void PokeReg( uint address, byte data ) { registers[ address & 0xF ] = data; }

        private byte Peek084( uint address )
        {
            byte data = registers[ 4 ];

            data &= 0x80;

            if ( sq1.Enabled ) data |= 0x01;
            if ( sq2.Enabled ) data |= 0x02;
            if ( wav.Enabled ) data |= 0x04;
            if ( noi.Enabled ) data |= 0x08;

            return data;
        }
        private void Poke080( uint address, byte data )
        {
            registers[ 0 ] = data;

            //  0-2   Sound 1-4 Master Volume RIGHT (0-7)
            //  3     Not used
            //  4-6   Sound 1-4 Master Volume LEFT (0-7)
            //  7     Not used

            rvolume = ( ( data >> 0 ) & 7 ) + 1;
            lvolume = ( ( data >> 4 ) & 7 ) + 1;

        }
        private void Poke081( uint address, byte data )
        {
            registers[ 1 ] = data;

            //  0-3   Sound 1-4 Enable Flags RIGHT (each Bit 0-3, 0=Disable, 1=Enable)
            //  4-7   Sound 1-4 Enable Flags LEFT (each Bit 4-7, 0=Disable, 1=Enable)

            sq1.renable = ( data & 0x01 ) != 0;
            sq2.renable = ( data & 0x02 ) != 0;
            wav.renable = ( data & 0x04 ) != 0;
            noi.renable = ( data & 0x08 ) != 0;

            sq1.lenable = ( data & 0x10 ) != 0;
            sq2.lenable = ( data & 0x20 ) != 0;
            wav.lenable = ( data & 0x40 ) != 0;
            noi.lenable = ( data & 0x80 ) != 0;
        }
        private void Poke082( uint address, byte data )
        {
            //  0-1   Sound # 1-4 Volume   (0=25%, 1=50%, 2=100%, 3=Prohibited)
            //  2     DMA Sound A Volume   (0=50%, 1=100%)
            //  3     DMA Sound B Volume   (0=50%, 1=100%)
            //  4-7   Not used
            registers[ 2 ] = data;

            psg_shift = data & 3;
            dsa.shift = ( ~data >> 2 ) & 1;
            dsb.shift = ( ~data >> 3 ) & 1;
        }
        private void Poke083( uint address, byte data )
        {
            registers[ 3 ] = data;

            dsa.renable = ( data & 0x01 ) != 0; //  0     DMA Sound A Enable RIGHT (0=Disable, 1=Enable)
            dsb.renable = ( data & 0x10 ) != 0; //  4     DMA Sound B Enable RIGHT (0=Disable, 1=Enable)

            dsa.lenable = ( data & 0x02 ) != 0; //  1     DMA Sound A Enable LEFT  (0=Disable, 1=Enable)
            dsb.lenable = ( data & 0x20 ) != 0; //  5     DMA Sound B Enable LEFT  (0=Disable, 1=Enable)

            dsa.timer = ( data >> 2 ) & 1; //  2     DMA Sound A Timer Select (0=Timer 0, 1=Timer 1)
            dsb.timer = ( data >> 6 ) & 1; //  6     DMA Sound B Timer Select (0=Timer 0, 1=Timer 1)

            if ( ( data & 0x08 ) != 0 ) dsa.Clear( ); //  3     DMA Sound A Reset FIFO   (1=Reset)
            if ( ( data & 0x80 ) != 0 ) dsb.Clear( ); //  7     DMA Sound B Reset FIFO   (1=Reset)
        }
        private void Poke084( uint address, byte data )
        {
            registers[ 4 ] = data;

            dsa.Enabled = ( data & 0x80 ) != 0;
            dsb.Enabled = ( data & 0x80 ) != 0;
        }
        private void Poke088( uint address, byte data )
        {
            registers[ 8 ] = data;

            bias = ( bias & ~0x0FF ) | ( data << 0 & 0x0FF );
        }
        private void Poke089( uint address, byte data )
        {
            registers[ 9 ] = data;

            bias = ( bias & ~0x300 ) | ( data << 8 & 0x300 );
        }

        #endregion

        private void Sample( )
        {
            int dsa = this.dsa.level >> this.dsa.shift;
            int dsb = this.dsb.level >> this.dsb.shift;
            int sq1 = this.sq1.Render( sample_timing.Period );
            int sq2 = this.sq2.Render( sample_timing.Period );
            int wav = this.wav.Render( sample_timing.Period );
            int noi = this.noi.Render( sample_timing.Period );

            int lsample = 0;// 0x200 - bias;
            int rsample = 0;// 0x200 - bias;

            //if ( psg_enable )
            {
                int lsequence = 0;
                if ( this.sq1.lenable ) lsequence += sq1;
                if ( this.sq2.lenable ) lsequence += sq2;
                if ( this.wav.lenable ) lsequence += wav;
                if ( this.noi.lenable ) lsequence += noi;

                int rsequence = 0;
                if ( this.sq1.renable ) rsequence += sq1;
                if ( this.sq2.renable ) rsequence += sq2;
                if ( this.wav.renable ) rsequence += wav;
                if ( this.noi.renable ) rsequence += noi;

                if ( this.psg_shift < 3 )
                {
                    lsample += ( lsequence * lvolume ) >> ( 2 - this.psg_shift );
                    rsample += ( rsequence * rvolume ) >> ( 2 - this.psg_shift );
                }
            }

            if ( this.dsa.lenable ) lsample += dsa; // -80h ~ 7Fh
            if ( this.dsb.lenable ) lsample += dsb; // -80h ~ 7Fh

            if ( this.dsa.renable ) rsample += dsa; // -80h ~ 7Fh
            if ( this.dsb.renable ) rsample += dsb; // -80h ~ 7Fh

            lsample = ( lsample > 511 ) ? 511 : ( lsample < -512 ) ? -512 : lsample;
            rsample = ( rsample > 511 ) ? 511 : ( rsample < -512 ) ? -512 : rsample;
            lsample <<= 6;
            rsample <<= 6;

            console.Audio.Sample( sample: ( short )lsample );
            console.Audio.Sample( sample: ( short )rsample );
        }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            sq1.Initialize( 0x060 );
            sq2.Initialize( 0x068 );
            wav.Initialize( 0x070 );
            noi.Initialize( 0x078 );

            console.Hook( 0x080, PeekReg, Poke080 );
            console.Hook( 0x081, PeekReg, Poke081 );
            console.Hook( 0x082, PeekReg, Poke082 );
            console.Hook( 0x083, PeekReg, Poke083 );
            console.Hook( 0x084, PeekReg, Poke084 );
            console.Hook( 0x085, PeekReg, PokeReg );
            console.Hook( 0x086, PeekReg, PokeReg );
            console.Hook( 0x087, PeekReg, PokeReg );
            console.Hook( 0x088, PeekReg, Poke088 );
            console.Hook( 0x089, PeekReg, Poke089 );
            console.Hook( 0x08A, PeekReg, PokeReg );
            console.Hook( 0x08B, PeekReg, PokeReg );
            console.Hook( 0x08C, PeekReg, PokeReg );
            console.Hook( 0x08D, PeekReg, PokeReg );
            console.Hook( 0x08E, PeekReg, PokeReg );
            console.Hook( 0x08F, PeekReg, PokeReg );

            console.Hook( 0x090, 0x09F, wav.Peek, wav.Poke );

            dsa.Initialize( console.Dma1, 0x0A0 );
            dsb.Initialize( console.Dma2, 0x0A4 );
        }

        public override void Update( int cycles )
        {
            course_timing.Cycles += cycles * course_timing.Single;

            while ( course_timing.Cycles >= course_timing.Period )
            {
                course_timing.Cycles -= course_timing.Period;

                switch ( course )
                {
                case 0:
                case 4:
                    sq1.ClockDuration( );
                    sq2.ClockDuration( );
                    wav.ClockDuration( );
                    noi.ClockDuration( );
                    break;

                case 2:
                case 6:
                    sq1.ClockDuration( );
                    sq2.ClockDuration( );
                    wav.ClockDuration( );
                    noi.ClockDuration( );

                    sq1.ClockSweep( );
                    break;

                case 7:
                    sq1.ClockEnvelope( );
                    sq2.ClockEnvelope( );
                    noi.ClockEnvelope( );
                    break;
                }

                course = ( course + 1 ) & 7;
            }

            sample_timing.Cycles += cycles * sample_timing.Single;

            while ( sample_timing.Cycles >= sample_timing.Period )
            {
                sample_timing.Cycles -= sample_timing.Period;
                Sample( );
            }
        }
    }
}