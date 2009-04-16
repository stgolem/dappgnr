using System;
using System.ComponentModel;
using System.Windows.Forms;
using AutoGen.I;
using DevExpress.XtraEditors;

namespace VI.MPS
{
    public partial class MethodControl : XtraUserControl, ITaskControl
    {
        private MethodTask _Task;
        private bool hasChanges = false;

        public MethodControl(MethodTask task)
        {
            InitializeComponent();
            _Task = task;
        }

        public MethodTask Task
        {
            get { return _Task; }
            set { _Task = value; }
        }

        private void MethodControl_Load(object sender, EventArgs e)
        {
            if (_Task != null)
            {
                textEdit1.Text = _Task.TaskName;
                textEdit1.Properties.EditValueChanged += Properties_EditValueChanged;
            }
        }

        void Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (TaskChanged != null)
                TaskChanged(this, new TaskChangeEventArgs(textEdit1.Text));
            hasChanges = true;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (_Task != null)
            {
                _Task.TaskName = textEdit1.Text;
                if (TaskSaved != null)
                    TaskSaved(this, new TaskChangeEventArgs(_Task.TaskName));
                hasChanges = false;
            }
        }

        #region ITaskControl Members

        /// <summary>
        /// Основной элемент, управляющий данными задачи
        /// </summary>
        public Control InnerControl
        {
            get { return this; }
        }

        /// <summary>
        /// Функция вызывается перед закрытием вкладки
        /// </summary>
        /// <param name="sender">Главная форма</param>
        /// <param name="e">Аргументы</param>
        public void ParentTabClosing(object sender, CancelEventArgs e)
        {
            if (hasChanges)
                if (XtraMessageBox.Show("Данные изменены.\nЗакрыть без сохранения?", "Внимание: " + _Task.TaskName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    e.Cancel = true;
        }

        /// <summary>
        /// Событие происходяшие в момент сохранения задачи
        /// </summary>
        public event TaskChangeEventHandler TaskSaved;

        /// <summary>
        /// Событие происходящие в момент изменения задачи
        /// </summary>
        public event TaskChangeEventHandler TaskChanged;

        #endregion

    }
}
