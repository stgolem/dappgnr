using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using AutoGen.I;

namespace AutoGen.App
{
    public class AutoGenBGWorker : IAutoGenWorker
    {
        private readonly BackgroundWorker backWorker;
        private DoWorkEventArgs args;

        public AutoGenBGWorker()
        {
            backWorker = new BackgroundWorker();
            backWorker.WorkerReportsProgress = true;
            backWorker.WorkerSupportsCancellation = true;
        }

        public BackgroundWorker BackWorker
        {
            get { return backWorker; }
        }

        public DoWorkEventArgs Args
        {
            get { return args; }
            set { args = value; }
        }

        public void RunWorker()
        {
            backWorker.RunWorkerAsync(this);
        }

        #region IAutoGenWorker Members

        public void ReportProgress(int ProgressPercent, string ProgressCaption)
        {
            backWorker.ReportProgress(ProgressPercent,
                                      new AGOutputArgs(null, ProgressCaption, AGOutputDirections.Status));
        }

        public void CancelProgress()
        {
            backWorker.CancelAsync();
            if (backWorker.CancellationPending)
            {
                args.Cancel = true;
                if (CancelSend != null) CancelSend(this, EventArgs.Empty);
            }
        }

        public void WriteOutputLine(string line)
        {
            backWorker.ReportProgress(0, new AGOutputArgs(line, null, AGOutputDirections.OutputWindow));
        }

        public event EventHandler CancelSend;

        public bool IsCanceled
        {
            get { return backWorker.CancellationPending; }
        }

        #endregion
    }

    public enum AGOutputDirections
    {
        None,
        Status,
        OutputWindow
    }

    public class AGOutputArgs
    {
        private string outputLine = string.Empty;
        private string outputCaption = string.Empty;
        private AGOutputDirections direction = AGOutputDirections.None;

        public string OutputLine
        {
            get { return outputLine; }
            set { outputLine = value; }
        }

        public string OutputCaption
        {
            get { return outputCaption; }
            set { outputCaption = value; }
        }

        public AGOutputDirections Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public AGOutputArgs(string outputLine, string outputCaption, AGOutputDirections direction)
        {
            this.outputLine = outputLine;
            this.outputCaption = outputCaption;
            this.direction = direction;
        }

        public AGOutputArgs(string outputLine)
        {
            this.outputLine = outputLine;
            direction = AGOutputDirections.OutputWindow;
        }
    }
}
