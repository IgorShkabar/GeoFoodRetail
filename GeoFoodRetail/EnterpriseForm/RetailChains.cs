/** 
 * Copyright (c) 2026 Igor Shkabar. All rights reserved.
 * The "Retail Chains" (RetailChains) form is designed to display the locations of the world's 
 * largest retail chains, as well as the distance from the user to a retail outlet 
 * and the distance between retail outlets selected by the user via mouse clicks.
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
using DatabaseServiceLib;
using Oracle.ManagedDataAccess.Client;

namespace GeoFoodRetail.EnterpriseForm
{
    internal partial class RetailChains : Form
    {  
        public RetailChains()
        {
            InitializeComponent();
        }
        public RetailChains(IDatabaseService service)
        {
            InitializeComponent();            
            DatabaseServiceGet = service;            
            LoadDataChainsToGrid();
            // Auto-adjust column width to fit content
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // Automatically adjusting the width of the DataGridView itself to match the sum of the widths of all its columns
            int totalColumnsWidth = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);

            // Add the width of the vertical scrollbar (if present) and the borders
            // int scrollBarWidth = dataGridView1.VerticalScrollBar.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
            int bordersWidth = dataGridView1.Width - dataGridView1.ClientRectangle.Width + 2;

            // Setting a new width for the DataGridView
            dataGridView1.Width = totalColumnsWidth + bordersWidth;            
        }
                
        static DataTable TableWithData { get; set; }
        // The property gets a value of _dbService from the MainForm
        public static IDatabaseService DatabaseServiceGet { get; set; }
        

        // load Retail Chains
        private void  LoadDataChainsToGrid()
        {
            /** While the table is fetching data from Oracle and linking it to the DataSource, 
             the visual component remains "frozen," avoiding the consumption of CPU resources
             for intermediate redraws of individual rows.    */

            /// From the `C##GIS_USER"."RETAIL_CHAINS` table of my DB
            string que =
                "SELECT CHAIN_ID, CHAIN_NAME, LEGAL_NAME, HQ_COUNTRY_CODE, HQ_STATE_PROVINCE, HQ_CITY, HQ_POSTAL_CODE, HQ_HOUSE_NUMBER, HQ_STREET, RETAIL_TYPE, BUSINESS_MODEL, WEB_SITE, TICKER_SYMBOL, STATUS, CREATED_AT, UPDATED_AT FROM C##GIS_USER.RETAIL_CHAINS ORDER BY CHAIN_ID";
            try
            { 
                dataGridView1.SuspendLayout();
                DataTable dt = DatabaseServiceGet.GetDataTable(que);                
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = dt;

                TableWithData = dt; // get Data Table
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading data:\n{ex.Message}", "Error DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // I guarantee that the grid is unfrozen, regardless of the outcome (success or failure).
                dataGridView1.ResumeLayout();
            }            
        }

       
        BindingSource bs = new BindingSource();

        private void FilterByHouseNmb()
        {
            // catch Data Source
            bs.DataSource = TableWithData;

            // Disables case sensitivity (the search will find "Text", "text", and "TEXT")
            TableWithData.CaseSensitive = false;

            // bind to DataGridView
            dataGridView1.DataSource = bs;                       
        }
        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            FilterByHouseNmb();

            // Escape single quotes to avoid SQL syntax errors
            string filterTxt = textBoxFilter.Text.Replace("'", "''");

            if (string.IsNullOrWhiteSpace(filterTxt)) bs.Filter = string.Empty; // If the field is empty, clear the filter.
            
            // filter by column CHAIN_NAME
            else
                bs.Filter = $"CHAIN_NAME LIKE '%{filterTxt}%'";
        }

        private void textBoxCityFilter_TextChanged(object sender, EventArgs e)
        {
            FilterByHouseNmb();

            string filterTxt = textBoxCityFilter.Text.Replace("'", "''");

            if (string.IsNullOrEmpty(filterTxt)) bs.Filter = string.Empty;
            
            else
                bs.Filter = $"HQ_CITY LIKE '%{filterTxt}%'";
        }

        private void textBoxHouseNumFilter_TextChanged(object sender, EventArgs e)
        {
            FilterByHouseNmb();
            string fltrText = textBoxHouseNumFilter.Text.Replace("'", "''");

            if (string.IsNullOrEmpty(fltrText)) bs.Filter = string.Empty;
            else
            {
                /** To filter the table so that it only contains houses with numbers greater 
                 * than the value entered by the user. First, we check whether the user has 
                 * entered a valid number into the text field. */

                if (int.TryParse(textBoxHouseNumFilter.Text, out int inputDigit))
                {
                    // convert the values in the 7-th column for all rows in the table
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // We leave a new (blank) row for input at the bottom of the table
                        if (row.IsNewRow) continue;

                        if (int.TryParse(row.Cells[6].Value?.ToString(), out int val))
                        {
                            if (val >= inputDigit)
                                bs.Filter = $"HQ_HOUSE_NUMBER LIKE '%{inputDigit}%'";
                            else MessageBox.Show("Invalid house number!");
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(textBoxHouseNumFilter.Text))
                {
                    // If the text field is empty, clear the filter (show all rows)
                    bs.RemoveFilter();
                }
                else
                {
                    MessageBox.Show("Please enter a valid house number. The house number may only be entered in numeric format.","Check Input",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }            
        }        
    }
}
