namespace Nintemulator.GBC
{
    public class Sio : GameboyColor.Component
    {
        public Sio(GameboyColor console)
            : base(console) { }

        private byte PeekFF01(uint address) { return 0xFF; }
        private byte PeekFF02(uint address) { return 0xFF; }
        private void PokeFF01(uint address, byte data) { }
        private void PokeFF02(uint address, byte data) { }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            console.Hook(0xFF01U, PeekFF01, PokeFF01);
            console.Hook(0xFF02U, PeekFF02, PokeFF02);
        }
    }
}