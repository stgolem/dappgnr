using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;

namespace AutoGen.GM
{
    public class TaskTableGridMenu : GridViewMenu
    {
        private TaskProperties _TaskForm;
        public TaskTableGridMenu(GridView view, TaskProperties taskForm) : base(view)
        {
            _TaskForm = taskForm;
        }

        protected override void CreateItems()
        {
            Items.Clear();
            Items.Add(CreateMenuItem("Удалить", Properties.Resources.Delete16, "Delete", true));
        }

        protected override void OnMenuItemClick(object sender, EventArgs e)
        {
            if (RaiseClickEvent(sender, EventArgs.Empty)) return;
            DXMenuItem item = sender as DXMenuItem;
            if (item != null)
                switch(item.Tag.ToString())
                {
                    case "Delete":
                        _TaskForm.DeleteSelectedRow();
                        break;
                }
        }
    }
}
