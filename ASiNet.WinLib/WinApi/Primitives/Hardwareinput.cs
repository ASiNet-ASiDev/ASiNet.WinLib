using System.Runtime.InteropServices;

namespace ASiNet.WinLib.WinApi.Primitives;
[StructLayout(LayoutKind.Sequential)]
public struct HardwareInput
{
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
}