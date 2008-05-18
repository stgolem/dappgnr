using System;
using System.ComponentModel;
using System.Windows.Forms;
using AutoGen.I;
using DevExpress.XtraEditors;

namespace AutoGen.GM
{
    public partial class TaskProperties : XtraUserControl, ITaskControl
    {
        private GradientTask _Task;

        public GradientTask Task
        {
            get { return _Task; }
            set { _Task = value; }
        }
        
        public TaskProperties()
        {
            InitializeComponent();
        }

        public TaskProperties(GradientTask Task)
        {
            InitializeComponent();
            _Task = Task;
        }

        private void TaskProperties_Load(object sender, EventArgs e)
        {
            if(_Task != null)
            {
                textBox1.Text = _Task.TaskName;
                textBox2.Text = _Task.TaskAutor;
            }
            textBox1.TextChanged += ContentChanged;
            textBox2.TextChanged += ContentChanged;
        }

        private void ContentChanged(object sender, EventArgs e)
        {
            if(TaskChanged != null)TaskChanged(this, new TaskChangeEventArgs(textBox1.Text));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_Task != null)
            {
                _Task.TaskName = textBox1.Text;
                _Task.TaskAutor = textBox2.Text;
                if (TaskSaved != null) TaskSaved(this, new TaskChangeEventArgs(_Task.TaskName));
            }
        }

        #region ITaskControl Members

        public Control InnerControl
        {
            get { return this; }
        }

        public void ParentTabClosing(object sender, CancelEventArgs e)
        {
            if (!_Task.TaskName.Equals(textBox1.Text) || !_Task.TaskAutor.Equals(textBox2.Text))
                if (MessageBox.Show("Данные изменены.\nЗакрыть без сохранения?", "Внимание: " + _Task.TaskName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) e.Cancel = true;
        }

        public event TaskChangeEventHandler TaskSaved;
        public event TaskChangeEventHandler TaskChanged;

        #endregion
    }
}
