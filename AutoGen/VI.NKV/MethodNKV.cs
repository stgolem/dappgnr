using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using AutoGen.I;
using AutoGen.TeXML;
using DevExpress.XtraEditors;

namespace VI.NKV
{
    public class MethodNKV : IAutoGenGenerator
    {
        private object generatorData;
        private Version pluginVersion = new Version(0, 0, 1);
        private string autor = "Rodionov V.I.";
        private string pluginName = "Метод наименьших квадратов";
        [NonSerialized] private IAutoGenApplication hostApplication;
        private Guid guid = new Guid("{304AAE7C-2B62-47a0-9FF3-9F157343C685}");

        #region IAutoGenGenerator Members

        /// <summary>
        /// Получить новый объект Задачи
        /// </summary>
        /// <param name="taskName">Имя задачи</param>
        /// <returns>Объект Задачи для редактирования</returns>
        public IAutoGenTask CreateTaskInstance(string taskName)
        {
            return new MethodTask(taskName);
        }

        /// <summary>
        /// Дополнительные данные плагина
        /// </summary>
        public object GeneratorData
        {
            get { return generatorData; }
            set { generatorData = value; }
        }

        #endregion

        #region IAutoGenPlugin Members

        /// <summary>
        /// Версия
        /// </summary>
        public Version PluginVersion
        {
            get { return pluginVersion; }
        }

        /// <summary>
        /// Автор
        /// </summary>
        public string Autor
        {
            get { return autor; }
        }

        /// <summary>
        /// Короткое название
        /// </summary>
        public string PluginName
        {
            get { return pluginName; }
        }

        /// <summary>
        /// Произвести инициализацию объекта плагина
        /// </summary>
        /// <param name="autoGenApp">Главное приложение</param>
        public void InitPlugin(IAutoGenApplication autoGenApp)
        {
            hostApplication = autoGenApp;
        }

        /// <summary>
        /// Отобразить информацию о плагине
        /// </summary>
        public void ShowAbout()
        {
            XtraMessageBox.Show("Программа генерирует данные для расчета\nметодом наименьших квадратов\nАвтор: Родионов В.И.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Получить уникальный идентификатор плагина
        /// </summary>
        public Guid GUID
        {
            get { return guid; }
        }

        #endregion
    }

    [Serializable]
    public class MethodTask : IAutoGenTask
    {
        private string taskName;
        private string taskDescription;
        private ITaskControl taskPropertiesControl;


        public MethodTask(string taskName)
        {
            this.taskName = taskName;
            taskPropertiesControl = new MethodControl(this);
        }

        #region IAutoGenTask Members

        /// <summary>
        /// Название
        /// </summary>
        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        /// <summary>
        /// Описание
        /// </summary>
        public string TaskDescription
        {
            get { return taskDescription; }
            set { taskDescription = value; }
        }

        /// <summary>
        /// Объект элемента управления
        /// </summary>
        public ITaskControl TaskPropertiesControl
        {
            get
            {
                if (taskPropertiesControl == null || taskPropertiesControl.InnerControl.IsDisposed)
                    taskPropertiesControl = new MethodControl(this);
                return taskPropertiesControl;
            }
        }

        /// <summary>
        /// Произвести генерацию задачи
        /// </summary>
        /// <param name="Parameters">Параметры</param>
        /// <param name="Worker">Обработчик</param>
        /// <returns>Объект формата TeXML</returns>
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

        #region ISerializable Members

        ///<summary>
        ///Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
        ///</summary>
        ///
        ///<param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization. </param>
        ///<param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data. </param>
        ///<exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
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
            output = answer = string.Empty;
            //Получаем значок & для подстановки в документ
            string amp = string.Empty;
            StringBuilder sb = new StringBuilder();
            new TeXSpec(TeXSpec.SpecSimbols.Align).RenderElement(new StringWriter(sb));
            amp = sb.ToString();
            int a, b, i, k, R1, R2, count;
            int[] ind, x, y, z, delta;
            bool Ok;
            ind = new int[n];
            x = new int[n];
            y = new int[n];
            z = new int[n];
            delta = new int[n];
            a = -5 + r.Next(10);
            b = -10 + r.Next(20);
            count = 0;
            while (count != 6)
            {
                count = 0;
                for (i = 0; i < n; i++)
                {
                    ind[i] = r.Next(2);
                    x[i] = i;
                    y[i] = a * i + b;
                    if (ind[i] == 1)
                        count++;
                }
            }
            Ok = true;
            while (Ok)
            {
                for (i = 0; i < n; i++)
                    if (ind[i] == 1)
                    {
                        delta[i] = -1 + r.Next(3);
                        z[i] = y[i] + delta[i];
                    }
                R1 = R2 = 0;
                for (i = 0; i < n; i++)
                    if (ind[i] == 1)
                    {
                        R1 += x[i] * delta[i];
                        R2 += delta[i];
                    }
                if (R1 == 0 && R2 == 0)
                {
                    Ok = false;
                    answer = "a=" + a + ", b=" + b;
                    output = @"
$$
\begin{tabular}{| c | c | c | c | c | c | c |} \hline
$  i=$";
                    k = 0;
                    
                    for (i = 0; i < n; i++)
                        if (ind[i] == 1)
                        {
                            k++;
                            output += @" " + amp + " " + k;
                        }
                    output += @" \\  \hline
$x_i=$";
                    for (i = 0; i < n; i++)
                        if (ind[i] == 1)
                            output += @" " + amp + " " + x[i];
                    output += @" \\  \hline
$y_i=$";
                    for (i = 0; i < n; i++)
                        if (ind[i] == 1)
                            output += @" " + amp + " " + z[i];
                    output += @" \\  \hline
\end{tabular}
$$
";
                }
            }
        }
    }
}
