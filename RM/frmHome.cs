using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM
{
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
        }
       /*
        private void frmHome_Load(object sender, EventArgs e)
        {
            Label testLabel = new Label();
            testLabel.Text = "Home Loaded!";
            testLabel.ForeColor = Color.Red;
            testLabel.Dock = DockStyle.Top;
            this.Controls.Add(testLabel);


        }*/

        private void frmHome_Load(object sender, EventArgs e)
        {
           /* Label lblHome = new Label();
            lblHome.Text = "Home Loaded!";
            lblHome.ForeColor = Color.Red;
            lblHome.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblHome.Dock = DockStyle.Fill;
            lblHome.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblHome);
           */
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
