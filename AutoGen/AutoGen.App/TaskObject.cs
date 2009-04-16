using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using AutoGen.I;

namespace AutoGen.App
{
    [Serializable]
    public class TaskObject
    {
        private IAutoGenTask _Task = null;
        private string _TaskPluginName = "";
        private string _TaskPluginVersion = "";
        private string _TaskPluginClass = "";
        private string _ObjName = "";
        private int _ID = -1;
        private int _ParentID = -1;
        private bool _IsFolder = false;

        public TaskObject(string _ObjName, bool _IsFolder, IAutoGenTask _Task, IAutoGenPlugin _Plugin)
        {
            Task = _Task;
            IsFolder = _IsFolder;
            ObjName = _ObjName;
            if (Task != null && _Plugin != null)
            {
                _TaskPluginClass = _Plugin.GetType().ToString();
                _TaskPluginName = _Plugin.PluginName;
                _TaskPluginVersion = _Plugin.PluginVersion.ToString();
            }
        }

        public TaskObject()
        {
        }

        public string TaskName
        {
            get
            {
                if (_Task != null)
                    return _Task.TaskName;
                return "";
            }
            set
            {
                if (_Task != null)
                    _Task.TaskName = value;
            }
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int ParentID
        {
            get { return _ParentID; }
            set { _ParentID = value; }
        }

        public bool IsFolder
        {
            get { return _IsFolder; }
            set { _IsFolder = value; }
        }

        public string PluginClass
        {
            get { return _TaskPluginClass; }
            set { _TaskPluginClass = value; }
        }

        public IAutoGenTask Task
        {
            get { return _Task; }
            set { _Task = value; }
        }

        public string ObjName
        {
            get
            {
                if (_Task != null)
                    return _Task.TaskName;
                return _ObjName;
            }
            set
            {
                if (_Task != null)
                    _Task.TaskName = value;
                _ObjName = value;
            }
        }

        public string TaskPluginName
        {
            get { return _TaskPluginName; }
        }

        public string TaskPluginVersion
        {
            get { return _TaskPluginVersion; }
        }
    }

    [Serializable]
    public class GridObject
    {
        private string _TaskName = "";
        private string _PluginName = "";
        private string _PluginVersion = "";
        private Image _Img = null;
        private int _ID = -1;
        private int _ParentID = -1;
        private bool _IsOpened = false;
        private bool _IsFolder = false;

        public GridObject()
        {
        }

        public GridObject(string _TaskName, Image _Img)
        {
            this._TaskName = _TaskName;
            this._Img = _Img;
        }

        public GridObject(TaskObject tObj)
        {
            _ID = tObj.ID;
            _ParentID = tObj.ParentID;
            _TaskName = tObj.ObjName;
            _PluginName = tObj.TaskPluginName;
            _PluginVersion = tObj.TaskPluginVersion;
            _IsFolder = tObj.IsFolder;
            _Img = tObj.IsFolder ? Properties.Resources.Folder : Properties.Resources.Task;
        }

        public string TaskName
        {
            get { return _TaskName; }
            set { _TaskName = value; }
        }

        public Image Img
        {
            get { return _Img; }
            set { _Img = value; }
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public int ParentID
        {
            get { return _ParentID; }
            set { _ParentID = value; }
        }

        public bool IsOpened
        {
            get { return _IsOpened; }
            set
            {
                _IsOpened = value;
            }
        }

        public bool IsFolder
        {
            get { return _IsFolder; }
            set { _IsFolder = value; }
        }

        public string PluginName
        {
            get { return _PluginName; }
            set { _PluginName = value; }
        }

        public string PluginVersion
        {
            get { return _PluginVersion; }
            set { _PluginVersion = value; }
        }
    }

    [Serializable]
    public class GridObjectList : List<GridObject>
    {
        public GridObjectList(IEnumerable<GridObject> collection)
            : base(collection)
        {
        }

        public GridObjectList(int capacity)
            : base(capacity)
        {
        }

        public GridObjectList()
        {
        }

        private static int CompareByFolder(GridObject go1, GridObject go2)
        {
            int go1F, go2F;
            go1F = go1.IsFolder ? 0 : 1;
            go2F = go2.IsFolder ? 0 : 1;
            if ((go1F - go2F) == 0)
            {
                return Math.Sign(go1.ID - go2.ID);
            }
            return go1F - go2F;
        }

        public void SortByFolders()
        {
            Sort(CompareByFolder);
        }
    }

    [Serializable]
    public class TaskObjectList : List<TaskObject>
    {
        public TaskObjectList(IEnumerable<TaskObject> collection)
            : base(collection)
        {
        }

        public TaskObjectList(int capacity)
            : base(capacity)
        {
        }

        public TaskObjectList()
        {
        }

        public TaskObject ByID(int id)
        {
            return Find(delegate(TaskObject to)
                            { return to.ID == id; });
        }

        public int GetTaskByID(int id)
        {
            return FindIndex(delegate(TaskObject to)
                                 { return to.ID == id; });
        }

        public int[] GetByFolderID(int id)
        {
            List<int> list = new List<int>();
            foreach (TaskObject o in this)
            {
                if (o.ParentID == id && !list.Contains(o.ID))
                    list.Add(o.ID);
            }
            return list.ToArray();
        }

        public int[] GetSubTreeByID(int id)
        {
            List<int> list = new List<int>();
            List<int> listRes = new List<int>();
            list.AddRange(GetByFolderID(id));
            listRes.AddRange(list);
            for (int i1 = 0; i1 < list.Count; i1++)
            {
                int i = list[i1];
                if (ByID(i).IsFolder)
                {
                    listRes.AddRange(GetSubTreeByID(i));
                }
            }
            return listRes.ToArray();
        }
    }
}
