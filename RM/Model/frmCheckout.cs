using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.Model
{
    public partial class frmCheckout : SampleAdd
    {
        public frmCheckout()
        {
            InitializeComponent();
        }

        public double amt;
        public int MainID = 0;

        //private void txtRecieved_TextChanged(object sender, EventArgs e)
        //{
        //    double amt = 0;
        //    double reciept = 0;
        //    double change = 0;
        //    double.TryParse(txtBillAmount.Text, out amt);
        //    double.TryParse(txtRecieved.Text, out reciept);


        //    change = Math.Abs( amt - reciept); //convert positive or negative to always positive


        //    txtChange.Text = change.ToString();

        //}



        private void txtRecieved_TextChanged(object sender, EventArgs e)
        {
            double bill = amt; // or Convert.ToDouble(txtBillAmount.Text)
            double received;

            // If empty or invalid, disable Save
            if (!double.TryParse(txtRecieved.Text, out received) || received < bill)
            {
                txtChange.Text = "0.00";
                txtChange.ForeColor = Color.Red;
                btnSave.Enabled = false;
                return;
            }

            // Valid received amount
            double change = received - bill;
            txtChange.Text = change.ToString("N2");
            txtChange.ForeColor = Color.Green;
            btnSave.Enabled = true;
        }



        //public override void btnSave_Click(object sender, EventArgs e)
        //{
        //    string qry = @" update tblMain set total = @total , received = @rec , change = @change , status = 'Paid'
        //                  where MainID = @id";

        //    Hashtable ht = new Hashtable();
        //    ht.Add("@id", MainID);
        //    ht.Add("@total",txtBillAmount.Text);
        //    ht.Add("@rec", txtRecieved.Text);
        //    ht.Add("@change", txtChange.Text);

        //    if (MainClass.SQL(qry, ht) > 0)
        //    {
        //        guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
        //        guna2MessageDialog1.Show("Saved Successfull");
        //        this.Close();
        //    }


        //}


        public override void btnSave_Click(object sender, EventArgs e)
        {
            double bill = Convert.ToDouble(txtBillAmount.Text);
            double received = Convert.ToDouble(txtRecieved.Text);

            if (received < bill)
            {
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Warning;
                guna2MessageDialog1.Show("Received amount cannot be less than bill amount!");
                return;
            }

            double change = received - bill;

            string qry = @"
UPDATE tblMain 
SET total = @total,
    received = @rec,
    change = @change,
    status = 'Paid',
    PaidDate = GETDATE()
WHERE MainID = @id";


            Hashtable ht = new Hashtable();
            ht.Add("@id", MainID);
            ht.Add("@total", bill);
            ht.Add("@rec", received);
            ht.Add("@change", change);

            if (MainClass.SQL(qry, ht) > 0)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Show("Payment completed successfully");
                this.Close();
            }
        }

        private void txtRecieved_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void frmCheckout_Shown(object sender, EventArgs e)
        {
            txtRecieved.Focus();
        }



        private void frmCheckout_Load(object sender, EventArgs e)
        {
            txtBillAmount.Text = amt.ToString();
            btnSave.Enabled = false; // disabled until valid input
        }

       

    }
}
