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
            CreateNotifyIcon();
            Application.Run();
        }

        static readonly NotifyIcon notifyIcon = new NotifyIcon();
        static Icon showIcon = new Icon("Icon_show.ico");
        static Icon hideIcon = new Icon("Icon_hide.ico");

        private static void CreateNotifyIcon()
        {
            notifyIcon.Icon = TaskbarHelper.IsNotAutoHide() ? showIcon : hideIcon;
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
            var isShow = TaskbarHelper.ChangeAutoHideState();
            notifyIcon.Icon = isShow ? showIcon : hideIcon;
        }

    }
}