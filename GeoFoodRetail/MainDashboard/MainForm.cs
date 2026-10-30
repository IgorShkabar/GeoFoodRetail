/** 
 * Copyright (c) 2024-26 Igor Shkabar. All rights reserved.
 * The main Form of the Solution with the strip menu & functionality.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Layout;
using System.Runtime.InteropServices;
using Oracle.ManagedDataAccess.Client;
using static GeoFoodRetail.CompatibleFood;
using DatabaseServiceLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using GeoFoodRetail.Utils;
using GeoFoodRetail.GIS.StreamingRTree;
using GeoFoodRetail.EnterpriseForm;

namespace GeoFoodRetail
{
    internal partial class MainForm : Form
    {  
        /// data fields:   
        
        private static bool     _disposVal = false;
        private static MainForm _frm = default;        
        private readonly IDatabaseService _dbService;
                
        public MainForm()
        {
            InitializeComponent();

                // Windows Form layout
            Rectangle screen = Screen.FromPoint(Cursor.Position).WorkingArea;
            int w = Width >= screen.Width ? screen.Width : (screen.Width + Width) / 3;
            int h = Height >= screen.Height ? screen.Height : (screen.Height + Height) / 3;
            Location = new Point(screen.Left + (screen.Width - w) / 3, screen.Top + (screen.Height - h) / 3);
            Size = new Size(w, h);
                // adjust listBox1
            listBox1.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold, GraphicsUnit.Point);
            listBox1.ForeColor = SystemColors.HotTrack;
            listBox1.FormattingEnabled = true;
            listBox1.TabIndex = 6;
            listBox1.Sorted = false;
            listBox1.Size = new Size(90, 116);
            listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
                // adjust ComboBox
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
           
            _dbService = OracleDbService.CreateBuilder(new string[0]).UseConfiguration().Build();
            
            listView1.View = View.Details;
            listView1.OwnerDraw = true;
            listView1.FullRowSelect = true;
            listView1.AllowDrop = true;

            saveFileDialog1.DefaultExt = ".docx";
            saveFileDialog1.Filter = "Text files(*.txt)|Plain text (*.txt)|Text in RTF format (*.rtf)|Portable Document File (*.pdf)|Word document 1997-2003 (*.doc)|Word document (*.docx)|Word document with macros support (*.docm)|Word template (*.dot)|Word template with macros support (*.dotm)|XPS document (*.xps)|XML document (*.xml)|Web page (*.htm;*.html)|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save Document";

        }
        
        public MainForm(IDatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            EnterpriseForm.RetailChains.DatabaseServiceGet = _dbService;
        }        

        /// Properties:         
        public static ListView ShowingListView => _frm.listView1;
        // Computation of the total (amount + tax)
        public static decimal Total { get; set; }
        public static bool ShowLoadingSpinner { get; private set; }
        public static string ContentForRichText { get; set; }


        private async void MainForm_Load(object sender, EventArgs e)
        {           
            string resourcesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            string outputFile = Path.Combine(resourcesDir, "Meal.txt");
                                    
            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.AddRange(new[]
                {
                    new ColumnHeader { Text = "Food name", Width = listView1.Width, TextAlign  = HorizontalAlignment.Left  },
                    new ColumnHeader { Text = "Volume or weight of food", Width = listView1.Width, TextAlign  = HorizontalAlignment.Left  }
                });                
            }
            
            listView1.RetrieveVirtualItem += listView1_RetrieveVirtualItem;
            
            listView1.CheckBoxes = true;            
            listView1.DrawItem += listView1_DrawDefault;
            listView1.DrawSubItem += listView1_DrawDefault;
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.ItemCheck += listView1_ItemCheck;

            if (!File.Exists(outputFile))
            {
                Directory.CreateDirectory(resourcesDir);
                await Task.Run(() => FoodParserUtility.ParseAndSaveFoodNames(outputFile));
            }
            await LoadTaxData();
            LoadFoodData();                        
            FillListView();
            LoadFoodsFromFile();
            
            label5.Visible = label6.Visible = label7.Visible = label15.Visible = label16.Visible = label17.Visible = label19.Visible = label20.Visible = false;
            listView1.ItemCheck += listView1_ItemCheck;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;                        
        }
        

        private async Task LoadTaxData()
        {
            // do without concatenation for globalization                    
            // load data (asyncronously, the Form will not freeze.
            await MainDashboard.TaxService.InitializeAsync();

            comboBox1.BeginUpdate();
            comboBox1.Items.Clear();
            // bind the loaded Dictionary to the ComboBox
            comboBox1.DataSource =
                new BindingSource(MainDashboard.TaxService.StateTaxes, null);
            comboBox1.DisplayMember = "Key";
            comboBox1.ValueMember = "Value";            
            comboBox1.EndUpdate();
        }

        private void LoadFoodData()
        {            
            // Output of the checked .NET Core version
            // on the target user`s PC (presumably).
            CheckVersion.ShowEnvironmentDetails();                    
        }
        
        private void viewOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FoodPurchasingOptionsForm frm = new FoodPurchasingOptionsForm();
            frm.Show();

            if (!_disposVal)
            {
                frm = null;
            }
            _disposVal = true;
            GC.SuppressFinalize(this);            
        }
                // call the HelpFeatureForm - Help
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new HelpFeatureForm();            
            f.Show();
        }                  

        // call About
        private void aboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var af = new AboutForm();            
            af.Show();
        }           
                // Info about OS & Drives
        private void oSAndPartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var os = new OSparts.OsPartsForm();
            os.Show();
            
        }     
          
        private void majorFoodRetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var e_f = new EnterpriseForm.RetailForm();
            e_f.Show();
        }

        private void retailChainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var r = new EnterpriseForm.RetailChains(_dbService);            
            r.Show();
        }
        
        private void deliveryZoneSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlv = new GIS.DeliveryZoneForm();            
            dlv.Show();            
        }

        private void storesLocationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sLoc = new MainDashboard.StoresLocations(_dbService);
            sLoc.Show();            
        }
        
        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Set a background color for column headers
            using (Brush br = new SolidBrush(Color.Beige))
            {
                e.Graphics.FillRectangle(br, e.Bounds);
            }
            ///ControlPaint.DrawBorder(e.Graphics, e.Bounds, Color.Gray, ButtonBorderStyle.Solid);

            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                e.Font,
                e.Bounds,
                e.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
                );            
        }

        private void listView1_DrawDefault(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true; // remain a standard rows rendering
        }

        private void listView1_DrawDefault(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true; // remain a standard subitems rendering
        }

        private void button5_Click(object sender, EventArgs e)
        {            
            if (listView1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select a row to remove.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }            
            var dialogResult = MessageBox.Show(
                $"Вы уверены, что хотите удалить {listView1.CheckedItems.Count} эл.?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );            
            if (dialogResult == DialogResult.Yes)
            {
                RemoveMealItem();
            }
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.EndUpdate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewOpenSafeCloseExit.NewDoc();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            string fileContent = NewOpenSafeCloseExit.OpenFileAndGetContent();
            ContentForRichText = fileContent;

            if (!string.IsNullOrEmpty(fileContent))
            {                
                var rtf = new RtfReadRichText(fileContent);
                rtf.Show();                                
            }
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            NewOpenSafeCloseExit.SaveAll(Application.OpenForms.Cast<Form>());
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            NewOpenSafeCloseExit.CloseDoc(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            NewOpenSafeCloseExit.ExitApplication();
        }

        private void visualisationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vis = new VisualisationForm(RetailChainsData.RetailList);
            vis.Show();            
        }

        private void demoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var vis = new GIS.StreamingRTree.DemoPlatform();
            vis.Show();
        }

        private void demoGisMapSchemeMenuItem_Click(object sender, EventArgs e)
        {
            var gis = new GIS.UI.GisMapScheme();
            gis.Show();
        }
    }
}





