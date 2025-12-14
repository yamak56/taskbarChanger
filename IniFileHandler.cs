using System.Runtime.InteropServices;
using System.Text;

namespace TaskBarChanger
{
    internal static class IniFileHandler
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.ini");
        private const string SectionName = "Settings";
        private const string KeyName = "ControlByExternalDisplay";

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static void WriteControlByExternalDisplay(bool value)
        {
            WritePrivateProfileString(SectionName, KeyName, value.ToString(), FilePath);
        }

        public static bool ReadControlByExternalDisplay(bool defaultValue)
        {
            StringBuilder retVal = new StringBuilder(255);
            GetPrivateProfileString(SectionName, KeyName, defaultValue.ToString(), retVal, 255, FilePath);
            if (bool.TryParse(retVal.ToString(), out bool result))
            {
                return result;
            }
            return defaultValue;
        }
    }
}
