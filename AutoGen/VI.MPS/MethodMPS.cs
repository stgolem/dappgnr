using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using AutoGen.I;
using AutoGen.TeXML;
using DevExpress.XtraEditors;

namespace VI.MPS
{
    public class MethodMPS : IAutoGenGenerator
    {
        #region Implementation of IAutoGenPlugin

        private Version pluginVersion = new Version(0, 0, 1);
        private string autor = "Rodionov V.I.";
        private string pluginName = "Метод покоординатного спуска";
        private Guid guid = new Guid("{575C10C5-5D58-49a6-BFAE-C6475B296A36}");

        private object generatorData;
        [NonSerialized] private IAutoGenApplication hostApplication;

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
            XtraMessageBox.Show("Программа генерирует данные для расчета\nметодом покоордтнатного спуска\nАвтор: Родионов В.И.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public Guid GUID
        {
            get { return guid; }
        }

        #endregion

        #region Implementation of IAutoGenGenerator

        public IAutoGenTask CreateTaskInstance(string taskName)
        {
            return new MethodTask(taskName);
        }

        public object GeneratorData
        {
            get { return generatorData; }
            set { generatorData = value; }
        }

        #endregion
    }

    [Serializable]
    public class MethodTask : IAutoGenTask
    {
        public MethodTask(string taskName)
        {
            this.taskName = taskName;
            taskPropertiesControl = new MethodControl(this);
        }

        #region Implementation of ISerializable

        private string taskName;
        private string taskDescription;
        private ITaskControl taskPropertiesControl;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(MethodTask));
            info.AddValue("taskName", taskName);
            info.AddValue("taskDescription", taskDescription);
        }

        public MethodTask(SerializationInfo info, StreamingContext context)
        {
            taskName = info.GetString("taskName");
            taskDescription = info.GetString("taskDescription");
        }

        #endregion

        #region Implementation of IAutoGenTask

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        public string TaskDescription
        {
            get { return taskDescription; }
            set { taskDescription = value; }
        }

        public ITaskControl TaskPropertiesControl
        {
            get
            {
                if (taskPropertiesControl == null || taskPropertiesControl.InnerControl.IsDisposed)
                    taskPropertiesControl = new MethodControl(this);
                return taskPropertiesControl;
            }
        }

        public TeXMLDocList GenerateTask(IAutoGenParameters Parameters, IAutoGenWorker Worker)
        {
            if (Worker.IsCanceled)
                return null;
            Worker.ReportProgress(0, "Начинаем генерацию задачи");

            TeXMLDoc myDocTask = new TeXMLDoc("Task");
            TeXMLDoc myDocResult = new TeXMLDoc("Result");

            TeXElement head = myDocTask.Root.Ins(
                new TeXRoot().PlainTeX().Ins(
                    new TeXText(
                        @"
\documentclass[12pt,a4paper]{article}
\usepackage[cp1251]{inputenc}
\usepackage{amsmath,amsfonts,amssymb,euscript}
\usepackage[russian]{babel}
\voffset=-1in   % Удаление верхнего поля драйвера в 1 дюйм
\topmargin=22mm % Верхнее (нижнее) поле в 22mm
\textheight=252mm       % Высота тела текста (290-22-26=272)
\headheight=0mm % место для колонтитула
\headsep=0mm    % отступ после колонтитула
\hoffset=-1in   % Удаление левого поля драйвера в 1 дюйм
\oddsidemargin=15mm     % Левое поле на нечетной странице, 15 мм
\evensidemargin=23mm    % Левое поле на четной странице, 23 мм
\textwidth=180.5mm      % Ширина тела текста, 218.5-15-23=180.5
")));

            TeXElement headRes = myDocResult.Root.Ins(new TeXRoot().PlainTeX().Ins(
                    new TeXText(
                        @"
\documentclass[12pt,a4paper]{article}
\usepackage[cp1251]{inputenc}
\usepackage{amsmath,amsfonts,amssymb,euscript}
\usepackage[russian]{babel}
\voffset=-1in   % Удаление верхнего поля драйвера в 1 дюйм
\topmargin=22mm % Верхнее (нижнее) поле в 22mm
\textheight=252mm       % Высота тела текста (290-22-26=272)
\headheight=0mm % место для колонтитула
\headsep=0mm    % отступ после колонтитула
\hoffset=-1in   % Удаление левого поля драйвера в 1 дюйм
\oddsidemargin=15mm     % Левое поле на нечетной странице, 15 мм
\evensidemargin=23mm    % Левое поле на четной странице, 23 мм
\textwidth=180.5mm      % Ширина тела текста, 218.5-15-23=180.5
")));
            Worker.ReportProgress(10, "Идет генерация по алгоритму");

            TeXElement body = new TeXEnv("document");
            TeXElement bodyRes = new TeXEnv("document");

            MethodGenerator g = new MethodGenerator();

            for (int i = 0; i < Parameters.Variants; i++)
            {
                body.Ins(new TeXEnv("center")
                             .Ins(new TeXCmd("bf").Params("Вариант " + (i + 1))));
                bodyRes.Ins(new TeXEnv("center")
                             .Ins(new TeXCmd("bf").Params("Вариант " + (i + 1))));
                TeXElement plain = new TeXRoot().PlainTeX();
                TeXElement en = new TeXEnv("enumerate");
                for (int j = 0; j < Parameters.CountInVariant; j++)
                {
                    if (Worker.IsCanceled)
                        return null;
                    Worker.ReportProgress(50, "Идет генерация: вариант " + (i + 1) + " задание " + (j + 1));
                    g.Generate();
                    string taskHead = @"\centerline{Задача $\No\,  " + (j + 1) + @"$}";
                    plain.Ins(new TeXText(taskHead + g.Output));
                    en.Ins(new TeXCmd("item").Gr(false))
                        .Ins(new TeXText(g.Answer));
                }
                bodyRes.Ins(en);
                body.Ins(plain);
            }

            Worker.ReportProgress(99, "Генерация задачи завершена");

            head.Ins(body);
            headRes.Ins(bodyRes);

            return new TeXMLDocList().Add(myDocTask).Add(myDocResult);
        }

        #endregion
    }

    public class MethodGenerator
    {
        private readonly int n = 10;
        private string output = "";
        private string answer = "";
        private readonly Random r = new Random();

        public string Output
        {
            get { return output; }
        }

        public string Answer
        {
            get { return answer; }
        }

        public void Generate()
        {
            output = string.Empty;
            answer = string.Empty;
            int a, b, c, x, y, x0, y0, z, u, v;
            char sign_u, sign_v, sign_c;
            string Str_a, Str_b, Str_c;

            a = 1 + r.Next(3);
            b = 1 + r.Next(3);
            c = -9 + r.Next(49);
            x = -5 + r.Next(11);
            y = -5 + r.Next(11);
            z = c - a*x*x - b*y*y;
            x0 = -5 + r.Next(11);
            y0 = -5 + r.Next(11);
            if (x == 0)
                x = 1;
            if (y == 0)
                y = -1;
            if (a == 2 && b == 2)
                b = 1;
            if (a == 3 && b == 3)
                b = 2;

            answer = "x="+x+", y="+y+", z="+z;
            u = -2*a*x;
            sign_u = u < 0 ? '-' : '+';
            v = -2*b*x;
            sign_v = v < 0 ? '-' : '+';
            sign_c = c < 0 ? '-' : '+';
            Str_a = a == 1 ? "" : a.ToString();
            Str_b = b == 1 ? "" : b.ToString();
            Str_c = c == 0 ? "" : sign_c + Math.Abs(c).ToString();
            output += @"
$$
\left\{\begin{array}{c}
";
            output += "z=" + Str_a + "x^2+" + Str_b + "y^2" + sign_u + Math.Abs(u) + "x" + sign_v + Math.Abs(v) + "y" +
                      Str_c + @"\,\to\,\min\\";
            output += @"
x_0=" + x0 + @",\quad y_0=" + y0 + @"\\
\end{array}\right.
$$";
        }
    }
}
