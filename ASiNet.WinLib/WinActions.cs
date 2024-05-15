using ASiNet.WinLib.Enums;
using ASiNet.WinLib.WinApi.Primitives;
using System.Runtime.InteropServices;
using System.Text;

namespace ASiNet.WinLib;

/// <summary>
/// All WinAPI functions implemented in this library,
/// </summary>
public static class WinActions
{
    private static int _inputSize = Marshal.SizeOf<Input>();
    #region KEYBOARD LAYOUT

    private const uint KLF_ACTIVATE = 0x00000001;
    private const int WM_INPUTLANGCHANGEREQUEST = 0x50;
    private const nint HWND_BROADCAST = 0xffff;

    /// <summary>
    /// Load keyboard layout 
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-loadkeyboardlayouta">See more info.</a> and
    /// <a href="https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/70feba9f-294e-491e-b6eb-56532684c37f">Windows Language Code Identifier</a>
    /// </summary>
    public static uint KeyboardLayoutLoad(int languageCode)
    {
        var lc = languageCode.ToString("x8");
        var pwsz = new StringBuilder(lc);
        var result = Functions.LoadKeyboardLayout(pwsz, 0);
        return result;
    }
    /// <summary>
    /// Load keyboard layout 
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-loadkeyboardlayouta">See more info.</a> and
    /// <a href="https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/70feba9f-294e-491e-b6eb-56532684c37f">Windows Language Code Identifier</a>
    /// </summary>
    public static uint KeyboardLayoutLoad(string languageCode)
    {
        var pwsz = new StringBuilder(languageCode);
        var result = Functions.LoadKeyboardLayout(pwsz, 0);
        return result;
    }

    /// <summary>
    /// Activate Keyboard Layout
    /// <a href="https://learn.microsoft.com/ru-ru/windows/win32/api/winuser/nf-winuser-activatekeyboardlayout">See more info</a> and
    /// <a href="https://learn.microsoft.com/ru-ru/windows/win32/api/winuser/nf-winuser-postmessagea">PostMessageA</a>
    /// </summary>
    /// <param name="hkl"></param>
    /// <returns></returns>
    public static bool KeyboardLayoutSet(uint hkl)
    {
        Functions.ActivateKeyboardLayout(hkl, 0);
        return Functions.PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, (int)KLF_ACTIVATE, (int)hkl);
    }

    /// <summary>
    /// Get Keyboard Layout 
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeyboardlayout">See more info</a>
    /// </summary>
    /// <param name="hkl"></param>
    /// <returns></returns>
    public static uint KeyboardLayoutGet(uint threadId = 0) =>
        Functions.GetKeyboardLayout(threadId);

    #endregion
    #region CLIPBOARD
    public static bool ClipboardSetUnicodeText(string text)
    {
        try
        {
            if (Functions.OpenClipboard(IntPtr.Zero))
            {
                if (!Functions.OpenClipboard(IntPtr.Zero))
                {

                    return false;
                }
                Functions.EmptyClipboard();
                Functions.SetClipboardData((int)ClipboardDataType.UnicodeText, Marshal.StringToHGlobalUni(text));
                Functions.CloseClipboard();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
    #region MOUSE INPUT
    /// <summary>
    /// Send mouse event: move, wheel, mouse button
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    public static uint SendMouseEvent(int x, int y, short mouseWheel, MouseButtonEvent buttonEvent, bool absolutePos = false, bool virtualDesk = false)
    {
        var (i1, isI2) = MouseInput1(x, y, mouseWheel, buttonEvent, absolutePos, virtualDesk);

        if (!isI2)
            return Functions.SendInput(1, [i1], _inputSize);
        var i2 = MouseInput2(buttonEvent);
        return Functions.SendInput(2, [i1, i2], _inputSize);
    }
    /// <summary>
    /// Send mouse event: move
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="absolutePos"></param>
    /// <param name="virtualDesk"></param>
    /// <returns></returns>
    public static uint SendMouseEvent(int x, int y, bool absolutePos = false, bool virtualDesk = false)
    {
        var i1 = MouseInput1(x, y, absolutePos, virtualDesk);
        return Functions.SendInput(1, [i1], _inputSize);
    }
    /// <summary>
    /// Send mouse event: wheel
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    /// <param name="mouseWheel"></param>
    /// <returns></returns>
    public static uint SendMouseEvent(short mouseWheel)
    {
        var i1 = MouseInput1(mouseWheel);
        return Functions.SendInput(1, [i1], _inputSize);
    }
    /// <summary>
    /// Send mouse event: mouse buttons
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    /// <param name="buttonEvent"></param>
    /// <returns></returns>
    public static uint SendMouseEvent(MouseButtonEvent buttonEvent)
    {
        var (i1, isI2) = MouseInput1(buttonEvent);

        if (!isI2)
            return Functions.SendInput(1, [i1], _inputSize);
        var i2 = MouseInput2(buttonEvent);
        return Functions.SendInput(2, [i1, i2], _inputSize);
    }
    /// <summary>
    /// Send mouse event: move, wheel and mouse buttons
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    public static uint SendMouseEvent(int x, int y, short mouseWheel, MouseEventFlag flags)
    {
        var i1 = MouseInput1(x, y, mouseWheel, flags);

        return Functions.SendInput(1, [i1], _inputSize);
    }

    private static Input MouseInput1(int x, int y, short mouseWheel, MouseEventFlag flags)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();

        mouseInput.dwFlags = flags;
        mouseInput.dx = x;
        mouseInput.dy = y;
        mouseInput.mouseData = mouseWheel;

        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return input;
    }

    private static (Input, bool GenerateSecond) MouseInput1(MouseButtonEvent buttons)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();
        var generateSecond = false;

        if (buttons != MouseButtonEvent.None)
        {
            generateSecond = buttons is MouseButtonEvent.LeftClick or MouseButtonEvent.RightClick or MouseButtonEvent.MiddleClick or MouseButtonEvent.XButtonClick;
            mouseInput.dwFlags |= MouseButtonEventConvert(buttons);
        }
        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return (input, generateSecond);
    }

    private static Input MouseInput1(short mouseWheel)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();


        mouseInput.dwFlags |= MouseEventFlag.Wheel;
        mouseInput.mouseData = mouseWheel;

        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return input;
    }

    private static Input MouseInput1(int x, int y, bool absolutePos, bool virtualDesk)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();

        mouseInput.dwFlags |= MouseEventFlag.Move;
        if (absolutePos)
            mouseInput.dwFlags |= MouseEventFlag.Absolute;
        if(virtualDesk)
            mouseInput.dwFlags |= MouseEventFlag.VirtualDesk;
        mouseInput.dx = x;
        mouseInput.dy = y;

        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return input;
    }

    private static (Input, bool GenerateSecond) MouseInput1(int x, int y, short mouseWheel, MouseButtonEvent buttons, bool absolutePos, bool virtualDesk)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();
        var generateSecond = false;

        mouseInput.dwFlags |= MouseEventFlag.Move;
        if(absolutePos)
            mouseInput.dwFlags |= MouseEventFlag.Absolute;
        if (virtualDesk)
            mouseInput.dwFlags |= MouseEventFlag.VirtualDesk;
        mouseInput.dx = x;
        mouseInput.dy = y;
        if (mouseWheel is not 0)
        {
            mouseInput.dwFlags |= MouseEventFlag.Wheel;
            mouseInput.mouseData = mouseWheel;
        }
        if (buttons != MouseButtonEvent.None)
        {
            generateSecond = buttons is MouseButtonEvent.LeftClick or MouseButtonEvent.RightClick or MouseButtonEvent.MiddleClick or MouseButtonEvent.XButtonClick;
            mouseInput.dwFlags |= MouseButtonEventConvert(buttons);
        }
        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return (input, generateSecond);
    }

    private static Input MouseInput2(MouseButtonEvent buttons)
    {
        var input = new Input
        {
            type = InputType.Mouse
        };
        var mouseInput = new MouseInput();

        mouseInput.dwFlags = buttons switch
        {
            MouseButtonEvent.LeftClick => MouseEventFlag.LeftUp,
            MouseButtonEvent.RightClick => MouseEventFlag.RightUp,
            MouseButtonEvent.MiddleClick => MouseEventFlag.MiddleUp,
            MouseButtonEvent.XButtonClick => MouseEventFlag.XUp,
            _ => 0,
        };

        mouseInput.dwExtraInfo = Functions.GetMessageExtraInfo();
        input.u = new() { mi = mouseInput };
        return input;
    }

    private static MouseEventFlag MouseButtonEventConvert(MouseButtonEvent mouseButtons) => mouseButtons switch
    {
        MouseButtonEvent.LeftClick => MouseEventFlag.LeftDown,
        MouseButtonEvent.RightClick => MouseEventFlag.RightDown,
        MouseButtonEvent.MiddleClick => MouseEventFlag.MiddleDown,
        MouseButtonEvent.XButtonClick => MouseEventFlag.XDown,
        MouseButtonEvent.LeftDown => MouseEventFlag.LeftDown,
        MouseButtonEvent.RightDown => MouseEventFlag.RightDown,
        MouseButtonEvent.MiddleDown => MouseEventFlag.MiddleDown,
        MouseButtonEvent.XButtonDown => MouseEventFlag.XDown,
        MouseButtonEvent.LeftUp => MouseEventFlag.LeftUp,
        MouseButtonEvent.RightUp => MouseEventFlag.RightUp,
        MouseButtonEvent.MiddleUp => MouseEventFlag.MiddleUp,
        MouseButtonEvent.XButtonUp => MouseEventFlag.XUp,
        _ => 0
    };

    #endregion
    #region KEYBOARD INPUT

    /// <summary>
    /// Send keyboard input
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    /// <param name="virtualKey"></param>
    /// <param name="keyState"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static uint SendKeyboardEvent(VirtualKeyCode virtualKey, KeyState keyState)
    {
        Input[] inputs = keyState switch
        {
            KeyState.Click => [
                    NewKeyboardInput(virtualKey, KeyEventFlag.KeyDown),
                    NewKeyboardInput(virtualKey, KeyEventFlag.KeyUp),
                ],
            KeyState.Down => [
                    NewKeyboardInput(virtualKey, KeyEventFlag.KeyDown),
                ],
            KeyState.Up => [
                    NewKeyboardInput(virtualKey, KeyEventFlag.KeyUp),
                ],
            _ => throw new NotImplementedException()
        };

        return Functions.SendInput((uint)inputs.Length, inputs, _inputSize);
    }
    /// <summary>
    /// Send keyboard input
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    /// <param name="unicode"></param>
    /// <param name="keyState"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static uint SendKeyboardEvent(char unicode, KeyState keyState)
    {
        Input[] inputs = keyState switch
        {
            KeyState.Click => [
                    NewKeyboardInput(unicode, KeyEventFlag.KeyDown),
                    NewKeyboardInput(unicode, KeyEventFlag.KeyUp),
                ],
            KeyState.Down => [
                    NewKeyboardInput(unicode, KeyEventFlag.KeyDown),
                ],
            KeyState.Up => [
                    NewKeyboardInput(unicode, KeyEventFlag.KeyUp),
                ],
            _ => throw new NotImplementedException()
        };

        return Functions.SendInput((uint)inputs.Length, inputs, _inputSize);
    }
    /// <summary>
    /// Send keyboard input
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput">See more info.</a>
    /// </summary>
    /// <param name="scanCode"></param>
    /// <param name="keyState"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static uint SendKeyboardEvent(ushort scanCode, KeyState keyState)
    {
        Input[] inputs = keyState switch
        {
            KeyState.Click => [
                    NewKeyboardInput(scanCode, KeyEventFlag.KeyDown),
                    NewKeyboardInput(scanCode, KeyEventFlag.KeyUp),
                ],
            KeyState.Down => [
                    NewKeyboardInput(scanCode, KeyEventFlag.KeyDown),
                ],
            KeyState.Up => [
                    NewKeyboardInput(scanCode, KeyEventFlag.KeyUp),
                ],
            _ => throw new NotImplementedException()
        };

        return Functions.SendInput((uint)inputs.Length, inputs, _inputSize);
    }

    private static Input NewKeyboardInput(VirtualKeyCode code, KeyEventFlag flag) =>
    new()
    {
        type = InputType.Keyboard,
        u = new InputUnion()
        {
            ki = new()
            {
                wVk = (ushort)code,
                wScan = 0,
                dwFlags = flag,
                dwExtraInfo = Functions.GetMessageExtraInfo(),
            }
        }
    };

    private static Input NewKeyboardInput(char unicode, KeyEventFlag flag) =>
        new()
        {
            type = InputType.Keyboard,
            u = new InputUnion()
            {
                ki = new()
                {
                    wVk = unicode,
                    wScan = 0,
                    dwFlags = flag | KeyEventFlag.Unicode,
                    dwExtraInfo = Functions.GetMessageExtraInfo(),
                }
            }
        };

    private static Input NewKeyboardInput(ushort scanCode, KeyEventFlag flag) =>
        new()
        {
            type = InputType.Keyboard,
            u = new InputUnion()
            {
                ki = new()
                {
                    wVk = scanCode,
                    wScan = 0,
                    dwFlags = flag | KeyEventFlag.Scancode,
                    dwExtraInfo = Functions.GetMessageExtraInfo(),
                }
            }
        };
    #endregion
    #region MAP KEY CODES
    /// <summary>
    /// Map scan code to virtual code
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya">See more info.</a>
    /// </summary>
    public static VirtualKeyCode MapScanCodeToVirtualCode(ushort scanCode) =>
        (VirtualKeyCode)Functions.MapVirtualKeyA(scanCode, MapType.ScanCodeToVirtualKey);
    /// <summary>
    /// Map virtual code to scan code
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya">See more info.</a>
    /// </summary>
    public static ushort MapVirtualCodeToScanCode(VirtualKeyCode scanCode) =>
        Functions.MapVirtualKeyA((ushort)scanCode, MapType.VirtualCodeToScanCode);
    /// <summary>
    /// Map scan code to virtual code
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya">See more info.</a>
    /// </summary>
    public static VirtualKeyCode MapScanCodeToVirtualCodeEx(ushort scanCode) =>
        (VirtualKeyCode)Functions.MapVirtualKeyA(scanCode, MapType.ScanCodeToVirtualKeyEx);
    /// <summary>
    /// Map virtual code to scan code
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya">See more info.</a>
    /// </summary>
    public static ushort MapVirtualCodeToScanCodeEx(VirtualKeyCode scanCode) =>
        Functions.MapVirtualKeyA((ushort)scanCode, MapType.VirtualKeyToScanCodeEx);
    /// <summary>
    /// Map virtual code to char
    /// <a href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mapvirtualkeya">See more info.</a>
    /// </summary>
    public static char MapVirtualCodeToChar(VirtualKeyCode scanCode) =>
        (char)Functions.MapVirtualKeyA((ushort)scanCode, MapType.VirtualKeyToChar);
    #endregion
}
