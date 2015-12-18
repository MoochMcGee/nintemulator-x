using System;
using System.IO;

namespace Nintemulator.Shared
{
    public delegate byte Peek(uint address);
    public delegate void Poke(uint address, byte data);

    public partial class Console : IDisposable
    {
        protected AudioRenderer audio;
        protected VideoRenderer video;

        public AudioRenderer Audio { get { return audio; } }
        public VideoRenderer Video { get { return video; } }

        protected virtual void Dispose<T>(ref T component)
            where T : class, IDisposable
        {
            if (component != null)
            {
                component.Dispose();
                component = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            Dispose(ref audio);
            Dispose(ref video);
        }

        public virtual void ResetHard() { }
        public virtual void ResetSoft() { }
        public virtual void StateLoad(BinaryReader reader) { }
        public virtual void StateSave(BinaryWriter writer) { }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
    public partial class Console<TConsole, TCpu, TPpu, TApu> : Console
        where TConsole : Console<TConsole, TCpu, TPpu, TApu>
        where TApu : Console<TConsole, TCpu, TPpu, TApu>.Processor
        where TCpu : Console<TConsole, TCpu, TPpu, TApu>.Processor
        where TPpu : Console<TConsole, TCpu, TPpu, TApu>.Processor
    {
        protected TApu apu;
        protected TCpu cpu;
        protected TPpu ppu;

        public TApu Apu { get { return apu; } }
        public TCpu Cpu { get { return cpu; } }
        public TPpu Ppu { get { return ppu; } }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Dispose(ref cpu);
            Dispose(ref ppu);
            Dispose(ref apu);
        }

        public override void ResetHard()
        {
            cpu.ResetHard();
            ppu.ResetHard();
            apu.ResetHard();
        }
        public override void ResetSoft()
        {
            cpu.ResetSoft();
            ppu.ResetSoft();
            apu.ResetSoft();
        }
    }
}