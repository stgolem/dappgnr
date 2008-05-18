using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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
        [NonSerialized] private Process p = null;
        [NonSerialized] private IAutoGenApplication hostApplication;

        #region IAutoGenPrinter Members

        public void Print(TeXMLDoc TeXDocument, IAutoGenWorker Worker)
        {
            if (Worker.IsCanceled) return;
            Worker.CancelSend += Worker_CancelSend;
            Worker.ReportProgress(0, "Начинаем печать в формат PDF");
            Worker.WriteOutputLine("Начинаем печать в формат PDF");
            string teXMLDir = hostApplication.MainTexDir + "TeXML\\";
            string teXPortDir = hostApplication.MainTexPortDir;
            string tmpFileTexML = Path.GetTempFileName();
            string tmpFileTex = Path.GetTempFileName();
            string tmpFilePDf = teXPortDir + Path.GetFileNameWithoutExtension(tmpFileTex) + ".pdf";
            string tmpFileTeXNew = Path.GetDirectoryName(tmpFileTex) + "\\" + Path.GetFileNameWithoutExtension(tmpFileTex) + ".tex";
            Worker.ReportProgress(10, "Начинаем генерацию файла TeXML");
            TeXDocument.WriteXml(tmpFileTexML);
            Worker.ReportProgress(25, "Генерация файла TeXML завершена");
            p = new Process();
            try
            {
                p.StartInfo.FileName = teXMLDir + "texml.exe";
                p.StartInfo.Arguments = "-e cp1251 " + tmpFileTexML + " " + tmpFileTex;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
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
            } catch (Exception ex)
            {
                Worker.WriteOutputLine(ex.Message);
            }
            p = new Process();
            try
            {
                Worker.ReportProgress(40, "Начинаем генерацию файла PDF");
                p.StartInfo.FileName = teXPortDir + "texify.bat";
                p.StartInfo.Arguments = "-c -p " + tmpFileTeXNew;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.WorkingDirectory = teXPortDir;
                //p.StartInfo.RedirectStandardOutput = true;
                Worker.ReportProgress(50, "Идет генерация файла PDF...");
                //Thread.Sleep(5000);
                p.Start();
                //while (!p.StandardOutput.EndOfStream)
                //{
                //    Worker.WriteOutputLine(p.StandardOutput.ReadLine());
                //}
                p.WaitForExit();
                Worker.ReportProgress(80, "Файл PDF сгенерирован. Копируем.");
                File.Delete(tmpFileTeXNew);
                if (File.Exists(teXMLDir + "pdfTmp.pdf"))
                    File.Delete(teXMLDir + "pdfTmp.pdf");
                File.Move(tmpFilePDf, teXMLDir + "pdfTmp.pdf");
                Worker.WriteOutputLine("=========================================");
                Worker.WriteOutputLine("Записан файл: " + teXMLDir + "pdfTmp.pdf");
                Worker.ReportProgress(100, "Файл PDF успешно создан.");
            } catch (Exception ex)
            {
                Worker.WriteOutputLine(ex.Message);
            }
        }

        void Worker_CancelSend(object sender, EventArgs e)
        {
            if (p != null) p.Close();
        }

        public void ShowProperties()
        {
            return;
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
        }

        public void ShowAbout()
        {
            MessageBox.Show("This is my about. Ver = " + PluginVersion, "PDF Printer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public Guid GUID
        {
            get { return new Guid("{B958D581-DF6C-474c-B58D-DC833DCBF638}"); }
        }

        #endregion

        public override string ToString()
        {
            return "PDF Printer";
        }
    }
}
