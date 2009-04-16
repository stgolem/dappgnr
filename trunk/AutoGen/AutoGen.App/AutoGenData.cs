using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using AutoGen.I;

namespace AutoGen.App
{
    [Serializable]
    public class AutoGenData
    {
        private TaskObject[] autoGenTaskArray = null;
        private int indexGenerator = 1;
        private AutoGenPlayList lastPlayList = null;
        [NonSerialized] private AutoGenProperties mainProperties = null;
        private AutogenCustomData customData = null;

        public TaskObject[] AutoGenTaskArray
        {
            get { return autoGenTaskArray; }
            set { autoGenTaskArray = value; }
        }

        public int IndexGenerator
        {
            get { return indexGenerator++; }
        }

        public AutoGenPlayList LastPlayList
        {
            get { return lastPlayList; }
            set { lastPlayList = value; }
        }

        public AutoGenProperties MainProperties
        {
            get { return mainProperties; }
            set { mainProperties = value; }
        }

        public AutogenCustomData CustomData
        {
            get { return customData; }
            set { customData = value; }
        }

        public bool SaveData(Main main)
        {
            bool res;
            if (!Directory.Exists(AutoGenBase.AppSaveDataPath))
                Directory.CreateDirectory(AutoGenBase.AppSaveDataPath);
            FileStream fs = new FileStream(AutoGenBase.AppSaveDataPath + AutoGenBase.SaveFile, FileMode.Create);
            try
            {
                byte[] buff = ObjectFormatter.FormatObject(this);
                for (int i = 0; i < buff.Length; i++)
                {
                    fs.WriteByte(buff[i]);
                }
                res = true;
            } catch (Exception ex)
            {
                res = false;
                XtraMessageBox.Show("Ошибка сохранения данных:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                fs.Close();
            }
            if (MainProperties != null)
                MainProperties.Save(main.MainDBSettings);
            return res;
        }

        public static AutoGenData LoadData(Main main)
        {
            AutoGenData agd = null;
            if(!File.Exists(AutoGenBase.AppSaveDataPath + AutoGenBase.SaveFile))
            {
                agd = new AutoGenData();
                agd.SaveData(main);
                return agd;
            }

            FileStream fs = new FileStream(AutoGenBase.AppSaveDataPath + AutoGenBase.SaveFile, FileMode.Open, FileAccess.Read);
            try
            {
                List<byte> lBuff = new List<byte>();
                while(fs.CanRead && fs.Position < fs.Length)
                {
                    lBuff.Add((byte) fs.ReadByte());
                }
                byte[] buff = lBuff.ToArray();
                agd = ObjectFormatter.GetObject(buff) as AutoGenData;
                if (agd != null)
                {
                    agd.MainProperties = AutoGenProperties.Load(main.MainDBSettings);
                }
            } catch (Exception ex)
            {
                agd = null;
                XtraMessageBox.Show("Ошибка загрузки данных:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally
            {
                fs.Close();
            }
            return agd;
        }
    }

    [Serializable]
    public class AutoGenProperties
    {
        private Guid defaultPrinter = Guid.Empty;
        private bool needUseTex = true;

        public Guid DefaultPrinterGuid
        {
            get { return defaultPrinter; }
            set { defaultPrinter = value; }
        }

        public bool NeedUseTex
        {
            get { return needUseTex; }
            set { needUseTex = value; }
        }

        public AutoGenProperties(Main main)
        {
            if (main.MyLoadedPrinters != null && main.MyLoadedPrinters.Length > 0)
                defaultPrinter = main.MyLoadedPrinters[0].GUID;
        }

        public AutoGenProperties(Guid defaultPrinter)
        {
            this.defaultPrinter = defaultPrinter;
        }

        public void Save(IAutoGenDBSettings db)
        {
            db.SaveValue("AutoGenProperties", "DefaultPrinter", DefaultPrinterGuid.ToString());
            db.SaveValue("AutoGenProperties", "NeedUseTex", NeedUseTex.ToString());
        }

        public static AutoGenProperties Load(IAutoGenDBSettings db)
        {
            Guid g = Guid.Empty;
            if (!string.IsNullOrEmpty(db.GetValue("AutoGenProperties", "DefaultPrinter")))
                g = new Guid(db.GetValue("AutoGenProperties", "DefaultPrinter"));
            AutoGenProperties agp = new AutoGenProperties(g);
            if (!string.IsNullOrEmpty(db.GetValue("AutoGenProperties", "NeedUseTex")))
                agp.NeedUseTex = bool.Parse(db.GetValue("AutoGenProperties", "NeedUseTex"));
            return agp;
        }
    }

    [Serializable]
    public class GeneratorData
    {
        private string className = "";
        private object data = null;

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        public GeneratorData()
        {
        }

        public GeneratorData(string className, object data)
        {
            this.className = className;
            this.data = data;
        }
    }

    [Serializable]
    public class AutogenCustomData : List<GeneratorData>
    {
        public AutogenCustomData(IEnumerable<GeneratorData> collection) : base(collection)
        {
        }

        public AutogenCustomData(int capacity) : base(capacity)
        {
        }

        public AutogenCustomData()
        {
        }

        public GeneratorData FindByClassName(string className)
        {
            return Find(delegate(GeneratorData data)
                                 {
                                     return data.ClassName.Equals(className);
                                 });
        }
    }
}
