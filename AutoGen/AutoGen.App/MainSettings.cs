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
    public partial class MainSettings : XtraForm
    {
        private readonly Main myAppMainForm = null;
        //private int selectedPrinter = 0;

        public MainSettings(Main mainForm)
        {
            InitializeComponent();
            myAppMainForm = mainForm;
        }

        private void MainSettings_Load(object sender, EventArgs e)
        {
            if (myAppMainForm.MyAppData.MainProperties == null)
                myAppMainForm.MyAppData.MainProperties = new AutoGenProperties(myAppMainForm);
            PrinterSettings();
        }

        private void PrinterSettings()
        {
            PluginContainerList pList = new PluginContainerList();
            if (myAppMainForm.MyLoadedPrinters != null)
                foreach (IAutoGenPlugin printer in myAppMainForm.MyLoadedPrinters)
                {
                    pList.Add(new PluginContainer(printer));
                }
            printerComboBox.Properties.Items.AddRange(pList.ToArray());
            if (printerComboBox.Properties.Items.Count < 1)
                printerComboBox.Enabled = false;
            else printerComboBox.SelectedIndex = pList.GetByPlugin(myAppMainForm.MyAppData.MainProperties.DefaultPrinter);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            myAppMainForm.MyAppData.MainProperties.DefaultPrinter = (IAutoGenPrinter) ((PluginContainer)printerComboBox.SelectedItem).Plugin;
            Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}