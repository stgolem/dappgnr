using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoGen.I;
using DevExpress.XtraEditors;

namespace AutoGen.App
{
    public partial class UTForm : XtraForm
    {
        private TaskObject _Task = new TaskObject();

        public int ID
        {
            get { return _Task.ID; }
        }

        public TaskObject Task
        {
            get { return _Task; }
        }

        public event EventHandler ChildTaskSaved;

        public UTForm()
        {
            InitializeComponent();
        }

        public void InitUserControl(TaskObject to)
        {
            _Task = to;
            Text = Task.TaskName;
            Task.Task.TaskPropertiesControl.InnerControl.Dock = DockStyle.Fill;
            Task.Task.TaskPropertiesControl.TaskSaved += TaskPropertiesControl_TaskSaved;
            Task.Task.TaskPropertiesControl.TaskChanged += TaskPropertiesControl_TaskChanged;
            panelControl1.Controls.Add(Task.Task.TaskPropertiesControl.InnerControl);
            Closing += Task.Task.TaskPropertiesControl.ParentTabClosing;
        }

        void TaskPropertiesControl_TaskChanged(ITaskControl sender, TaskChangeEventArgs args)
        {
            Text = args.TaskName + " *";
        }

        void TaskPropertiesControl_TaskSaved(object sender, TaskChangeEventArgs e)
        {
            Text = e.TaskName;
            if (ChildTaskSaved != null)
                ChildTaskSaved(sender, e);
        }
    }
}