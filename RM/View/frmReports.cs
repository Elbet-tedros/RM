using RM.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.View
{
    public partial class frmReports : Form
    {
        public frmReports()
        {
            InitializeComponent();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            string qry = "SELECT * FROM products";

            SqlDataAdapter da = new SqlDataAdapter(qry, MainClass.con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            rptMenu cr = new rptMenu();

            cr.SetDataSource(dt);
            cr.Refresh();   // VERY IMPORTANT

            frmPrint frm = new frmPrint();
            frm.crystalReportViewer1.ReportSource = cr;
            frm.crystalReportViewer1.RefreshReport(); // better than Refresh()
            frm.Show();
        }


        private void btnStaff_Click(object sender, EventArgs e)
        {
            string qry = "SELECT * FROM Staff";

            SqlDataAdapter da = new SqlDataAdapter(qry, MainClass.con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            rptStaffList cr = new rptStaffList();
            cr.SetDataSource(dt);
            cr.Refresh();

            frmPrint frm = new frmPrint();
            frm.crystalReportViewer1.ReportSource = cr;
            frm.crystalReportViewer1.RefreshReport();
            frm.Show();
        }


        private void btnSaleCat_Click(object sender, EventArgs e)
        {
            frmSaleByCategory frm = new frmSaleByCategory();
            frm.ShowDialog();
        }

        private void frmReports_Load(object sender, EventArgs e)
        {
            
        }
    }
}
