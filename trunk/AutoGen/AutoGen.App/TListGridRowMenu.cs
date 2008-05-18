using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;

namespace AutoGen.App
{
    public class TListGridRowMenu : GridViewMenu
    {
        private readonly TListForm mListForm;
        private readonly bool forTaskRow = false;
        public TListGridRowMenu(GridView view, TListForm listForm, bool forRow)
            : base(view)
        {
            mListForm = listForm;
            forTaskRow = forRow;
        }

        protected override void CreateItems()
        {
            Items.Clear();
            if (forTaskRow)
            {
                Items.Add(CreateMenuItem("Изменить", Properties.Resources.Edit16, "Edit", true));
                Items.Add(CreateMenuItem("Удалить", Properties.Resources.Delete16, "Delete", true));
            }
            Items.Add(CreateMenuItem("Новая папка", Properties.Resources.NewFolder16, "Folder", true));
            Items.Add(CreateMenuItem("Новая задача", Properties.Resources.NewTask16, "Task", true));
        }

        protected override void OnMenuItemClick(object sender, EventArgs e)
        {
            if (RaiseClickEvent(sender, EventArgs.Empty)) return;
            DXMenuItem item = sender as DXMenuItem;
            if (item != null)
                switch(item.Tag.ToString())
                {
                    case "Edit":
                        mListForm.EditCurrentObject();
                        break;
                    case "Delete":
                        mListForm.RemoveSelectedItem();
                        break;
                    case "Folder":
                        mListForm.NewFolder();
                        break;
                    case "Task":
                        mListForm.AddTaskInstance();
                        break;
                }
        }
    }
}
