using XInput = SlimDX.XInput;

namespace Nintemulator.Shared
{
    public partial class Console<TConsole, TCpu, TPpu, TApu>
    {
        public class Input : Component
        {
            private XInput.Controller controller;
            private XInput.State state;
            private XInput.GamepadButtonFlags[] buttons;

            public Input( TConsole console, int index, int buttons )
                : base( console )
            {
                this.controller = new XInput.Controller( ( XInput.UserIndex )( index ) );
                this.buttons = new XInput.GamepadButtonFlags[ buttons ];
            }

            public void Map( int index, string button )
            {
                switch ( button )
                {
                case "A": buttons[ index ] = XInput.GamepadButtonFlags.A; break;
                case "B": buttons[ index ] = XInput.GamepadButtonFlags.B; break;
                case "X": buttons[ index ] = XInput.GamepadButtonFlags.X; break;
                case "Y": buttons[ index ] = XInput.GamepadButtonFlags.Y; break;

                case "Back": buttons[ index ] = XInput.GamepadButtonFlags.Back; break;
                case "Menu": buttons[ index ] = XInput.GamepadButtonFlags.Start; break;

                case "DPad-U": buttons[ index ] = XInput.GamepadButtonFlags.DPadUp; break;
                case "DPad-D": buttons[ index ] = XInput.GamepadButtonFlags.DPadDown; break;
                case "DPad-L": buttons[ index ] = XInput.GamepadButtonFlags.DPadLeft; break;
                case "DPad-R": buttons[ index ] = XInput.GamepadButtonFlags.DPadRight; break;

                case "L-Shoulder": buttons[ index ] = XInput.GamepadButtonFlags.LeftShoulder; break;
                case "R-Shoulder": buttons[ index ] = XInput.GamepadButtonFlags.RightShoulder; break;
                }
            }

            public bool Pressed( int index )
            {
                return ( state.Gamepad.Buttons & buttons[ index ] ) != 0;
            }

            public virtual void Update( )
            {
                if ( controller.IsConnected )
                {
                    state = controller.GetState( );
                }
            }
        }
    }
}