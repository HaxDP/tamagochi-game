using System.Runtime.InteropServices;

namespace ConsoleTamagotchi.Presentation.Input;

public sealed class ConsoleMouseInput : IDisposable
{
    private const int StdInputHandle = -10;
    private const uint EnableMouseInput = 0x0010;
    private const uint EnableQuickEditMode = 0x0040;
    private const uint EnableExtendedFlags = 0x0080;
    private const uint LeftButtonPressed = 0x0001;
    private const short MouseEventType = 0x0002;

    private readonly nint _inputHandle;
    private readonly bool _supported;
    private readonly uint _originalMode;

    public ConsoleMouseInput()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        _inputHandle = GetStdHandle(StdInputHandle);
        if (_inputHandle == 0 || _inputHandle == -1)
        {
            return;
        }

        if (!GetConsoleMode(_inputHandle, out _originalMode))
        {
            return;
        }

        var mode = _originalMode | EnableMouseInput | EnableExtendedFlags;
        mode &= ~EnableQuickEditMode;
        SetConsoleMode(_inputHandle, mode);
        _supported = true;
    }

    public IReadOnlyList<(int X, int Y)> ReadLeftClicks()
    {
        if (!_supported)
        {
            return [];
        }

        var clicks = new List<(int X, int Y)>();
        var peekBuffer = new InputRecord[1];

        for (var i = 0; i < 64; i++)
        {
            if (!PeekConsoleInput(_inputHandle, peekBuffer, 1, out var peeked) || peeked == 0)
            {
                break;
            }

            if (peekBuffer[0].EventType != MouseEventType)
            {
                break;
            }

            if (!ReadConsoleInput(_inputHandle, peekBuffer, 1, out var read) || read == 0)
            {
                break;
            }

            var mouse = peekBuffer[0].Event.MouseEvent;
            if (mouse.EventFlags != 0 || (mouse.ButtonState & LeftButtonPressed) == 0)
            {
                continue;
            }

            clicks.Add((mouse.MousePosition.X, mouse.MousePosition.Y));
        }

        return clicks;
    }

    public void Dispose()
    {
        if (_supported)
        {
            SetConsoleMode(_inputHandle, _originalMode);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Coord
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseEventRecord
    {
        public Coord MousePosition;
        public uint ButtonState;
        public uint ControlKeyState;
        public uint EventFlags;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputRecordUnion
    {
        [FieldOffset(0)]
        public MouseEventRecord MouseEvent;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct InputRecord
    {
        public short EventType;
        public InputRecordUnion Event;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetNumberOfConsoleInputEvents(nint hConsoleInput, out uint lpcNumberOfEvents);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool PeekConsoleInput(
        nint hConsoleInput,
        [Out] InputRecord[] lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadConsoleInput(
        nint hConsoleInput,
        [Out] InputRecord[] lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead);
}