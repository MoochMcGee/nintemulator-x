using Nintemulator.Shared;

namespace Nintemulator.SFC.PAD
{
    public sealed class Pad : SuperFamicom.Input
    {
        internal Register16 latch;
        internal int index;

        public Pad( SuperFamicom console, int index )
            : base( console, index, 12 )
        {
            this.index = index;

            Map(  0, "A" );
            Map(  1, "X" );
            Map(  2, "Back" );
            Map(  3, "Menu" );
            Map(  4, "DPad-U" );
            Map(  5, "DPad-D" );
            Map(  6, "DPad-L" );
            Map(  7, "DPad-R" );
            Map(  8, "B" );
            Map(  9, "Y" );
            Map( 10, "L-Shoulder" );
            Map( 11, "R-Shoulder" );
        }

        public override void Update( )
        {
            base.Update( );

            latch.w = 0x0000;

            if ( Pressed( 0x0 ) ) latch.w |= 0x8000;
            if ( Pressed( 0x1 ) ) latch.w |= 0x4000;
            if ( Pressed( 0x2 ) ) latch.w |= 0x2000;
            if ( Pressed( 0x3 ) ) latch.w |= 0x1000;
            if ( Pressed( 0x4 ) ) latch.w |= 0x0800;
            if ( Pressed( 0x5 ) ) latch.w |= 0x0400;
            if ( Pressed( 0x6 ) ) latch.w |= 0x0200;
            if ( Pressed( 0x7 ) ) latch.w |= 0x0100;
            if ( Pressed( 0x8 ) ) latch.w |= 0x0080;
            if ( Pressed( 0x9 ) ) latch.w |= 0x0040;
            if ( Pressed( 0xA ) ) latch.w |= 0x0020;
            if ( Pressed( 0xB ) ) latch.w |= 0x0010;
        }
    }
}