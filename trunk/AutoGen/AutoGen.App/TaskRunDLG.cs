using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutoGen.I;
using DevExpress.XtraEditors;

namespace AutoGen.App
{
    public partial class TaskRunDLG : XtraForm
    {
        private string _EditName = "";

        public string EditName
        {
            get { return _EditName; }
        }

        public IAutoGenGenerator SelectedPlugin
        {
            get
            {
                if (comboBoxEdit1.SelectedItem != null && comboBoxEdit1.SelectedItem is PluginContainer)
                    return ((PluginContainer) comboBoxEdit1.SelectedItem).Plugin as IAutoGenGenerator;
                return null;
            }
        }

        public TaskRunDLG(string objName, bool isFolder, IEnumerable<IAutoGenPlugin> myLoadedPlugins)
        {
            InitializeComponent();
            if (objName != null) _EditName = objName;
            if (isFolder)
                Text = "Папка";
            else
                Text = "Задача";
            textEdit1.Text = _EditName;
            PluginContainerList pList = new PluginContainerList();
            foreach (IAutoGenPlugin plugin in myLoadedPlugins)
                pList.Add(new PluginContainer(plugin));
            comboBoxEdit1.Properties.Items.AddRange(pList.ToArray());
            if (comboBoxEdit1.Properties.Items.Count < 1)
                simpleButton2.Enabled = false;
            else
                comboBoxEdit1.SelectedIndex = 0;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textEdit1.Text))
            {
                XtraMessageBox.Show(Properties.Resources.EmtyNameError, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else
            {
                _EditName = textEdit1.Text;
                DialogResult = DialogResult.OK;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    simpleButton2_Click(sender, EventArgs.Empty);
                    break;
                case Keys.Escape:
                    simpleButton1_Click(sender, EventArgs.Empty);
                    break;
            }
        }
    }
}