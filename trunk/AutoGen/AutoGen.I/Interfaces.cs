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

    public class AutoGenParameters
    {
        private int _CountInVariant;
        private int _Variants;
        private bool _NeedAnswer;

        public int CountInVariant
        {
            get { return _CountInVariant; }
            set { _CountInVariant = value; }
        }

        public int Variants
        {
            get { return _Variants; }
            set { _Variants = value; }
        }

        public bool NeedAnswer
        {
            get { return _NeedAnswer; }
            set { _NeedAnswer = value; }
        }

        public AutoGenParameters(int countInVariant, int variants, bool needAnswer)
        {
            CountInVariant = countInVariant;
            Variants = variants;
            NeedAnswer = needAnswer;
        }

        public AutoGenParameters()
        {
            CountInVariant = 1;
            Variants = 1;
            NeedAnswer = true;
        }
    }

    public interface IAutoGenApplication
    {
        object MainRibbonToolbar { get; }
        string MainApplicationDir { get; }
        string MainTexDir { get; }
        string MainTexPortDir { get; }
        string MainDataDir { get; }
    }

    public interface IAutoGenPlugin
    {
        Version PluginVersion { get; }
        string Autor { get; }
        string PluginName { get; }
        void InitPlugin(IAutoGenApplication autoGenApp);
        void ShowAbout();
        Guid GUID { get; }
    }

    public interface IAutoGenGenerator : IAutoGenPlugin
    {
        IAutoGenTask CreateTaskInstance(string taskName);
        Object GeneratorData { get; set; }
    }

    public interface IAutoGenTask : ISerializable
    {
        /// <summary>
        /// Name of the task instance
        /// </summary>
        string TaskName { get; set;}
        /// <summary>
        /// Description of the task instance
        /// </summary>
        string TaskDescription { get; set; }
        /// <summary>
        /// Control that provide access to task
        /// </summary>
        ITaskControl TaskPropertiesControl { get;}
        /// <summary>
        /// Generate task inside plugin with some parameters
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        TeXMLDoc GenerateTask(AutoGenParameters Parameters, IAutoGenWorker Worker);
    }

    public interface ITaskControl
    {
        /// <summary>
        /// Main control to manipulate task object data
        /// </summary>
        Control InnerControl{ get;}

        /// <summary>
        /// occurs before tab close
        /// </summary>
        /// <param name="sender">form</param>
        /// <param name="e">args</param>
        /// <returns>whether to close tab</returns>
        void ParentTabClosing(object sender, CancelEventArgs e);
        /// <summary>
        /// Must occur after task object saved
        /// </summary>
        event TaskChangeEventHandler TaskSaved;
        /// <summary>
        /// Must occur on task object changed
        /// </summary>
        event TaskChangeEventHandler TaskChanged;
    }

    public interface IAutoGenPrinter : IAutoGenPlugin
    {
        void Print(TeXMLDoc TeXDocument, IAutoGenWorker Worker);
        void ShowProperties();
    }

    public interface IAutoGenWorker
    {
        void ReportProgress(int ProgressPercent, string ProgressCaption);
        void CancelProgress();
        void WriteOutputLine(string line);
        event EventHandler CancelSend;
        bool IsCanceled { get; }
    }
}
