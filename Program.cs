using Microsoft.Win32;

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
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            UpdateIcons(); // アイコンを初期化
            UpdateTaskbarState(); // 初期状態を設定
            CreateNotifyIcon();
            Application.Run();
        }

        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                UpdateIcons();
                UpdateNotifyIcon(TaskbarHelper.IsNotAutoHide());
            }
        }

        private static void UpdateIcons()
        {
            var isLight = TaskbarHelper.IsSystemUsesLightTheme();
            showIcon = isLight ? showIconLight : showIconDark;
            hideIcon = isLight ? hideIconLight : hideIconDark;
        }

        private static void UpdateTaskbarState()
        {
            var isAutoHide = System.Windows.Forms.Screen.AllScreens.Length == 1;
            var actualIsAutoHide = TaskbarHelper.SetAutoHideState(isAutoHide);
            UpdateNotifyIcon(actualIsAutoHide); // アイコンは "表示" の状態に合わせる
        }

        static bool _controlByExternalDisplay = IniFileHandler.ReadControlByExternalDisplay(true);

        private static void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            if (_controlByExternalDisplay)
            {
                UpdateTaskbarState(); // ディスプレイ設定変更時にタスクバーの状態を更新
            }
        }

        static readonly NotifyIcon notifyIcon = new NotifyIcon();
        // ライトテーマ用アイコン
        static Icon showIconLight = new Icon("Icon_show_light.ico");
        static Icon hideIconLight = new Icon("Icon_hide_light.ico");
        // ダークテーマ用アイコン (仮に同じものを設定)
        static Icon showIconDark = new Icon("Icon_show_dark.ico");
        static Icon hideIconDark = new Icon("Icon_hide_dark.ico");

        // 現在のテーマに応じたアイコン
        static Icon showIcon = showIconDark;
        static Icon hideIcon= hideIconDark;


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

            var externalDisplayControlMenuItem = new ToolStripMenuItem("外部ディスプレイによる制御");
            externalDisplayControlMenuItem.CheckOnClick = true;
            externalDisplayControlMenuItem.Checked = _controlByExternalDisplay;
            externalDisplayControlMenuItem.Click += (s, e) =>
            {
                _controlByExternalDisplay = externalDisplayControlMenuItem.Checked;
                IniFileHandler.WriteControlByExternalDisplay(_controlByExternalDisplay);
                if (_controlByExternalDisplay)
                {
                    UpdateTaskbarState(); // 有効になったらすぐに状態を更新
                }
            };
            menu.Items.Add(externalDisplayControlMenuItem);

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