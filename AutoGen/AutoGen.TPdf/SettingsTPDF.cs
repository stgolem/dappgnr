using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AutoGen.TPdf
{
    public partial class SettingsTPDF : XtraForm
    {
        private readonly PDFPrinter _Printer;
        public SettingsTPDF(PDFPrinter printer)
        {
            InitializeComponent();
            _Printer = printer;
        }

        private void buttonEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            switch((string)e.Button.Tag)
            {
                case "Select":
                    SelectFolder();
                    break;
                case "Open":
                    OpenCurrent();
                    break;
            }
        }

        private void OpenCurrent()
        {
            if (Directory.Exists(_Printer.Settings.FolderPath))
            {
                Process p = new Process();
                p.StartInfo.FileName = _Printer.Settings.FolderPath;
                p.Start();
            }
        }

        private void SelectFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = buttonEdit1.Text;
            fbd.ShowNewFolderButton = true;
            if(fbd.ShowDialog()==DialogResult.OK)
            {
                buttonEdit1.Text = fbd.SelectedPath;
                _Printer.Settings.FolderPath = fbd.SelectedPath;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            _Printer.Settings.FolderPath = buttonEdit1.Text;
            _Printer.Settings.NeedShow = checkEdit1.Checked;
            _Printer.Settings.NeedOpen = checkEdit2.Checked;
            _Printer.Settings.NeedPDF = checkEdit3.Checked;
            Close();
        }

        private void SettingsTPDF_Load(object sender, EventArgs e)
        {
            if (_Printer.Settings == null)
                _Printer.Settings = new TPDFPrinterSettings(_Printer.HostApplication);
            checkEdit1.Checked = _Printer.Settings.NeedShow;
            checkEdit2.Checked = _Printer.Settings.NeedOpen;
            checkEdit3.Checked = _Printer.Settings.NeedPDF;
            buttonEdit1.Text = _Printer.Settings.FolderPath;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}