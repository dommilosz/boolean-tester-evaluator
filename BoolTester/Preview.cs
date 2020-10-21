using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoolTester
{
    public partial class Preview : Form
    {
        public string exp_x = "";
        public string exp_y = "";
        public Preview()
        {
            InitializeComponent();
        }
        public Preview(string exp_x, string exp_y)
        {
            InitializeComponent();
            this.exp_x = exp_x;
            this.exp_y = exp_y;
        }
    }
}
