using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoGen.I;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace AutoGen.App
{
    public partial class TListForm : XtraForm
    {
        #region .:Private members

        private AutoGenData myAppData;
        private GridObject currentGO = new GridObject();
        private readonly Main myAppMainForm;
        private TaskObjectList tList = new TaskObjectList();

        public TListForm(Main mainForm)
        {
            InitializeComponent();
            myAppMainForm = mainForm;
        }

        private void FillDefaultList()
        {
            myAppData = AutoGenData.LoadData();
            if (myAppData != null && myAppData.AutoGenTaskArray != null)
                tList = new TaskObjectList(myAppData.AutoGenTaskArray);
        }

        public AutoGenData MyAppData
        {
            get { return myAppData; }
        }

        private TaskObject GetTOByID(int id)
        {
            foreach (TaskObject taskObject in tList)
            {
                if (taskObject.ID == id)
                    return taskObject;
            }
            return new TaskObject();
        }

        private List<GridObject> GetChildTasks(GridObject obj)
        {
            List<GridObject> cTasks = new List<GridObject>();
            GridObjectList oList = new GridObjectList();
            if (obj.ID > 0)
            {
                CreateBackTree(cTasks, obj);
                obj.IsOpened = true;
                obj.Img = Properties.Resources.FolderBack;
                cTasks.Add(obj);
            }
            foreach (TaskObject o in tList)
            {
                if (o.ParentID == obj.ID)
                {
                    oList.Add(new GridObject(o));
                }
            }
            oList.SortByFolders();
            cTasks.AddRange(oList.ToArray());
            return cTasks;
        }

        private void CreateBackTree(List<GridObject> tasks, GridObject obj)
        {
            if (obj.ParentID > 0)
            {
                GridObject gob = new GridObject(GetTOByID(obj.ParentID));
                CreateBackTree(tasks, gob);
                gob.IsOpened = true;
                gob.Img = Properties.Resources.FolderBack;
                tasks.Add(gob);
            }
        }

        private void BindGrid(List<GridObject> goList)
        {
            gridControl1.DataSource = goList;
        }

        private void TListForm_Load(object sender, EventArgs e)
        {
            FillDefaultList();
            BindGrid(GetChildTasks(currentGO));
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            GridHitInfo hi =
                gridView1.CalcHitInfo(((Control)sender).PointToClient(MousePosition));

            if (hi.RowHandle >= 0)
            {
                GridObject go = GetSelectedObject();
                if (go != null)
                {
                    if (go.IsFolder)
                    {
                        if (!go.IsOpened)
                        {
                            currentGO = go;
                        }
                        else
                        {
                            currentGO = new GridObject(GetTOByID(go.ParentID));
                        }
                        BindGrid(GetChildTasks(currentGO));
                    }
                    else EditCurrentObject();
                }
            }
        }

        #endregion

        public void BeforeClosing(object sender, FormClosingEventArgs e)
        {
            if (myAppData != null)
            {
                myAppData.AutoGenTaskArray = tList.ToArray();
                if (!myAppData.SaveData())
                    e.Cancel = true;
            }
        }

        public void NewFolder()
        {
            NameDLG nd = new NameDLG(null, true);
            if (nd.ShowDialog() == DialogResult.OK)
            {
                NewFolderCreate(nd.EditName);
            }
        }

        public void NewFolderCreate(string folderName)
        {
            TaskObject to = new TaskObject(folderName, true, null, null);
            to.ID = myAppData.IndexGenerator;
            to.ParentID = currentGO.ID;
            tList.Add(to);
            BindGrid(GetChildTasks(currentGO));
        }

        public void EditCurrentObject()
        {
            if (gridView1.SelectedRowsCount > 0)
            {
                GridObject goi = GetSelectedObject();
                if (goi != null)
                {
                    if (!goi.IsFolder)
                    {
                        myAppMainForm.ShowEditTaskTab(goi);
                    }
                    else if (!goi.IsOpened)
                    {
                        NameDLG ndF = new NameDLG(goi.TaskName, true);
                        if (ndF.ShowDialog() == DialogResult.OK)
                        {
                            TaskObject to = GetTOByID(goi.ID);
                            to.ObjName = ndF.EditName;
                            BindGrid(GetChildTasks(currentGO));
                        }
                    }
                }
            }
        }

        public GridObject GetSelectedObject()
        {
            return gridView1.GetRow(gridView1.FocusedRowHandle) as GridObject;
        }

        public UTForm MakeNewTaskForm(GridObject obj)
        {
            TaskObject to = GetTOByID(obj.ID);
            UTForm newFrom = new UTForm();
            newFrom.InitUserControl(to);
            newFrom.ChildTaskSaved += newFrom_ChildTaskChanged;
            return newFrom;
        }

        void newFrom_ChildTaskChanged(object sender, EventArgs e)
        {
            RefreshTree();
        }

        public void AddTaskInstance()
        {
            TaskRunDLG ndT = new TaskRunDLG(null, false, myAppMainForm.MyLoadedGenerators);
            if (ndT.ShowDialog() == DialogResult.OK)
            {
                TaskObject to =
                    new TaskObject(ndT.EditName, false, ndT.SelectedPlugin.CreateTaskInstance(ndT.EditName), ndT.SelectedPlugin);
                AddNewTaskItem(to);
            }
        }

        public void AddNewTaskItem(TaskObject to)
        {
            to.ID = myAppData.IndexGenerator;
            to.ParentID = currentGO.ID;
            tList.Add(to);
            BindGrid(GetChildTasks(currentGO));
        }

        public void RefreshTree()
        {
            BindGrid(GetChildTasks(currentGO));
        }

        public TeXML.TeXMLDoc StartSelectedTask(IAutoGenWorker Worker)
        {
            TaskObject to = GetTOByID(GetSelectedObject().ID);
            return to.Task.GenerateTask(new AutoGenParameters(), Worker);
        }

        GridHitInfo hitInfo = null;

        private void DoShowMenu(GridHitInfo info)
        {
            if (info.InRow && info.RowHandle >= 0)
            {
                gridView1.SelectRow(info.RowHandle);
                gridControl1.Refresh();
                TListGridRowMenu menu = new TListGridRowMenu(gridView1, this, true);
                menu.Init(info);
                menu.Show(info.HitPoint);
            }else if(info.HitTest==GridHitTest.EmptyRow)
            {
                gridView1.SelectRow(info.RowHandle);
                gridControl1.Refresh();
                TListGridRowMenu menu = new TListGridRowMenu(gridView1, this, false);
                menu.Init(info);
                menu.Show(info.HitPoint);
            }
        }

        private void gridControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (hitInfo == null) return;
            if (e.Button != MouseButtons.Left) return;
            Rectangle dragRect = new Rectangle(new Point(
                hitInfo.HitPoint.X - SystemInformation.DragSize.Width / 2,
                hitInfo.HitPoint.Y - SystemInformation.DragSize.Height / 2), SystemInformation.DragSize);
            if (!dragRect.Contains(new Point(e.X, e.Y)))
            {
                if (hitInfo.InRow && hitInfo.RowHandle >= 0)
                {
                    gridControl1.DoDragDrop(GetTOByID(GetSelectedObject().ID), DragDropEffects.Copy);
                }
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                //myAppMainForm.StartButtonEnabled = !GetSelectedObject().IsFolder;
            }
        }

        public void RemoveSelectedItem()
        {
            GridObject so = GetSelectedObject();
            if (so.IsFolder && so.IsOpened)
            {
                XtraMessageBox.Show("Эта запись не может быть удалена. Попробуйте из родительской папки.", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!so.IsFolder && XtraMessageBox.Show("Удаление экземпляра задачи.\nВы уверены?", "Удаление", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                myAppMainForm.CloseAllTabs(so.ID);
                tList.RemoveAt(tList.GetTaskByID(so.ID));
                RefreshTree();
            }
        }

        private void gridControl1_MouseUp(object sender, MouseEventArgs e)
        {
            hitInfo = gridView1.CalcHitInfo(new Point(e.X, e.Y));
            if (e.Button == MouseButtons.Right)
            {
                DoShowMenu(hitInfo);
            }
        }
    }
}