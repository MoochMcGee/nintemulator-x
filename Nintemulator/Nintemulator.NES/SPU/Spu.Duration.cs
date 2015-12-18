﻿namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public class Duration
        {
            private static readonly int[] lookup =
            {
                0x0A, 0xFE, 0x14, 0x02, 0x28, 0x04, 0x50, 0x06,
                0xA0, 0x08, 0x3C, 0x0A, 0x0E, 0x0C, 0x1A, 0x0E,
                0x0C, 0x10, 0x18, 0x12, 0x30, 0x14, 0x60, 0x16,
                0xC0, 0x18, 0x48, 0x1A, 0x10, 0x1C, 0x20, 0x1E, 
            };

            public bool Halted;
            public int Counter;
            public int Enabled;

            public void Clock( )
            {
                if ( Counter != 0 && !Halted )
                    Counter = ( Counter - 1 ) & Enabled;
            }
            public void SetCounter( byte value )
            {
                Counter = lookup[ value >> 3 ] & Enabled;
            }
            public void SetEnabled( bool value )
            {
                Enabled = value ? 0xFF : 0x00;
                Counter = Counter & Enabled;
            }
        }
    }
}