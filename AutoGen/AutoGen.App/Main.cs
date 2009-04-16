using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AutoGen.I;
using AutoGen.PL;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTabbedMdi;
using DevExpress.XtraEditors.Controls;

namespace AutoGen.App
{
    public partial class Main : DevExpress.XtraBars.Ribbon.RibbonForm, IAutoGenApplication
    {
        private PluginLoader.AvailablePlugin[] generatorPlugins;
        private IAutoGenPlugin[] myLoadedGenerators;
        private PluginLoader.AvailablePlugin[] printerPlugins;
        private IAutoGenPlugin[] myLoadedPrinters;
        private readonly string defaultStatus = "Готов";
        private AutoGenData myAppData;
        private PlayList playList;

        #region IAutoGenApplication Members

        public IAutoGenDBSettings MainDBSettings
        {
            get { return new AutoGenDBSettings(); }
        }

        public object MainForm
        {
            get { return this; }
        }

        public string MainApplicationDir
        {
            get { return AutoGenBase.AppPath; }
        }

        public string MainTexDir
        {
            get { return AutoGenBase.AppTexPath; }
        }

        public string MainTexPortDir
        {
            get { return AutoGenBase.TexPortFolder; }
        }

        public string MainDataDir
        {
            get { return AutoGenBase.AppSaveDataPath; }
        }

        #endregion

        public IAutoGenPlugin GetDefaultPrinter()
        {
            foreach (IAutoGenPlugin printer in myLoadedPrinters)
            {
                if (printer.GUID.Equals(MyAppData.MainProperties.DefaultPrinterGuid))
                    return printer;
            }
            return null;
        }

        public PlayList PlayList
        {
            get { return playList; }
        }

        public BarManager AppBarManager
        {
            get { return appBarManager; }
        }

        public class RusBarLocilizer : BarLocalizer
        {
            public override string Language
            {
                get
                {
                    return "ru-Ru";
                }
            }

            public override string GetLocalizedString(BarString id)
            {
                switch (id)
                {
                    case BarString.RibbonToolbarAbove:
                        return "Расположить панель быстрого доступа сверху";
                    case BarString.RibbonToolbarBelow:
                        return "Расположить панель быстрого доступа снизу";
                    case BarString.RibbonToolbarAdd:
                        return "Добавить в панель быстрого доступа";
                    case BarString.RibbonToolbarMinimizeRibbon:
                        return "Свернуть ленту";
                    case BarString.RibbonToolbarRemove:
                        return "Убрать из панели быстрого доступа";
                }
                return base.GetLocalizedString(id);
            }
        }


        public class RusControlLocalizer : Localizer
        {
            public override string Language
            {
                get
                {
                    return "ru-Ru";
                }
            }

            public override string GetLocalizedString(StringId id)
            {
                switch (id)
                {
                    case StringId.CalcError:
                        return "Ошибка вычисления";
                    case StringId.Cancel:
                        return "Отмена";
                    case StringId.CalcButtonBack:
                        return "Назад";
                    case StringId.TextEditMenuCopy:
                        return "Копировать";
                    case StringId.TextEditMenuCut:
                        return "Вырезать";
                    case StringId.TextEditMenuPaste:
                        return "Вставить";
                    case StringId.TextEditMenuDelete:
                        return "Удалить";
                    case StringId.TextEditMenuSelectAll:
                        return "Выделить все";
                    case StringId.TextEditMenuUndo:
                        return "Отмена";
                    case StringId.XtraMessageBoxAbortButtonText:
                        return "Прервать";
                    case StringId.XtraMessageBoxCancelButtonText:
                        return "Отмена";
                    case StringId.XtraMessageBoxOkButtonText:
                        return "Ок";
                    case StringId.XtraMessageBoxNoButtonText:
                        return "Нет";
                    case StringId.XtraMessageBoxYesButtonText:
                        return "Да";
                    case StringId.XtraMessageBoxIgnoreButtonText:
                        return "Пропустить";
                    case StringId.XtraMessageBoxRetryButtonText:
                        return "Повтор";
                }
                return base.GetLocalizedString(id);
            }
        }

        public Main()
        {
            InitializeComponent();
            BarLocalizer.Active = new RusBarLocilizer();
            Localizer.Active = new RusControlLocalizer();
        }

        public void SetStatus(string statusText)
        {
            barStaticTextItem.Caption = statusText;
        }

        public void CloseAllTabs(int taskId)
        {
            foreach (XtraMdiTabPage page in appMainTabManager.Pages)
            {
                if (page.MdiChild is UTForm)
                {
                    if (((UTForm)page.MdiChild).ID == taskId)
                    {
                        page.MdiChild.Close();
                        return;
                    }
                }
            }
        }

        public void ShowEditTaskTab(GridObject gObj)
        {
            foreach (XtraMdiTabPage page in appMainTabManager.Pages)
            {
                if (page.MdiChild is UTForm)
                {
                    if (((UTForm) page.MdiChild).ID == gObj.ID)
                    {
                        appMainTabManager.SelectedPage = page;
                        return;
                    }
                }
            }
            UTForm newF = taskList.MakeNewTaskForm(gObj);
            newF.MdiParent = this;
            newF.Show();
            appMainTabManager.Pages[newF].Tooltip = newF.Text;
            newF.Text = newF.Text.Length > 10 ? (newF.Text.Remove(10) + "...") : newF.Text;
        }

        TListForm taskList;
        private void Main_Load(object sender, EventArgs e)
        {
            SetStatus("Загружаем плагины");
            LoadPlugins();
            SetStatus("Загружаем задачи");
            taskList = new TListForm(this);
            taskList.MdiParent = this;
            taskList.ControlBox = false;
            taskList.Show();
            myAppData = taskList.MyAppData;
            LoadCustomData();
            LoadSettings();
            SetStatus(defaultStatus);
        }

        private void LoadCustomData()
        {
            if (myAppData.CustomData == null) myAppData.CustomData = new AutogenCustomData();
            for (int i = 0; i < generatorPlugins.Length; i++)
            {
                PluginLoader.AvailablePlugin plugin = generatorPlugins[i];
                IAutoGenGenerator generator = (IAutoGenGenerator) myLoadedGenerators[i];
                GeneratorData data = MyAppData.CustomData.FindByClassName(plugin.ClassName);
                if (data != null) generator.GeneratorData = data.Data;
            }
        }

        private void LoadSettings()
        {
            if (MyAppData.MainProperties == null)
                myAppData.MainProperties = new AutoGenProperties(this);
        }

        public AutoGenData MyAppData
        {
            get { return myAppData; }
        }

        public IAutoGenPlugin[] MyLoadedGenerators
        {
            get { return myLoadedGenerators; }
        }

        public IAutoGenPlugin[] MyLoadedPrinters
        {
            get { return myLoadedPrinters; }
        }

        #region AssemblyLoad

        public void LoadAssemblyByFileName(String file, AppDomain domain)
        {
            byte[] bytes = GetAssemblyAsBytes(file);
            if (bytes.Length == 0)
            {
                throw new Exception("Невозможно загрузить сборку. Длина файла составила 0 байт.");
            }
            domain.Load(bytes);
        }

        public byte[] GetAssemblyAsBytes(String path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Невозможно загрузить сборку. Файл не найден", path);
            }
            try
            {
                FileInfo file = new FileInfo(path);
                byte[] bytes = new byte[file.Length];
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                stream.Read(bytes, 0, bytes.Length);
                stream.Close();
                return bytes;
            } catch (Exception ex)
            {
                throw new Exception("Невозможно загрузить сборку.", ex);
            }
        } 

        #endregion

        private void LoadPlugins()
        {
            generatorPlugins =
                PluginLoader.FindPlugins(AutoGenBase.AppPluginPath, AutoGenInterfaces.IAutoGenGenerator);
            List<IAutoGenPlugin> lPlugin = new List<IAutoGenPlugin>();
            if (generatorPlugins != null)
                for (int index = 0; index < generatorPlugins.Length; index++)
                {
                    try
                    {
                        IAutoGenPlugin agp = (IAutoGenPlugin)PluginLoader.CreateInstance(generatorPlugins[index]);
                        agp.InitPlugin(this);
                        lPlugin.Add(agp);
                    } catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            myLoadedGenerators = lPlugin.ToArray();

            printerPlugins =
                PluginLoader.FindPlugins(AutoGenBase.AppPluginPath, AutoGenInterfaces.IAutoGenPrinter);
            lPlugin = new List<IAutoGenPlugin>();
            if (printerPlugins != null)
                for (int index = 0; index < printerPlugins.Length; index++)
                {
                    try
                    {
                        IAutoGenPlugin agp = (IAutoGenPlugin)PluginLoader.CreateInstance(printerPlugins[index]);
                        agp.InitPlugin(this);
                        lPlugin.Add(agp);
                    } catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            myLoadedPrinters = lPlugin.ToArray();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PlayList != null)
                PlayList.Close();

            SaveCustomData();

            taskList.BeforeClosing(sender, e);
        }

        private void SaveCustomData()
        {
            for (int i = 0; i < generatorPlugins.Length; i++)
            {
                PluginLoader.AvailablePlugin plugin = generatorPlugins[i];
                IAutoGenGenerator generator = (IAutoGenGenerator)myLoadedGenerators[i];
                GeneratorData data = MyAppData.CustomData.FindByClassName(plugin.ClassName);
                if (data != null) data.Data = generator.GeneratorData;
                else MyAppData.CustomData.Add(new GeneratorData(plugin.ClassName, generator.GeneratorData));
            }
        }

        private void ribbonPageGroup2_CaptionButtonClick(object sender, DevExpress.XtraBars.Ribbon.RibbonPageGroupEventArgs e)
        {
            TListForm taskList1 = new TListForm(this);
            taskList1.MdiParent = this;
            taskList1.Show();
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateNewFolder();
        }

        private void CreateNewFolder()
        {
            taskList.NewFolder();
        }

        private void bbPluginList_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenPluginsList();
        }

        private void OpenPluginsList()
        {
            ListPL lpl = new ListPL(generatorPlugins, myLoadedGenerators, myLoadedPrinters, this);
            lpl.ShowDialog();
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            EditCurrentObject();
        }

        private void EditCurrentObject()
        {
            taskList.EditCurrentObject();
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddNewTaskItem();
        }

        private void AddNewTaskItem()
        {
            taskList.AddTaskInstance();
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowPlayList();
        }

        public void ShowPlayList()
        {
            barProgressBar.Visibility = BarItemVisibility.Never;
            barButtonItem7.Enabled = false;
            if (playList != null && playList.IsOpened)
            {
                playList.Visible = false;
                playList.Show();
            }
            else
            {
                playList = new PlayList(this);
                playList.Show();
                playList.Closed += playList_Closed;
            }
        }

        void playList_Closed(object sender, EventArgs e)
        {
            barProgressBar.Visibility = BarItemVisibility.Never;
            barButtonItem7.Enabled = true;
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportPlugin();
        }

        private void ImportPlugin()
        {
            ArrayList al = new ArrayList();
            al.AddRange(printerPlugins);
            al.AddRange(generatorPlugins);
            PluginLoader.AvailablePlugin[] allPlugins = new PluginLoader.AvailablePlugin[al.Count];
            al.CopyTo(allPlugins);
            PluginManager.UploadPlugin(allPlugins);
        }

        public void PlayListHide()
        {
            barProgressBar.Visibility = BarItemVisibility.Always;
            barButtonItem7.Enabled = true;
        }

        public void PlayListShow()
        {
            barProgressBar.Visibility = BarItemVisibility.Never;
            barButtonItem7.Enabled = false;
        }

        private void barProgressBar_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowPlayList();
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            RemoveSelectedItem();
        }

        private void RemoveSelectedItem()
        {
            taskList.RemoveSelectedItem();
        }

        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            MainSettings ms = new MainSettings(this);
            ms.ShowDialog();
        }

        private void appMainTabManager_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                BaseTabHitInfo hi = appMainTabManager.CalcHitInfo(new Point(e.X, e.Y));
                if(hi.IsValid && hi.HitTest == XtraTabHitTest.PageHeader && hi.Page != null)
                {
                    XtraMdiTabPage hPage =((XtraMdiTabPage)hi.Page);
                    ((XtraTabbedMdiManager) hi.Page.TabControl).SelectedPage = hPage;
                    if(hPage.MdiChild is UTForm)
                    {
                        hPage.MdiChild.Close();
                    }
                }
            }
        }

        private void appMainTabManager_SelectedPageChanged(object sender, EventArgs e)
        {
            if(appMainTabManager.SelectedPage.MdiChild is UTForm)
            {
                ribbonPageGroup2.Visible = false;
            } else
            {
                ribbonPageGroup2.Visible = true;
            }
        }
    }
}

