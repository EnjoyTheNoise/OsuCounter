using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Counter
{
    class InterceptKeys
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x0100;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static readonly LowLevelKeyboardProc Proc = HookCallback;
        private static IntPtr _hookId = IntPtr.Zero;

        private static int _xCount;
        private static int _zCount;


        public static void Main()
        {
            _hookId = SetHook(Proc);
            Application.Run();
            UnhookWindowsHookEx(_hookId);
        }


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())

            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WhKeyboardLl, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }


        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr) WmKeydown)
            {
                var vkCode = (Keys) Marshal.ReadInt32(lParam);

                if (vkCode.Equals(Keys.X))
                    _xCount++;
                if (vkCode.Equals(Keys.Z))
                    _zCount++;

                WriteStats();
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void WriteStats()
        {
            Console.Clear();
            Console.WriteLine("X: " + _xCount + "\n" + "Z: " + _zCount);
        }
    }
}
