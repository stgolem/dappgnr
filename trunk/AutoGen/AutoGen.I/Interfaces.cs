using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using AutoGen.TeXML;

namespace AutoGen.I
{
    public static class AutoGenInterfaces
    {
        private static readonly string iAutoGenPlugin = "AutoGen.I.IAutoGenPlugin";
        private static readonly string iAutoGenApplication = "AutoGen.I.IAutoGenApplication";
        private static readonly string iAutoGenGenerator = "AutoGen.I.IAutoGenGenerator";
        private static readonly string iAutoGenTask = "AutoGen.I.IAutoGenTask";
        private static readonly string iAutoGenTaskControl = "AutoGen.I.ITaskControl";
        private static readonly string iAutoGenPrinter = "AutoGen.I.IAutoGenPrinter";

        public static string IAutoGenPlugin
        {
            get { return iAutoGenPlugin; }
        }

        public static string IAutoGenApplication
        {
            get { return iAutoGenApplication; }
        }

        public static string IAutoGenTask
        {
            get { return iAutoGenTask; }
        }

        public static string IAutoGenTaskControl
        {
            get { return iAutoGenTaskControl; }
        }

        public static string IAutoGenGenerator
        {
            get { return iAutoGenGenerator; }
        }

        public static string IAutoGenPrinter
        {
            get { return iAutoGenPrinter; }
        }
    }

    /// <summary>
    /// ��������� ���������� ���������
    /// </summary>
    public interface IAutoGenParameters
    {
        /// <summary>
        /// ���������� ������� � ��������
        /// </summary>
        int CountInVariant { get; set; }
        /// <summary>
        /// ���������� ���������
        /// </summary>
        int Variants { get; set; }
        /// <summary>
        /// ����� �� ����� �� ������
        /// </summary>
        bool NeedAnswer { get; set; }
        /// <summary>
        /// �������� ������
        /// </summary>
        string TaskName { get; set; }
    }
    /// <summary>
    /// ��������� �������������� ������ � ������ 
    /// ��������� ������ � ���������������� ����
    /// </summary>
    public interface IAutoGenDBSettings
    {
        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <param name="set">��� ������ ������</param>
        /// <param name="name">��������</param>
        /// <param name="value">������</param>
        void SaveValue(string set, string name, string value);
        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="set">��� ������ ������</param>
        /// <param name="name">��������</param>
        /// <returns>������</returns>
        string GetValue(string set, string name);
    }
    /// <summary>
    /// ��������� �������� ����������
    /// </summary>
    public interface IAutoGenApplication
    {
        /// <summary>
        /// ������������� ������ ������� � ����������
        /// </summary>
        IAutoGenDBSettings MainDBSettings { get; }
        /// <summary>
        /// ������� ����� ���������
        /// </summary>
        object MainForm { get; }
        /// <summary>
        /// ���������� ����������
        /// </summary>
        string MainApplicationDir { get; }
        /// <summary>
        /// ������������� \���
        /// </summary>
        string MainTexDir { get; }
        /// <summary>
        /// ������������ ����������� ������ ���
        /// </summary>
        string MainTexPortDir { get; }
        /// <summary>
        /// ������������ ������ ������
        /// </summary>
        string MainDataDir { get; }
    }
    /// <summary>
    /// ��������� ������� - ����������
    /// </summary>
    public interface IAutoGenPlugin
    {
        /// <summary>
        /// ������
        /// </summary>
        Version PluginVersion { get; }
        /// <summary>
        /// �����
        /// </summary>
        string Autor { get; }
        /// <summary>
        /// �������� ��������
        /// </summary>
        string PluginName { get; }
        /// <summary>
        /// ���������� ������������� ������� �������
        /// </summary>
        /// <param name="autoGenApp">������� ����������</param>
        void InitPlugin(IAutoGenApplication autoGenApp);
        /// <summary>
        /// ���������� ���������� � �������
        /// </summary>
        void ShowAbout();
        /// <summary>
        /// �������� ���������� ������������� �������
        /// </summary>
        Guid GUID { get; }
    }
    /// <summary>
    /// ��������� ���������� - ����������
    /// </summary>
    public interface IAutoGenGenerator : IAutoGenPlugin
    {
        /// <summary>
        /// �������� ����� ������ ������
        /// </summary>
        /// <param name="taskName">��� ������</param>
        /// <returns>������ ������ ��� ��������������</returns>
        IAutoGenTask CreateTaskInstance(string taskName);
        /// <summary>
        /// �������������� ������ �������
        /// </summary>
        Object GeneratorData { get; set; }
    }
    /// <summary>
    /// ��������� ���������� ������ ��� ���������
    /// </summary>
    public interface IAutoGenTask : ISerializable
    {
        /// <summary>
        /// ��������
        /// </summary>
        string TaskName { get; set;}
        /// <summary>
        /// ��������
        /// </summary>
        string TaskDescription { get; set; }
        /// <summary>
        /// ������ �������� ����������
        /// </summary>
        ITaskControl TaskPropertiesControl { get;}
        /// <summary>
        /// ���������� ��������� ������
        /// </summary>
        /// <param name="Parameters">���������</param>
        /// <param name="Worker">����������</param>
        /// <returns>������ ������� TeXML</returns>
        TeXMLDocList GenerateTask(IAutoGenParameters Parameters, IAutoGenWorker Worker);
    }
    /// <summary>
    /// ��������� �������� ���������� �������
    /// </summary>
    public interface ITaskControl
    {
        /// <summary>
        /// �������� �������, ����������� ������� ������
        /// </summary>
        Control InnerControl { get; }

        /// <summary>
        /// ������� ���������� ����� ��������� �������
        /// </summary>
        /// <param name="sender">������� �����</param>
        /// <param name="e">���������</param>
        void ParentTabClosing(object sender, CancelEventArgs e);
        /// <summary>
        /// ������� ������������ � ������ ���������� ������
        /// </summary>
        event TaskChangeEventHandler TaskSaved;
        /// <summary>
        /// ������� ������������ � ������ ��������� ������
        /// </summary>
        event TaskChangeEventHandler TaskChanged;
    }
    /// <summary>
    /// ��������� ������ ��������
    /// </summary>
    public interface IAutoGenPrinter : IAutoGenPlugin
    {
        /// <summary>
        /// ��������� ������ ���������
        /// </summary>
        /// <param name="TeXDocumentList">�������� � ������� TeXML</param>
        /// <param name="Worker">����������</param>
        /// <param name="Parameters">���������</param>
        void Print(TeXMLDocList TeXDocumentList, IAutoGenWorker Worker, IAutoGenParameters Parameters);
        /// <summary>
        /// ���������� ��������� ������
        /// </summary>
        void ShowProperties();
    }
    /// <summary>
    /// ��������� ���������� ����������� �������
    /// </summary>
    public interface IAutoGenWorker
    {
        /// <summary>
        /// �������� � ����������
        /// </summary>
        /// <param name="ProgressPercent">������� ����������</param>
        /// <param name="ProgressCaption">������ ����������</param>
        void ReportProgress(int ProgressPercent, string ProgressCaption);
        /// <summary>
        /// �������� ��������� � ���
        /// </summary>
        /// <param name="line">������ ���������</param>
        void WriteOutputLine(string line);
        /// <summary>
        /// ������� ������ ������ ����������
        /// </summary>
        void CancelProgress();
        /// <summary>
        /// ����������, ����� ���-���� �� ����������
        /// �������� ������ ������
        /// </summary>
        event EventHandler CancelSend;
        /// <summary>
        /// ������� ����, ��� �������� ������
        /// </summary>
        bool IsCanceled { get; }
    }
}
