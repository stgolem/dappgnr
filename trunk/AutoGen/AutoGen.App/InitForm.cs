using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Win32;

namespace AutoGen.App
{
    public partial class InitForm : XtraForm
    {
        private static readonly string RegKeyName = "TeX-portable";
        private static readonly string RegValueName = "MTPortLocation";
        public bool needDos;

        public bool NeedDos
        {
            get { return needDos; }
        }

        public InitForm(ProgramArgs _programArgs)
        {
            InitializeComponent();
            needDos = _programArgs.ShowDos;
            Load += InitForm_Load;
        }

        void InitForm_Load(object sender, EventArgs e)
        {
            RunRegister();
        }

        public void RunRegister()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.RunWorkerAsync(this);
        }

        static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ((Form)e.Result).DialogResult = DialogResult.OK;
        }

        static void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            TryRegisterTeX(((InitForm)e.Argument).NeedDos);
            e.Result = e.Argument;
        }

        public static void TryRegisterTeX(bool nd)
        {
            RegistryKey tp = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(RegKeyName);
            if (tp != null)
            {
                if ((string)tp.GetValue(RegValueName) == AutoGenBase.AppPath)
                    return;
                else
                {
                    UnRegisterMtPort(nd);
                    RegisterMtPort(nd);
                }
            } else
            {
                RegisterMtPort(nd);
            }
            tp = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(RegKeyName, true);
            if (tp == null)
            {
                RegisterMtPort(nd);
                tp = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey(RegKeyName, true);
            }
            tp.SetValue(RegValueName, AutoGenBase.AppPath, RegistryValueKind.String);
        }

        public static void RegisterMtPort(bool nd)
        {
            Process p = new Process();
            p.StartInfo.FileName = AutoGenBase.TexPortRegister;
            if (!nd)
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            p.StartInfo.WorkingDirectory = AutoGenBase.AppTexPath;
            p.Start();
            p.WaitForExit();
        }

        public static void UnRegisterMtPort(bool nd)
        {
            Process p = new Process();
            p.StartInfo.FileName = AutoGenBase.TexPortUnRegister;
            if (!nd)
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            p.StartInfo.WorkingDirectory = AutoGenBase.AppTexPath;
            p.Start();
            p.WaitForExit();
        }
    }
}
