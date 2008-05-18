using System;
using System.Collections.Generic;
using System.Text;

namespace AutoGen.I
{
    public class TaskChangeEventArgs : EventArgs
    {
        public string TaskName;

        public TaskChangeEventArgs(string taskName)
        {
            TaskName = taskName;
        }
    }

    public delegate void TaskChangeEventHandler(ITaskControl sender, TaskChangeEventArgs args);
}
