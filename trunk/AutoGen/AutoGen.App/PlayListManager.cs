using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AutoGen.App
{
    public static class PlayListManager
    {
        private const string ListExtension = "agl";
        public static AutoGenPlayList LoadPlayList()
        {
            AutoGenPlayList res = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Плэйлист АвтоГенератора | *." + ListExtension;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if(File.Exists(ofd.FileName))
                {
                    FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                    try
                    {
                        List<byte> lBuff = new List<byte>();
                        while (fs.CanRead && fs.Position < fs.Length)
                        {
                            lBuff.Add((byte)fs.ReadByte());
                        }
                        byte[] buff = lBuff.ToArray();
                        res = ObjectFormatter.GetObject(buff) as AutoGenPlayList;
                    }
                    catch (Exception ex)
                    {
                        res = null;
                        XtraMessageBox.Show("Ошибка загрузки данных:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }
            return res;
        }

        public static bool SavePlayList(AutoGenPlayList list)
        {
            bool res = false;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.OverwritePrompt = true;
            sfd.Filter = "Плэйлист АвтоГенератора | *." + ListExtension;
            sfd.AddExtension = true;
            sfd.DefaultExt = ListExtension;
            sfd.FileName = list.PlayListName;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                list.PlayListName = Path.GetFileNameWithoutExtension(Path.GetFileName(sfd.FileName));
                try
                {
                    byte[] buff = ObjectFormatter.FormatObject(list);
                    for (int i = 0; i < buff.Length; i++)
                    {
                        fs.WriteByte(buff[i]);
                    }
                    res = true;
                }
                catch (Exception ex)
                {
                    res = false;
                    XtraMessageBox.Show("Ошибка сохранения данных:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    fs.Close();
                }
            }
            return res;
        }
    }
}
