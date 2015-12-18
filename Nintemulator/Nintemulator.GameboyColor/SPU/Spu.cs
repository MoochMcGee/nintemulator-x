namespace Nintemulator.GBC.SPU
{
    // Name Addr 7654 3210 Function
    // -----------------------------------------------------------------
    //        Control/Status
    // NR50 FF24 ALLL BRRR Vin L enable, Left vol, Vin R enable, Right vol
    // NR51 FF25 NW21 NW21 Left enables, Right enables
    // NR52 FF26 P--- NW21 Power control/status, Channel length statuses

    //        Wave Table
    //      FF30 0000 1111 Samples $00 and $01
    //      ....
    //      FF3F 0000 1111 Samples $1E and $1F

    public partial class Spu : GameboyColor.Processor
    {
        private ChannelSq1 sq1;
        private ChannelSq2 sq2;
        private ChannelNoi noi;
        private ChannelWav wav;
        private Timing course_timing;
        private Timing sample_timing;
        private byte[] reg;
        private int course;

        public Spu(GameboyColor console, Timing.System system)
            : base(console, system)
        {
            Single = system.Apu;

            course_timing.Period = system.Period / 512;
            course_timing.Single = system.Apu;

            sample_timing.Period = DELAY;
            sample_timing.Single = PHASE;

            sq1 = new ChannelSq1(console, system);
            sq2 = new ChannelSq2(console, system);
            noi = new ChannelNoi(console, system);
            wav = new ChannelWav(console, system);
            reg = new byte[3];
        }

        private byte PeekNull(uint address) { return 0xFF; }
        private byte PeekNR50(uint address) { return reg[0]; }
        private byte PeekNR51(uint address) { return reg[1]; }
        private byte PeekNR52(uint address)
        {
            return (byte)(
                (reg[2] & 0x80) |
                (noi.Enabled ? 8 : 0) |
                (wav.Enabled ? 4 : 0) |
                (sq2.Enabled ? 2 : 0) |
                (sq1.Enabled ? 1 : 0) | 0x70);
        }
        private void PokeNull(uint address, byte data) { }
        private void PokeNR50(uint address, byte data)
        {
            if ((reg[2] & 0x80) == 0)
                return;

            reg[0] = data;

            Mixer.Level[0] = (data >> 4) & 0x7;
            Mixer.Level[1] = (data >> 0) & 0x7;
        }
        private void PokeNR51(uint address, byte data)
        {
            if ((reg[2] & 0x80) == 0)
                return;

            reg[1] = data;

            Mixer.Flags[0] = (data >> 4) & 0xF;
            Mixer.Flags[1] = (data >> 0) & 0xF;
        }
        private void PokeNR52(uint address, byte data)
        {
            if (((reg[2] ^ data) & 0x80) != 0)
            {
                switch (data & 0x80)
                {
                case 0x00:
                    PokeNR50(0x0000, 0x00);
                    PokeNR51(0x0000, 0x00);

                    sq1.PowerOff();
                    sq2.PowerOff();
                    wav.PowerOff();
                    noi.PowerOff();
                    break;

                case 0x80:
                    sq1.PowerOn();
                    sq2.PowerOn();
                    wav.PowerOn();
                    noi.PowerOn();
                    break;
                }
            }

            reg[2] = data;
        }

        private void Sample()
        {
            var samples = Mixer.MixSamples(
                sq1.Sample(),
                sq2.Sample(),
                wav.Sample(),
                noi.Sample());

            console.Audio.Sample(sample: (short)samples[0]);
            console.Audio.Sample(sample: (short)samples[1]);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            sq1.Initialize(0xFF10);
            sq2.Initialize(0xFF15);
            wav.Initialize(0xFF1A);
            noi.Initialize(0xFF1F);

            console.Hook(0xFF24, /*   */ PeekNR50, PokeNR50);
            console.Hook(0xFF25, /*   */ PeekNR51, PokeNR51);
            console.Hook(0xFF26, /*   */ PeekNR52, PokeNR52);
            console.Hook(0xFF27, 0xFF2F, PeekNull, PokeNull);
            console.Hook(0xFF30, 0xFF3F, wav.Peek, wav.Poke);
        }

        public override void Update()
        {
            if (course_timing.Clock())
            {
                switch (course)
                {
                case 0:
                case 4:
                    sq1.ClockDuration();
                    sq2.ClockDuration();
                    wav.ClockDuration();
                    noi.ClockDuration();
                    break;

                case 2:
                case 6:
                    sq1.ClockDuration();
                    sq2.ClockDuration();
                    wav.ClockDuration();
                    noi.ClockDuration();

                    sq1.ClockSweep();
                    break;

                case 7:
                    sq1.ClockEnvelope();
                    sq2.ClockEnvelope();
                    noi.ClockEnvelope();
                    break;
                }

                course = (course + 1) & 0x7;
            }

            if (sample_timing.Clock())
            {
                Sample();
            }
        }
    }
}