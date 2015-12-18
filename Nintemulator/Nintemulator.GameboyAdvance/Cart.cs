using Nintemulator.Shared;
using System.IO;
using half = System.UInt16;
using word = System.UInt32;

namespace Nintemulator.GBA
{
    public class Cart : GameboyAdvance.Component
    {
        private Register32 latch;
        private half[] buffer;
        private word counter;
        private word mask;
        private string filename;

        public int WRamAccess = 4;
        public int[] Access1 = new int[3] { 4, 4, 4 };
        public int[] Access2 = new int[3] { 2, 4, 8 };

        public Cart(GameboyAdvance console, string filename)
            : base(console)
        {
            this.filename = filename;
        }

        private uint Round(uint length)
        {
            length--;
            length |= length >> 1;
            length |= length >> 2;
            length |= length >> 4;
            length |= length >> 8;
            length |= length >> 16;
            length++;

            return length;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var fileInfo = new FileInfo(filename);

            if (fileInfo.Exists)
            {
                var length = Round((uint)fileInfo.Length) >> 1;
                var stream = fileInfo.OpenRead();
                var reader = new BinaryReader(stream);

                buffer = new half[length];

                for (int i = 0; i < fileInfo.Length / 2; i++)
                    buffer[i] = reader.ReadUInt16();

                mask = (length - 1U);
            }
        }

        public byte PeekByte(uint address)
        {
            switch (address & 1U)
            {
            default:
            case 0U: return (byte)(PeekHalf(address));
            case 1U: return (byte)(PeekHalf(address) >> 8);
            }
        }
        public half PeekHalf(uint address)
        {
            var compare = (address >>= 1) & 0xFFFFU;

            if (counter != compare)
            {
                cpu.Cycles += Access1[(address >> 24) & 3U];
                counter = compare;
            }

            cpu.Cycles += Access2[(address >> 24) & 3U];
            counter++;

            return buffer[address & mask];
        }
        public word PeekWord(uint address)
        {
            address = (address & ~3U);

            latch.uw0 = PeekHalf(address);
            latch.uw1 = PeekHalf(address | 2U);

            return latch.ud0;
        }
    }
}