#define LOROM

using Nintemulator.Shared;
using System.IO;
using word = System.UInt16;

namespace Nintemulator.SFC.CPU
{
    public partial class Cpu
    {
        public const int SRAM_SIZE = 1 << 13;
        public const int SRAM_MASK = SRAM_SIZE - 1;

        public const int SPEED_FAST = 6;
        public const int SPEED_NORM = 8;
        public const int SPEED_SLOW = 12;

        private Peek[] peek = new Peek[1 << 16];
        private Poke[] poke = new Poke[1 << 16];

        internal Register24 wram_addr;
        internal byte open;
        internal byte[] cart;
        internal byte[] sram = new byte[SRAM_SIZE];
        internal byte[] wram = new byte[1 << 17];
        internal uint cart_mask;
        internal int cart_time = SPEED_NORM;

        private int Speed(uint addr)
        {
            if ((addr & 0x408000) != 0)
            {
                if ((addr & 0x800000) != 0) return cart_time;

                return SPEED_NORM;
            }

            if (((addr + 0x6000) & 0x4000) != 0) return SPEED_NORM;
            if (((addr - 0x4000) & 0x7E00) != 0) return SPEED_FAST;

            return SPEED_SLOW;
        }

        private byte PeekCart(uint address)
        {
#if LOROM
            address = (((address & 0x7f0000u) >> 1) | (address & 0x007fffu)) & cart_mask;
#else
            address = (address & cart_mask);
#endif
            return cart[address];
        }
        private byte PeekOpen(uint address) { return open; }
        private byte PeekSRam(uint address) { return sram[address & SRAM_MASK]; }
        private byte PeekWRam(uint address) { return wram[address & 0x01ffffu]; }
        private void PokeCart(uint address, byte data) { }
        private void PokeOpen(uint address, byte data) { open = data; }
        private void PokeSRam(uint address, byte data) { sram[address & SRAM_MASK] = data; }
        private void PokeWRam(uint address, byte data) { wram[address & 0x01ffffu] = data; }

        public byte PeekBusA(uint address)
        {
            if ((address & 0xfe0000u) == 0x7e0000u) { return PeekWRam(address); }
            if ((address & 0x708000u) == 0x700000u) { return PeekSRam(address); }
            if ((address & 0x408000u) != 0x000000u) { return PeekCart(address); }

            return peek[address &= 0xffffu](address);
        }
        public byte PeekBusB(uint address)
        {
            if ((address & 0xffc0) == 0x2140)
            {
                // S-SMP Registers
                return spu.ReadPort((int)address & 3, 0);
            }

            switch (address & 0x00ffu)
            {
            // S-PPU Registers
            case 0x34U: return gpu.Peek2134(address);
            case 0x35U: return gpu.Peek2135(address);
            case 0x36U: return gpu.Peek2136(address);
            case 0x37U: return gpu.Peek2137(address);
            case 0x38U: return gpu.Peek2138(address);
            case 0x39U: return gpu.Peek2139(address);
            case 0x3AU: return gpu.Peek213A(address);
            case 0x3BU: return gpu.Peek213B(address);
            case 0x3CU: return gpu.Peek213C(address);
            case 0x3DU: return gpu.Peek213D(address);
            case 0x3EU: return gpu.Peek213E(address);
            case 0x3FU: return gpu.Peek213F(address);

            // W-RAM Registers
            case 0x80U: return wram[wram_addr.d++ & 0x1ffffu];
            case 0x81U: return open;
            case 0x82U: return open;
            case 0x83U: return open;
            }

            return open;
        }
        public byte PeekByte(uint address)
        {
            cpu.Cycles += Speed(address);

            if ((address & 0x40ff00u) == 0x002100u)
            {
                return open = PeekBusB(address);
            }
            else
            {
                return open = PeekBusA(address);
            }
        }
        public void PokeBusA(uint address, byte data)
        {
            if ((address & 0xfe0000u) == 0x7e0000u) { PokeWRam(address, data); return; }
            if ((address & 0x708000u) == 0x700000u) { PokeSRam(address, data); return; }
            if ((address & 0x408000u) != 0x000000u) { PokeCart(address, data); return; }

            poke[address &= 0xffffu](address, data);
        }
        public void PokeBusB(uint address, byte data)
        {
            if ((address & 0xfffc) == 0x2140)
            {
                // S-SMP Registers
                spu.WritePort((int)address & 3, data, 0);
                return;
            }

            switch (address & 0x00ffu)
            {
            // S-PPU Registers
            case 0x00U: gpu.Poke2100(address, data); break;
            case 0x01U: gpu.Poke2101(address, data); break;
            case 0x02U: gpu.Poke2102(address, data); break;
            case 0x03U: gpu.Poke2103(address, data); break;
            case 0x04U: gpu.Poke2104(address, data); break;
            case 0x05U: gpu.Poke2105(address, data); break;
            case 0x06U: gpu.Poke2106(address, data); break;
            case 0x07U: gpu.Poke2107(address, data); break;
            case 0x08U: gpu.Poke2108(address, data); break;
            case 0x09U: gpu.Poke2109(address, data); break;
            case 0x0AU: gpu.Poke210A(address, data); break;
            case 0x0BU: gpu.Poke210B(address, data); break;
            case 0x0CU: gpu.Poke210C(address, data); break;
            case 0x0DU: gpu.Poke210D(address, data); break;
            case 0x0EU: gpu.Poke210E(address, data); break;
            case 0x0FU: gpu.Poke210F(address, data); break;
            case 0x10U: gpu.Poke2110(address, data); break;
            case 0x11U: gpu.Poke2111(address, data); break;
            case 0x12U: gpu.Poke2112(address, data); break;
            case 0x13U: gpu.Poke2113(address, data); break;
            case 0x14U: gpu.Poke2114(address, data); break;
            case 0x15U: gpu.Poke2115(address, data); break;
            case 0x16U: gpu.Poke2116(address, data); break;
            case 0x17U: gpu.Poke2117(address, data); break;
            case 0x18U: gpu.Poke2118(address, data); break;
            case 0x19U: gpu.Poke2119(address, data); break;
            case 0x1AU: gpu.Poke211A(address, data); break;
            case 0x1BU: gpu.Poke211B(address, data); break;
            case 0x1CU: gpu.Poke211C(address, data); break;
            case 0x1DU: gpu.Poke211D(address, data); break;
            case 0x1EU: gpu.Poke211E(address, data); break;
            case 0x1FU: gpu.Poke211F(address, data); break;
            case 0x20U: gpu.Poke2120(address, data); break;
            case 0x21U: gpu.Poke2121(address, data); break;
            case 0x22U: gpu.Poke2122(address, data); break;
            case 0x23U: gpu.Poke2123(address, data); break;
            case 0x24U: gpu.Poke2124(address, data); break;
            case 0x25U: gpu.Poke2125(address, data); break;
            case 0x26U: gpu.Poke2126(address, data); break;
            case 0x27U: gpu.Poke2127(address, data); break;
            case 0x28U: gpu.Poke2128(address, data); break;
            case 0x29U: gpu.Poke2129(address, data); break;
            case 0x2AU: gpu.Poke212A(address, data); break;
            case 0x2BU: gpu.Poke212B(address, data); break;
            case 0x2CU: gpu.Poke212C(address, data); break;
            case 0x2DU: gpu.Poke212D(address, data); break;
            case 0x2EU: gpu.Poke212E(address, data); break;
            case 0x2FU: gpu.Poke212F(address, data); break;
            case 0x30U: gpu.Poke2130(address, data); break;
            case 0x31U: gpu.Poke2131(address, data); break;
            case 0x32U: gpu.Poke2132(address, data); break;
            case 0x33U: gpu.Poke2133(address, data); break;

            // W-RAM Registers
            case 0x80U: wram[wram_addr.d++ & 0x1ffffu] = data; break;
            case 0x81U: wram_addr.l = data; break;
            case 0x82U: wram_addr.h = data; break;
            case 0x83U: wram_addr.b = data; break;
            }
        }
        public void PokeByte(uint address, byte data)
        {
            cpu.Cycles += Speed(address);

            if ((address & 0x40ff00u) == 0x002100u)
            {
                PokeBusB(address, open = data);
            }
            else
            {
                PokeBusA(address, open = data);
            }
        }

        public void Hook(uint addr, Peek peek)
        {
            this.peek[addr] = peek;
        }
        public void Hook(uint addr, Poke poke)
        {
            this.poke[addr] = poke;
        }
        public void Hook(uint addr, Peek peek, Poke poke)
        {
            Hook(addr, peek);
            Hook(addr, poke);
        }
        public void Hook(uint addr, uint last, Peek peek)
        {
            for (; addr <= last; addr++)
                Hook(addr, peek);
        }
        public void Hook(uint addr, uint last, Poke poke)
        {
            for (; addr <= last; addr++)
                Hook(addr, poke);
        }
        public void Hook(uint addr, uint last, Peek peek, Poke poke)
        {
            for (; addr <= last; addr++)
                Hook(addr, peek, poke);
        }

        public void LoadCart(string filename)
        {
            cart = File.ReadAllBytes(filename);

            cart_mask = (uint)cart.Length;
            cart_mask--;

            cart_mask |= cart_mask >> 1;
            cart_mask |= cart_mask >> 2;
            cart_mask |= cart_mask >> 4;
            cart_mask |= cart_mask >> 8;
            cart_mask |= cart_mask >> 16;
        }

        private byte operand_byte()
        {
            return PeekByte(ea.d);
        }
        private word operand_word()
        {
            var l = PeekByte(ea.d + 0u);
            var h = PeekByte(ea.d + 1u);

            return (word)((h << 8) | l);
        }
        private void operand_byte(byte value)
        {
            PokeByte(ea.d, value);
        }
        private void operand_word(word value)
        {
            PokeByte(ea.d + 0u, (byte)(value >> 0));
            PokeByte(ea.d + 1u, (byte)(value >> 8));
        }
    }
}