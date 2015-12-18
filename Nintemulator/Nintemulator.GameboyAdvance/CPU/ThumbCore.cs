using Nintemulator.Shared;
using System;

namespace Nintemulator.GBA.CPU
{
    public partial class Cpu
    {
        private const int T_OP_AND = 0x0;
        private const int T_OP_EOR = 0x1;
        private const int T_OP_LSL = 0x2;
        private const int T_OP_LSR = 0x3;
        private const int T_OP_ASR = 0x4;
        private const int T_OP_ADC = 0x5;
        private const int T_OP_SBC = 0x6;
        private const int T_OP_ROR = 0x7;
        private const int T_OP_TST = 0x8;
        private const int T_OP_NEG = 0x9;
        private const int T_OP_CMP = 0xA;
        private const int T_OP_CMN = 0xB;
        private const int T_OP_ORR = 0xC;
        private const int T_OP_MUL = 0xD;
        private const int T_OP_BIC = 0xE;
        private const int T_OP_MVN = 0xF;

        private Action[] thumbCodes = null;

        private void ThumbExecute( )
        {
            if ( pipeline.refresh )
            {
                pipeline.refresh = false;
                pipeline.fetch.addr = pc.value & ~1U;
                pipeline.fetch.data = console.PeekHalf( pipeline.fetch.addr );

                ThumbStep( );
            }

            ThumbStep( );

            if ( irqline && cpsr.i == 0 ) // irq after pipeline initialized in correct mode
            {
                Isr( Mode.IRQ, Vector.IRQ );
                lr.value += 2U;
                return;
            }

            code = pipeline.execute.data;

            thumbCodes[ code >> 8 ]( );
        }
        private void ThumbInitialize( )
        {
            this.thumbCodes = new Action[ 256 ]
            {
                TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,    TOpShift,
                TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,    TOpShift,
                TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,   TOpShift,    TOpShift,
                TOpAdjust,  TOpAdjust,  TOpAdjust,  TOpAdjust,  TOpAdjust,  TOpAdjust,  TOpAdjust,   TOpAdjust,
                TOpMovImm,  TOpMovImm,  TOpMovImm,  TOpMovImm,  TOpMovImm,  TOpMovImm,  TOpMovImm,   TOpMovImm,
                TOpCmpImm,  TOpCmpImm,  TOpCmpImm,  TOpCmpImm,  TOpCmpImm,  TOpCmpImm,  TOpCmpImm,   TOpCmpImm,
                TOpAddImm,  TOpAddImm,  TOpAddImm,  TOpAddImm,  TOpAddImm,  TOpAddImm,  TOpAddImm,   TOpAddImm,
                TOpSubImm,  TOpSubImm,  TOpSubImm,  TOpSubImm,  TOpSubImm,  TOpSubImm,  TOpSubImm,   TOpSubImm,
                TOpAlu,     TOpAlu,     TOpAlu,     TOpAlu,     TOpAddHi,   TOpCmpHi,   TOpMovHi,    TOpBx,
                TOpLdrPc,   TOpLdrPc,   TOpLdrPc,   TOpLdrPc,   TOpLdrPc,   TOpLdrPc,   TOpLdrPc,    TOpLdrPc,
                TOpStrReg,  TOpStrReg,  TOpStrhReg, TOpStrhReg, TOpStrbReg, TOpStrbReg, TOpLdrsbReg, TOpLdrsbReg,
                TOpLdrReg,  TOpLdrReg,  TOpLdrhReg, TOpLdrhReg, TOpLdrbReg, TOpLdrbReg, TOpLdrshReg, TOpLdrshReg,
                TOpStrImm,  TOpStrImm,  TOpStrImm,  TOpStrImm,  TOpStrImm,  TOpStrImm,  TOpStrImm,   TOpStrImm,
                TOpLdrImm,  TOpLdrImm,  TOpLdrImm,  TOpLdrImm,  TOpLdrImm,  TOpLdrImm,  TOpLdrImm,   TOpLdrImm,
                TOpStrbImm, TOpStrbImm, TOpStrbImm, TOpStrbImm, TOpStrbImm, TOpStrbImm, TOpStrbImm,  TOpStrbImm,
                TOpLdrbImm, TOpLdrbImm, TOpLdrbImm, TOpLdrbImm, TOpLdrbImm, TOpLdrbImm, TOpLdrbImm,  TOpLdrbImm,
                TOpStrhImm, TOpStrhImm, TOpStrhImm, TOpStrhImm, TOpStrhImm, TOpStrhImm, TOpStrhImm,  TOpStrhImm,
                TOpLdrhImm, TOpLdrhImm, TOpLdrhImm, TOpLdrhImm, TOpLdrhImm, TOpLdrhImm, TOpLdrhImm,  TOpLdrhImm,
                TOpStrSp,   TOpStrSp,   TOpStrSp,   TOpStrSp,   TOpStrSp,   TOpStrSp,   TOpStrSp,    TOpStrSp,
                TOpLdrSp,   TOpLdrSp,   TOpLdrSp,   TOpLdrSp,   TOpLdrSp,   TOpLdrSp,   TOpLdrSp,    TOpLdrSp,
                TOpAddPc,   TOpAddPc,   TOpAddPc,   TOpAddPc,   TOpAddPc,   TOpAddPc,   TOpAddPc,    TOpAddPc, 
                TOpAddSp,   TOpAddSp,   TOpAddSp,   TOpAddSp,   TOpAddSp,   TOpAddSp,   TOpAddSp,    TOpAddSp,
                TOpSubSp,   TOpUnd,     TOpUnd,     TOpUnd,     TOpPush,    TOpPush,    TOpUnd,      TOpUnd,
                TOpUnd,     TOpUnd,     TOpUnd,     TOpUnd,     TOpPop,     TOpPop,     TOpUnd,      TOpUnd,
                TOpStmia,   TOpStmia,   TOpStmia,   TOpStmia,   TOpStmia,   TOpStmia,   TOpStmia,    TOpStmia, 
                TOpLdmia,   TOpLdmia,   TOpLdmia,   TOpLdmia,   TOpLdmia,   TOpLdmia,   TOpLdmia,    TOpLdmia,
                TOpBCond,   TOpBCond,   TOpBCond,   TOpBCond,   TOpBCond,   TOpBCond,   TOpBCond,    TOpBCond,
                TOpBCond,   TOpBCond,   TOpBCond,   TOpBCond,   TOpBCond,   TOpBCond,   TOpUnd,      TOpSwi,
                TOpB,       TOpB,       TOpB,       TOpB,       TOpB,       TOpB,       TOpB,        TOpB,
                TOpUnd,     TOpUnd,     TOpUnd,     TOpUnd,     TOpUnd,     TOpUnd,     TOpUnd,      TOpUnd,
                TOpBl1,     TOpBl1,     TOpBl1,     TOpBl1,     TOpBl1,     TOpBl1,     TOpBl1,      TOpBl1, 
                TOpBl2,     TOpBl2,     TOpBl2,     TOpBl2,     TOpBl2,     TOpBl2,     TOpBl2,      TOpBl2
            };
        }
        private void ThumbStep( )
        {
            pc.value += 2U;

            pipeline.execute = pipeline.decode;
            pipeline.decode = pipeline.fetch;
            pipeline.fetch.addr = pc.value & ~1U;
            pipeline.fetch.data = console.PeekHalf( pipeline.fetch.addr );
        }

        #region Opcodes
        private void TOpShift( )
        {
            // lsl rd, rm, #nn
            var rd = ( code >> 0 ) & 7U;
            var rm = ( code >> 3 ) & 7U;
            var nn = ( code >> 6 ) & 31U;

            switch ( code >> 11 & 3U )
            {
            case 0U: registers[ rd ].value = Mov( Lsl( registers[ rm ].value, nn ) ); break;
            case 1U: registers[ rd ].value = Mov( Lsr( registers[ rm ].value, nn == 0U ? 32U : nn ) ); break;
            case 2U: registers[ rd ].value = Mov( Asr( registers[ rm ].value, nn == 0U ? 32U : nn ) ); break;
            }
        }
        private void TOpAdjust( )
        {
            // add rd, rn, rm
            var rd = ( code >> 0 ) & 7U;
            var rn = ( code >> 3 ) & 7U;
            var rm = ( code >> 6 ) & 7U;

            switch ( code >> 9 & 3U )
            {
            case 0U: registers[ rd ].value = Add( registers[ rn ].value, registers[ rm ].value ); break;
            case 1U: registers[ rd ].value = Sub( registers[ rn ].value, registers[ rm ].value ); break;
            case 2U: registers[ rd ].value = Add( registers[ rn ].value, rm ); break;
            case 3U: registers[ rd ].value = Sub( registers[ rn ].value, rm ); break;
            }
        }
        private void TOpMovImm( )
        {
            // mov rd, #nn
            var rd = ( code >> 8 ) & 7U;
            var nn = ( code >> 0 ) & 255U;
            registers[ rd ].value = Mov( nn );
        }
        private void TOpCmpImm( )
        {
            // cmp rn, #nn
            var rd = ( code >> 8 ) & 7U;
            var nn = ( code >> 0 ) & 255U;
            Sub( registers[ rd ].value, nn );
        }
        private void TOpAddImm( )
        {
            // add rd, #nn
            var rd = ( code >> 8 ) & 7U;
            var nn = ( code >> 0 ) & 255U;
            registers[ rd ].value = Add( registers[ rd ].value, nn );
        }
        private void TOpSubImm( )
        {
            // sub rd, #nn
            var rd = ( code >> 8 ) & 7U;
            var nn = ( code >> 0 ) & 255U;
            registers[ rd ].value = Sub( registers[ rd ].value, nn );
        }
        private void TOpAlu( )
        {
            var rd = ( code >> 0 ) & 7U;
            var rn = ( code >> 3 ) & 7U;

            switch ( ( code >> 6 ) & 15U )
            {
            case T_OP_AND: registers[ rd ].value = Mov( registers[ rd ].value & registers[ rn ].value ); break;
            case T_OP_EOR: registers[ rd ].value = Mov( registers[ rd ].value ^ registers[ rn ].value ); break;
            case T_OP_LSL: registers[ rd ].value = Mov( Lsl( registers[ rd ].value, registers[ rn ].value & 255U ) ); break;
            case T_OP_LSR: registers[ rd ].value = Mov( Lsr( registers[ rd ].value, registers[ rn ].value & 255U ) ); break;
            case T_OP_ASR: registers[ rd ].value = Mov( Asr( registers[ rd ].value, registers[ rn ].value & 255U ) ); break;
            case T_OP_ADC: registers[ rd ].value = Add( registers[ rd ].value, registers[ rn ].value, cpsr.c ); break;
            case T_OP_SBC: registers[ rd ].value = Sub( registers[ rd ].value, registers[ rn ].value, cpsr.c ); break;
            case T_OP_ROR: registers[ rd ].value = Mov( Ror( registers[ rd ].value, registers[ rn ].value & 255U ) ); break;
            case T_OP_TST: Mov( registers[ rd ].value & registers[ rn ].value ); break;
            case T_OP_NEG: registers[ rd ].value = Sub( 0U, registers[ rn ].value ); break;
            case T_OP_CMP: Sub( registers[ rd ].value, registers[ rn ].value ); break;
            case T_OP_CMN: Add( registers[ rd ].value, registers[ rn ].value ); break;
            case T_OP_ORR: registers[ rd ].value = Mov( registers[ rd ].value | registers[ rn ].value ); break;
            case T_OP_MUL: registers[ rd ].value = Mul( 0U, registers[ rd ].value, registers[ rn ].value ); break;
            case T_OP_BIC: registers[ rd ].value = Mov( registers[ rd ].value & ~registers[ rn ].value ); break;
            case T_OP_MVN: registers[ rd ].value = Mov( ~registers[ rn ].value ); break;
            }
        }
        private void TOpAddHi( )
        {
            var rd = ( ( this.code & ( 1 << 7 ) ) >> 4 ) | ( this.code & 0x7 );
            var rm = ( this.code >> 3 ) & 0xF;

            registers[ rd ].value += registers[ rm ].value;

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~1U;
                pipeline.refresh = true;
            }
        }
        private void TOpCmpHi( )
        {
            var rd = ( ( this.code & ( 1 << 7 ) ) >> 4 ) | ( this.code & 0x7 );
            var rm = ( this.code >> 3 ) & 0xF;

            uint alu = registers[ rd ].value - registers[ rm ].value;

            cpsr.n = alu >> 31;
            cpsr.z = alu == 0 ? 1U : 0U;
            this.OverflowCarrySub( registers[ rd ].value, registers[ rm ].value, alu );
        }
        private void TOpMovHi( )
        {
            var rd = ( ( this.code & ( 1 << 7 ) ) >> 4 ) | ( this.code & 0x7 );
            var rm = ( this.code >> 3 ) & 0xF;

            registers[ rd ].value = registers[ rm ].value;

            if ( rd == 15 )
            {
                registers[ rd ].value &= ~1U;
                pipeline.refresh = true;
            }
        }
        private void TOpBx( )
        {
            var rm = ( code >> 3 ) & 15U;
            cpsr.t = registers[ rm ].value & 1U;

            pc.value = registers[ rm ].value & ~1U;
            pipeline.refresh = true;
        }
        private void TOpLdrPc( )
        {
            var rd = ( this.code >> 8 ) & 0x7;

            registers[ rd ].value = this.console.LoadWord( ( pc.value & ~2U ) + ( uint )( ( this.code & 0xFF ) * 4 ) );

            this.Cycles++;
        }
        private void TOpStrReg( )
        {
            this.console.PokeWord( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value, registers[ this.code & 0x7 ].value );
        }
        private void TOpStrhReg( )
        {
            this.console.PokeHalf( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value, ( ushort )( registers[ this.code & 0x7 ].value & 0xFFFF ) );
        }
        private void TOpStrbReg( )
        {
            this.console.PokeByte( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value, ( byte )( registers[ this.code & 0x7 ].value & 0xFF ) );
        }
        private void TOpLdrsbReg( )
        {
            registers[ this.code & 0x7 ].value = this.console.PeekByte( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value );

            if ( ( registers[ this.code & 0x7 ].value & ( 1 << 7 ) ) != 0 )
            {
                registers[ this.code & 0x7 ].value |= 0xFFFFFF00;
            }

            this.Cycles++;
        }
        private void TOpLdrReg( )
        {
            registers[ this.code & 0x7 ].value = this.console.LoadWord( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value );

            this.Cycles++;
        }
        private void TOpLdrhReg( )
        {
            registers[ this.code & 0x7 ].value = this.console.PeekHalf( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value );

            this.Cycles++;
        }
        private void TOpLdrbReg( )
        {
            registers[ this.code & 0x7 ].value = this.console.PeekByte( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value );

            this.Cycles++;
        }
        private void TOpLdrshReg( )
        {
            registers[ this.code & 0x7 ].value = this.console.PeekHalf( registers[ ( this.code >> 3 ) & 0x7 ].value + registers[ ( this.code >> 6 ) & 0x7 ].value );

            if ( ( registers[ this.code & 0x7 ].value & ( 1 << 15 ) ) != 0 )
            {
                registers[ this.code & 0x7 ].value |= 0xFFFF0000;
            }

            this.Cycles++;
        }
        private void TOpStrImm( )
        {
            this.console.PokeWord( registers[ ( this.code >> 3 ) & 0x7 ].value + ( uint )( ( ( this.code >> 6 ) & 0x1F ) * 4 ), registers[ this.code & 0x7 ].value );
        }
        private void TOpLdrImm( )
        {
            registers[ this.code & 0x7 ].value = this.console.LoadWord( registers[ ( this.code >> 3 ) & 0x7 ].value + ( uint )( ( ( this.code >> 6 ) & 0x1F ) * 4 ) );

            this.Cycles++;
        }
        private void TOpStrbImm( )
        {
            this.console.PokeByte( registers[ ( this.code >> 3 ) & 0x7 ].value + ( uint )( ( this.code >> 6 ) & 0x1F ), ( byte )( registers[ this.code & 0x7 ].value & 0xFF ) );
        }
        private void TOpLdrbImm( )
        {
            registers[ this.code & 0x7 ].value = this.console.PeekByte( registers[ ( this.code >> 3 ) & 0x7 ].value + ( uint )( ( this.code >> 6 ) & 0x1F ) );

            this.Cycles++;
        }
        private void TOpStrhImm( )
        {
            this.console.PokeHalf( registers[ ( this.code >> 3 ) & 0x7 ].value + ( uint )( ( ( this.code >> 6 ) & 0x1F ) * 2 ), ( ushort )( registers[ this.code & 0x7 ].value & 0xFFFF ) );
        }
        private void TOpLdrhImm( )
        {
            registers[ this.code & 0x7 ].value = this.console.PeekHalf( registers[ ( this.code >> 3 ) & 0x7 ].value + ( uint )( ( ( this.code >> 6 ) & 0x1F ) * 2 ) );

            this.Cycles++;
        }
        private void TOpStrSp( )
        {
            console.PokeWord( sp.value + ( ( code & 255U ) * 4 ), registers[ ( code >> 8 ) & 7U ].value );
        }
        private void TOpLdrSp( )
        {
            registers[ ( code >> 8 ) & 7U ].value = console.LoadWord( sp.value + ( ( code & 255U ) * 4 ) );
        }
        private void TOpAddPc( )
        {
            registers[ ( code >> 8 ) & 7U ].value = ( pc.value & ~2U ) + ( ( code & 255U ) * 4 );
        }
        private void TOpAddSp( )
        {
            registers[ ( code >> 8 ) & 7U ].value = ( sp.value & ~0U ) + ( ( code & 255U ) * 4 );
        }
        private void TOpSubSp( )
        {
            if ( ( code & ( 1U << 7 ) ) != 0U )
                sp.value -= ( code & 127U ) * 4U;
            else
                sp.value += ( code & 127U ) * 4U;
        }
        private void TOpPush( )
        {
            if ( ( code & 0x100U ) != 0U )
            {
                sp.value -= 4U;
                console.PokeWord( sp.value, lr.value );
            }

            for ( int i = 7; i >= 0; i-- )
            {
                if ( ( ( code >> i ) & 1U ) != 0 )
                {
                    sp.value -= 4U;
                    console.PokeWord( sp.value, registers[ i ].value );
                }
            }
        }
        private void TOpPop( )
        {
            for ( int i = 0; i < 8; i++ )
            {
                if ( ( ( code >> i ) & 1U ) != 0 )
                {
                    registers[ i ].value = console.LoadWord( sp.value );
                    sp.value += 4;
                }
            }

            if ( ( code & 0x100U ) != 0U )
            {
                pc.value = console.LoadWord( sp.value ) & ~1U;
                sp.value += 4U;
                pipeline.refresh = true;
            }

            Cycles++;
        }
        private void TOpStmia( )
        {
            var rn = ( code >> 8 ) & 7U;

            for ( int i = 0; i < 8; i++ )
            {
                if ( ( ( code >> i ) & 1 ) != 0 )
                {
                    console.PokeWord( registers[ rn ].value & ~3U, registers[ i ].value );
                    registers[ rn ].value += 4;
                }
            }
        }
        private void TOpLdmia( )
        {
            var rn = ( code >> 8 ) & 0x7;

            uint address = registers[ rn ].value;

            for ( int i = 0; i < 8; i++ )
            {
                if ( ( ( code >> i ) & 1 ) != 0 )
                {
                    registers[ i ].value = console.PeekWord( address & ~3U );
                    address += 4;
                }
            }

            registers[ rn ].value = address;
        }
        private void TOpBCond( )
        {
            if ( GetCondition( code >> 8 ) )
            {
                pc.value += MathHelper.SignExtend( code & 0xFFU, 8 ) << 1;
                pipeline.refresh = true;
            }
        }
        private void TOpSwi( )
        {
            Isr( Mode.SVC, Vector.SWI );
        }
        private void TOpB( )
        {
            pc.value += MathHelper.SignExtend( code, 11 ) << 1;
            pipeline.refresh = true;
        }
        private void TOpBl1( )
        {
            lr.value = pc.value + ( MathHelper.SignExtend( code, 11 ) << 12 );
        }
        private void TOpBl2( )
        {
            pc.value = lr.value + ( ( code & 0x7FFU ) << 1 );
            lr.value = pipeline.decode.addr | 1U;
            pipeline.refresh = true;
        }
        private void TOpUnd( )
        {
            Isr( Mode.UND, Vector.UND );
        }
        #endregion
    }
}