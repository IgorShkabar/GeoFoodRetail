using GeoFoodRetail.AddManyObjects;

namespace GeoFoodRetail.GIS.AddManyObjects
{
    partial class Add50KPointsToMap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Add50KPointsToMap));
            this.webViewMap = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnChangeColor = new System.Windows.Forms.Button();
            this.mapPanel1 = new GeoFoodRetail.AddManyObjects.MapPanel();
            this.panel1.SuspendLayout();
            this.mapPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webViewMap
            // 
            this.webViewMap.CreationProperties = null;
            this.webViewMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webViewMap.Location = new System.Drawing.Point(0, 0);
            this.webViewMap.Name = "webViewMap";
            this.webViewMap.Size = new System.Drawing.Size(800, 395);
            this.webViewMap.TabIndex = 0;
            this.webViewMap.Text = "webView21";
            this.webViewMap.ZoomFactor = 1D;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnChangeColor);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 55);
            this.panel1.TabIndex = 1;
            // 
            // btnChangeColor
            // 
            this.btnChangeColor.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnChangeColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnChangeColor.Location = new System.Drawing.Point(17, 7);
            this.btnChangeColor.Name = "btnChangeColor";
            this.btnChangeColor.Size = new System.Drawing.Size(155, 39);
            this.btnChangeColor.TabIndex = 0;
            this.btnChangeColor.Text = "Change Color";
            this.btnChangeColor.UseVisualStyleBackColor = false;
            this.btnChangeColor.Click += new System.EventHandler(this.btnChangeColor_Click);
            // 
            // mapPanel1
            // 
            this.mapPanel1.Controls.Add(this.webViewMap);
            this.mapPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPanel1.Location = new System.Drawing.Point(0, 55);
            this.mapPanel1.Name = "mapPanel1";
            this.mapPanel1.Size = new System.Drawing.Size(800, 395);
            this.mapPanel1.TabIndex = 2;
            // 
            // Add50KPointsToMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mapPanel1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Add50KPointsToMap";
            this.Text = "Add 50_000 Points to the Map";
            this.panel1.ResumeLayout(false);
            this.mapPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webViewMap;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnChangeColor;
        private MapPanel mapPanel1;
    }
}