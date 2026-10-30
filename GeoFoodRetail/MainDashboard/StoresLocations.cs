/** 
 Copyright (c) 2026 Igor Shkabar. All rights reserved.
 Demonstration of connection to my Database 'oracl'. 
 */
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
using Oracle.ManagedDataAccess.Client;
using DatabaseServiceLib;

namespace GeoFoodRetail.MainDashboard
{
    internal partial class StoresLocations : Form
    {
        private readonly IDatabaseService _dbService;
                
        public StoresLocations()
        {
            InitializeComponent();
        }
        // Primary constructor 
        // for working with data
        public StoresLocations(IDatabaseService dbSrv)
        {
            InitializeComponent();
            _dbService = dbSrv;
            // Loading data into  
            // DataGridView while launching the Form. 
            LoadDataToGrid();
        }

        // the test SELECT
        private void LoadDataToGrid()
        {
            /** While the table is fetching data from DB & linking it to the DataSource, 
             the visual component remains "frozen," avoiding the consumption of CPU resources 
            for intermediate redraws of individual rows. */

            string query =
                "SELECT STORE_ID, CHAIN_ID, STATE_CODE, STORE_NUMBER, OPENING_DATE FROM C##GIS_USER.STORES_LOCATIONS ORDER BY STORE_ID";
            try
            {
                dataGridView1.SuspendLayout();
                // Universal call without code duplication for OracleConnection
                DataTable dt = _dbService.GetDataTable(query);
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading data:\n{ex.Message}", "Error DB", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                /** I ensure the grid is unfrozen in any case 
                 (whether successful or in the event of an error).
                It prevents the control's layout state from getting 
                stuck in the event that the database returns an error. */

                dataGridView1.ResumeLayout();
            }
        }

        private bool TryGetCoordinates(out float lat, out float lon)
        {            
            lat = 0;
            lon = 0;

            // Using the current OS culture   
            // (it supports the comma or the point depending on your system).
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;

            if (!float.TryParse(txtLat.Text.Trim(), System.Globalization.NumberStyles.Any, currentCulture, out lat) ||
                !float.TryParse(txtLon.Text.Trim(), System.Globalization.NumberStyles.Any, currentCulture, out lon))
            {
                MessageBox.Show("Enter correct coordinates (for example 36,37 or 36.37).", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private string GetSanitizedStoreName()
        {
            string name = txtName.Text.Trim();
            return string.IsNullOrEmpty(name) ? "New Store" : name;
        }

        private string FormatWktPoint(float lat, float lon)
        {
            // The point is always nedeed for Oracle WKT, 
            // therefore we use InvariantCulture for a string.  
            var invariant = System.Globalization.CultureInfo.InvariantCulture;
            return $"POINT({lat.ToString(invariant)} {lon.ToString(invariant)})";
        }

        // move the query execution to an async method 
        // (async/await), so that the thread is not blocked.
        
        private async Task<bool> TryInsertStoreData(string storeName, string wktPoint)
        {
            string query = @"
                INSERT INTO C##GIS_USER.STORES_LOCATIONS(STORE_ID, CHAIN_ID, STATE_CODE, STORE_NUMBER, OPENING_DATE, STORE_GEOM)
                VALUES (
                    (SELECT NVL(MAX(STORE_ID), 0) + 1 FROM C##GIS_USER.STORES_LOCATIONS), 
                    1, 'DE', :storeNumber, SYSDATE, SDO_UTIL.FROM_WKTGEOMETRY(:wktGeom)
                    )";
            try
            {
                // Pass parameters safely using an OracleParameter array
                var prms = new OracleParameter[]
                {
                    new OracleParameter("storeNumber", storeName),
                    new OracleParameter("wktGeom", wktPoint)
                };
                // We call a universal service method — the connection string is hidden inside!
                await _dbService.ExecuteNonQueryAsync(query, prms);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving to the Database:\n{ex.Message}", "SQL error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }  

        private async void btnSave_Click(object sender, EventArgs e)
        {  
            if (!TryGetCoordinates(out float lat, out float lon)) return;

            string storeName = GetSanitizedStoreName();
            string wktPoint = FormatWktPoint(lat, lon);

            if (await TryInsertStoreData(storeName, wktPoint))
            {
                MessageBox.Show("Data successfully saved to Oracle!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid();
            }            
        }
    }
}


