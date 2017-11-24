using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

class InterceptKeys

{
    private const int WH_KEYBOARD_LL = 13;

    private const int WM_KEYDOWN = 0x0100;

    private static readonly LowLevelKeyboardProc _proc = HookCallback;

    private static IntPtr _hookID = IntPtr.Zero;

    private static int _xCount = 0;
    private static int _zCount = 0;


    public static void Main()

    {
        _hookID = SetHook(_proc);

        Application.Run();

        UnhookWindowsHookEx(_hookID);
    }


    private static IntPtr SetHook(LowLevelKeyboardProc proc)

    {
        using (Process curProcess = Process.GetCurrentProcess())

        using (ProcessModule curModule = curProcess.MainModule)

        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }


    private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);


    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)

    {
        if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
        {
            Keys vkCode = (Keys) Marshal.ReadInt32(lParam);

            if (vkCode.Equals(Keys.X))
                AddX();
            if (vkCode.Equals(Keys.Z))
                AddZ();

            WriteStats();
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static void WriteStats()
    {
        Console.Clear();
        Console.WriteLine("X: " + _xCount + "\n" + "Z: " + _zCount);
    }

    private static void AddZ()
    {
        _zCount++;
    }

    private static void AddX()
    {
        _xCount++;
    }


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);


    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
