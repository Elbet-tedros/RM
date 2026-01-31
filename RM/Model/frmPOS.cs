using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.Model
{
    public partial class frmPOS : Form
    {

        public frmPOS()
        {
            InitializeComponent();
        }

        public int MainID = 0;
        public string OrderType = "";
        public int driverID = 0;
        public string customerName = "";
        public string customerPhone = "";
        public string OrderStatus = "";





        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProductPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        //private void frmPOS_Load(object sender, EventArgs e)
        //{
        //    guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;


        //    AddCategory();

        //    ProductPanel.Controls.Clear();
        //    LoadProducts();

        //    btnKot.Enabled = false;   // 🔴 disabled initially
        //}

        private void frmPOS_Load(object sender, EventArgs e)
        {
            guna2MessageDialog1.Parent = this; // ✅ CENTER FIX

            guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;
            AddCategory();
            ProductPanel.Controls.Clear();
            LoadProducts();
            btnKot.Enabled = false;
            btnCheckout.Enabled = false;
        }


        private void AddCategory()
        {
            string qry = "Select * from Category";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            CategoryPanel.Controls.Clear();

            // 👉 ALL CATEGORIES BUTTON (ADD FIRST)
            Guna.UI2.WinForms.Guna2Button allBtn = new Guna.UI2.WinForms.Guna2Button();
            allBtn.FillColor = Color.FromArgb(50, 55, 89);
            allBtn.Size = new Size(180, 45);
            allBtn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            allBtn.Text = "All Categories";
            allBtn.Checked = true; // default selected
            allBtn.Click += new EventHandler(b_Click);

            CategoryPanel.Controls.Add(allBtn);




            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Guna.UI2.WinForms.Guna2Button b = new Guna.UI2.WinForms.Guna2Button();
                    b.FillColor = Color.FromArgb(50, 55, 89);
                    b.Size = new Size(180, 45);
                    b.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                    b.Text = row["catName"].ToString();

                    //event for click
                    b.Click += new EventHandler(b_Click);

                    CategoryPanel.Controls.Add(b);

                }


            }
        }

        private void b_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button b = (Guna.UI2.WinForms.Guna2Button)sender;

            if (b.Text == "All Categories")
            {
                txtSearch.Text = "1";
                txtSearch.Text = "";
                return;
            }



            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.PCategory.ToLower().Contains(b.Text.Trim().ToLower());
            }
        }

        private void AddItems(string id, string proID, string name, string cat, string price, Image pimage)
        {
            var w = new ucProduct()
            {
                PName = name,
                PPrice = price,
                PCategory = cat,
                PImage = pimage,
                id = Convert.ToInt32(proID)

            };
            ProductPanel.Controls.Add(w);

            w.onSelect += (ss, ee) =>
            {
                var wdg = (ucProduct)ss;


                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    //this will check if product already exist then a one to quantity and update price
                    if (Convert.ToInt32(item.Cells["dgvproID"].Value) == wdg.id)
                    {
                        item.Cells["dgvQty"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) + 1;
                        item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) *
                                                        double.Parse(item.Cells["dgvPrice"].Value.ToString());

                        GetTotal();
                        UpdateKotButtonState();
                        return;

                    }
                }

                //this line add new product 
                guna2DataGridView1.Rows.Add(new object[] { 0, 0, wdg.id, wdg.PName, 1, wdg.PPrice, wdg.PPrice });
                GetTotal();
                UpdateKotButtonState();
            };
            
        }


        //getting product from database 


        private void LoadProducts()
        {
            string qry = "Select * from products inner join category on catID = CategoryID ";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                Byte[] imagearray = (byte[])item["pImage"];
                byte[] imagebytearray = imagearray;

                AddItems("0", item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(),
                    item["pPrice"].ToString(), Image.FromStream(new MemoryStream(imagearray)));
            }

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach (var item in ProductPanel.Controls)
            {
                var pro = (ucProduct)item;
                pro.Visible = pro.PName.ToLower().Contains(txtSearch.Text.ToLower());
            }

        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //for searil no
            int count = 0;
            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }
        private void GetTotal()
        {
            double tot = 0;
            lblTotal.Text = "";
            foreach (DataGridViewRow item in guna2DataGridView1.Rows)
            {
                tot += double.Parse(item.Cells["dgvAmount"].Value.ToString());
            }
            lblTotal.Text = tot.ToString("N2");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            lblTotal.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            guna2DataGridView1.Rows.Clear();
            MainID = 0;
            lblTotal.Text = "00";
            OrderStatus = "";

            lblWaiter.Visible = false;
            lblTable.Visible = false;

            lblDriverName.Text =
            $"Customer Name: {customerName}  Phone: {customerPhone}";
            lblDriverName.Visible = false;

            UpdateKotButtonState();

        }

        private void BtnDelivery_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Delivery";
            UpdateKotButtonState();

            frmAddCustomer frm = new frmAddCustomer();
            frm.mainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);
            if (frm.txtName.Text != "")//as take away didnt have driver information
            {
                driverID = frm.driverID;
                lblDriverName.Text = "Customer Name: " + frm.txtName.Text + " Phone: " + frm.txtPhone.Text + " Driver: " + frm.cbDriver.Text;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;

            }

        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            OrderType = "Take Away";
            UpdateKotButtonState();

            frmAddCustomer frm = new frmAddCustomer();
            frm.mainID = MainID;
            frm.orderType = OrderType;
            MainClass.BlurBackground(frm);
            if (frm.txtName.Text != "")//as take away didnt have driver information
            {
                driverID = frm.driverID;
                lblDriverName.Text = "Customer Name: " + frm.txtName.Text + " Phone: " + frm.txtPhone.Text;
                lblDriverName.Visible = true;
                customerName = frm.txtName.Text;
                customerPhone = frm.txtPhone.Text;

            }
        }

        private void btnDin_Click(object sender, EventArgs e)
        {
            OrderType = "Din In";
            UpdateKotButtonState();
            lblDriverName.Visible=false;
            //need to create form for table selection and waiter selection 
            frmTableSelect frm = new frmTableSelect();
            MainClass.BlurBackground(frm);
            if (frm.TableName != "")
            {
                lblTable.Text = frm.TableName;
                lblTable.Visible = true;
            }
            else
            {
                lblTable.Text = "";
                lblTable.Visible = false;
            }
            frmWaiterSelect frm2 = new frmWaiterSelect();
            MainClass.BlurBackground(frm2);
            if (frm2.waiterName != "")
            {
                lblWaiter.Text = frm2.waiterName;
                lblWaiter.Visible = true;
            }
            else
            {
                lblWaiter.Text = "";
                lblWaiter.Visible = false;

            }

        }

        private void btnKot_Click(object sender, EventArgs e)
        {
            //save the data in database
            //create tables
            //need to add feild to table to store additional info
            string qry1 = ""; //main table
            string qry2 = ""; //Detail table

            int detailID = 0;

            if (MainID == 0) //Insert
            {
                qry1 = @"
INSERT INTO tblMain
(aData, aTime, TableName, WaiterName, status, orderType, total, received, change, driverID, CustName, CustPhone)
VALUES
(@aData, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @driverID, @CustName, @CustPhone);
SELECT SCOPE_IDENTITY();
";

                //this line will create recent add id value
            }
            else //update
            {
                qry1 = @"Update tblMain Set status = @status ,total = @total ,received = @received ,change = @change where MainID = @ID ";
            }





            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@aData", Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Pending");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text)); // as we only saving data for kichen value will update when payment recieved
            //double total = 0;
            //double.TryParse(lblTotal.Text, out total);
            //cmd.Parameters.AddWithValue("@total", total);

            cmd.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@driverID", driverID);
            cmd.Parameters.AddWithValue("@CustName", customerName);
            cmd.Parameters.AddWithValue("@CustPhone", customerPhone);


            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainID == 0) { MainID = Convert.ToInt32(cmd.ExecuteScalar()); } else { cmd.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {

                 detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

               

                if (detailID == 0) //insert 
                {
                    qry2 = @"INSERT INTO tblDetails VALUES ( @MainID, @proID, @qty, @price, @amount)";
                }
                else //update
                {
                    qry2 = @" Update tblDetails Set proID = @proID ,qty = @qty ,price = @price ,amount = @amount
                                where DetailID = @ID";
                }


                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteNonQuery();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }
              
               

            }

            guna2MessageDialog1.Show("Order Has Been Made");
            MainID = 0;
            detailID = 0;
            guna2DataGridView1.Rows.Clear();
            lblTotal.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";
            lblDriverName.Text = "";

            driverID = 0;
            customerName = "";
            customerPhone = "";


            OrderType = "";
            OrderStatus = "Pending";
            UpdateKotButtonState();



        }

        public int id = 0;
        private void btnBill_Click(object sender, EventArgs e)
        {
            frmBillList frm = new frmBillList();
            MainClass.BlurBackground(frm);



            if (frm.MainID > 0)
            {
                id = frm.MainID;
                MainID = frm.MainID;
                LoadEntries();
            }
        }

        private void LoadEntries()
        {
            string qry = @"Select * from tblMain m
                  inner join tblDetails d on m.MainID = d.MainID
                  inner join products p on p.pID = d.proID
                  where m.MainID = " + id + "";



            SqlCommand cmd2 = new SqlCommand(qry, MainClass.con);
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            da2.Fill(dt2);


            // ✅ SAFETY CHECK
            if (dt2.Rows.Count == 0)
            {
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Warning;
                guna2MessageDialog1.Show("No order details found for this bill.");
                return;
            }
            OrderType = dt2.Rows[0]["orderType"].ToString();
            OrderStatus = dt2.Rows[0]["status"].ToString();
            // ✅ Restore customer & driver info
            customerName = dt2.Rows[0]["CustName"].ToString();
            customerPhone = dt2.Rows[0]["CustPhone"].ToString();
            driverID = Convert.ToInt32(dt2.Rows[0]["driverID"]);
           



            // ✅ SAFE TO ACCESS dt2.Rows[0] BELOW


            if (dt2.Rows[0]["orderType"].ToString() == "Delivery")
            {
                BtnDelivery.Checked = true;
                lblWaiter.Visible = false;
                lblTable.Visible = false;

                lblDriverName.Text =
                $"Customer: {customerName}  Phone: {customerPhone}  ";
                lblDriverName.Visible = true;


            }
            else if (dt2.Rows[0]["orderType"].ToString() == "Take Away")
            {
                btnTake.Checked = true;
                lblWaiter.Visible = false;
                lblTable.Visible = false;

                lblDriverName.Text =
                $"Customer Name: {customerName}  Phone: {customerPhone}";
                lblDriverName.Visible = true;

            }
            else
            {
                btnDin.Checked = true;
                lblWaiter.Visible = true;
                lblTable.Visible = true;


                lblDriverName.Visible = false;

            }


            guna2DataGridView1.Rows.Clear();

            foreach (DataRow item in dt2.Rows)
            {
                lblTable.Text = item["TableName"].ToString();
                lblWaiter.Text = item["WaiterName"].ToString();


                string detailid = item["DetailID"].ToString();
                string proName = item["pName"].ToString();
                string proid = item["proID"].ToString();
                string qty = item["qty"].ToString();
                string price = item["price"].ToString();
                string amount = item["amount"].ToString();



                object[] obj = { 0, detailid, proid, proName, qty, price, amount };
                guna2DataGridView1.Rows.Add(obj);
            }

            GetTotal();


            UpdateKotButtonState();



        }


       

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            frmCheckout frm = new frmCheckout();
            frm.MainID = id;
            frm.amt = Convert.ToDouble(lblTotal.Text);
            MainClass.BlurBackground(frm);



            MainID = 0;
            guna2DataGridView1.Rows.Clear();
            lblTotal.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";
            OrderType = "";
            driverID = 0;
            customerName = "";
            customerPhone = "";

            OrderStatus = "Complete";
            UpdateKotButtonState();



        }

        private void btnHold_Click(object sender, EventArgs e)
        {

            //save the data in database
            //create tables
            string qry1 = ""; //main table
            string qry2 = ""; //Detail table

            int detailID = 0;

            if (MainID == 0) //Insert
            {
                
                //this line will create recent add id value


                qry1 = @" INSERT INTO tblMain
                         (aData, aTime, TableName, WaiterName, status, orderType, total, received, change, driverID, CustName, CustPhone)
                         VALUES
                         (@aData, @aTime, @TableName, @WaiterName, @status, @orderType, @total, @received, @change, @driverID, @CustName, @CustPhone);
                         SELECT SCOPE_IDENTITY();
                            ";




            }
            else //update
            {
                qry1 = @"Update tblMain Set status = @status ,total = @total ,received = @received ,change = @change where MainID = @ID ";
            }





            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("@ID", MainID);
            cmd.Parameters.AddWithValue("@aData", Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Hold");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text)); // as we only saving data for kichen value will update when payment recieved

            //double total = 0;
            //double.TryParse(lblTotal.Text, out total);
            //cmd.Parameters.AddWithValue("@total", total);


            cmd.Parameters.AddWithValue("@received", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));
            cmd.Parameters.AddWithValue("@driverID", driverID);
            cmd.Parameters.AddWithValue("@CustName", customerName);
            cmd.Parameters.AddWithValue("@CustPhone", customerPhone);



            if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
            if (MainID == 0) { MainID = Convert.ToInt32(cmd.ExecuteScalar()); } else { cmd.ExecuteNonQuery(); }
            if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {

                detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

                if (detailID == 0) //insert 
                {
                    qry2 = @"INSERT INTO tblDetails VALUES ( @MainID, @proID, @qty, @price, @amount)";
                }
                else //update
                {
                    qry2 = @" Update tblDetails Set proID = @proID ,qty = @qty ,price = @price ,amount = @amount
                                where DetailID = @ID";
                }


                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("@ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID", Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qty", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amount", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed) { MainClass.con.Open(); }
                cmd2.ExecuteNonQuery();
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

                
            }
            guna2MessageDialog1.Show("Saved successfuly");
            MainID = 0;
            detailID = 0;
            guna2DataGridView1.Rows.Clear();
            lblTotal.Text = "";
            lblWaiter.Text = "";
            lblTable.Visible = false;
            lblWaiter.Visible = false;
            lblTotal.Text = "00";
            lblDriverName.Text = "";
            OrderStatus = "Hold";
            UpdateKotButtonState();

        }


        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // prevent header click
            if (e.RowIndex < 0) return;

            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "dgvdel")
            {
                guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question;
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo;

                if (guna2MessageDialog1.Show("Are you sure you want to delete this item?") == DialogResult.Yes)
                {
                    int detailID = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["dgvid"].Value);

                    // 🔴 CASE 1: Item already saved in DB
                    if (detailID > 0)
                    {
                        string qry = "DELETE FROM tblDetails WHERE DetailID = @id";
                        Hashtable ht = new Hashtable();
                        ht.Add("@id", detailID);

                        MainClass.SQL(qry, ht);
                    }

                    // 🔴 CASE 2: Remove from grid (always)
                    guna2DataGridView1.Rows.RemoveAt(e.RowIndex);

                    // 🔴 Recalculate total
                    GetTotal();
                    UpdateKotButtonState();

                    guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
                    guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                    guna2MessageDialog1.Show("Item deleted successfully");
                }
            }
        }

        private void UpdateKotButtonState()
        {
            bool hasOrderType = !string.IsNullOrEmpty(OrderType);
            bool hasItems = guna2DataGridView1.Rows.Count > 0;

            bool isCompleted = OrderStatus == "Complete" || OrderStatus == "Paid";

            btnKot.Enabled = hasOrderType && hasItems && !isCompleted;
            // Checkout button: ONLY when order is complete
            btnCheckout.Enabled = (OrderStatus == "Complete");
        }






    }
}
