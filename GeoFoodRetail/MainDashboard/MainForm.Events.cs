/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * The partial class `MainForm.Events`, intended to separate the `MainForm` code 
 * & move event handlers into it.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.MainDashboard;

namespace GeoFoodRetail
{
    internal partial class MainForm
    {
        private void WithSalesTaxButton2_Click(object sender, EventArgs e)
        {
            try
            {
                // check label3 savety withot falls
                if (!decimal.TryParse(label3.Text, out decimal accum) || accum <= 0)
                {
                    MessageBox.Show("First, perform a preliminary calculation of the amount!",
                        "Calculate Amount", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // check if a country is selected in ComboBox
                if (comboBox1.SelectedValue is decimal combinedTaxMax)
                {
                    decimal tax = accum * combinedTaxMax;
                    decimal total = decimal.Round(accum + tax, 2, MidpointRounding.ToEven);

                    textBox3.Text = tax.ToString("C2"); // "C2" outputs in the currency format (exzample: $10.75 or 10,75 rubles)
                                                        // labelTotal.Text = total.ToString("C2"); 
                    Total = total;

                    label5.Visible = label6.Visible = label16.Visible = label19.Visible = button3.Enabled = true;
                    label14.Text = total.ToString();
                }
                ConnectMyOracleDB();
            }
            catch (FormatException x)
            {
                // Instead of a simple MessageBox, it is better to adopt a "Defensive Programming"  
                // (Defensive Programming) — approach—using decimal.TryParse before performing calculations (in which case 
                // a try-catch block wouldn't even be needed for standard input!)—and reserving 
                // the catch block for error logging:
                // A useful way to handle unexpected failures.
                MessageBox.Show($"An unexpected calculation error occurred:\n{x.Message}", "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// Func-s & Methods
        // result before tax 
        private void PretaxAmountButton_Click(object sender, EventArgs e)
        {
            switch (Payment.Button4wasClicked)
            {
                case true:
                    Payment.Button4wasClicked = true;
                    label3.ResetText();
                    label14.ResetText();
                    Payment.StorePayment = default;
                    break;

                default:
                    Payment.Button4wasClicked = false;
                    WarnAboutEmptyFields();
                    break;
            }
        }

        private void WarnAboutEmptyFields()
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Please, make sure the field 'Food', 'Quantity' or 'Cost' are not empty.\n\nAnd press 'Enter' in the fields after filling in.");
                textBox1.Focus();
                button1.Enabled = false;
            }
            else if (textBox2.Text == string.Empty)
            {
                button1.Enabled = false;
                textBox2.Focus();
                MessageBox.Show("The 'Quantity or Weight' text field is empty. Please, type your value into this text box and press Enter.");
                label17.Visible = true;
            }
            else if (textBox5.Text == string.Empty)
            {
                MessageBox.Show("Please, enter a name of food, to know for what you will _pay.");
                textBox5.Focus();
                NameFoodGet();
            }
            else
            {
                ConditionByDefault();
            }
        }

        private void ConditionByDefault()
        {
            button1.Enabled = true;
            label17.Visible = false;
            try
            {
                Payment.Price = Convert.ToDecimal(textBox1.Text);
                Payment.Quantity = Convert.ToDecimal(textBox2.Text);

                label3.Text = Convert.ToString((decimal.Round(Pretax(), 2, MidpointRounding.ToEven)));
                label7.Visible = true;
                button3.Enabled = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                textBox1.Focus();
            }
            catch (FormatException)
            {
                textBox1.Focus();
            }
            catch (OverflowException)
            {
                OverFlowNum();
            }
        }

        private void ToBuyerButton_Click(object sender, EventArgs e)
        {
            try
            {
                if ((label3.Text == string.Empty) && (label14.Text == string.Empty))
                {
                    MessageBox.Show("The total amount is unknown, calculate it first. Please.", "First", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button3.Enabled = false;
                }
                else
                {
                    WarnAboutTotal();
                }
            }
            catch (System.OverflowException)
            {
                OverFlowNum();
            }
        }

        private void WarnAboutTotal()
        {
            if (label14.Text == string.Empty)
            {
                MessageBox.Show("The total sum is unknown.\nCompute it before, please..");
                button3.Enabled = false;
            }
            else
            {
                button3.Enabled = true;

                if (textBox4.Text == string.Empty)
                {
                    toolTip1.SetToolTip(button3, "Enter the buyer`s money amount. Please.");
                    label21.Text = string.Empty;
                    label15.Visible = false;
                }
                else if (textBox6.Text == "Click on the corresponding banknote denomination. Please")
                {
                    label21.Text = string.Empty;
                    label15.Visible = false;
                }

                switch (textBox4.Text)
                {
                    case "":
                        label21.Text = string.Empty;
                        label15.Visible = false;
                        BuyerMoney();
                        break;
                    default:
                        BuyerMoney();
                        label15.Visible = true;
                        break;
                }
            }
        }

        // to clean all results
        private void button_Clean_Click(object sender, EventArgs e)
        {
            Payment.StorePayment = default;
            label3.ResetText();
            label14.ResetText();
            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox6.Text = label21.Text = label3.Text = label14.Text = label16.Text = string.Empty;
            textBox5.Focus();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox6.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
            textBox4.Text = label21.Text = string.Empty;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox3.Text = string.Empty;

            try
            {
                StopEnterDigits();
            }
            catch (OverflowException)
            {
                OverFlowNum();
                label20.Visible = true;
                Position.FindAndMoveMsgBox(600, 800, true, "To big Number");
                MessageBox.Show("Please enter the lower number.. ", "Too big Number");
            }
        }

        // to handle pressing Key in the _price text field
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                if ((textBox1.Text.Length <= 2) && (textBox1.Text == "0." || textBox1.Text == "0,")) // avoid 00. at the start 
                {
                    if (e.KeyChar == '0')
                    {
                        e.Handled = true;
                    }
                    else return;
                }
                if (textBox1.SelectionStart == 0)
                {
                    if (e.KeyChar == '0')
                    {
                        NoolKeyPress();
                        e.Handled = true;
                    }
                    else return;
                }
                if (textBox1.Text.Length == 0)
                {
                    if (e.KeyChar == '0')
                        e.Handled = true;
                    else return;
                }
                return;
            }
            PressCommaKey(e);
            if (e.KeyChar == '.')
            {
                IndexOfPoint(e);
                return;
            }
            if (char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    EnterKeyPress();
                    return;
                }
                if (e.KeyChar == (char)Keys.Back)
                {
                    BackKeyPress();
                    return;
                }
                if (e.KeyChar == (char)Keys.Delete)
                {
                    DeletekeyPress();
                    return;
                }
                if (e.KeyChar == (char)Keys.Space)
                {
                    SpaceKeyPress();
                    return;
                }
                if (e.KeyChar == (char)Keys.Separator)
                {
                    textBox1.Text = "  ";
                    return;
                }
                if (e.KeyChar == (char)Keys.CapsLock)
                {
                    return;
                }
            }
            e.Handled = true;
            NotDigitInput();
        }

        private void NoolKeyPress()
        {
            textBox1.Text = "0.";
            textBox1.SelectionStart = 3;
        }

        private void SpaceKeyPress()
        {
            textBox1.Text = " ";
        }

        private void DeletekeyPress()
        {
            textBox1.Text = "\b";
            listView1.Focus();
        }

        private void BackKeyPress()
        {
            textBox1.Text.Replace("\b", "");
        }

        private void EnterKeyPress()
        {
            button1.Focus();
            if (button1.Enabled == false) button1.Enabled = true;
        }

        private void IndexOfPoint(KeyPressEventArgs e)
        {
            if ((textBox1.Text.IndexOf('.') != -1) ||
                                                (textBox1.Text.Length == 0))
            {
                e.Handled = true;
            }
        }

        private static void PressCommaKey(KeyPressEventArgs e)
        {
            if (e.KeyChar == ',')
            {
                e.KeyChar = '.';
            }
        }

        private static void NotDigitInput()
        {
            if (!char.IsDigit(Payment.Symb))
            {
                Position.FindAndMoveMsgBox(600, 710, true, "Invalid");
                MessageBox.Show("Please, enter digits instead other symbols.", "Invalid");
            }
        }
    }
}
