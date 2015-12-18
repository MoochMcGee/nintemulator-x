using System;

namespace Nintemulator.Shared
{
    public partial class Console<TConsole, TCpu, TPpu, TApu>
    {
        public class Component : IDisposable
        {
            protected TConsole console;
            protected TCpu cpu;
            protected TPpu gpu;
            protected TApu spu;

            public Component( TConsole console )
            {
                this.console = console;
            }

            protected virtual void Dispose( bool disposing ) { }

            protected virtual void OnInitialize( ) { }
            protected virtual void OnResetHard( ) { }
            protected virtual void OnResetSoft( ) { }

            public void Dispose( )
            {
                Dispose( true );

                GC.SuppressFinalize( this );
            }
            public void Initialize( )
            {
                this.cpu = console.Cpu;
                this.gpu = console.Ppu;
                this.spu = console.Apu;

                OnInitialize( );
            }
            public void ResetHard( ) { OnResetHard( ); }
            public void ResetSoft( ) { OnResetSoft( ); }
        }
    }
}