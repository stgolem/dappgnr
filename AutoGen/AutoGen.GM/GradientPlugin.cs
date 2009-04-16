using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
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
            get { return new Guid(AssemblyGuid); }
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
    }

    [Serializable]
    public class GradientTask : IAutoGenTask
    {
        private string taskName = "";
        private string taskAutor = "";
        private string taskDescr = "";
        private ITaskControl taskPropertiesControl;
        private GMTask baseTask = null;
        [NonSerialized]private GradientPlugin hostPlugin = null;

        public GradientTask(string _taskName, GradientPlugin host)
        {
            taskName = _taskName;
            hostPlugin = host;
            taskPropertiesControl = new TaskProperties(this);
            baseTask = new GMTask();
        }

        public GradientTask()
        {
        }

        public string TaskAutor
        {
            get { return taskAutor; }
            set { taskAutor = value; }
        }

        public GMTask BaseTask
        {
            get { return baseTask; }
            set { baseTask = value; }
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

        public TeXMLDocList GenerateTask(IAutoGenParameters Parameters, IAutoGenWorker Worker)
        {
            if (Worker.IsCanceled) return null;
            Worker.ReportProgress(0, "�������� ��������� ������");

            TeXMLDoc myDoc = new TeXMLDoc("T");

            TeXElement head = myDoc.Root.Ins(
                new TeXRoot().PlainTeX().Ins(
                    new TeXText(
                        @"
\documentclass[12pt,a4paper]{article}
\usepackage[cp1251]{inputenc}
\usepackage{amsmath,amsfonts,amssymb,euscript}
\usepackage[russian]{babel}
\voffset=-1in   % �������� �������� ���� �������� � 1 ����
\topmargin=22mm % ������� (������) ���� � 22mm
\textheight=252mm       % ������ ���� ������ (290-22-26=272)
\headheight=0mm % ����� ��� �����������
\headsep=0mm    % ������ ����� �����������
\hoffset=-1in   % �������� ������ ���� �������� � 1 ����
\oddsidemargin=15mm     % ����� ���� �� �������� ��������, 15 ��
\evensidemargin=23mm    % ����� ���� �� ������ ��������, 23 ��
\textwidth=180.5mm      % ������ ���� ������, 218.5-15-23=180.5
")));
            GMGenerator generator = new GMGenerator();

            Worker.ReportProgress(10, "���� ��������� �� ���������");

            TeXElement body = new TeXEnv("document");

            for (int i = 0; i < Parameters.Variants; i++)
            {
                body.Ins(new TeXEnv("center")
                             .Ins(new TeXCmd("bf").Params("������� " + (i + 1))));
                TeXElement plain = new TeXRoot().PlainTeX();
                TeXElement en = new TeXEnv("enumerate");
                for (int j = 0; j < Parameters.CountInVariant; j++)
                {
                    if (Worker.IsCanceled)
                        return null;
                    Worker.ReportProgress(50, "���� ���������: ������� " + (i + 1) + " ������� " + (j + 1));

                    generator.Generate(BaseTask);
                    en.Ins(new TeXCmd("item").Gr(false))
                        .Ins(new TeXText(BaseTask.Output));
                }
                body.Ins(plain.Ins(en));
            }

            Worker.ReportProgress(99, "��������� ������ ���������");

            head.Ins(body);

            return new TeXMLDocList().Add(myDoc);
        }

        #endregion

        /*
            myDoc.Root
                .Ins(new TeXRoot().PlainTeX()
                         .Ins(new TeXText(@"
\documentclass[10pt,a5paper,twoside]{report}
\usepackage[cp1251]{inputenc}
\usepackage{amsmath,amsfonts,amssymb,euscript}
\usepackage[russian]{babel}
\voffset=-1in   % �������� �������� ���� �������� � 1 ����
\topmargin=22mm % ������� (������) ���� � 22mm
\textheight=162mm       % ������ ���� ������ (210-22-26=162)
\headheight=0mm % ����� ��� �����������
\headsep=0mm    % ������ ����� �����������
\hoffset=-1in   % �������� ������ ���� �������� � 1 ����
\oddsidemargin=15mm     % ����� ���� �� �������� ��������, 15 ��
\evensidemargin=23mm    % ����� ���� �� ������ ��������, 23 ��
\textwidth=110.5mm      % ������ ���� ������, 148.5-15-23=110.5
")))
                .Ins(new TeXEnv("document")
                         .Ins(new TeXEnv("center")
                                  .Ins(new TeXCmd("bf").Params("�������-�������������� � ��� ����������")))
                         .Ins(new TeXEnv("center")
                                  .Ins(new TeXGroup()
                                           .Ins(new TeXCmd("bf").Gr(false))
                                           .Ins(new TeXText(@"�.�. �����������, �.�. Ը������ 5 ����"))
                                  ))
                         .Ins(new TeXRoot().PlainTeX()
                                  .Ins(new TeXText(@"
�������-�������������� (��) ������ ����������� ��� ������������ �������������� 
�������� � ����������� ��������� �������.
 
��������������� ��������� �������� ����������� �������,
�������  ����� ������� ��������-��������� 
��������������.  
����� �� ����� �������� �������� ��� ������. ���� �� �������� 
������������� ��� --- �������� $\rm R$---$\rm R$ ����� �����
���������� ���� �� ������ ���������� $\rm R,$ ���������� ������� 
��������� ����������.
��� �������-�������������� �������������� ������ ������������� ����� ���������� 
�� �������� ��������, ���������� �� ���������� ��������� ����� ������, 
���������� � �������. ����� ������� �������� ����� �������-������������� (������), 
������� ������������ ����� ������� ���� ���������� --- �������� � ��������.   
������������ �������-������� �����������, ���� �������� �� ���������-��������� ��������� 
������ ��������� ��������� ���������� ������. ��� ���� ���������� ���
���������� ����� ���������� ������. �����������  ($\rm R$-����) ���������� ����� 
���������� �������, � ������� �������� ��������� ������ ��� ����� ���������.
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

� �������� �� � ����������� ������� �� ����� ���� � ����������� ���������.
�������-������������ ����������� �������-�������������� ���������� 
� ������� ��������� ������ ��������.
������������� ������� ������������ ��� ��������� 
������������� �� ��������� ���� ���������� �������������� ����������.

�������� � ���������� ����������� ����� �������� �������� ������ ������������ 
��� ������������� �����������.

��������� ������ �� ��������� �������� ������ ��������� 
������� � ��� ������������. ������� ����������� ����� ���� ������� ����� 
��������� ��������� ����������, �, �������������, ����� ������� ��������� 
����������� ����������� ��������������� ����������.

� ��������� ������� ���������� �� �������-��������� ������ � 
���������� ������������� ������� --- ��� �������� �������������� ��������. 
������ ��������������� ������ (�����������) ��������� ��������� �������������.
")))
                         .Ins(new TeXCmd("vfill").Gr(false).NlAfter().NlBefore())
                         .Ins(new TeXCmd("hrule").Gr(false).NlAfter())
                         .Ins(new TeXCmd("vspace").Params("2pt").NlAfter())
                         .Ins(new TeXRoot().Escape(false).Ligatures(true)
                                  .Ins(new TeXGroup()
                                           .Ins(new TeXCmd("small").Gr(false))
                                           .Ins(new TeXText(@"������� ������������: �.\,�. ���������, �.�.-�.�., ������")))));
 
 */

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(GradientTask));
            info.AddValue("taskName", taskName);
            info.AddValue("taskAutor", taskAutor);
            info.AddValue("taskDescr", taskDescr);
            info.AddValue("baseTask", baseTask);
        }

        public GradientTask(SerializationInfo info, StreamingContext context)
        {
            taskName = info.GetString("taskName");
            taskAutor = info.GetString("taskAutor");
            taskDescr = info.GetString("taskDescr");
            baseTask = (GMTask) info.GetValue("baseTask", typeof (GMTask));
        }

        #endregion
    }
}
