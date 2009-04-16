using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AutoGen.I;
using AutoGen.PL;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace AutoGen.App
{
    public partial class ListPL : XtraForm
    {
        private readonly PluginLoader.AvailablePlugin[] programPlugins;
        private readonly IAutoGenPlugin[] myPlugins;
        private readonly IAutoGenPlugin[] myPrinters;
        private readonly IAutoGenApplication myApplication;

        public ListPL()
        {
            InitializeComponent();
        }

        public ListPL(PluginLoader.AvailablePlugin[] _programPlugins, IAutoGenPlugin[] _myGenerators, IAutoGenPlugin[] _myPrinters, IAutoGenApplication _myApplication)
        {
            InitializeComponent();
            programPlugins = _programPlugins;
            myPlugins = _myGenerators;
            myPrinters = _myPrinters;
            myApplication = _myApplication;
        }

        private void ListPL_Load(object sender, EventArgs e)
        {
            if (myApplication != null)
            {
                if (myPlugins != null && myPlugins.Length > 0)
                {
                    gridControl1.DataSource = new List<IAutoGenPlugin>(myPlugins);
                }
                if (myPrinters != null && myPrinters.Length > 0)
                {
                    gridControl2.DataSource = new List<IAutoGenPlugin>(myPrinters);
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowSelectedInfo();
        }

        private void ShowSelectedInfo()
        {
            IAutoGenPlugin iap = GetSelectedObject();
            if (iap != null) iap.ShowAbout();
        }

        private IAutoGenPlugin GetSelectedObject()
        {
            if (gridView1.IsFocusedView && gridView1.SelectedRowsCount > 0)
                return gridView1.GetRow(gridView1.FocusedRowHandle) as IAutoGenPlugin;
            if (gridView2.IsFocusedView && gridView2.SelectedRowsCount > 0)
                return gridView2.GetRow(gridView2.FocusedRowHandle) as IAutoGenPlugin;
            return null;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportPlugin();
        }

        private void ImportPlugin()
        {
            if (PluginManager.UploadPlugin(programPlugins))
            {
                gridControl1.RefreshDataSource();
                gridControl2.RefreshDataSource();
            }
        }

        private void gridContro_DoubleClick(object sender, EventArgs e)
        {
            GridHitInfo hi =
                (GridHitInfo) ((GridControl)sender).DefaultView.CalcHitInfo(((Control)sender).PointToClient(MousePosition));
            if (hi.RowHandle >= 0)
            {
                ShowSelectedInfo();
            }
        }
    }
}

