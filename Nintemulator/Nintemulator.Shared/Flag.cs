using System.Runtime.InteropServices;

namespace Nintemulator.Shared
{
    [StructLayout( LayoutKind.Explicit )]
    public struct Flag
    {
        [FieldOffset( 0 )] public bool b;
        [FieldOffset( 0 )] public  int i;
        [FieldOffset( 0 )] public uint u;
    }
}