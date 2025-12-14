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
            menu.Items.Add("�I��", null, (s, e) =>
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


    internal static class TaskbarHelper {
        private const int TrayAbsAutoHide = 1;

        private const int TrayAbsAlwaysOnTop = 2;

        private const uint TrayAbmSetState = 10;

        private const uint TrayAbmGetState = 4;
        private static AppbarData _msg;


        public static bool IsNotAutoHide() => SHAppBarMessage(TrayAbmGetState, ref _msg) == IntPtr.Zero;

        public static bool ChangeAutoHideState()
        {
            _msg.lParam = IsNotAutoHide() ? TrayAbsAutoHide : TrayAbsAlwaysOnTop;
            _ = SHAppBarMessage(TrayAbmSetState, ref _msg);
            return _msg.lParam == TrayAbsAlwaysOnTop;
        }

        /// <summary>
        ///     This function returns a message-dependent value.
        /// </summary>
        /// <param name="dwMessage"></param>
        /// <param name="pData"></param>
        /// <returns></returns>
        [DllImport("shell32.dll",
                   EntryPoint = "SHAppBarMessage",
                   CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SHAppBarMessage(uint dwMessage, ref AppbarData pData);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AppbarData
    {
        public uint cbSize;

        public IntPtr hWnd;

        public uint uCallbackMessage;

        public uint uEdge;

        public TagRect rc;

        public int lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TagRect
    {
        public int left;

        public int top;

        public int right;

        public int bottom;

        public static bool operator ==(TagRect left, TagRect right)
            => left.left == right.left
               && left.top == right.top
               && left.right == right.right
               && left.bottom == right.bottom;

        public static bool operator !=(TagRect left, TagRect right)
            => !(left == right);

        public bool Equals(TagRect other)
            => left == other.left
               && top == other.top
               && right == other.right
               && bottom == other.bottom;

        public override bool Equals(object obj)
            => obj is TagRect other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = left;
                hashCode = hashCode * 397 ^ top;
                hashCode = hashCode * 397 ^ right;
                hashCode = hashCode * 397 ^ bottom;
                return hashCode;
            }
        }
    }

}