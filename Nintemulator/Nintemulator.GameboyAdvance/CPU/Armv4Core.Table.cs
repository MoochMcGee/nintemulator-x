using Nintemulator.Shared;
using System;
using word = System.UInt32;
using half = System.UInt16;

namespace Nintemulator.GBA.CPU
{
    public partial class Cpu
    {
        private void OpAlu( uint value )
        {
            uint rd = ( code >> 12 ) & 15U;
            uint rn = ( code >> 16 ) & 15U;
            bool check = false;

            switch ( code >> 21 & 15U )
            {
            case 0x0U: check = true; registers[ rd ].value = Mov( registers[ rn ].value & value ); break; // AND
            case 0x1U: check = true; registers[ rd ].value = Mov( registers[ rn ].value ^ value ); break; // EOR
            case 0x2U: check = true; registers[ rd ].value = Sub( registers[ rn ].value, value ); break; // SUB
            case 0x3U: check = true; registers[ rd ].value = Sub( value, registers[ rn ].value ); break; // RSB
            case 0x4U: check = true; registers[ rd ].value = Add( registers[ rn ].value, value ); break; // ADD
            case 0x5U: check = true; registers[ rd ].value = Add( registers[ rn ].value, value, cpsr.c ); break; // ADC
            case 0x6U: check = true; registers[ rd ].value = Sub( registers[ rn ].value, value, cpsr.c ); break; // SBC
            case 0x7U: check = true; registers[ rd ].value = Sub( value, registers[ rn ].value, cpsr.c ); break; // RSC
            case 0x8U: Mov( registers[ rn ].value & value ); break; // TST
            case 0x9U: Mov( registers[ rn ].value ^ value ); break; // TEQ
            case 0xAU: Sub( registers[ rn ].value, value ); break; // CMP
            case 0xBU: Add( registers[ rn ].value, value ); break; // CMN
            case 0xCU: check = true; registers[ rd ].value = Mov( registers[ rn ].value | value ); break; // ORR
            case 0xDU: check = true; registers[ rd ].value = Mov( value ); break; // MOV
            case 0xEU: check = true; registers[ rd ].value = Mov( registers[ rn ].value & ~value ); break; // BIC
            case 0xFU: check = true; registers[ rd ].value = Mov( ~value ); break; // MVN
            }

            if ( rd == 15U && check )
                DataProcessingWriteToR15( );
        }
        private void OpAluImm( )
        {
            uint shift = ( code >> 8 ) & 15U;
            uint value = ( code >> 0 ) & 255U;

            carryout = cpsr.c;
            if ( shift != 0U ) value = Ror( value, 2 * shift );

            OpAlu( value );
        }
        private void OpAluRegImm( )
        {
            uint shift = ( code >> 7 ) & 31U;
            uint value = registers[ ( code >> 0 ) & 15U ].value;

            carryout = cpsr.c;

            switch ( code >> 5 & 3U )
            {
            case 0U: value = Lsl( value, shift ); break;
            case 1U: value = Lsr( value, shift == 0U ? 32U : shift ); break;
            case 2U: value = Asr( value, shift == 0U ? 32U : shift ); break;
            case 3U: value = shift != 0U ? Ror( value, shift ) : Rrx( value ); break;
            }

            OpAlu( value );
        }
        private void OpAluRegReg( )
        {
            uint rs = ( code >> 8 ) & 15U;
            uint rm = ( code >> 0 ) & 15U;
            uint shift = registers[ rs ].value & 255U;
            uint value = registers[ rm ].value;

            if ( rm == 15 ) value += 4;
            carryout = cpsr.c;

            switch ( code >> 5 & 3U )
            {
            case 0U: value = Lsl( value, shift < 33 ? shift : 33 ); break;
            case 1U: value = Lsr( value, shift < 33 ? shift : 33 ); break;
            case 2U: value = Asr( value, shift < 32 ? shift : 32 ); break;
            case 3U: if ( shift != 0U ) value = Ror( value, ( shift & 31U ) == 0U ? 32U : shift & 31U ); break;
            }

            OpAlu( value );
        }
        private void OpMultiply( )
        {
            bool accumulate = ( ( code >> 21 ) & 1U ) != 0U;
            uint d = ( code >> 16 ) & 15U;
            uint n = ( code >> 12 ) & 15U;
            uint s = ( code >> 8 ) & 15U;
            uint m = ( code >> 0 ) & 15U;

            registers[ d ].value = Mul( accumulate ? registers[ n ].value : 0U, registers[ m ].value, registers[ s ].value );
        }
        private void OpMultiplyLong( )
        {
            bool signextend = ( ( code >> 22 ) & 1U ) != 0U;
            bool accumulate = ( ( code >> 21 ) & 1U ) != 0U;
            bool save = ( ( code >> 20 ) & 1U ) != 0U;
            uint dhi = ( code >> 16 ) & 15U;
            uint dlo = ( code >> 12 ) & 15U;
            uint s = ( code >> 8 ) & 15U;
            uint m = ( code >> 0 ) & 15U;

            ulong rm = registers[ m ].value;
            ulong rs = registers[ s ].value;

            if ( signextend )
            {
                rm = ( rm ^ 0x80000000UL ) - 0x80000000UL;
                rs = ( rs ^ 0x80000000UL ) - 0x80000000UL;
            }

            ulong rd = rm * rs;
            if ( accumulate ) rd += ( ( ulong )registers[ dhi ].value << 32 ) + ( ( ulong )registers[ dlo ].value << 0 );

            registers[ dhi ].value = ( word )( rd >> 32 );
            registers[ dlo ].value = ( word )( rd );

            if ( save )
            {
                cpsr.n = ( registers[ dhi ].value >> 31 );
                cpsr.z = ( registers[ dhi ].value == 0U ) && ( registers[ dlo ].value == 0U ) ? 1U : 0U;
            }
        }
        private void OpMoveHalfImm( )
        {
            uint p = ( code >> 24 ) & 1U;
            uint u = ( code >> 23 ) & 1U;
            uint w = ( code >> 21 ) & 1U;
            uint l = ( code >> 20 ) & 1U;
            uint n = ( code >> 16 ) & 15U;
            uint d = ( code >> 12 ) & 15U;
            uint ih = ( code >> 8 ) & 15U;
            uint il = ( code >> 0 ) & 15U;

            uint rn = registers[ n ].value;
            uint nn = ( ih << 4 ) + ( il << 0 );
            
            if ( p == 1 ) rn = u != 0U ? rn + nn : rn - nn;
            if ( l == 1 ) registers[ d ].value = console.PeekHalf( rn ); // todo: load half
            if ( l == 0 ) console.PokeHalf( rn, ( half )registers[ d ].value ); // todo: store half
            if ( p == 0 ) rn = u != 0U ? rn + nn : rn - nn;
            if ( p == 0 || w == 1 && n != d ) registers[ n ].value = rn;

            if ( d == 15 ) pipeline.refresh = true;
        }
        private void OpMoveHalfReg( )
        {
            uint p = ( code >> 24 ) & 1U;
            uint u = ( code >> 23 ) & 1U;
            uint w = ( code >> 21 ) & 1U;
            uint l = ( code >> 20 ) & 1U;
            uint n = ( code >> 16 ) & 15U;
            uint d = ( code >> 12 ) & 15U;
            uint m = ( code >> 0 ) & 15U;

            uint rn = registers[ n ].value;
            uint rm = registers[ m ].value;

            if ( p == 1 ) rn = u != 0U ? rn + rm : rn - rm;
            if ( l == 1 ) registers[ d ].value = console.PeekHalf( rn ); // todo: load half
            if ( l == 0 ) console.PokeHalf( rn, ( half )registers[ d ].value ); // todo: store half
            if ( p == 0 ) rn = u != 0U ? rn + rm : rn - rm;
            if ( p == 0 || w == 1 && n != d ) registers[ n ].value = rn;

            if ( d == 15 ) pipeline.refresh = true;
        }
        private void OpLoadImm( )
        {
            uint p = ( code >> 24 ) & 1U;
            uint u = ( code >> 23 ) & 1U;
            uint w = ( code >> 21 ) & 1U;
            uint n = ( code >> 16 ) & 15U;
            uint d = ( code >> 12 ) & 15U;
            uint ih = ( code >> 8 ) & 15U;
            uint h = ( code >> 5 ) & 1U;
            uint il = ( code >> 0 ) & 15U;

            uint rn = registers[ n ].value;
            uint nn = ( ih << 4 ) + ( il << 0 );

            if ( p == 1U ) rn = u != 0U ? rn + nn : rn - nn;
            if ( h != 0U )
                registers[ d ].value = ( word )( short )console.PeekHalf( rn ); // todo: load half
            else
                registers[ d ].value = ( word )( sbyte )console.PeekByte( rn ); // todo: load byte
            if ( p == 0U ) rn = u != 0U ? rn + nn : rn - nn;
            if ( p == 0U || w == 1U && n != d ) registers[ n ].value = rn;

            if ( d == 15 ) pipeline.refresh = true;
        }
        private void OpLoadReg( )
        {
            uint p = ( code >> 24 ) & 1U;
            uint u = ( code >> 23 ) & 1U;
            uint w = ( code >> 21 ) & 1U;
            uint n = ( code >> 16 ) & 15U;
            uint d = ( code >> 12 ) & 15U;
            uint h = ( code >> 5 ) & 1U;
            uint m = ( code >> 0 ) & 15U;

            uint rn = registers[ n ].value;
            uint rm = registers[ m ].value;

            if ( p == 1U ) rn = u != 0U ? rn + rm : rn - rm;
            if ( h != 0U )
                registers[ d ].value = ( word )( short )console.PeekHalf( rn ); // todo: load half
            else
                registers[ d ].value = ( word )( sbyte )console.PeekByte( rn ); // todo: load byte
            if ( p == 0U ) rn = u != 0U ? rn + rm : rn - rm;
            if ( p == 0U || w == 1U && n != d ) registers[ n ].value = rn;

            if ( d == 15 ) pipeline.refresh = true;
        }

        #region Delegate Dispatcher

        private void OpSwap( )
        {
            uint rn = ( code >> 16 ) & 15;
            uint rd = ( code >> 12 ) & 15;
            uint rm = ( code >>  0 ) & 15;
            uint tmp;

            switch ( code >> 22 & 1 )
            {
            case 0:
                tmp = console.LoadWord( registers[ rn ].value );
                console.PokeWord( registers[ rn ].value, ( word )registers[ rm ].value );
                registers[ rd ].value = tmp;
                break;

            case 1:
                tmp = console.PeekByte( registers[ rn ].value );
                console.PokeByte( registers[ rn ].value, ( byte )registers[ rm ].value );
                registers[ rd ].value = tmp;
                break;
            }
        }

        private void Op207( )
        {
            // MRS rd, cpsr
            registers[ ( code >> 12 ) & 15 ].value = cpsr.Save( );
        }
        private void Op208( )
        {
            // MRS rd, spsr
            if ( spsr == null ) return;
            registers[ ( code >> 12 ) & 15 ].value = spsr.Save( );
        }
        private void Op209( )
        {
            // MSR cpsr, rm
            var value = registers[ code & 15 ].value;

            if ( ( code & ( 1 << 16 ) ) != 0 && cpsr.m != Mode.USR )
            {
                ChangeRegisters( value & 31 );
                cpsr.m = ( value >> 0 ) & 31;
                cpsr.t = ( value >> 5 ) & 1;
                cpsr.f = ( value >> 6 ) & 1;
                cpsr.i = ( value >> 7 ) & 1;
            }

            if ( ( code & ( 1 << 19 ) ) != 0 )
            {
                cpsr.v = ( value >> 28 ) & 1;
                cpsr.c = ( value >> 29 ) & 1;
                cpsr.z = ( value >> 30 ) & 1;
                cpsr.n = ( value >> 31 ) & 1;
            }
        }
        private void Op210( )
        {
            if ( this.spsr != null )
            {
                uint spsr = this.spsr.Save( );
                uint rm = registers[ code & 0xF ].value;

                if ( ( code & ( 1 << 16 ) ) != 0 ) { spsr &= 0xFFFFFF00; spsr |= rm & 0x000000FF; }
                if ( ( code & ( 1 << 17 ) ) != 0 ) { spsr &= 0xFFFF00FF; spsr |= rm & 0x0000FF00; }
                if ( ( code & ( 1 << 18 ) ) != 0 ) { spsr &= 0xFF00FFFF; spsr |= rm & 0x00FF0000; }
                if ( ( code & ( 1 << 19 ) ) != 0 ) { spsr &= 0x00FFFFFF; spsr |= rm & 0xFF000000; }

                this.spsr.Load( spsr );
            }
        }
        private void Op211( )
        {
            // MSR cpsr, #nn
            var value = ( code >> 0 ) & 255;
            var shift = ( code >> 7 ) & 30;

            value = Ror( value, shift );

            if ( ( code & ( 1 << 16 ) ) != 0 && cpsr.m != Mode.USR )
            {
                ChangeRegisters( value & 31 );
                cpsr.m = ( value >> 0 ) & 31;
                cpsr.t = ( value >> 5 ) & 1;
                cpsr.f = ( value >> 6 ) & 1;
                cpsr.i = ( value >> 7 ) & 1;
            }

            if ( ( code & ( 1 << 19 ) ) != 0 )
            {
                cpsr.v = ( value >> 28 ) & 1;
                cpsr.c = ( value >> 29 ) & 1;
                cpsr.z = ( value >> 30 ) & 1;
                cpsr.n = ( value >> 31 ) & 1;
            }
        }
        private void Op212( )
        {
            // MSR spsr, immed
            if ( this.spsr != null )
            {
                uint spsr = this.spsr.Save( );
                uint value = ( this.code >> 0 ) & 255;
                uint shift = ( this.code >> 7 ) & 30;

                value = Ror( value, shift );

                if ( ( this.code & ( 1 << 16 ) ) == 1 << 16 ) { spsr &= 0xFFFFFF00; spsr |= value & 0x000000FF; }
                if ( ( this.code & ( 1 << 17 ) ) == 1 << 17 ) { spsr &= 0xFFFF00FF; spsr |= value & 0x0000FF00; }
                if ( ( this.code & ( 1 << 18 ) ) == 1 << 18 ) { spsr &= 0xFF00FFFF; spsr |= value & 0x00FF0000; }
                if ( ( this.code & ( 1 << 19 ) ) == 1 << 19 ) { spsr &= 0x00FFFFFF; spsr |= value & 0xFF000000; }

                this.spsr.Load( spsr );
            }
        }
        private void OpBx( )
        {
            var rm = ( code >> 0 ) & 15U;
            cpsr.t = registers[ rm ].value & 1U;

            registers[ 15 ].value = registers[ rm ].value & ~1U;
            pipeline.refresh = true;
        }

        private void Op338( )
        {
            uint rn, rd, offset, alu;
            // STR rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op339( )
        {
            uint rn, rd, offset, alu;
            // STRT rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op340( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op341( )
        {
            uint rn, rd, offset, alu;
            // STRBT rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op342( )
        {
            uint rn, rd, alu;
            // STR rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op343( )
        {
            uint rn, rd, alu;
            // STRT rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op344( )
        {
            uint rn, rd, alu;
            // STRB rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op345( )
        {
            uint rn, rd, alu;
            // STRBT rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op346( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + offset, alu );
        }
        private void Op347( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op348( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + offset, ( byte )( alu & 0xFF ) );
        }
        private void Op349( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op350( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + ( this.code & 0xFFF ), alu );
        }
        private void Op351( )
        {
            uint rn, rd, alu;
            // STRT rd, [rn, immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.code & 0xFFF;
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op352( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + ( this.code & 0xFFF ), ( byte )( alu & 0xFF ) );
        }
        private void Op353( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.code & 0xFFF;
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op354( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op355( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op356( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op357( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, -immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op358( )
        {
            uint rn, rd;
            // LDR rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op359( )
        {
            uint rn, rd;
            // LDRT rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op360( )
        {
            uint rn, rd;
            // LDRB rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op361( )
        {
            uint rn, rd;
            // LDRBT rd, rn, immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += this.code & 0xFFF;
        }
        private void Op362( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op363( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op364( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op365( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.code & 0xFFF;
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op366( )
        {
            uint rn, rd;
            // LDR rd, [rn, immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + ( this.code & 0xFFF ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op367( )
        {
            uint rn, rd;
            // LDR rd, [rn, immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.code & 0xFFF;
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op368( )
        {
            uint rn, rd;
            // LDRB rd, [rn, immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + ( this.code & 0xFFF ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op369( )
        {
            uint rn, rd;
            // LDRB rd, [rn, immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.code & 0xFFF;
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op370( )
        {
            uint rn, rd, offset, alu;
            // STR rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op371( )
        {
            uint rn, rd, offset, alu;
            // STR rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op372( )
        {
            uint rn, rd, offset, alu;
            // STR rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op373( )
        {
            uint rn, rd, offset, alu;
            // STR rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op374( )
        {
            uint rn, rd, offset, alu;
            // STRT rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op375( )
        {
            uint rn, rd, offset, alu;
            // STRT rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op376( )
        {
            uint rn, rd, offset, alu;
            // STRT rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op377( )
        {
            uint rn, rd, offset, alu;
            // STRT rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += offset;
        }
        private void Op378( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op379( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op380( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op381( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op382( )
        {
            uint rn, rd, offset, alu;
            // STRBT rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op383( )
        {
            uint rn, rd, offset, alu;
            // STRBT rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op384( )
        {
            uint rn, rd, offset, alu;
            // STRBT rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op385( )
        {
            uint rn, rd, offset, alu;
            // STRBT rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += offset;
        }
        private void Op386( )
        {
            uint rn, rd, alu;
            // STR rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterLslImm( );
        }
        private void Op387( )
        {
            uint rn, rd, alu;
            // STR rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterLsrImm( );
        }
        private void Op388( )
        {
            uint rn, rd, alu;
            // STR rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterAsrImm( );
        }
        private void Op389( )
        {
            uint rn, rd, alu;
            // STR rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterRorImm( );
        }
        private void Op390( )
        {
            uint rn, rd, alu;
            // STRT rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterLslImm( );
        }
        private void Op391( )
        {
            uint rn, rd, alu;
            // STRT rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterLsrImm( );
        }
        private void Op392( )
        {
            uint rn, rd, alu;
            // STRT rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterAsrImm( );
        }
        private void Op393( )
        {
            uint rn, rd, alu;
            // STRT rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value, alu );
            registers[ rn ].value += this.BarrelShifterRorImm( );
        }
        private void Op394( )
        {
            uint rn, rd, alu;
            // STRB rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterLslImm( );
        }
        private void Op395( )
        {
            uint rn, rd, alu;
            // STRB rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterLsrImm( );
        }
        private void Op396( )
        {
            uint rn, rd, alu;
            // STRB rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterAsrImm( );
        }
        private void Op397( )
        {
            uint rn, rd, alu;
            // STRB rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterRorImm( );
        }
        private void Op398( )
        {
            uint rn, rd, alu;
            // STRBT rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterLslImm( );
        }
        private void Op399( )
        {
            uint rn, rd, alu;
            // STRBT rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterLsrImm( );
        }
        private void Op400( )
        {
            uint rn, rd, alu;
            // STRBT rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterAsrImm( );
        }
        private void Op401( )
        {
            uint rn, rd, alu;
            // STRBT rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
            registers[ rn ].value += this.BarrelShifterRorImm( );
        }
        private void Op402( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + offset, alu );
        }
        private void Op403( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + offset, alu );
        }
        private void Op404( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + offset, alu );
        }
        private void Op405( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + offset, alu );
        }
        private void Op406( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op407( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op408( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op409( )
        {
            uint rn, rd, offset, alu;
            // STR rd, [rn, -rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op410( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + offset, ( byte )( alu & 0xFF ) );
        }
        private void Op411( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + offset, ( byte )( alu & 0xFF ) );
        }
        private void Op412( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + offset, ( byte )( alu & 0xFF ) );
        }
        private void Op413( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + offset, ( byte )( alu & 0xFF ) );
        }
        private void Op414( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op415( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op416( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op417( )
        {
            uint rn, rd, offset, alu;
            // STRB rd, [rn, -rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += offset;
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op418( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + this.BarrelShifterLslImm( ), alu );
        }
        private void Op419( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + this.BarrelShifterLsrImm( ), alu );
        }
        private void Op420( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + this.BarrelShifterAsrImm( ), alu );
        }
        private void Op421( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeWord( registers[ rn ].value + this.BarrelShifterRorImm( ), alu );
        }
        private void Op422( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterLslImm( );
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op423( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterLsrImm( );
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op424( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterAsrImm( );
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op425( )
        {
            uint rn, rd, alu;
            // STR rd, [rn, rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterRorImm( );
            this.console.PokeWord( registers[ rn ].value, alu );
        }
        private void Op426( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + this.BarrelShifterLslImm( ), ( byte )( alu & 0xFF ) );
        }
        private void Op427( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + this.BarrelShifterLsrImm( ), ( byte )( alu & 0xFF ) );
        }
        private void Op428( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + this.BarrelShifterAsrImm( ), ( byte )( alu & 0xFF ) );
        }
        private void Op429( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            this.console.PokeByte( registers[ rn ].value + this.BarrelShifterRorImm( ), ( byte )( alu & 0xFF ) );
        }
        private void Op430( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterLslImm( );
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op431( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterLsrImm( );
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op432( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterAsrImm( );
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op433( )
        {
            uint rn, rd, alu;
            // STRB rd, [rn, rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            alu = registers[ rd ].value;
            if ( rd == 15 ) alu += 4;

            registers[ rn ].value += this.BarrelShifterRorImm( );
            this.console.PokeByte( registers[ rn ].value, ( byte )( alu & 0xFF ) );
        }
        private void Op434( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op435( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op436( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op437( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op438( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op439( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op440( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op441( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op442( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op443( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op444( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op445( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op446( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, -rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op447( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, -rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op448( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, -rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op449( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, -rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op450( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op451( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op452( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op453( )
        {
            uint rn, rd, offset;
            // LDR rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op454( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op455( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op456( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op457( )
        {
            uint rn, rd, offset;
            // LDRT rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op458( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op459( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op460( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op461( )
        {
            uint rn, rd, offset;
            // LDRB rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op462( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, rm lsl immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op463( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, rm lsr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op464( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, rm asr immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op465( )
        {
            uint rn, rd, offset;
            // LDRBT rd, rn, rm ror immed
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }

            if ( rn != rd )
                registers[ rn ].value += offset;
        }
        private void Op466( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op467( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op468( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op469( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op470( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op471( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op472( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op473( )
        {
            uint rn, rd, offset;
            // LDR rd, [rn, -rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op474( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op475( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op476( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op477( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + offset );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op478( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLslImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op479( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterLsrImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op480( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterAsrImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op481( )
        {
            uint rn, rd, offset;
            // LDRB rd, [rn, -rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            offset = this.BarrelShifterRorImm( );
            offset = ( uint )-offset;

            registers[ rn ].value += offset;
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op482( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + this.BarrelShifterLslImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op483( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + this.BarrelShifterLsrImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op484( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + this.BarrelShifterAsrImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op485( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value + this.BarrelShifterRorImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op486( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterLslImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op487( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterLsrImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op488( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterAsrImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op489( )
        {
            uint rn, rd;
            // LDR rd, [rn, rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterRorImm( );
            registers[ rd ].value = this.console.LoadWord( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op490( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm lsl immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + this.BarrelShifterLslImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op491( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm lsr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + this.BarrelShifterLsrImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op492( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm asr immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + this.BarrelShifterAsrImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op493( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm ror immed].value
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value + this.BarrelShifterRorImm( ) );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op494( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm lsl immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterLslImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op495( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm lsr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterLsrImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op496( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm asr immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterAsrImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void Op497( )
        {
            uint rn, rd;
            // LDRB rd, [rn, rm ror immed].value!
            rn = ( this.code >> 16 ) & 0xF;
            rd = ( this.code >> 12 ) & 0xF;

            registers[ rn ].value += this.BarrelShifterRorImm( );
            registers[ rd ].value = this.console.PeekByte( registers[ rn ].value );

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~3U;
                this.pipeline.refresh = true;
            }
        }
        private void OpB( )
        {
            registers[ 15 ].value += MathHelper.SignExtend( code, 24 ) << 2;
            pipeline.refresh = true;
        }
        private void OpBl( )
        {
            registers[ 14 ].value = pipeline.decode.addr;
            registers[ 15 ].value += MathHelper.SignExtend( code, 24 ) << 2;
            pipeline.refresh = true;
        }
        private void OpSwi( ) { Isr( Mode.SVC, Vector.SWI ); }
        private void OpUnd( ) { Isr( Mode.UND, Vector.UND ); }
        private void Op___( ) { LoadStoreMultiple( ); }

        private Action[] armv4Codes = null;

        private void InitializeDispatchFunc( )
        {
            this.armv4Codes = new Action[ 4096 ]
            {
                #region $000-$0FF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
	            #endregion
                #region $100-$1FF
                Op207, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                Op209, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                Op208, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                Op210, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $200-$2FF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $300-$3FF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                Op211, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                Op212, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $400-$4FF
                Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, Op338, // 0
                Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, Op354, // 1
                Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, Op339, // 2
                Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, Op355, // 3
                Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, Op340, // 4
                Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, Op356, // 5
                Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, Op341, // 6
                Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, Op357, // 7
                Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, Op342, // 8
                Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, Op358, // 9
                Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, Op343, // A
                Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, Op359, // B
                Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, Op344, // C
                Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, Op360, // D
                Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, Op345, // E
                Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, Op361, // F
                #endregion
                #region $500-$5FF
                Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, Op346, // 0
                Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, Op362, // 1
                Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, Op347, // 2
                Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, Op363, // 3
                Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, Op348, // 4
                Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, Op364, // 5
                Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, Op349, // 6
                Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, Op365, // 7
                Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, Op350, // 8
                Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, Op366, // 9
                Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, Op351, // A
                Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, Op367, // B
                Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, Op352, // C
                Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, Op368, // D
                Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, Op353, // E
                Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, Op369, // F
                #endregion
                #region $600-$6FF
                Op370, OpUnd, Op371, OpUnd, Op372, OpUnd, Op373, OpUnd, Op370, OpUnd, Op371, OpUnd, Op372, OpUnd, Op373, OpUnd, // 0
                Op434, OpUnd, Op435, OpUnd, Op436, OpUnd, Op437, OpUnd, Op434, OpUnd, Op435, OpUnd, Op436, OpUnd, Op437, OpUnd, // 1
                Op374, OpUnd, Op375, OpUnd, Op376, OpUnd, Op377, OpUnd, Op374, OpUnd, Op375, OpUnd, Op376, OpUnd, Op377, OpUnd, // 2
                Op438, OpUnd, Op439, OpUnd, Op440, OpUnd, Op441, OpUnd, Op438, OpUnd, Op439, OpUnd, Op440, OpUnd, Op441, OpUnd, // 3
                Op378, OpUnd, Op379, OpUnd, Op380, OpUnd, Op381, OpUnd, Op378, OpUnd, Op379, OpUnd, Op380, OpUnd, Op381, OpUnd, // 4
                Op442, OpUnd, Op443, OpUnd, Op444, OpUnd, Op445, OpUnd, Op442, OpUnd, Op443, OpUnd, Op444, OpUnd, Op445, OpUnd, // 5
                Op382, OpUnd, Op383, OpUnd, Op384, OpUnd, Op385, OpUnd, Op382, OpUnd, Op383, OpUnd, Op384, OpUnd, Op385, OpUnd, // 6
                Op446, OpUnd, Op447, OpUnd, Op448, OpUnd, Op449, OpUnd, Op446, OpUnd, Op447, OpUnd, Op448, OpUnd, Op449, OpUnd, // 7
                Op386, OpUnd, Op387, OpUnd, Op388, OpUnd, Op389, OpUnd, Op386, OpUnd, Op387, OpUnd, Op388, OpUnd, Op389, OpUnd, // 8
                Op450, OpUnd, Op451, OpUnd, Op452, OpUnd, Op453, OpUnd, Op450, OpUnd, Op451, OpUnd, Op452, OpUnd, Op453, OpUnd, // 9
                Op390, OpUnd, Op390, OpUnd, Op391, OpUnd, Op391, OpUnd, Op392, OpUnd, Op392, OpUnd, Op393, OpUnd, Op393, OpUnd, // A
                Op454, OpUnd, Op455, OpUnd, Op456, OpUnd, Op457, OpUnd, Op454, OpUnd, Op455, OpUnd, Op456, OpUnd, Op457, OpUnd, // B
                Op394, OpUnd, Op395, OpUnd, Op396, OpUnd, Op397, OpUnd, Op394, OpUnd, Op395, OpUnd, Op396, OpUnd, Op397, OpUnd, // C
                Op458, OpUnd, Op459, OpUnd, Op460, OpUnd, Op461, OpUnd, Op458, OpUnd, Op459, OpUnd, Op460, OpUnd, Op461, OpUnd, // D
                Op398, OpUnd, Op399, OpUnd, Op400, OpUnd, Op401, OpUnd, Op398, OpUnd, Op399, OpUnd, Op400, OpUnd, Op401, OpUnd, // E
                Op462, OpUnd, Op463, OpUnd, Op464, OpUnd, Op465, OpUnd, Op462, OpUnd, Op463, OpUnd, Op464, OpUnd, Op465, OpUnd, // F
                #endregion
                #region $700-$7FF
                Op402, OpUnd, Op403, OpUnd, Op404, OpUnd, Op405, OpUnd, Op402, OpUnd, Op403, OpUnd, Op404, OpUnd, Op405, OpUnd, // 0
                Op466, OpUnd, Op467, OpUnd, Op468, OpUnd, Op469, OpUnd, Op466, OpUnd, Op467, OpUnd, Op468, OpUnd, Op469, OpUnd, // 1
                Op406, OpUnd, Op407, OpUnd, Op408, OpUnd, Op409, OpUnd, Op406, OpUnd, Op407, OpUnd, Op408, OpUnd, Op409, OpUnd, // 2
                Op470, OpUnd, Op471, OpUnd, Op472, OpUnd, Op473, OpUnd, Op470, OpUnd, Op471, OpUnd, Op472, OpUnd, Op473, OpUnd, // 3
                Op410, OpUnd, Op411, OpUnd, Op412, OpUnd, Op413, OpUnd, Op410, OpUnd, Op411, OpUnd, Op412, OpUnd, Op413, OpUnd, // 4
                Op474, OpUnd, Op475, OpUnd, Op476, OpUnd, Op477, OpUnd, Op474, OpUnd, Op475, OpUnd, Op476, OpUnd, Op477, OpUnd, // 5
                Op414, OpUnd, Op415, OpUnd, Op416, OpUnd, Op417, OpUnd, Op414, OpUnd, Op415, OpUnd, Op416, OpUnd, Op417, OpUnd, // 6
                Op478, OpUnd, Op479, OpUnd, Op480, OpUnd, Op481, OpUnd, Op478, OpUnd, Op479, OpUnd, Op480, OpUnd, Op481, OpUnd, // 7
                Op418, OpUnd, Op419, OpUnd, Op420, OpUnd, Op421, OpUnd, Op418, OpUnd, Op419, OpUnd, Op420, OpUnd, Op421, OpUnd, // 8
                Op482, OpUnd, Op483, OpUnd, Op484, OpUnd, Op485, OpUnd, Op482, OpUnd, Op483, OpUnd, Op484, OpUnd, Op485, OpUnd, // 9
                Op422, OpUnd, Op423, OpUnd, Op424, OpUnd, Op425, OpUnd, Op422, OpUnd, Op423, OpUnd, Op424, OpUnd, Op425, OpUnd, // A
                Op486, OpUnd, Op487, OpUnd, Op488, OpUnd, Op489, OpUnd, Op486, OpUnd, Op487, OpUnd, Op488, OpUnd, Op489, OpUnd, // B
                Op426, OpUnd, Op427, OpUnd, Op428, OpUnd, Op429, OpUnd, Op426, OpUnd, Op427, OpUnd, Op428, OpUnd, Op429, OpUnd, // C
                Op490, OpUnd, Op491, OpUnd, Op492, OpUnd, Op493, OpUnd, Op490, OpUnd, Op491, OpUnd, Op492, OpUnd, Op493, OpUnd, // D
                Op430, OpUnd, Op431, OpUnd, Op432, OpUnd, Op433, OpUnd, Op430, OpUnd, Op431, OpUnd, Op432, OpUnd, Op433, OpUnd, // E
                Op494, OpUnd, Op495, OpUnd, Op496, OpUnd, Op497, OpUnd, Op494, OpUnd, Op495, OpUnd, Op496, OpUnd, Op497, OpUnd, // F
                #endregion
                #region $800-$8FF
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 0
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 1
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 2
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 3
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 4
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 5
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 6
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 7
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 8
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 9
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // A
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // B
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // C
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // D
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // E
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // F
                #endregion
                #region $900-$9FF
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 0
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 1
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 2
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 3
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 4
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 5
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 6
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 7
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 8
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // 9
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // A
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // B
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // C
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // D
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // E
                Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, Op___, // F
                #endregion
                #region $A00-$AFF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $B00-$BFF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $C00-$CFF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $D00-$DFF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $E00-$EFF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
                #region $F00-$FFF
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 0
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 1
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 2
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 3
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 4
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 5
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 6
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 7
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 8
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // 9
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // A
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // B
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // C
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // D
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // E
                OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, OpUnd, // F
                #endregion
            };

            Armv4Map( "---- 0000 00-- ---- ---- ---- 1001 ----", OpMultiply );
            Armv4Map( "---- 0000 1--- ---- ---- ---- 1001 ----", OpMultiplyLong );
            Armv4Map( "---- 0001 0-00 ---- ---- 0000 1001 ----", OpSwap );

            Armv4Map( "---- 0000 ---- ---- ---- ---- ---0 ----", OpAluRegImm );
            Armv4Map( "---- 0001 0--1 ---- ---- ---- ---0 ----", OpAluRegImm );
            Armv4Map( "---- 0001 1--- ---- ---- ---- ---0 ----", OpAluRegImm );
            Armv4Map( "---- 0000 ---- ---- ---- ---- 0--1 ----", OpAluRegReg );
            Armv4Map( "---- 0001 0--1 ---- ---- ---- 0--1 ----", OpAluRegReg );
            Armv4Map( "---- 0001 1--- ---- ---- ---- 0--1 ----", OpAluRegReg );
            Armv4Map( "---- 0010 ---- ---- ---- ---- ---- ----", OpAluImm );
            Armv4Map( "---- 0011 0--1 ---- ---- ---- ---- ----", OpAluImm );
            Armv4Map( "---- 0011 1--- ---- ---- ---- ---- ----", OpAluImm );

            Armv4Map( "---- 0000 -00- ---- ---- 0000 1011 ----", OpMoveHalfReg );
            Armv4Map( "---- 0001 -0-- ---- ---- 0000 1011 ----", OpMoveHalfReg );
            Armv4Map( "---- 0000 -10- ---- ---- ---- 1011 ----", OpMoveHalfImm );
            Armv4Map( "---- 0001 -1-- ---- ---- ---- 1011 ----", OpMoveHalfImm );

            Armv4Map( "---- 0000 -001 ---- ---- 0000 11-1 ----", OpLoadReg );
            Armv4Map( "---- 0001 -0-1 ---- ---- 0000 11-1 ----", OpLoadReg );
            Armv4Map( "---- 0000 -101 ---- ---- ---- 11-1 ----", OpLoadImm );
            Armv4Map( "---- 0001 -1-1 ---- ---- ---- 11-1 ----", OpLoadImm );

            Armv4Map( "---- 0001 0010 ---- ---- ---- 0001 ----", OpBx );
            Armv4Map( "---- 1010 ---- ---- ---- ---- ---- ----", OpB );
            Armv4Map( "---- 1011 ---- ---- ---- ---- ---- ----", OpBl );
            Armv4Map( "---- 1111 ---- ---- ---- ---- ---- ----", OpSwi );
        }
        #endregion
    }
}