using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using AutoGen.I;

namespace AutoGen.App
{
    [Serializable]
    public class AutoGenPlayListItem
    {
        private TaskObject _TaskObject = null;
        private IAutoGenPrinter _Printer = null;
        private int _Count = 1;
        private bool _NeedGenerate = true;
        private int _Variants = 1;

        public AutoGenPlayListItem(TaskObject _TaskObject, IAutoGenPrinter _Printer)
        {
            this._TaskObject = _TaskObject;
            this._Printer = _Printer;
        }

        public AutoGenPlayListItem()
        {
        }

        public void From(PlayListObject pObject)
        {
            Count = pObject.Count;
            Variants = pObject.Variants;
            NeedGenerate = pObject.NeedGenerate;
            Printer = (IAutoGenPrinter) pObject.Printer.Plugin;
        }

        public TaskObject TaskObject
        {
            get { return _TaskObject; }
            set { _TaskObject = value; }
        }

        public IAutoGenPrinter Printer
        {
            get { return _Printer; }
            set { _Printer = value; }
        }

        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        public int Variants
        {
            get { return _Variants; }
            set { _Variants = value; }
        }

        public bool NeedGenerate
        {
            get { return _NeedGenerate; }
            set { _NeedGenerate = value; }
        }
    }

    [Serializable]
    public class AutoGenPlayList
    {
        private string playListName = "";
        private AutoGenPlayListItem[] itemCollection = null;

        public string PlayListName
        {
            get { return playListName; }
            set { playListName = value; }
        }

        public AutoGenPlayListItem[] ItemCollection
        {
            get { return itemCollection; }
            set { itemCollection = value; }
        }
    }

    [Serializable]
    public class PlayListObject
    {
        private AutoGenPlayListItem playListItem = null;
        private string _TaskName = "";
        private int _ID = -1;
        private PluginContainer _Printer = null;
        private int _Count = 0;
        private bool _NeedGenerate = true;
        private Image _Img = null;
        private int _Variants = 0;

        public PlayListObject(AutoGenPlayListItem plItem)
        {
            TaskName = plItem.TaskObject.TaskName;
            Printer = new PluginContainer(plItem.Printer);
            Count = plItem.Count;
            Variants = plItem.Variants;
            NeedGenerate = plItem.NeedGenerate;
            ID = plItem.TaskObject.ID;
            Img = Properties.Resources.Task;
            playListItem = plItem;
        }

        public AutoGenPlayListItem PlayListItem
        {
            get { return playListItem; }
        }

        public PlayListObject(int _ID, string _TaskName)
        {
            this._ID = _ID;
            this._TaskName = _TaskName;
            _Img = Properties.Resources.Task;
        }

        public string TaskName
        {
            get { return _TaskName; }
            set { _TaskName = value; }
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public PluginContainer Printer
        {
            get { return _Printer; }
            set { _Printer = value; }
        }

        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        public int Variants
        {
            get { return _Variants; }
            set { _Variants = value; }
        }

        public bool NeedGenerate
        {
            get { return _NeedGenerate; }
            set { _NeedGenerate = value; }
        }

        public Image Img
        {
            get { return _Img; }
            set { _Img = value; }
        }
    }
}
