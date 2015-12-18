using System.Runtime.InteropServices;
using half = System.UInt16;
using word = System.UInt32;

namespace Nintemulator.Shared
{
    [StructLayout(LayoutKind.Explicit)]
    public class MemoryChip
    {
        [field: FieldOffset(0)] public byte[] b;
        [field: FieldOffset(0)] public half[] h;
        [field: FieldOffset(0)] public word[] w;
        [field: FieldOffset(4)] public word mask;
        [field: FieldOffset(8)] public bool writable;

        public MemoryChip(byte[] buffer)
        {
            this.w = null;
            this.h = null;
            this.b = buffer.Clone() as byte[];

            this.writable = false;
            this.mask = (word)(buffer.Length - 1);
        }
        public MemoryChip(word capacity)
        {
            this.w = null;
            this.h = null;
            this.b = new byte[capacity];

            this.writable = true;
            this.mask = capacity - 1u;
        }

        public byte PeekByte(word address) { return b[address >> 0]; }
        public half PeekHalf(word address) { return h[address >> 1]; }
        public uint PeekWord(word address) { return w[address >> 2]; }
        public void PokeByte(word address, byte data) { b[address >> 0] = data; }
        public void PokeHalf(word address, half data) { h[address >> 1] = data; }
        public void PokeWord(word address, word data) { w[address >> 2] = data; }
    }
}