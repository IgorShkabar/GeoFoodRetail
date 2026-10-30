/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFoodRetail.FoodOptions;
using static GeoFoodRetail.CompatibleFood;

namespace GeoFoodRetail
{
    internal partial class FoodPurchasingOptionsForm : Form
    {
        // Form owns of the concrete object with data
        private readonly CompatibleFood ob = new CompatibleFood();
        private string mainLightText(string text) => text;

        public FoodPurchasingOptionsForm()
        {
            InitializeComponent();

            // Windows Form layout
            Rectangle screen = Screen.FromPoint(Cursor.Position).WorkingArea;
            int w = Width >= screen.Width ? screen.Width : (screen.Width + Width) / 3;
            int h = Height >= screen.Height ? screen.Height : (screen.Height + Height) / 3;
            Location = new Point(screen.Left + (screen.Width - w) / 3, screen.Top + (screen.Height - h) / 3);
            Size = new Size(w, h);            
        }        

        internal static decimal MassFoodfrUserInput { get; set; }

        private void Form2_Load(object sender, EventArgs e)
        {
            label3.BorderStyle = label4.BorderStyle = label5.BorderStyle = label6.BorderStyle = label7.BorderStyle = label8.BorderStyle = label9.BorderStyle = label10.BorderStyle = BorderStyle.None;
            label3.Visible = label4.Visible = label9.Visible = label10.Visible = button2.Visible = false;
        }

        /* The code will become clean and shed hundreds of 
         * lines of reference data—specifically, the output of various food sets.  */

        private void showOptionsButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AmountFoodTextBox.Text)) // empty field, no input
                {
                    MessageBox.Show("You did not enter value in the 'Amount of food' text box.");
                    AmountFoodTextBox.Focus();
                    return;
                }

                MassFoodfrUserInput = decimal.Parse(AmountFoodTextBox.Text);
                ob.MassFood = MassFoodfrUserInput;

                ProcessLabelsUpdate();
                FormSizeManage();
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid numeric value for food amount.");
                AmountFoodTextBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            /* Resizing a form when displaying content in labels */
            if (label8.Text == string.Empty && label6.Text == string.Empty &&
                label7.Text == string.Empty && label9.Text == string.Empty && label10.Text == string.Empty)
            {
                Size = new Size(818, 497);
            }
            else
            {
                Size = new Size(1007, 641);
            }
        }
        

        private void SetBordersForActiveLabels()
        {
            // Setting boundaries for all active labels
            label5.BorderStyle = label3.BorderStyle = label4.BorderStyle =
            label6.BorderStyle = label7.BorderStyle = label8.BorderStyle =
            label9.BorderStyle = label10.BorderStyle = BorderStyle.Fixed3D;
        }

        private void FormSizeManage()
        {
            // Managing form size
            if (label5.Text == "" && label6.Text == "" && label7.Text == "" && label8.Text == "")
            {
                this.Size = new System.Drawing.Size(818, 497);
            }
            else
            {
                this.Size = new System.Drawing.Size(1007, 641);
            }
        }

        // Options for food sets from the List
        private void fromFoodAssortmentButton_Click(object sender, EventArgs e)
        {
            try
            {
                label5.Enabled = true;
                var ob = new CompatibleFood();
                MassFoodfrUserInput = decimal.Parse(AmountFoodTextBox.Text);
                ob.MassFood = MassFoodfrUserInput;

                SaveIndexesToCheck();

                label11.BorderStyle = BorderStyle.Fixed3D;
                label12.BorderStyle = BorderStyle.Fixed3D;
                label13.BorderStyle = BorderStyle.Fixed3D;
            }
            catch (System.FormatException)
            {
                if (FoodNameTextBox.Text == string.Empty)
                {
                    MessageBox.Show("You did not enter value ​​in the 'Food name' text box.");
                    FoodNameTextBox.Focus();
                }
                else if (AmountFoodTextBox.Text == string.Empty)
                {
                    MessageBox.Show("You did not enter value in the 'Amount of food' text boxes.");
                    AmountFoodTextBox.Focus();
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Uncorrect input.");
            }
        }

        // user pressed keys in 'Amound of food' field
        private void AmountFoodTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Instead of manually replacing the Delete, Space, and Backspace keys, use a standard C# check:
            if (char.IsControl(e.KeyChar)) return;
            // Replacing a comma with a period for a consistent format
            if (e.KeyChar == ',') e.KeyChar = '.';
            
            if (char.IsDigit(e.KeyChar)) return;
            // allow only one point
            if (e.KeyChar == '.' && !AmountFoodTextBox.Text.Contains(".")) return;
            // prohibit everything else
            e.Handled = true;
        }

        // user pressed keys in 'Food name' field
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'a' && e.KeyChar <= 'z') ||
                (e.KeyChar >= 'A' && e.KeyChar <= 'Z') ||
                (e.KeyChar == '-') || (e.KeyChar == ' ') ||
                char.IsControl(e.KeyChar))
            {
                return;
            }
            else
            {
                MessageBox.Show("Please pay attention on the input language or enter only letters in this field.");
                e.Handled = true;
            }
        }

        /* automatically capitalize the first letter of text entered into a TextBox */
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (FoodNameTextBox.Text.Length == 1 && char.IsLower(FoodNameTextBox.Text[0]))
            {
                FoodNameTextBox.Text = FoodNameTextBox.Text.ToUpper();
                FoodNameTextBox.SelectionStart = FoodNameTextBox.Text.Length;
            }
            button2.Visible = true;
        }

        private void buttonClean_Click(object sender, EventArgs e)
        {
            button2.Visible = true;

            if (FoodNameTextBox.Text != string.Empty)
                FoodNameTextBox.Text = string.Empty;
            FoodNameTextBox.Focus();
        }        
    }
}
