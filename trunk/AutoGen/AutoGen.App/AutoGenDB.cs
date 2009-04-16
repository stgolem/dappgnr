using System.IO;
using System.Xml;
using AutoGen.I;

namespace AutoGen.App
{
    public class AutoGenDB
    {
        protected string configFile;

        public string ConfigFile
        {
            get { return configFile; }
        }
    }

    public class AutoGenDBSettings : AutoGenDB, IAutoGenDBSettings
    {
        //private static readonly string config = 

        public AutoGenDBSettings()
        {
            configFile = AutoGenBase.AppSaveDataPath + AutoGenBase.ConfigFile;
        }

        #region IAutoGenDBSettings Members

        public void SaveValue(string set, string name, string value)
        {
            XmlDocument xDB;

            if (!File.Exists(AutoGenBase.AppSaveDataPath + AutoGenBase.ConfigFile))
            {
                xDB = new XmlDocument();
                xDB.AppendChild(xDB.CreateElement("AutoGenSettings"));
                xDB.Save(ConfigFile);
            }

            xDB = new XmlDocument();
            xDB.Load(ConfigFile);

            XmlElement root = xDB.DocumentElement;
            XmlElement xSet = (XmlElement) root.SelectSingleNode(set);
            if (xSet == null)
            {
                xSet = xDB.CreateElement(set);
                root.AppendChild(xSet);
            }
            XmlElement xValue = (XmlElement) xSet.SelectSingleNode(name);
            if (xValue == null)
            {
                xValue = xDB.CreateElement(name);
                xSet.AppendChild(xValue);
            }
            xValue.SetAttribute("value", value);

            xDB.Save(ConfigFile);
        }

        public string GetValue(string set, string name)
        {
            XmlDocument xDB;

            if (!File.Exists(AutoGenBase.AppSaveDataPath + AutoGenBase.ConfigFile))
            {
                xDB = new XmlDocument();
                xDB.AppendChild(xDB.CreateElement("AutoGenSettings"));
                xDB.Save(ConfigFile);
            }

            xDB = new XmlDocument();
            xDB.Load(ConfigFile);

            XmlElement root = xDB.DocumentElement;
            XmlElement xSet = (XmlElement)root.SelectSingleNode(set);
            if (xSet == null)
            {
                return null;
            }
            XmlElement xValue = (XmlElement)xSet.SelectSingleNode(name);
            if (xValue == null)
            {
                return null;
            }
            return xValue.GetAttribute("value");
        }

        #endregion
    }
}
