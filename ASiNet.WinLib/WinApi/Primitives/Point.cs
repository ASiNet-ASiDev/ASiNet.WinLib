using System.Runtime.InteropServices;

namespace ASiNet.WinLib.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X;
    public int Y;
}