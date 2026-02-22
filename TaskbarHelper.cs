using System.Runtime.InteropServices;
using Microsoft.Win32;

public static class TaskbarHelper
{
    private const int TrayAbsAutoHide = 1;

    private const int TrayAbsAlwaysOnTop = 2;

    private const uint TrayAbmSetState = 10;

    private const uint TrayAbmGetState = 4;
    private static AppbarData _msg;


    public static bool IsAutoHide() => SHAppBarMessage(TrayAbmGetState, ref _msg) == TrayAbsAutoHide;

    public static bool ToggleAutoHideState()
    {
        return SetAutoHideState(!IsAutoHide());
    }

    public static bool SetAutoHideState(bool isAutoHide)
    {
        _msg.lParam = isAutoHide ? TrayAbsAutoHide : TrayAbsAlwaysOnTop;
        _ = SHAppBarMessage(TrayAbmSetState, ref _msg);
        return _msg.lParam == TrayAbsAutoHide;
    }

    public static bool IsSystemUsesLightTheme()
    {
        using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
        {
            return key?.GetValue("SystemUsesLightTheme") is 1;
        }
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

    public override bool Equals(object? obj)
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
