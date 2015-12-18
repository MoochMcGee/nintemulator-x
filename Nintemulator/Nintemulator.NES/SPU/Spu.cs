using Nintemulator.FC.CPU;
using Nintemulator.Shared;

namespace Nintemulator.FC.SPU
{
    public partial class Spu : Famicom.Processor
    {
        private ChannelSqr sq1;
        private ChannelSqr sq2;
        private ChannelTri tri;
        private ChannelNoi noi;
        private ChannelDmc dmc;
        private ChannelExt ext;
        private Register16 reg4017;
        private bool irq_enabled;
        private bool irq_pending;
        private int mode;
        private int step;

        private int sample_timer;

        public Spu( Famicom console, Timing.System system )
            : base( console, system )
        {
            Single = system.Apu;

            sq1 = new ChannelSqr( console, system );
            sq2 = new ChannelSqr( console, system );
            tri = new ChannelTri( console, system );
            noi = new ChannelNoi( console, system );
            dmc = new ChannelDmc( console, system );
        }

        partial void InitializeSequence( );

        private void ClockHalf( )
        {
            sq1.Duration.Clock( );
            sq2.Duration.Clock( );
            tri.Duration.Clock( );
            noi.Duration.Clock( );

            sq1.ClockSweep( -1 );
            sq2.ClockSweep( +0 );

            if ( ext != null )
                ext.ClockHalf( );
        }
        private void ClockQuad( )
        {
            sq1.Envelope.Clock( );
            sq2.Envelope.Clock( );
            noi.Envelope.Clock( );

            tri.ClockLinearCounter( );

            if ( ext != null )
                ext.ClockQuad( );
        }

        private byte Peek4015( uint address )
        {
            var data = ( byte )(
                ( sq1.Enabled ? 0x01 : 0x00 ) |
                ( sq2.Enabled ? 0x02 : 0x00 ) |
                ( tri.Enabled ? 0x04 : 0x00 ) |
                ( noi.Enabled ? 0x08 : 0x00 ) |
                ( dmc.Enabled ? 0x10 : 0x00 ) |
                ( irq_pending ? 0x40 : 0x00 ) |
                ( dmc.IrqPending ? 0x80 : 0x00 ) );

            irq_pending = false;
            cpu.Irq( 0u );

            return data;
        }
        private void Poke4015( uint address, byte data )
        {
            dmc.IrqPending = false;
            cpu.Irq( 0u );

            sq1.Enabled = ( data & 0x01 ) != 0;
            sq2.Enabled = ( data & 0x02 ) != 0;
            tri.Enabled = ( data & 0x04 ) != 0;
            noi.Enabled = ( data & 0x08 ) != 0;
            dmc.Enabled = ( data & 0x10 ) != 0;
        }
        private void Poke4017( uint address, byte data )
        {
            reg4017.l = data;
            reg4017.h = 1;
        }

        private void Sample( )
        {
            if ( ext != null )
            {
                console.Audio.Sample( Mixer.MixSamples(
                        sq1.Render( ),
                        sq2.Render( ),
                        tri.Render( ),
                        noi.Render( ),
                        dmc.Render( ),
                        ext.Render( ) ) );
            }
            else
            {
                console.Audio.Sample( Mixer.MixSamples(
                        sq1.Render( ),
                        sq2.Render( ),
                        tri.Render( ),
                        noi.Render( ),
                        dmc.Render( ) ) );
            }
        }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            InitializeSequence( );

            sq1.Initialize( 0x4000 );
            sq2.Initialize( 0x4004 );
            tri.Initialize( 0x4008 );
            noi.Initialize( 0x400C );
            dmc.Initialize( 0x4010 );

            cpu.Hook( 0x4015, Peek4015, Poke4015 );
            cpu.Hook( 0x4017, /*     */ Poke4017 );
        }

        public void Hook( ChannelExt external )
        {
            this.ext = external;
        }

        public override void Update( )
        {
            ClockSequence( );

            dmc.Clock( );

            if ( reg4017.h != 0 )
            {
                reg4017.h = 0;

                irq_enabled = ( reg4017.l & 0x40 ) == 0;
                mode = ( reg4017.l & 0x80 ) >> 7;
                step = 0;

                frameTimer.Cycles = TimingTable[ System.Serial ][ mode ][ step ];

                if ( mode == 1 )
                {
                    ClockQuad( );
                    ClockHalf( );
                }
                else
                {
                    ClockQuad( );
                }

                if ( !irq_enabled )
                {
                    irq_pending = false;
                    cpu.Irq( 0u );
                }
            }

            sample_timer += PHASE * 2;

            if ( sample_timer >= DELAY )
            {
                sample_timer -= DELAY;
                Sample( );
            }
        }
    }
}