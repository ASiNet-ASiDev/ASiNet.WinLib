using System.Runtime.InteropServices;
using ASiNet.WinLib.Enums;

namespace ASiNet.WinLib.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
    public int dx;
    public int dy;
    public int mouseData;
    public MouseEventFlag dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
}