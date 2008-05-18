using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AutoGen.UC
{
    public class MyGridViewColumn : DataGridViewColumn
    {
        public MyGridViewColumn(DataGridViewCell cellTemplate) : base(cellTemplate)
        {
        }

        public MyGridViewColumn()
        {
        }

    }
}