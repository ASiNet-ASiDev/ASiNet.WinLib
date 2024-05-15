using System.Runtime.InteropServices;
using ASiNet.WCP.WinApi.Primitives;

namespace ASiNet.WinLib.WinApi.Primitives;
[StructLayout(LayoutKind.Explicit)]
public struct InputUnion
{
    [FieldOffset(0)] public MouseInput mi;
    [FieldOffset(0)] public KeyboardInput ki;
    [FieldOffset(0)] public HardwareInput hi;
}