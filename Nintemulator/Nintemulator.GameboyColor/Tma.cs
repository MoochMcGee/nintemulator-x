using Nintemulator.GBC.CPU;
using Nintemulator.Shared;

namespace Nintemulator.GBC
{
    public class Tma : GameboyColor.Component
    {
        private static readonly byte[] Lut = { 0x01, 0x40, 0x10, 0x04 };

        private Register16 div;
        private Register16 tma;
        private byte cnt;
        private byte mod;

        public Tma(GameboyColor console)
            : base(console) { }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            console.Hook(0xFF04U, (a) => div.h, (a, d) => div.h = 0);
            console.Hook(0xFF05U, (a) => tma.h, (a, d) => tma.h = d);
            console.Hook(0xFF06U, (a) => mod,    (a, d) => mod    = d);
            console.Hook(0xFF07U, (a) => cnt,    (a, d) => cnt    = d);
        }

        public void Update()
        {
            div.w += Lut[3U];

            if ((cnt & 0x4) != 0)
            {
                tma.w += Lut[cnt & 3U];

                if (tma.w < Lut[cnt & 3U])
                {
                    tma.h = mod;
                    cpu.RequestInterrupt(Cpu.Interrupt.Elapse);
                }
            }
        }
    }
}