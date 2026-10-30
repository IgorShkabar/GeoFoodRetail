/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * The partial class `MainForm.EventsContinuation`, intended to continue separating 
 * the `MainForm` code and moving event handlers into it.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.MainDashboard;

namespace GeoFoodRetail
{
    internal partial class MainForm
    {
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(this.comboBox1, "If cost calculations for all products are not completed, \nplease do not change the country name in the Country list at this time.");
            textBox3.Text = label14.Text = string.Empty;
            label5.Visible = label6.Visible = false;           
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            textBox4.Text = label21.Text = string.Empty;
            label15.Visible = false;

            if (textBox6.Text == string.Empty || label21.Text == string.Empty)
                label15.Visible = false;
        }

        // handle user`s input
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9'))
            {
                label17.Visible = false;
                return; 
            }
            if (e.KeyChar == '.')
            {                
                if ((textBox2.Text.IndexOf('.') != -1)
                    || (textBox2.Text.Length == 0))
                {
                    e.Handled = true;
                }
                return;
            }
            if (Char.IsControl(e.KeyChar))
            {
                // Enter / BackSapce / Esc 
                if (e.KeyChar == (char)Keys.Enter)
                {                    
                    button1.Enabled = true;
                    button1.Focus();
                    return;
                }
            }
            e.Handled = true; 
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label17.Visible = true;
            textBox2.Focus();
        }

        // user types in the Money text field
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox6.Text = label21.Text = "";
            label15.Visible = false;

            try
            {
                int a = textBox4.Text.IndexOf('.');
                int lng = textBox4.Text.Length;

                if (a > -1 && (lng - a) > 3)
                {
                    textBox4.Text = textBox4.Text.Remove(a + 3);
                    textBox4.SelectionStart = textBox4.Text.Length;
                }
            }
            catch (System.OverflowException)
            {
                OverFlowNum();
                Position.FindAndMoveMsgBox(600, 800, true, "To big Number");
                MessageBox.Show("Please enter the lower number.. ", "Too big Number");
            }
            catch (System.FormatException)
            {
                // TODO: clever only!
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 
            if (e.KeyChar == ',')
                e.KeyChar = '.';
             
            if (char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    button3.Focus();
                    if (!button3.Enabled)
                        button3.Enabled = true;
                }
                return; 
            }
            
            bool isDigit = char.IsDigit(e.KeyChar);
            bool isDot = e.KeyChar == '.' && !textBox4.Text.Contains('.');

            if (isDigit || isDot)
            {
                return; 
            }            
            e.Handled = true;

            Position.FindAndMoveMsgBox(600, 710, true, "Invalid");
            MessageBox.Show("Please, enter digits instead other symbols.", "Invalid");
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
            {
                if ((textBox6.Text.IndexOf('.') != -1)
                    || (textBox6.Text.Length == 0))
                {
                    e.Handled = true;
                }
                return;
            }
            if (e.KeyChar == ',')
            {
                e.KeyChar = '.';
            }

            // Enter / BackSpace / Delete
            if (char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    button3.Focus();
                    return;
                }
                if (e.KeyChar == (char)Keys.Back)
                {
                    textBox6.Text.Replace("\b", "");
                    return;
                }
                if (e.KeyChar == (char)Keys.Delete)
                {
                    textBox6.Text = "\b";
                    return;
                }
            }
            e.Handled = true;
        }

        // search by typing in the Food text field
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            ListViewItem founditem = listView1.FindItemWithText(textBox5.Text, true, 0, true);
            if (founditem != null)
            {
                listView1.TopItem = founditem;
                listView1.TopItem.BackColor = SystemColors.GradientActiveCaption;

                if (textBox5.Text == string.Empty)
                {
                    listView1.TopItem.BackColor = Color.Empty;
                }
            }
            else
            {
                founditem = null;
                listView1.TopItem.BackColor = Color.Empty;
            }
            if (textBox5.Text == string.Empty)
            {
                founditem.BackColor = Color.Empty;
            }
        }
    }
}

