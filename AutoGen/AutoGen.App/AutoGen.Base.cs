using System.IO;
using System.Windows.Forms;

namespace AutoGen.App
{
    public static class AutoGenBase
    {
        private static readonly string pluginFolder = "\\Plugins\\";
        private static readonly string saveFolder = "\\Data\\";
        private static readonly string texFolder = "\\Tex\\";
        private static readonly string texPortFolder = "MTPort\\";
        private static readonly string saveFile = "AutoGen.agd";

        public static string PluginFolder
        {
            get { return pluginFolder; }
        }

        public static string SaveFolder
        {
            get { return saveFolder; }
        }

        public static string SaveFile
        {
            get { return saveFile; }
        }

        public static string TexFolder
        {
            get { return texFolder; }
        }

        public static string AppPath
        {
            get { return Path.GetDirectoryName(Application.ExecutablePath); }
        }

        public static string AppPluginPath
        {
            get { return AppPath + PluginFolder; }
        }

        public static string AppSaveDataPath
        {
            get { return AppPath + SaveFolder; }
        }

        public static string AppTexPath
        {
            get { return AppPath + TexFolder; }
        }

        public static string TexPortFolder
        {
            get { return AppTexPath + texPortFolder; }
        }
    }
}
