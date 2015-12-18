using Nintemulator.Shared;
using System;

namespace Nintemulator.GBA.CPU
{
    public partial class Cpu
    {
        private const uint OP_AND = 0x0;
        private const uint OP_EOR = 0x1;
        private const uint OP_SUB = 0x2;
        private const uint OP_RSB = 0x3;
        private const uint OP_ADD = 0x4;
        private const uint OP_ADC = 0x5;
        private const uint OP_SBC = 0x6;
        private const uint OP_RSC = 0x7;
        private const uint OP_TST = 0x8;
        private const uint OP_TEQ = 0x9;
        private const uint OP_CMP = 0xA;
        private const uint OP_CMN = 0xB;
        private const uint OP_ORR = 0xC;
        private const uint OP_MOV = 0xD;
        private const uint OP_BIC = 0xE;
        private const uint OP_MVN = 0xF;

        private uint carryout;

        private uint Armv4Encode( uint code )
        {
            return ( ( code >> 16 ) & 0xff0 ) | ( ( code >> 4 ) & 0x00f );
        }
        private void Armv4Execute( )
        {
            if ( pipeline.refresh )
            {
                pipeline.refresh = false;
                pipeline.fetch.addr = pc.value & ~3U;
                pipeline.fetch.data = console.PeekWord( pipeline.fetch.addr );

                Armv4Step( );
            }

            Armv4Step( );

            if ( irqline && cpsr.i == 0 ) // irq after pipeline initialized in correct mode
            {
                Isr( Mode.IRQ, Vector.IRQ );
                return;
            }

            code = pipeline.execute.data;

            if ( GetCondition( code >> 28 ) )
                armv4Codes[ ( ( code >> 16 ) & 0xFF0 ) | ( ( code >> 4 ) & 0xF ) ]( );
        }
        private void Armv4Initialize( )
        {
            this.InitializeDispatchFunc( );
        }
        private void Armv4Map( string pattern, Action code )
        {
            var mask = Armv4Encode( Utility.Mask( pattern ) );
            var zero = Armv4Encode( Utility.Flat( pattern ) );
            var full = Armv4Encode( Utility.Full( pattern ) );

            for ( uint i = zero; i <= full; i++ )
            {
                if ( ( i & mask ) == zero )
                {
                    armv4Codes[ i ] = code;
                }
            }
        }
        private void Armv4Step( )
        {
            pc.value += 4U;

            pipeline.execute = pipeline.decode;
            pipeline.decode = pipeline.fetch;
            pipeline.fetch.addr = pc.value & ~3U;
            pipeline.fetch.data = console.PeekWord( pipeline.fetch.addr );
        }

        private void DataProcessingWriteToR15( )
        {
            if ( ( code & ( 1U << 20 ) ) != 0 && spsr != null )
            {
                cpsr.Load( spsr.Save( ) );
                ChangeMode( spsr.m );
            }

            pipeline.refresh = true;
        }
        private int MultiplyCycleCalculation( uint rs )
        {
            // Multiply cycle calculations
            if ( ( rs & 0xFFFFFF00 ) == 0 || ( rs & 0xFFFFFF00 ) == 0xFFFFFF00 ) { return 1; }
            if ( ( rs & 0xFFFF0000 ) == 0 || ( rs & 0xFFFF0000 ) == 0xFFFF0000 ) { return 2; }
            if ( ( rs & 0xFF000000 ) == 0 || ( rs & 0xFF000000 ) == 0xFF000000 ) { return 3; }

            return 4;
        }

        private uint BarrelShifterLslImm( )
        {
            // rm lsl immed
            uint rm = registers[ this.code & 0xF ].value;
            int amount = ( int )( ( this.code >> 7 ) & 0x1F );

            if ( amount == 0 )
            {
                this.carryout = this.cpsr.c;
                return rm;
            }
            else
            {
                this.carryout = ( rm >> ( 32 - amount ) ) & 1;
                return rm << amount;
            }
        }
        private uint BarrelShifterLslReg( )
        {
            // rm lsl rs
            uint rm = registers[ this.code & 0xF ].value;
            uint rs = ( this.code >> 8 ) & 0xF;

            int amount;
            if ( rs == 15 )
            {
                amount = ( int )( ( registers[ rs ].value + 0x4 ) & 0xFF );
            }
            else
            {
                amount = ( int )( registers[ rs ].value & 0xFF );
            }

            if ( ( this.code & 0xF ) == 15 )
            {
                rm += 4;
            }

            if ( amount == 0 )
            {
                this.carryout = this.cpsr.c;
                return rm;
            }

            if ( amount < 32 )
            {
                this.carryout = ( rm >> ( 32 - amount ) ) & 1;
                return rm << amount;
            }
            else if ( amount == 32 )
            {
                this.carryout = rm & 1;
                return 0;
            }
            else
            {
                this.carryout = 0;
                return 0;
            }
        }
        private uint BarrelShifterLsrImm( )
        {
            // rm lsr immed
            uint rm = registers[ this.code & 0xF ].value;
            int amount = ( int )( ( this.code >> 7 ) & 0x1F );

            if ( amount == 0 )
            {
                this.carryout = ( rm >> 31 ) & 1;
                return 0;
            }
            else
            {
                this.carryout = ( rm >> ( amount - 1 ) ) & 1;
                return rm >> amount;
            }
        }
        private uint BarrelShifterLsrReg( )
        {
            // rm lsr rs
            uint rm = registers[ this.code & 0xF ].value;
            uint rs = ( this.code >> 8 ) & 0xF;

            int amount;
            if ( rs == 15 )
            {
                amount = ( int )( ( registers[ rs ].value + 0x4 ) & 0xFF );
            }
            else
            {
                amount = ( int )( registers[ rs ].value & 0xFF );
            }

            if ( ( this.code & 0xF ) == 15 )
            {
                rm += 4;
            }

            if ( amount == 0 )
            {
                this.carryout = this.cpsr.c;
                return rm;
            }

            if ( amount < 32 )
            {
                this.carryout = ( rm >> ( amount - 1 ) ) & 1;
                return rm >> amount;
            }
            else if ( amount == 32 )
            {
                this.carryout = ( rm >> 31 ) & 1;
                return 0;
            }
            else
            {
                this.carryout = 0;
                return 0;
            }
        }
        private uint BarrelShifterAsrImm( )
        {
            // rm asr immed
            uint rm = registers[ this.code & 0xF ].value;
            int amount = ( int )( ( this.code >> 7 ) & 0x1F );

            if ( amount == 0 )
            {
                if ( ( rm & ( 1 << 31 ) ) == 0 )
                {
                    this.carryout = 0;
                    return 0;
                }
                else
                {
                    this.carryout = 1;
                    return 0xFFFFFFFF;
                }
            }
            else
            {
                this.carryout = ( rm >> ( amount - 1 ) ) & 1;
                return ( uint )( ( ( int )rm ) >> amount );
            }
        }
        private uint BarrelShifterAsrReg( )
        {
            // rm asr rs
            uint rm = registers[ this.code & 0xF ].value;
            uint rs = ( this.code >> 8 ) & 0xF;

            int amount;
            if ( rs == 15 )
            {
                amount = ( int )( ( registers[ rs ].value + 0x4 ) & 0xFF );
            }
            else
            {
                amount = ( int )( registers[ rs ].value & 0xFF );
            }

            if ( ( this.code & 0xF ) == 15 )
            {
                rm += 4;
            }

            if ( amount == 0 )
            {
                this.carryout = this.cpsr.c;
                return rm;
            }

            if ( amount >= 32 )
            {
                if ( ( rm & ( 1 << 31 ) ) == 0 )
                {
                    this.carryout = 0;
                    return 0;
                }
                else
                {
                    this.carryout = 1;
                    return 0xFFFFFFFF;
                }
            }
            else
            {
                this.carryout = ( rm >> ( amount - 1 ) ) & 1;
                return ( uint )( ( ( int )rm ) >> amount );
            }
        }
        private uint BarrelShifterRorImm( )
        {
            // rm ror immed
            uint rm = registers[ this.code & 0xF ].value;
            int amount = ( int )( ( this.code >> 7 ) & 0x1F );

            if ( amount == 0 )
            {
                // Actually an RRX
                this.carryout = rm & 1;
                return ( this.cpsr.c << 31 ) | ( rm >> 1 );
            }
            else
            {
                this.carryout = ( rm >> ( amount - 1 ) ) & 1;
                return ( rm >> amount ) | ( rm << ( 32 - amount ) );
            }
        }
        private uint BarrelShifterRorReg( )
        {
            // rm ror rs
            uint rm = registers[ this.code & 0xF ].value;
            uint rs = ( this.code >> 8 ) & 0xF;

            int amount;
            if ( rs == 15 )
            {
                amount = ( int )( ( registers[ rs ].value + 0x4 ) & 0xFF );
            }
            else
            {
                amount = ( int )( registers[ rs ].value & 0xFF );
            }

            if ( ( this.code & 0xF ) == 15 )
            {
                rm += 4;
            }

            if ( amount == 0 )
            {
                this.carryout = this.cpsr.c;
                return rm;
            }

            if ( ( amount & 0x1F ) == 0 )
            {
                this.carryout = ( rm >> 31 ) & 1;
                return rm;
            }
            else
            {
                amount &= 0x1F;
                this.carryout = ( rm >> amount ) & 1;
                return ( rm >> amount ) | ( rm << ( 32 - amount ) );
            }
        }
        private uint BarrelShifterImm( )
        {
            uint immed = this.code & 0xFF;
            int rotateAmount = ( int )( ( ( this.code >> 8 ) & 0xF ) * 2 );

            if ( rotateAmount == 0 )
            {
                this.carryout = this.cpsr.c;
                return immed;
            }
            else
            {
                immed = ( immed >> rotateAmount ) | ( immed << ( 32 - rotateAmount ) );
                this.carryout = ( immed >> 31 ) & 1;
                return immed;
            }
        }

        #region Opcodes

        private void LoadStoreMultiple( )
        {
            uint rn = ( this.code >> 16 ) & 0xF;

            uint curCpsr = this.cpsr.Save( );

            bool up = ( this.code & ( 1 << 23 ) ) != 0;
            bool um = ( this.code & ( 1 << 22 ) ) != 0;
            bool wb = ( this.code & ( 1 << 21 ) ) != 0;
            bool ld = ( this.code & ( 1 << 20 ) ) != 0;

            uint store_address;
            uint address;
            uint bitsSet = 0;
            for ( int i = 0; i < 16; i++ ) if ( ( ( this.code >> i ) & 1 ) != 0 ) bitsSet++;

            if ( ( this.code & ( 1 << 24 ) ) != 0 )
            {
                if ( up )
                {
                    // Increment before
                    address = this.registers[ rn ].value + 4;
                    store_address = this.registers[ rn ].value + bitsSet * 4;
                }
                else
                {
                    // Decrement before
                    address = this.registers[ rn ].value - ( bitsSet * 4 );
                    store_address = this.registers[ rn ].value - bitsSet * 4;
                }
            }
            else
            {
                if ( up )
                {
                    // Increment after
                    address = this.registers[ rn ].value;
                    store_address = this.registers[ rn ].value + bitsSet * 4;
                }
                else
                {
                    // Decrement after
                    address = this.registers[ rn ].value - ( bitsSet * 4 ) + 4;
                    store_address = this.registers[ rn ].value - bitsSet * 4;
                }
            }

            if ( ( this.code & ( 1 << 20 ) ) != 0 )
            {
                if ( um && ( ( this.code >> 15 ) & 1 ) == 0 )
                {
                    ChangeRegisters( Mode.USR );
                }
                
                if ( ( code & 0x0001 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  0 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0002 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  1 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0004 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  2 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0008 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  3 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0010 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  4 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0020 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  5 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0040 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  6 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0080 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  7 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0100 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  8 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0200 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[  9 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0400 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[ 10 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x0800 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[ 11 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x1000 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[ 12 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x2000 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[ 13 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x4000 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[ 14 ].value = console.PeekWord( address & ~3u ); address += 4; }
                if ( ( code & 0x8000 ) != 0 ) { if ( wb ) registers[ rn ].value = store_address; registers[ 15 ].value = console.PeekWord( address & ~3u ); address += 4; pipeline.refresh = true; }

                if ( um )
                {
                    if ( ( code & 0x8000 ) != 0 && spsr != null )
                    {
                        ChangeMode( spsr.m );
                        cpsr.Load( spsr.Save( ) );
                    }
                    else
                    {
                        ChangeRegisters( curCpsr & 31 );
                    }
                }
            }
            else
            {
                if ( um )
                    ChangeRegisters( Mode.USR );

                if ( ( code & 0x0001 ) != 0 ) { console.PokeWord( address, registers[  0 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0002 ) != 0 ) { console.PokeWord( address, registers[  1 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0004 ) != 0 ) { console.PokeWord( address, registers[  2 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0008 ) != 0 ) { console.PokeWord( address, registers[  3 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0010 ) != 0 ) { console.PokeWord( address, registers[  4 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0020 ) != 0 ) { console.PokeWord( address, registers[  5 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0040 ) != 0 ) { console.PokeWord( address, registers[  6 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0080 ) != 0 ) { console.PokeWord( address, registers[  7 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0100 ) != 0 ) { console.PokeWord( address, registers[  8 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0200 ) != 0 ) { console.PokeWord( address, registers[  9 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0400 ) != 0 ) { console.PokeWord( address, registers[ 10 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x0800 ) != 0 ) { console.PokeWord( address, registers[ 11 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x1000 ) != 0 ) { console.PokeWord( address, registers[ 12 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x2000 ) != 0 ) { console.PokeWord( address, registers[ 13 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x4000 ) != 0 ) { console.PokeWord( address, registers[ 14 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }
                if ( ( code & 0x8000 ) != 0 ) { console.PokeWord( address, registers[ 15 ].value ); address += 4; if ( wb ) registers[ rn ].value = store_address; }

                if ( um )
                    ChangeRegisters( curCpsr & 31 );
            }
        }

        #endregion Opcodes
    }
}