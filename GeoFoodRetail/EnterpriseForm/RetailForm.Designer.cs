namespace GeoFoodRetail.EnterpriseForm
{
    partial class RetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RetailForm));
            this.mapPanel1 = new GeoFoodRetail.AddManyObjects.MapPanel();
            this.SuspendLayout();
            // 
            // mapPanel1
            // 
            this.mapPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPanel1.Location = new System.Drawing.Point(0, 0);
            this.mapPanel1.Name = "mapPanel1";
            this.mapPanel1.Size = new System.Drawing.Size(800, 450);
            this.mapPanel1.TabIndex = 0;
            // 
            // RetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mapPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RetailForm";
            this.Text = "Global Enterprise Food Retail";
            this.ResumeLayout(false);

        }

        #endregion

        private AddManyObjects.MapPanel mapPanel1;
    }
}