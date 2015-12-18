using Nintemulator.Shared;

namespace Nintemulator.GBA
{
    public class Pad : GameboyAdvance.Input
    {
        public static bool AutofireState;

        private Register16 data;
        private Register16 mask;

        public Pad( GameboyAdvance console )
            : base( console, 0, 12 )
        {
            Map( 0, "A" );          //  0    Button A        (0=Pressed, 1=Released)
            Map( 1, "X" );          //  1    Button B        (etc.)
            Map( 2, "Back" );       //  2    Select          (etc.)
            Map( 3, "Menu" );       //  3    Start           (etc.)
            Map( 4, "DPad-R" );     //  4    Right           (etc.)
            Map( 5, "DPad-L" );     //  5    Left            (etc.)
            Map( 6, "DPad-U" );     //  6    Up              (etc.)
            Map( 7, "DPad-D" );     //  7    Down            (etc.)
            Map( 8, "R-Shoulder" ); //  8    Button R        (etc.)
            Map( 9, "L-Shoulder" ); //  9    Button L        (etc.)
            Map( 10, "B" );
            Map( 11, "Y" );
        }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            console.Hook( 0x130, ( a ) => data.l );
            console.Hook( 0x131, ( a ) => data.h );
            console.Hook( 0x132, ( a ) => mask.l, ( a, data ) => mask.l = data );
            console.Hook( 0x133, ( a ) => mask.h, ( a, data ) => mask.h = data );
        }

        public override void Update( )
        {
            base.Update( );

            data.w = 0x3FF;

            if ( Pressed( 0 ) ) data.w ^= 0x0001;
            if ( Pressed( 1 ) ) data.w ^= 0x0002;
            if ( Pressed( 2 ) ) data.w ^= 0x0004;
            if ( Pressed( 3 ) ) data.w ^= 0x0008;
            if ( Pressed( 4 ) ) data.w ^= 0x0010;
            if ( Pressed( 5 ) ) data.w ^= 0x0020;
            if ( Pressed( 6 ) ) data.w ^= 0x0040;
            if ( Pressed( 7 ) ) data.w ^= 0x0080;
            if ( Pressed( 8 ) ) data.w ^= 0x0100;
            if ( Pressed( 9 ) ) data.w ^= 0x0200;

            if ( AutofireState )
            {
                if ( Pressed( 10 ) ) data.w ^= 0x0001;
                if ( Pressed( 11 ) ) data.w ^= 0x0002;
            }

            if ( ( mask.w & 0x4000 ) != 0 )
            {
                if ( ( mask.w & 0x8000 ) != 0 )
                {
                    if ( ( ~data.w & mask.w & 0x3FF ) == mask.w )
                        cpu.Interrupt( CPU.Cpu.Source.Joypad );
                }
                else
                {
                    if ( ( ~data.w & mask.w & 0x3FF ) != 0 )
                        cpu.Interrupt( CPU.Cpu.Source.Joypad );
                }
            }
        }
    }
}