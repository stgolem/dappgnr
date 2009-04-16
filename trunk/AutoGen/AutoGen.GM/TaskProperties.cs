using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using AutoGen.I;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace AutoGen.GM
{
    public partial class TaskProperties : XtraUserControl, ITaskControl
    {
        private GradientTask _Task;
        private bool hasChanges = false;

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
                textEdit3.Text = _Task.TaskName;
                textEdit4.Text = _Task.TaskAutor;
                textEdit1.Text = _Task.BaseTask.Prefix;
                textEdit2.Text = _Task.BaseTask.Suffix;
                memoEdit1.Text = _Task.BaseTask.Template;
                textEdit5.Text = _Task.BaseTask.Answer;
                textEdit6.Text = _Task.BaseTask.Lookfor;
                gridControl1.DataSource = _Task.BaseTask.Table;
                BindContentChanges();
            }
        }

        void BindContentChanges()
        {
            textEdit1.Properties.EditValueChanged += ContentChanged;
            textEdit2.Properties.EditValueChanged += ContentChanged;
            textEdit3.Properties.EditValueChanged += ContentChanged;
            textEdit4.Properties.EditValueChanged += ContentChanged;
            textEdit5.Properties.EditValueChanged += ContentChanged;
            textEdit6.Properties.EditValueChanged += ContentChanged;
            memoEdit1.Properties.EditValueChanged += ContentChanged;
            gridView1.RowUpdated += gridView1_RowUpdated;
        }

        private void ContentChanged(object sender, EventArgs e)
        {
            if(TaskChanged != null)TaskChanged(this, new TaskChangeEventArgs(textEdit3.Text));
            hasChanges = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_Task != null)
            {
                _Task.TaskName = textEdit3.Text;
                _Task.TaskAutor = textEdit4.Text;
                _Task.BaseTask.Prefix = textEdit1.Text;
                _Task.BaseTask.Suffix = textEdit2.Text;
                _Task.BaseTask.Template = memoEdit1.Text;
                _Task.BaseTask.Answer = textEdit5.Text;
                _Task.BaseTask.Lookfor = textEdit6.Text;
                if (TaskSaved != null) TaskSaved(this, new TaskChangeEventArgs(_Task.TaskName));
                hasChanges = false;
            }
        }

        #region ITaskControl Members

        public Control InnerControl
        {
            get { return this; }
        }

        public void ParentTabClosing(object sender, CancelEventArgs e)
        {
            if (hasChanges)
                if (XtraMessageBox.Show("Данные изменены.\nЗакрыть без сохранения?", "Внимание: " + _Task.TaskName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) e.Cancel = true;
        }

        public event TaskChangeEventHandler TaskSaved;
        public event TaskChangeEventHandler TaskChanged;

        #endregion

        private void gridView1_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            GMTaskProp prop = gridView1.GetRow(e.RowHandle) as GMTaskProp;
            if (prop != null)
            {
                prop.CanGenerate = true;
                prop.Measure = "ед";
                prop.Koeff = 1;
                prop.MinValue = 0;
                prop.MaxValue = 0;
                prop.Accuracy = 1;
            }
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            ContentChanged(sender, EventArgs.Empty);
        }

        public void DeleteSelectedRow()
        {
            if(gridView1.FocusedRowHandle>=0)
            {
                _Task.BaseTask.Table.RemoveAt(gridView1.FocusedRowHandle);
                gridControl1.RefreshDataSource();
            }
        }

        private void gridControl1_MouseUp(object sender, MouseEventArgs e)
        {
            GridHitInfo hitInfo = gridView1.CalcHitInfo(new Point(e.X, e.Y));
            if (e.Button == MouseButtons.Right)
            {
                DoShowMenu(hitInfo);
            }
        }

        private void DoShowMenu(GridHitInfo info)
        {
            if (info.InRow && info.RowHandle >= 0)
            {
                gridView1.SelectRow(info.RowHandle);
                gridControl1.Refresh();
                TaskTableGridMenu menu = new TaskTableGridMenu(gridView1, this);
                menu.Init(info);
                menu.Show(info.HitPoint);
            }
        }

        private void repositoryItemCalcEdit1_FormatEditValue(object sender, ConvertEditValueEventArgs e)
        {
            e.Value = Convert.ToDouble(e.Value).ToString("R");
        }
    }
}
