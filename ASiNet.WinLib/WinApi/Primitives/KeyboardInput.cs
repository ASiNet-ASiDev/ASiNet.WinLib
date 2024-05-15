using System.Runtime.InteropServices;
using ASiNet.WinLib.Enums;

namespace ASiNet.WinLib.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct KeyboardInput
{
    public ushort wVk;
    public ushort wScan;
    public KeyEventFlag dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}
