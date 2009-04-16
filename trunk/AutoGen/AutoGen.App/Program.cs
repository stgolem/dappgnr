using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AutoGen.App
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ProgramArgs pa = new ProgramArgs();
            pa.ShowDos = false;
            pa.NoTex = false;
            pa.RegTex = false;
            pa.UnRegTex = false;
            if (args != null && args.Length > 0)
            {
                if (args.Length == 1 && args[0].Equals("/?"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("/sd - показывать окно дос при регистрации или разрегистрации TeX");
                    sb.AppendLine();
                    sb.AppendLine("/regtex - принудительная регистрация TeX (игнорирует /notex)");
                    sb.AppendLine();
                    sb.AppendLine("/notex - позволяет не регистрировать TeX при первом запуске");
                    sb.AppendLine();
                    sb.AppendLine("/rmtex - принудительная разрегистрация TeX и закрытие программы");
                    sb.AppendLine();
                    sb.AppendLine("/? - отображает это сообщение и выходит из программы");
                    XtraMessageBox.Show(sb.ToString(), "Параметры коммандной строки", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                List<string> argList = new List<string>();
                foreach (string arg in args)
                {
                    argList.Add(arg.ToLower());
                }
                pa.ShowDos = argList.Contains("/sd");
                pa.NoTex = argList.Contains("/notex");
                pa.RegTex = argList.Contains("/regtex");
                pa.UnRegTex = argList.Contains("/rmtex");
            }
            DevExpress.Skins.SkinManager.EnableFormSkins();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BeforeAppStart(pa);
            if (pa.UnRegTex)
            {
                UnregisterTex(pa);
                return;
            }
            AppDomain.CurrentDomain.AssemblyResolve += ObjectFormatter.CurrentDomain_AssemblyResolve;
            Application.Run(new Main());
        }

        static void BeforeAppStart(ProgramArgs nd)
        {
            AutoGenProperties agp = AutoGenProperties.Load(new AutoGenDBSettings());
            if (nd.RegTex || nd.NoTex)
            {
                agp.NeedUseTex = !nd.NoTex || nd.RegTex;
                agp.Save(new AutoGenDBSettings());
            }
            if (agp.NeedUseTex)
            {
                InitForm initForm = new InitForm(nd);
                initForm.ShowDialog();
            }
        }

        static void UnregisterTex(ProgramArgs nd)
        {
            InitForm.UnRegisterMtPort(nd.ShowDos);
        }
    }

    public class ProgramArgs
    {
        private bool showDos;
        private bool noTex;
        private bool regTex;
        private bool unRegTex;

        public bool ShowDos
        {
            get { return showDos; }
            set { showDos = value; }
        }

        public bool NoTex
        {
            get { return noTex; }
            set { noTex = value; }
        }

        public bool RegTex
        {
            get { return regTex; }
            set { regTex = value; }
        }

        public bool UnRegTex
        {
            get { return unRegTex; }
            set { unRegTex = value; }
        }
    }
}