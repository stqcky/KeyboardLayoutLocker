using System;
using System.Runtime.InteropServices;
using System.Timers;

namespace KeyboardLayoutLocker
{
    internal class LayoutLocker
    {
        private IntPtr _lockedLayout;
        private const int _checkInterval = 100;
        // This timer can be replaced by listening to the WM_INPUTLANGCHANGE event, but I couldn't figure out how to do it.
        private Timer _ensureLayoutLockTimer = new Timer(_checkInterval);

        private static readonly UInt32 WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private static readonly IntPtr HWND_BROADCAST = (IntPtr)0xFFFF;

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(UInt32 idThread);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, UInt32 wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        public LayoutLocker()
        {
            _ensureLayoutLockTimer.Elapsed += (object sender, ElapsedEventArgs e) => EnsureLayoutLock();
        }

        public void Lock()
        {
            _lockedLayout = GetCurrentKeyboardLayout();
            _ensureLayoutLockTimer.Start();
        }

        public void Unlock()
        {
            _ensureLayoutLockTimer.Stop();
        }

        private void EnsureLayoutLock()
        {
            if (GetCurrentKeyboardLayout() != _lockedLayout)
                SendMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, _lockedLayout);
        }

        private IntPtr GetCurrentKeyboardLayout()
        {
            return GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
        }
    }
}
