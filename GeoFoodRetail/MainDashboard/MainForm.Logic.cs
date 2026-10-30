/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * Splitting the MainForm class into parts (partial classes) to 
 * break up the form's excessively long code.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace GeoFoodRetail
{
    internal partial class MainForm
    {
        /// <summary>
        /// connect to DB instance Oracle
        /// Connection string. 
        /// connString = "User Id=my_user;Password=my_password;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));";
        /// "User Id=c##gis_user;Password=i27g65oR;Data Source=localhost:1521/oracl.gis;";  <== open unsecure style  DON`T DELETE this line! 
        /// </summary>

        private string _cnnStr = /** "User Id=c##gis_user;Password=i27g65oR;Data Source=localhost:1521/oracl.gis;";  <== open unsecure style  DON`T DELETE this line! */
            ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Server=myServer;Database=myDB;";
        
        // Connection to DB
        private void ConnectMyOracleDB()
        {
            using (var cnn = new OracleConnection(_cnnStr))
            {
                try
                {
                    cnn.Open();
                    MessageBox.Show("Connection to Oracle successfully established!","Connection to Database Test",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    // Here we can call methods for writing or reading data
                }
                catch (Exception x)
                {
                    MessageBox.Show("Connection error: " + x.Message);
                }
            }
        }        

        // avoid type more 2 digits after '.'    
        public void StopEnterDigits()
        {
            int a = textBox1.Text.IndexOf('.');
            int lng = textBox1.Text.Length;

            if (a > -1 && (lng - a) > 3)
            {
                textBox1.Text = textBox1.Text.Remove(a + 3);
                textBox1.SelectionStart = textBox1.Text.Length;
            }
        }

        // user typed the overflow number
        public void OverFlowNum()
        {
            Position.FindAndMoveMsgBox(450, 420, true, "Overflow");
            MessageBox.Show("The number you entered exeeds the maximum number that can be calculated by the computer.\n\nPlease, _pay attention to your entered value in the text fields.", "Overflow");
        }

        /** load the text strings to ListBox 
         * from the Meal.txt file.  */
        void FillListView()
        {            
            var lines = File.ReadAllLines(Application.StartupPath + "\\Meal.txt");
            
            for (int i = 0; i + 1 < lines.Length; i += 2)
            {
                listView1.Items.AddRange(new[] {
                        new ListViewItem(new[] { lines[i], lines[i + 1], lines[i + 2] })
                    });
            }
            // resize the columns
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.Columns[i].Width = -2;
            }
            textBox5.TextChanged += new EventHandler(textBox5_TextChanged);
        }

        // fill in the Food name text field
        private string NameFoodGet()
        {
            string nm;
            try
            {
                listView1.Focus();
                var fullrow = listView1.FullRowSelect.Equals(false);

                ListViewItem item = listView1.SelectedItems[0];
                textBox5.Text = item.Text;
                textBox5.Text = item.SubItems[0].Text;
                nm = textBox5.Text;
                return nm;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                listView1.Focus();
                var fullrow = listView1.FullRowSelect.Equals(false);
                return string.Empty;
            }
        }

        // compute Pretax amount - Do not touch!!!
        private decimal Pretax()
        {
            try
            {
                Payment.Price = Convert.ToDecimal(textBox1.Text);
                Payment.Quantity = Convert.ToDecimal(textBox2.Text);

                Payment.StorePayment += decimal.Round(Payment.Price * Payment.Quantity, Payment.Nmb, MidpointRounding.ToEven);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Uncorrect input.");
            }
            return Payment.StorePayment;
        }

        private decimal BuyerMoney()
        {
            decimal change = 0.0m;  
            try
            {
                if (textBox6.Text != string.Empty &&
                    Convert.ToDecimal(textBox6.Text) >= Convert.ToDecimal(label14.Text))
                {
                    change = Convert.ToDecimal(textBox6.Text) - Convert.ToDecimal(label14.Text);
                    label21.Text = change.ToString();
                    return change;
                }
                else if (textBox4.Text != string.Empty)
                {
                    if (Convert.ToDecimal(textBox4.Text) >= Convert.ToDecimal(label14.Text))
                    {
                        change = Convert.ToDecimal(textBox4.Text) - Convert.ToDecimal(label14.Text);
                        label21.Text = change.ToString();
                    }
                    else
                    {
                        MessageBox.Show("The amount of money is less than the payment amount.");
                    }
                    return change;
                }
                else
                {
                    return NotEnoughToPayShow(ref change);
                }
            }
            catch (System.FormatException)
            {
                return 0;
            }
        }

        private decimal NotEnoughToPayShow(ref decimal change)
        {
            if (Convert.ToDecimal(label14.Text) > Convert.ToDecimal(textBox6.Text) ||
                                    (Convert.ToDecimal(label14.Text) > Convert.ToDecimal(textBox4.Text)))
            {
                MessageBox.Show("The amount is not enough to _pay.");
                label15.Visible = false;
                label21.Text = string.Empty;
                return 0.00m;
            }
            change = Convert.ToDecimal(textBox4.Text) - Convert.ToDecimal(label14.Text);
            label21.Text = change.ToString();
            return change;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.MultiSelect = false;

            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                textBox5.Text = item.SubItems[0].Text;
            }
            else
            {
                textBox5.Text = string.Empty;
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            button1.Enabled = true;

            switch (e.IsSelected)
            {
                case true:
                    textBox5.Text = e.Item.SubItems[0].Text;    // [0] - because 'Food name' is interested
                    textBox1.Text = "";
                    break;
                case false:
                    textBox5.Text = null;
                    break;
                default:
                    break;
            }
        }
    }
}
