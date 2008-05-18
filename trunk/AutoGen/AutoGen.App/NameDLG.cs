using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AutoGen.App
{
    public partial class NameDLG : XtraForm
    {
        private string _EditName = "";

        public string EditName
        {
            get { return _EditName; }
        }

        public NameDLG(string objName, bool isFolder)
        {
            InitializeComponent();
            if (objName != null) _EditName = objName;
            if (isFolder)
                Text = "Папка";
            else
                Text = "Задача";
            textEdit1.Text = _EditName;
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