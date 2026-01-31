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
    public partial class frmCategoryAdd : SampleAdd
    {
        public frmCategoryAdd()
        {
            InitializeComponent();
        }
        public int id = 0;

        public override void btnSave_Click(object sender, EventArgs e)
        {
            string qry = ""; 
            if (id == 0) //insert
            {
                qry = "Insert into category values(@Name) ";
            }
            else //update
            {
                qry = "Update category Set catName = @Name where catID = @id";
            }

            Hashtable ht = new Hashtable();
            ht.Add("@id",id);
            ht.Add("@Name", txtName.Text);

           if( MainClass.SQL(qry, ht) > 0)
            {
                // ✅ Dialog will now appear centered
                // ✅ Center Guna2MessageDialog on this form
                guna2MessageDialog1.Parent = this;
                guna2MessageDialog1.Show("Saved Successfully..");




                id= 0;
                txtName.Text = "";
                txtName.Focus();
            }
               


        }

       


    }
}
