using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardLayoutLocker
{
    public partial class KeyboardLayoutLocker : Form
    {
        private NotifyIcon _notifyIcon;
        private bool _locked = false;
        private LayoutLocker _layoutLocker = new LayoutLocker();

        public KeyboardLayoutLocker()
        {
            InitializeComponent();
            InitializeTrayIcon();
            HideForm();
        }

        private void HideForm()
        {
            this.Visible = false;
            this.Hide();
        }

        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon();

            MenuItem toggleMenuItem = new MenuItem("Enable", new EventHandler(ToggleLayoutLocker));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            _notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
            {
                toggleMenuItem,
                exitMenuItem,
            });

            _notifyIcon.Icon = Properties.Resources.Unlocked;
            _notifyIcon.Text = "Keyboard Layout Locker";
            _notifyIcon.Visible = true;
        }

        private void ToggleLayoutLocker(object sender, EventArgs e)
        {
            _locked = !_locked;
            _notifyIcon.ContextMenu.MenuItems[0].Text = _locked ? "Disable" : "Enable";
            _notifyIcon.Icon = _locked ? Properties.Resources.Locked : Properties.Resources.Unlocked;

            if (_locked)
                _layoutLocker.Lock();
            else
                _layoutLocker.Unlock();
        }

        private void Exit(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
