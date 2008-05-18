using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AutoGen.I;
using AutoGen.PL;
using DevExpress.XtraEditors;

namespace AutoGen.App
{
    [Serializable]
    public class PluginManager
    {
        protected static void CopyPlugin(PluginLoader.AvailablePlugin plugin)
        {
            File.Copy(plugin.AssemblyPath, AutoGenBase.AppPluginPath + Path.GetFileName(plugin.AssemblyPath), true);
        }

        public static bool UploadPlugin(PluginLoader.AvailablePlugin[] myPlugins)
        {
            List<PluginLoader.AvailablePlugin> myList;
            if (myPlugins != null)
                myList = new List<PluginLoader.AvailablePlugin>(myPlugins);
            else
                myList = new List<PluginLoader.AvailablePlugin>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Плагины АвтоГенератора | *.dll";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    if(File.Exists(ofd.FileNames[i]))
                    {
                        PluginLoader.AvailablePlugin[] fPL = PluginLoader.FindInFile(ofd.FileNames[i], AutoGenInterfaces.IAutoGenPlugin);
                        if(fPL != null)
                        {
                            List<PluginLoader.AvailablePlugin> fPll = new List<PluginLoader.AvailablePlugin>(fPL);
                            foreach (PluginLoader.AvailablePlugin plugin in fPll)
                            {
                                bool canCopy = true;
                                foreach (PluginLoader.AvailablePlugin availablePlugin in myList)
                                {
                                    if (availablePlugin.ClassName.Equals(plugin.ClassName) ||
                                        Path.GetFileName(availablePlugin.AssemblyPath).Equals(Path.GetFileName(plugin.AssemblyPath)))
                                    {
                                        canCopy =
                                            XtraMessageBox.Show(
                                                plugin.AssemblyPath + "\nУже есть такой плагин, продолжить?", "Info",
                                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
                                    }
                                }
                                if (canCopy)
                                    CopyPlugin(plugin);
                            }
                        }
                    }
                }
            } else
                return false;
            XtraMessageBox.Show("Все плагины успешно загружены. Перезапустите программу.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
    }

    [Serializable]
    public class PluginContainer
    {
        private readonly IAutoGenPlugin _Plugin;

        public IAutoGenPlugin Plugin
        {
            get { return _Plugin; }
        }

        public PluginContainer(IAutoGenPlugin _plugin)
        {
            _Plugin = _plugin;
        }
        public override string ToString()
        {
            return Plugin.PluginName + " (ver " + Plugin.PluginVersion + ")";
        }
    }

    [Serializable]
    public class PluginContainerList : List<PluginContainer>
    {
        public PluginContainerList(IEnumerable<PluginContainer> collection) : base(collection)
        {
        }

        public PluginContainerList(int capacity) : base(capacity)
        {
        }

        public PluginContainerList()
        {
        }

        public int GetByPlugin(IAutoGenPlugin plugin)
        {
            return FindIndex(delegate(PluginContainer pc)
                                 { return plugin != null && pc.Plugin.GUID == plugin.GUID; });
        }
    }
}