namespace AutoGen.App
{
    partial class TListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.imgColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.nameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.plugNameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.plugVersionColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.AllowDrop = true;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.EmbeddedNavigator.Name = "";
            this.gridControl1.FormsUseDefaultLookAndFeel = false;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPictureEdit1,
            this.repositoryItemTextEdit1});
            this.gridControl1.Size = new System.Drawing.Size(570, 353);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl1.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragOver);
            this.gridControl1.DoubleClick += new System.EventHandler(this.gridControl1_DoubleClick);
            this.gridControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridControl1_MouseUp);
            this.gridControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragDrop);
            this.gridControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridControl1_MouseMove);
            this.gridControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridControl1_KeyPress);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.imgColumn,
            this.nameColumn,
            this.plugNameColumn,
            this.plugVersionColumn});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowColumnResizing = false;
            this.gridView1.OptionsMenu.EnableColumnMenu = false;
            this.gridView1.OptionsView.RowAutoHeight = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
            // 
            // imgColumn
            // 
            this.imgColumn.Caption = "...";
            this.imgColumn.ColumnEdit = this.repositoryItemPictureEdit1;
            this.imgColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.imgColumn.FieldName = "Img";
            this.imgColumn.MinWidth = 32;
            this.imgColumn.Name = "imgColumn";
            this.imgColumn.OptionsColumn.AllowEdit = false;
            this.imgColumn.OptionsColumn.AllowFocus = false;
            this.imgColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.imgColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.imgColumn.OptionsColumn.AllowMove = false;
            this.imgColumn.OptionsColumn.AllowSize = false;
            this.imgColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.imgColumn.OptionsColumn.FixedWidth = true;
            this.imgColumn.OptionsColumn.ReadOnly = true;
            this.imgColumn.OptionsColumn.ShowInCustomizationForm = false;
            this.imgColumn.OptionsFilter.AllowAutoFilter = false;
            this.imgColumn.OptionsFilter.AllowFilter = false;
            this.imgColumn.OptionsFilter.ImmediateUpdateAutoFilter = false;
            this.imgColumn.Visible = true;
            this.imgColumn.VisibleIndex = 0;
            this.imgColumn.Width = 32;
            // 
            // repositoryItemPictureEdit1
            // 
            this.repositoryItemPictureEdit1.AllowFocused = false;
            this.repositoryItemPictureEdit1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            this.repositoryItemPictureEdit1.ShowMenu = false;
            this.repositoryItemPictureEdit1.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.repositoryItemPictureEdit1.UseParentBackground = true;
            // 
            // nameColumn
            // 
            this.nameColumn.Caption = "Название задачи";
            this.nameColumn.ColumnEdit = this.repositoryItemTextEdit1;
            this.nameColumn.FieldName = "TaskName";
            this.nameColumn.MinWidth = 50;
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.OptionsColumn.AllowEdit = false;
            this.nameColumn.OptionsColumn.AllowFocus = false;
            this.nameColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.nameColumn.OptionsColumn.AllowMove = false;
            this.nameColumn.OptionsColumn.AllowSize = false;
            this.nameColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.nameColumn.OptionsColumn.ReadOnly = true;
            this.nameColumn.OptionsColumn.ShowInCustomizationForm = false;
            this.nameColumn.OptionsFilter.AllowAutoFilter = false;
            this.nameColumn.OptionsFilter.AllowFilter = false;
            this.nameColumn.OptionsFilter.ImmediateUpdateAutoFilter = false;
            this.nameColumn.ToolTip = "Название задачи";
            this.nameColumn.Visible = true;
            this.nameColumn.VisibleIndex = 1;
            this.nameColumn.Width = 367;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AllowFocused = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // plugNameColumn
            // 
            this.plugNameColumn.Caption = "Плагин";
            this.plugNameColumn.ColumnEdit = this.repositoryItemTextEdit1;
            this.plugNameColumn.FieldName = "PluginName";
            this.plugNameColumn.Name = "plugNameColumn";
            this.plugNameColumn.OptionsColumn.AllowEdit = false;
            this.plugNameColumn.OptionsColumn.AllowFocus = false;
            this.plugNameColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.plugNameColumn.OptionsColumn.AllowMove = false;
            this.plugNameColumn.OptionsColumn.AllowSize = false;
            this.plugNameColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.plugNameColumn.OptionsColumn.ReadOnly = true;
            this.plugNameColumn.OptionsColumn.ShowInCustomizationForm = false;
            this.plugNameColumn.OptionsFilter.AllowAutoFilter = false;
            this.plugNameColumn.OptionsFilter.AllowFilter = false;
            this.plugNameColumn.OptionsFilter.ImmediateUpdateAutoFilter = false;
            this.plugNameColumn.ToolTip = "Название плагина";
            this.plugNameColumn.Visible = true;
            this.plugNameColumn.VisibleIndex = 2;
            this.plugNameColumn.Width = 100;
            // 
            // plugVersionColumn
            // 
            this.plugVersionColumn.Caption = "Версия";
            this.plugVersionColumn.ColumnEdit = this.repositoryItemTextEdit1;
            this.plugVersionColumn.FieldName = "PluginVersion";
            this.plugVersionColumn.Name = "plugVersionColumn";
            this.plugVersionColumn.OptionsColumn.AllowEdit = false;
            this.plugVersionColumn.OptionsColumn.AllowFocus = false;
            this.plugVersionColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.plugVersionColumn.OptionsColumn.AllowMove = false;
            this.plugVersionColumn.OptionsColumn.AllowSize = false;
            this.plugVersionColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.plugVersionColumn.OptionsColumn.ReadOnly = true;
            this.plugVersionColumn.OptionsColumn.ShowInCustomizationForm = false;
            this.plugVersionColumn.OptionsFilter.AllowAutoFilter = false;
            this.plugVersionColumn.OptionsFilter.AllowFilter = false;
            this.plugVersionColumn.OptionsFilter.ImmediateUpdateAutoFilter = false;
            this.plugVersionColumn.ToolTip = "Версия плагина";
            this.plugVersionColumn.Visible = true;
            this.plugVersionColumn.VisibleIndex = 3;
            this.plugVersionColumn.Width = 100;
            // 
            // TListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 353);
            this.Controls.Add(this.gridControl1);
            this.LookAndFeel.SkinName = "Black";
            this.Name = "TListForm";
            this.Text = "Список задач";
            this.Load += new System.EventHandler(this.TListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn imgColumn;
        private DevExpress.XtraGrid.Columns.GridColumn nameColumn;
        protected DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
        protected DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn plugNameColumn;
        private DevExpress.XtraGrid.Columns.GridColumn plugVersionColumn;
    }
}