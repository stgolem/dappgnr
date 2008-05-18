using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoGen.I;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace AutoGen.App
{
    public partial class PlayList : XtraForm
    {
        private readonly Main myAppMainForm = null;
        private List<PlayListObject> playListItems = new List<PlayListObject>();
        private List<AutoGenPlayListItem> playListItemCollection = null;
        private bool isOpened = false;

        public bool IsOpened
        {
            get { return isOpened; }
        }

        public PlayList(Main mainForm)
        {
            InitializeComponent();
            myAppMainForm = mainForm;
            barProgress.Visibility = BarItemVisibility.Never;
            barButtonHide.Visibility = BarItemVisibility.Never;
            barButtonItem3.Visibility = BarItemVisibility.Never;
            outputDockPanel.Visibility = DockVisibility.Hidden;
        }

        private void PlayList_Load(object sender, EventArgs e)
        {
            FillLastPlayList();
            FillPrinterList();
            Dock = DockStyle.Right;
            isOpened = true;
        }

        private void FillPrinterList()
        {
            PluginContainerList pList = new PluginContainerList();
            foreach (IAutoGenPlugin printer in myAppMainForm.MyLoadedPrinters)
            {
                pList.Add(new PluginContainer(printer));
            }
            printerComboSelect.Items.AddRange(pList.ToArray());
            if (printerComboSelect.Items.Count < 1)
                printerComboSelect.Enabled = false;
        }

        private void FillLastPlayList()
        {
            if (myAppMainForm.MyAppData.LastPlayList != null)
            {
                playListItemCollection = new List<AutoGenPlayListItem>(myAppMainForm.MyAppData.LastPlayList.ItemCollection);
                playListItems = MakePloList(playListItemCollection);
            }
            else
            {
                playListItemCollection = new List<AutoGenPlayListItem>();
                myAppMainForm.MyAppData.LastPlayList = new AutoGenPlayList();
                myAppMainForm.MyAppData.LastPlayList.PlayListName = "Новый список";
            }
            Text = myAppMainForm.MyAppData.LastPlayList.PlayListName;
            gridControl1.DataSource = playListItems;
        }

        private static List<PlayListObject> MakePloList(IEnumerable<AutoGenPlayListItem> items)
        {
            List<PlayListObject> res = new List<PlayListObject>();
            foreach (AutoGenPlayListItem item in items)
            {
                res.Add(new PlayListObject(item));
            }
            return res;
        }

        private void gridControl1_DragDrop(object sender, DragEventArgs e)
        {
            AutoGenPlayListItem item = new AutoGenPlayListItem((TaskObject)e.Data.GetData(typeof(TaskObject)), myAppMainForm.MyAppData.MainProperties.DefaultPrinter);
            if (item.TaskObject != null)
            {
                if (!item.TaskObject.IsFolder)
                {
                    GridHitInfo hi =
                        gridView1.CalcHitInfo(((Control)sender).PointToClient(MousePosition));
                    if (hi.RowHandle < 0)
                    {
                        playListItemCollection.Add(item);
                        ((List<PlayListObject>)gridControl1.DataSource).Add(new PlayListObject(item));
                    }
                    else
                    {
                        playListItemCollection.Insert(hi.RowHandle, item);
                        ((List<PlayListObject>)gridControl1.DataSource).Insert(hi.RowHandle, new PlayListObject(item));
                    }
                    playListItems = MakePloList(playListItemCollection);
                    gridControl1.RefreshDataSource();
                }
            }
        }

        private void gridControl1_DragEnter(object sender, DragEventArgs e)
        {
            TaskObject to = (TaskObject) e.Data.GetData(typeof (TaskObject));
            if (!to.IsFolder)
            {
                Focus();
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void PlayList_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool canClose = true;
            if (InGeneration)
            {
                if (XtraMessageBox.Show("Генерация еще не завершена. Прервать и закрыть список?", "Прервать генерацию?", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK)
                {
                    canClose = true;
                    if (bwork.BackWorker.IsBusy)
                        bwork.CancelProgress();
                }
                else canClose = false;
            }
            if (canClose)
            {
                for (int i = 0; i < ((List<PlayListObject>)gridControl1.DataSource).Count; i++)
                {
                    PlayListObject listObject = ((List<PlayListObject>)gridControl1.DataSource)[i];
                    playListItemCollection[i].From(listObject);
                }
                myAppMainForm.MyAppData.LastPlayList.ItemCollection = playListItemCollection.ToArray();
            }
            else e.Cancel = true;
            isOpened = !canClose;
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            StartListGenerate();
        }

        private readonly string defaultStatus = "Готов";
        private AutoGenBGWorker bwork;
        private bool InGeneration = false;
        public void StartListGenerate()
        {
            barProgress.Visibility = BarItemVisibility.Always;
            barButtonHide.Visibility = BarItemVisibility.Always;
            barButtonItem3.Visibility = BarItemVisibility.Always;
            outputDockPanel.Visibility = DockVisibility.Visible;
            outputMemoEdit.Text = "";
            ControlBox = false;
            gridControl1.Enabled = false;
            bar2.Visible = false;
            InGeneration = true;
            SetStatus("Начинаем генерацию");
            bwork = new AutoGenBGWorker();
            bwork.BackWorker.WorkerReportsProgress = true;
            bwork.BackWorker.WorkerSupportsCancellation = true;
            bwork.BackWorker.DoWork += bw_DoWork;
            bwork.BackWorker.RunWorkerCompleted += bw_RunWorkerCompleted;
            bwork.BackWorker.ProgressChanged += bw_ProgressChanged;
            bwork.RunWorker();
        }

        void OutputSend(AGOutputArgs args)
        {
            outputMemoEdit.Text += args.OutputLine + "\r\n";
            outputMemoEdit.Select(outputMemoEdit.Text.Length - 1, 0);
            outputMemoEdit.ScrollToCaret();
        }

        public void SetStatus(string statusText)
        {
            barStaticText.Caption = statusText;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AGOutputArgs arg = e.UserState as AGOutputArgs;
            if (arg != null)
            {
                switch (arg.Direction)
                {
                    case AGOutputDirections.Status:
                        SetStatus(arg.OutputCaption + "(" + e.ProgressPercentage + @"%)");
                        break;
                    case AGOutputDirections.OutputWindow:
                        OutputSend(arg);
                        break;
                }
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            barProgress.Visibility = BarItemVisibility.Never;
            barButtonHide.Visibility = BarItemVisibility.Never;
            barButtonItem3.Visibility = BarItemVisibility.Never;
            bar2.Visible = true;
            gridControl1.Enabled = true;
            myAppMainForm.PlayListShow();
            SetStatus(defaultStatus);
            if (e.Error != null) OutputSend(new AGOutputArgs(e.Error.Message));
            if (bwork.IsCanceled || e.Cancelled) OutputSend(new AGOutputArgs("Прервано пользователем"));
            else
            {
                if (!Visible) Show(myAppMainForm);
                foreach (Form ownedForm in OwnedForms)
                {
                    ownedForm.Close();
                }
                string finalText = "Генерация завершена";
                XtraMessageBox.Show(this, finalText, "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            OutputSend(new AGOutputArgs(""));
            InGeneration = false;
            ControlBox = true;
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            AutoGenBGWorker agbw = e.Argument as AutoGenBGWorker;
            if (agbw != null) agbw.Args = e;

            StartSelectedTasks((IAutoGenWorker)e.Argument);
        }

        private void StartSelectedTasks(IAutoGenWorker worker)
        {
            for (int i = 0; i < ((List<PlayListObject>)gridControl1.DataSource).Count; i++)
            {
                if (worker.IsCanceled) return;
                PlayListObject listObject = ((List<PlayListObject>)gridControl1.DataSource)[i];
                if (listObject.NeedGenerate)
                {
                    AutoGenParameters agp = new AutoGenParameters();
                    agp.CountInVariant = listObject.Count;
                    listObject.Printer.Plugin.InitPlugin(myAppMainForm);
                    ((IAutoGenPrinter)listObject.Printer.Plugin).Print(
                        listObject.PlayListItem.TaskObject.Task.GenerateTask(agp, worker), worker);
                }
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            DeleteSelected();
        }

        private void DeleteSelected()
        {
            for (int i = gridView1.GetSelectedRows().Length - 1; i >= 0; i--)
            {
                int row = gridView1.GetSelectedRows()[i];
                gridView1.UnselectRow(row);
                ((List<PlayListObject>)gridControl1.DataSource).RemoveAt(row);
                playListItemCollection.RemoveAt(row);
            }
            playListItems = MakePloList(playListItemCollection);
            gridControl1.RefreshDataSource();
        }

        public PlayListObject GetSelectedObject()
        {
            return gridView1.GetRow(gridView1.FocusedRowHandle) as PlayListObject;
        }

        private void barButtonHide_ItemClick(object sender, ItemClickEventArgs e)
        {
            HideList();
        }

        private void HideList()
        {
            Visible = false;
            Hide();
            myAppMainForm.PlayListHide();
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("Вы верены что хотите прервать текущее задание генерации?", "Прерывание", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                bwork.CancelProgress();
            }
        }

        private void PlayList_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
        }

        private void PlayList_Deactivate(object sender, EventArgs e)
        {
            Opacity = 0.7;
        }

        private void gridControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((Keys)e.KeyChar)
            {
                case Keys.Escape:
                    if (InGeneration) HideList();
                    else Close();
                    break;
            }
        }

        private void outputMemoEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((Keys)e.KeyChar)
            {
                case Keys.Escape:
                    if (InGeneration) HideList();
                    else Close();
                    break;
            }
        }

        private void gridControl1_MouseClick(object sender, MouseEventArgs e)
        {
            GridHitInfo hi =
                gridView1.CalcHitInfo(((Control)sender).PointToClient(MousePosition));
            if (hi.InColumn && hi.Column == imgColumn && e.Button == MouseButtons.Left)
            {
                if (imgColumn.Tag == null) imgColumn.Tag = false;
                bool flag = (bool) imgColumn.Tag;
                for (int i = 0; i < ((List<PlayListObject>) gridControl1.DataSource).Count; i++)
                {
                    PlayListObject listObject = ((List<PlayListObject>) gridControl1.DataSource)[i];
                    listObject.NeedGenerate = !flag;
                    playListItemCollection[i].NeedGenerate = !flag;
                }
                imgColumn.Tag = !flag;
                playListItems = MakePloList(playListItemCollection);
                gridControl1.RefreshDataSource();
            }
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            SavePlayList();
        }

        private void SavePlayList()
        {
            AutoGenPlayList list = new AutoGenPlayList();
            list.ItemCollection = playListItemCollection.ToArray();
            list.PlayListName = myAppMainForm.MyAppData.LastPlayList.PlayListName;
            PlayListManager.SavePlayList(list);
            Text = list.PlayListName;
            myAppMainForm.MyAppData.LastPlayList = list;
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadPlayList();
        }

        private void LoadPlayList()
        {
            AutoGenPlayList list = PlayListManager.LoadPlayList();
            if (list != null)
            {
                Text = list.PlayListName;
                myAppMainForm.MyAppData.LastPlayList = list;
                playListItemCollection = new List<AutoGenPlayListItem>(list.ItemCollection);
                playListItems = MakePloList(playListItemCollection);
                gridControl1.DataSource = playListItems;
                gridControl1.RefreshDataSource();
            }
        }
    }
}
