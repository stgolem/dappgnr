using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using AutoGen.I;
using AutoGen.TeXML;

namespace AutoGen.GM
{
    public class GradientPlugin : IAutoGenGenerator
    {
        private readonly Version pluginVersion = new Version(0, 0, 0, 1);
        private readonly string autor = "Kononov Anton";
        private readonly string pluginName = "Plugin GM";
        [NonSerialized] private IAutoGenApplication hostApplication;
        private object generatorData = null;

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

        public void InitPlugin(IAutoGenApplication appl)
        {
            hostApplication = appl;
        }

        public void ShowAbout()
        {
            AboutGM agm = new AboutGM();
            agm.ShowDialog();
        }

        public Guid GUID
        {
            get { return new Guid("{9D28737F-36FA-4f32-AF50-4A3E03FD242F}"); }
        }

        public IAutoGenTask CreateTaskInstance(string taskName)
        {
            GradientTask nTask = new GradientTask(taskName, this);
            return nTask;
        }

        public object GeneratorData
        {
            get { return generatorData; }
            set { generatorData = value; }
        }

        #endregion
    }

    [Serializable]
    public class GradientTask : IAutoGenTask
    {
        private string taskName = "";
        private string taskAutor = "";
        private string taskDescr = "";
        private ITaskControl taskPropertiesControl;
        [NonSerialized]private GradientPlugin hostPlugin = null;

        public GradientTask(string _taskName, GradientPlugin host)
        {
            taskName = _taskName;
            hostPlugin = host;
            taskPropertiesControl = new TaskProperties(this);
        }

        public GradientTask()
        {
        }

        public string TaskAutor
        {
            get { return taskAutor; }
            set { taskAutor = value; }
        }

        #region IAutoGenTask Members

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        public string TaskDescription
        {
            get { return taskDescr; }
            set { taskDescr = value; }
        }

        public ITaskControl TaskPropertiesControl
        {
            get
            {
                if (taskPropertiesControl == null || taskPropertiesControl.InnerControl.IsDisposed)
                    taskPropertiesControl = new TaskProperties(this);
                return taskPropertiesControl;
            }
        }

        public TeXMLDoc GenerateTask(AutoGenParameters Parameters, IAutoGenWorker Worker)
        {
            if (Worker.IsCanceled) return null;
            Worker.ReportProgress(0, "Начинаем генерацию задачи");
            TeXMLDoc myDoc = new TeXMLDoc();
            myDoc.Root
                .Ins(new TeXRoot().PlainTeX()
                         .Ins(new TeXText(@"
\documentclass[10pt,a5paper,twoside]{report}
\usepackage[cp1251]{inputenc}
\usepackage{amsmath,amsfonts,amssymb,euscript}
\usepackage[russian]{babel}
\voffset=-1in   % Удаление верхнего поля драйвера в 1 дюйм
\topmargin=22mm % Верхнее (нижнее) поле в 22mm
\textheight=162mm       % Высота тела текста (210-22-26=162)
\headheight=0mm % место для колонтитула
\headsep=0mm    % отступ после колонтитула
\hoffset=-1in   % Удаление левого поля драйвера в 1 дюйм
\oddsidemargin=15mm     % Левое поле на нечетной странице, 15 мм
\evensidemargin=23mm    % Левое поле на четной странице, 23 мм
\textwidth=110.5mm      % Ширина тела текста, 148.5-15-23=110.5
")))
                .Ins(new TeXEnv("document")
                         .Ins(new TeXEnv("center")
                                  .Ins(new TeXCmd("bf").Params("ВЕЙВЛЕТ-ПРЕОБРАЗОВАНИЕ И ЕГО ПРИМЕНЕНИЕ")))
                         .Ins(new TeXEnv("center")
                                  .Ins(new TeXGroup()
                                           .Ins(new TeXCmd("bf").Gr(false))
                                           .Ins(new TeXText(@"Е.Н. Колесникова, М.А. Фёдорова 5 курс"))
                                  ))
                         .Ins(new TeXRoot().PlainTeX()
                                  .Ins(new TeXText(@"
Вейвлет-преобразование (ВП) широко применяется для исследования нестационарных 
сигналов и изображений различной природы.
 
Нестационарными сигналами являются медицинские сигналы,
которые  имеет сложные частотно-временные 
характеристики.  
Одним из таких сигналов является ЭКГ сигнал. Одна из основных 
характеристик ЭКГ --- интервал $\rm R$---$\rm R$ между двумя
следующими друг за другом импульсами $\rm R,$ отражающий частоту 
сердечных сокращений.
При вейвлет-преобразовании нестационарный сигнал анализируется путем разложения 
по базисным функциям, полученным из некоторого прототипа путем сжатий, 
растяжений и сдвигов. Таким образом получаем набор вейвлет-коэффициентов (спектр), 
который представляет собой функцию двух переменных --- масштаба и смещения.   
Избыточность вейвлет-спектра сокращается, если наносить на масштабно-временную плоскость 
только положения локальных максимумов модуля. При этом образуются так
называемые линии максимумов модуля. Особенности  ($\rm R$-пики) выделяются путем 
нахождения абсцисс, к которым сходятся максимумы модуля при малых масштабах.
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

В практике ВП в большинстве случаев мы имеем дело с дискретными сигналами.
Вейвлет-коэффициенты дискретного вейвлет-преобразования вычисляюся 
с помощью алгоритма набора фильтров.
Представление сигнала определяется как множество 
коэффициентов по масштабам плюс оставшаяся низкочастотная информация.

Перепады в структурах изображений часто являются наиболее важной особенностью 
при распознавании изображения.

Максимумы модуля ВП сохраняют свойства резких переходов 
сигнала и его особенностей. Порядок особенности может быть изменен путем 
изменения амплитуды максимумов, и, следовательно, можно удалить некоторые 
особенности подавлением соответствующих максимумов.

В двумерных случаях определяют ли вейвлет-максимумы полное и 
устойчивое представление сигнала --- еще открытая математическая проблема. 
Однако восстановленный сигнал (изображение) зрительно идентичен оригинальному.
")))
                         .Ins(new TeXCmd("vfill").Gr(false).NlAfter().NlBefore())
                         .Ins(new TeXCmd("hrule").Gr(false).NlAfter())
                         .Ins(new TeXCmd("vspace").Params("2pt").NlAfter())
                         .Ins(new TeXRoot().Escape(false).Ligatures(true)
                                  .Ins(new TeXGroup()
                                           .Ins(new TeXCmd("small").Gr(false))
                                           .Ins(new TeXText(@"Научный руководитель: А.\,А. Айзикович, к.ф.-м.н., доцент")))));
            Worker.ReportProgress(100, "Генерация задачи завершена");
            return myDoc;
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(GradientTask));
            info.AddValue("taskName", taskName);
            info.AddValue("taskAutor", taskAutor);
        }

        public GradientTask(SerializationInfo info, StreamingContext context)
        {
            taskName = info.GetString("taskName");
            taskAutor = info.GetString("taskAutor");
        }

        #endregion
    }
}
