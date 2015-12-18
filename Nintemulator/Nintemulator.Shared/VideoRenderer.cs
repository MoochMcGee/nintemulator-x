using SlimDX.Direct3D9;
using System;
using System.Drawing;

namespace Nintemulator.Shared
{
    public class VideoRenderer : IDisposable
    {
        private Params parameters;
        private Device device;
        private Direct3D direct3D = new Direct3D( );
        private IntPtr handle;
        private Sprite sprite;
        private Texture buffer;
        private Texture screen;

        public VideoRenderer( IntPtr handle, Params parameters )
        {
            this.handle = handle;
            this.parameters = parameters;
        }

        private void CopyColors( int[][] colors )
        {
            var data = buffer.LockRectangle( 0, LockFlags.DoNotWait ).Data;

            for ( int i = 0; i < parameters.SizeY; i++ )
                data.WriteRange( colors[ i ], 0, parameters.SizeX );

            buffer.UnlockRectangle( 0 );
        }

        protected virtual void Dispose( bool disposing )
        {
            device.Dispose( );
            device = null;

            sprite.Dispose( );
            sprite = null;

            buffer.Dispose( );
            buffer = null;

            screen.Dispose( );
            screen = null;

            direct3D.Dispose( );
            direct3D = null;
        }

        public void Dispose( )
        {
            Dispose( true );

            GC.SuppressFinalize( this );
        }
        public void Initialize( )
        {
            var presentParams = new PresentParameters( );
            presentParams.BackBufferCount = 1;
            presentParams.BackBufferWidth = parameters.SizeX;
            presentParams.BackBufferHeight = parameters.SizeY;
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.Multisample = MultisampleType.None;
            presentParams.PresentationInterval = PresentInterval.One;

            device = new Device( direct3D, 0, DeviceType.Hardware, handle, CreateFlags.SoftwareVertexProcessing, presentParams );
            sprite = new Sprite( device );
            buffer = new Texture( device, parameters.SizeX, parameters.SizeY, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.SystemMemory );
            screen = new Texture( device, parameters.SizeX, parameters.SizeY, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default );
        }
        public void Render( int[][] colors )
        {
            CopyColors( colors );

            device.UpdateTexture( buffer, screen );
            device.BeginScene( );

            sprite.Begin( SpriteFlags.None );
            sprite.Draw( screen, Color.White );
            sprite.End( );

            device.EndScene( );
            device.Present( );
        }

        public class Params
        {
            public int SizeX;
            public int SizeY;

            public Params( int x, int y )
            {
                if ( x == 0 ) throw new ArgumentException( "Parameter 'x' must be non-zero." );
                if ( y == 0 ) throw new ArgumentException( "Parameter 'y' must be non-zero." );

                this.SizeX = x;
                this.SizeY = y;
            }
        }
    }
}