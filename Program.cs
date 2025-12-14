using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TaskBarChanger
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
            UpdateTaskbarState(); // 初期状態を設定
            CreateNotifyIcon();
            Application.Run();
        }

        private static void UpdateTaskbarState()
        {
            var isAutoHide = System.Windows.Forms.Screen.AllScreens.Length == 1;
            var actualIsAutoHide = TaskbarHelper.SetAutoHideState(isAutoHide);
            UpdateNotifyIcon(actualIsAutoHide); // アイコンは "表示" の状態に合わせる
        }

        private static void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            UpdateTaskbarState(); // ディスプレイ設定変更時にタスクバーの状態を更新
        }

        static readonly NotifyIcon notifyIcon = new NotifyIcon();
        static Icon showIcon = new Icon("Icon_show.ico");
        static Icon hideIcon = new Icon("Icon_hide.ico");

        private static void CreateNotifyIcon()
        {
            notifyIcon.ContextMenuStrip = ContextMenu();
            notifyIcon.Text = "TaskBarChanger";
            notifyIcon.MouseDoubleClick += NotifyIconOnMouseDoubleClick;
            notifyIcon.Visible = true;
        }

        private static ContextMenuStrip ContextMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("終了", null, (s, e) =>
            {
                Application.Exit();
            });
            return menu;
        }

        private static void NotifyIconOnMouseDoubleClick(object? s, MouseEventArgs e)
        {
            var isAutoHide = TaskbarHelper.ChangeAutoHideState();
            UpdateNotifyIcon(isAutoHide);
        }

        private static void UpdateNotifyIcon(bool isAutoHide)
        {
            notifyIcon.Icon = isAutoHide ? hideIcon : showIcon;
        }
    }
}