namespace Nintemulator.GBC
{
    public class Pad : GameboyColor.Input
    {
        private bool p14;
        private bool p15;

        public Pad(GameboyColor console)
            : base(console, 0, 8)
        {
            Map( 0, "Menu" );
            Map( 1, "Back" );
            Map( 2, "X" );
            Map( 3, "A" );
            Map( 4, "DPad-D" );
            Map( 5, "DPad-U" );
            Map( 6, "DPad-L" );
            Map( 7, "DPad-R" );
        }

        private byte Peek(uint address)
        {
            base.Update();

            byte data = 0xFF;

            if (p15)
            {
                data ^= 0x20;

                if (Pressed(0x0)) data ^= 0x8;
                if (Pressed(0x1)) data ^= 0x4;
                if (Pressed(0x2)) data ^= 0x2;
                if (Pressed(0x3)) data ^= 0x1;
            }
            else if (p14)
            {
                data ^= 0x10;

                if (Pressed(0x4)) data ^= 0x8;
                if (Pressed(0x5)) data ^= 0x4;
                if (Pressed(0x6)) data ^= 0x2;
                if (Pressed(0x7)) data ^= 0x1;
            }

            return data;
        }
        private void Poke(uint address, byte data)
        {
            p15 = (data & 0x20) == 0;
            p14 = (data & 0x10) == 0;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            console.Hook(0xFF00, Peek, Poke);
        }
    }
}