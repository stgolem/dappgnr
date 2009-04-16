using System;
using System.Collections.Generic;
using System.Text;

namespace AutoGen.GM
{
    public class GMString
    {
        private readonly List<GMStringParametr> items = null;
        private string template = "";
        private readonly GMTask task;

        public GMString(string template, GMTask _task)
        {
            this.template = template;
            items = new List<GMStringParametr>();
            task = _task;
        }

        public List<GMStringParametr> Items
        {
            get { return items; }
        }

        public string Template
        {
            get { return template; }
            set { template = value; }
        }

        public string Format()
        {
            string text = Template;
            foreach (GMStringParametr item in items)
            {
                text = text.Replace(task.Prefix + item.Name + task.Suffix, item.Value);
            }
            return text;
        }
    }

    public class GMStringParametr
    {
        private string name = "";
        private string value = "";

        public GMStringParametr(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
