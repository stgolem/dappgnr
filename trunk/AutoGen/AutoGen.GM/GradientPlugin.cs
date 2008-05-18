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
            Worker.ReportProgress(0, "�������� ��������� ������");
            TeXMLDoc myDoc = new TeXMLDoc();
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
            Worker.ReportProgress(100, "��������� ������ ���������");
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
