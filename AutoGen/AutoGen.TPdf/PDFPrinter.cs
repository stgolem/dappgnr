using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using AutoGen.I;
using AutoGen.TeXML;

namespace AutoGen.TPdf
{
    [Serializable]
    public class PDFPrinter : IAutoGenPrinter
    {
        private readonly Version pluginVersion = new Version(0,0,0,1);
        private readonly string autor = "Kononov Anton";
        private readonly string pluginName = "PDF Printer";
        [NonSerialized] private TPDFPrinterSettings settings = null;
        [NonSerialized] private Process p = null;
        [NonSerialized] private IAutoGenApplication hostApplication;

        #region IAutoGenPrinter Members

        public void Print(TeXMLDocList TeXDocumentList, IAutoGenWorker Worker, IAutoGenParameters Parameters)
        {
            if (Worker.IsCanceled) return;
            Worker.CancelSend += Worker_CancelSend;
            Worker.ReportProgress(0, "Начинаем печать в формат PDF");
            Worker.WriteOutputLine("Начинаем печать в формат PDF");
            int cou = 0;
            foreach (TeXMLDoc TeXDocument in TeXDocumentList)
            {
                cou++;
                string teXMLDir = hostApplication.MainTexDir + "TeXML\\";
                string teXPortDir = hostApplication.MainTexPortDir;
                string tmpFileTexML = Path.GetTempFileName();
                string tmpFileTex = Path.GetTempFileName();
                string tmpFilePDf = teXPortDir + Path.GetFileNameWithoutExtension(tmpFileTex) + ".pdf";
                string tmpFileTeXNew = Path.GetDirectoryName(tmpFileTex) + "\\" +
                                       Path.GetFileNameWithoutExtension(tmpFileTex) + ".tex";
                Worker.ReportProgress(10, "Начинаем генерацию файла TeXML");
                TeXDocument.WriteXml(tmpFileTexML);
                Worker.ReportProgress(25, "Генерация файла TeXML завершена");
                p = new Process();
                try
                {
                    p.StartInfo.FileName = teXMLDir + "texml.exe";
                    p.StartInfo.Arguments = "-e cp1251 " + tmpFileTexML + " " + tmpFileTex;
                    if (!Settings.NeedShow)
                    {
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.ErrorDialog = false;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    }
                    p.StartInfo.WorkingDirectory = teXMLDir;
                    //p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                    //while (!p.StandardOutput.EndOfStream)
                    //{
                    //    Worker.WriteOutputLine(p.StandardOutput.ReadLine());
                    //}
                    p.WaitForExit();
                    File.Delete(tmpFileTexML);
                    File.Move(tmpFileTex, tmpFileTeXNew);
                    Worker.ReportProgress(35, "Файл ТеХ успешно записан.");
                }
                catch (Exception ex)
                {
                    Worker.WriteOutputLine(ex.Message);
                }
                p = new Process();
                try
                {
                    string addName = (TeXDocumentList.Count > 1
                                          ? ("_" +
                                             (string.IsNullOrEmpty(TeXDocument.TagName)
                                                  ? cou.ToString()
                                                  : TeXDocument.TagName))
                                          : "");
                    string finalName = Settings.FolderPath + "\\" + Parameters.TaskName + addName;
                    Worker.ReportProgress(40, "Копируем файл TeX");
                    File.Copy(tmpFileTeXNew, finalName + ".tex", true);
                    Worker.WriteOutputLine("=========================================");
                    Worker.WriteOutputLine("Записан файл TeX: " + finalName + ".tex");
                    if (Settings.NeedPDF)
                    {
                        Worker.ReportProgress(45, "Начинаем генерацию файла PDF");
                        p.StartInfo.FileName = teXPortDir + "texify.bat";
                        p.StartInfo.Arguments = "-c -p " + tmpFileTeXNew;
                        if (!Settings.NeedShow)
                        {
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.ErrorDialog = false;
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        }
                        p.StartInfo.WorkingDirectory = teXPortDir;
                        //p.StartInfo.RedirectStandardOutput = true;
                        Worker.ReportProgress(50, "Идет генерация файла PDF...");
                        p.Start();
                        //while (!p.StandardOutput.EndOfStream)
                        //{
                        //    Worker.WriteOutputLine(p.StandardOutput.ReadLine());
                        //}
                        p.WaitForExit();
                        Worker.ReportProgress(80, "Файл PDF сгенерирован. Копируем.");
                        File.Delete(tmpFileTeXNew);
                        if (File.Exists(finalName + ".pdf"))
                            File.Delete(finalName + ".pdf");
                        File.Move(tmpFilePDf, finalName + ".pdf");
                        Worker.ReportProgress(100, "Файл PDF успешно создан.");
                        if (Settings.NeedOpen)
                        {
                            p = new Process();
                            p.StartInfo.FileName = finalName + ".pdf";
                            p.Start();
                        }
                        Worker.WriteOutputLine("Записан файл PDF: " + finalName + ".pdf");
                    }
                }
                catch (Exception ex)
                {
                    Worker.WriteOutputLine(ex.Message);
                }
            }
        }

        void Worker_CancelSend(object sender, EventArgs e)
        {
            if (p != null) p.Close();
        }

        public void ShowProperties()
        {
            SettingsTPDF setting = new SettingsTPDF(this);
            setting.ShowDialog();
        }

        #endregion

        #region IAutoGenPlugin Members

        public Version PluginVersion
        {
            get { return pluginVersion; }
        }

        public string Autor
        {
            get { return autor; }
        }

        public string PluginName
        {
            get { return pluginName; }
        }

        public void InitPlugin(IAutoGenApplication autoGenApp)
        {
            hostApplication = autoGenApp;
            settings = new TPDFPrinterSettings(HostApplication);
        }

        public void ShowAbout()
        {
            AboutTPDF about = new AboutTPDF();
            about.ShowDialog();
        }

        public Guid GUID
        {
            get { return new Guid(AssemblyGuid); }
        }

        #endregion

        public string AssemblyGuid
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((GuidAttribute)attributes[0]).Value;
            }
        }

        public override string ToString()
        {
            return "TeX2PDF Printer";
        }

        public TPDFPrinterSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public IAutoGenApplication HostApplication
        {
            get { return hostApplication; }
        }
    }

    [Serializable]
    public class TPDFPrinterSettings
    {
        private readonly IAutoGenDBSettings db;

        public TPDFPrinterSettings(IAutoGenApplication app)
        {
            if (app != null)
            {
                db = app.MainDBSettings;
                if (string.IsNullOrEmpty(FolderPath))
                    FolderPath = app.MainApplicationDir;
            }
        }

        public bool NeedShow
        {
            get { return !string.IsNullOrEmpty(db.GetValue("TPDFPrinterSettings", "NeedShow")); }
            set { db.SaveValue("TPDFPrinterSettings", "NeedShow", value ? "1" : ""); }
        }

        public bool NeedOpen
        {
            get { return !string.IsNullOrEmpty(db.GetValue("TPDFPrinterSettings", "NeedOpen")); }
            set { db.SaveValue("TPDFPrinterSettings", "NeedOpen", value ? "1" : ""); }
        }

        public string FolderPath
        {
            get { return db.GetValue("TPDFPrinterSettings", "FolderPath"); }
            set { db.SaveValue("TPDFPrinterSettings", "FolderPath", value); }
        }

        public bool NeedPDF
        {
            get { return !string.IsNullOrEmpty(db.GetValue("TPDFPrinterSettings", "NeedPDF")); }
            set { db.SaveValue("TPDFPrinterSettings", "NeedPDF", value ? "1" : ""); }
        }
    }
}
