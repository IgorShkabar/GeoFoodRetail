namespace GeoFoodRetail
{
    partial class FoodPurchasingOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FoodPurchasingOptionsForm));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.showOptionsButton = new System.Windows.Forms.Button();
            this.AmountFoodTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FoodNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.fromFoodAssortmentButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label14 = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(310, 878);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.showOptionsButton);
            this.groupBox1.Controls.Add(this.AmountFoodTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.FoodNameTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(283, 293);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "User`s input";
            // 
            // showOptionsButton
            // 
            this.showOptionsButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.showOptionsButton.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.showOptionsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.showOptionsButton.Location = new System.Drawing.Point(15, 204);
            this.showOptionsButton.Margin = new System.Windows.Forms.Padding(48, 12, 12, 12);
            this.showOptionsButton.Name = "showOptionsButton";
            this.showOptionsButton.Size = new System.Drawing.Size(251, 76);
            this.showOptionsButton.TabIndex = 0;
            this.showOptionsButton.Text = "Show purchasing Options\r\n";
            this.showOptionsButton.UseVisualStyleBackColor = false;
            this.showOptionsButton.Click += new System.EventHandler(this.showOptionsButton_Click);
            // 
            // AmountFoodTextBox
            // 
            this.AmountFoodTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AmountFoodTextBox.Location = new System.Drawing.Point(15, 161);
            this.AmountFoodTextBox.Name = "AmountFoodTextBox";
            this.AmountFoodTextBox.Size = new System.Drawing.Size(188, 28);
            this.AmountFoodTextBox.TabIndex = 3;
            this.AmountFoodTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AmountFoodTextBox_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(15, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 40);
            this.label2.TabIndex = 2;
            this.label2.Text = "Amount of food \r\n(kg., gr., lb., l., etc.):";
            // 
            // FoodNameTextBox
            // 
            this.FoodNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FoodNameTextBox.Location = new System.Drawing.Point(15, 67);
            this.FoodNameTextBox.Name = "FoodNameTextBox";
            this.FoodNameTextBox.Size = new System.Drawing.Size(251, 28);
            this.FoodNameTextBox.TabIndex = 1;
            this.FoodNameTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.FoodNameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(11, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Food name:";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.fromFoodAssortmentButton);
            this.flowLayoutPanel2.Controls.Add(this.button2);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 320);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(295, 514);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // fromFoodAssortmentButton
            // 
            this.fromFoodAssortmentButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.fromFoodAssortmentButton.BackColor = System.Drawing.SystemColors.Info;
            this.fromFoodAssortmentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.fromFoodAssortmentButton.Location = new System.Drawing.Point(12, 25);
            this.fromFoodAssortmentButton.Margin = new System.Windows.Forms.Padding(12, 25, 3, 3);
            this.fromFoodAssortmentButton.Name = "fromFoodAssortmentButton";
            this.fromFoodAssortmentButton.Size = new System.Drawing.Size(263, 71);
            this.fromFoodAssortmentButton.TabIndex = 5;
            this.fromFoodAssortmentButton.Text = "From Food Assortment";
            this.fromFoodAssortmentButton.UseVisualStyleBackColor = false;
            this.fromFoodAssortmentButton.Click += new System.EventHandler(this.fromFoodAssortmentButton_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.ForeColor = System.Drawing.SystemColors.Info;
            this.button2.Location = new System.Drawing.Point(12, 123);
            this.button2.Margin = new System.Windows.Forms.Padding(12, 24, 3, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(263, 62);
            this.button2.TabIndex = 4;
            this.button2.Text = "Clear for Changes\r\n";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.buttonClean_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label14);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 840);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(200, 30);
            this.flowLayoutPanel3.TabIndex = 11;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(143, 17);
            this.label14.TabIndex = 0;
            this.label14.Text = "© Igor Shkabar, 2024";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel4.Controls.Add(this.label5);
            this.flowLayoutPanel4.Controls.Add(this.label3);
            this.flowLayoutPanel4.Controls.Add(this.label4);
            this.flowLayoutPanel4.Controls.Add(this.label6);
            this.flowLayoutPanel4.Controls.Add(this.label9);
            this.flowLayoutPanel4.Controls.Add(this.label7);
            this.flowLayoutPanel4.Controls.Add(this.label8);
            this.flowLayoutPanel4.Controls.Add(this.label10);
            this.flowLayoutPanel4.Controls.Add(this.label11);
            this.flowLayoutPanel4.Controls.Add(this.label12);
            this.flowLayoutPanel4.Controls.Add(this.label13);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(310, 0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(490, 878);
            this.flowLayoutPanel4.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.Firebrick;
            this.label5.Location = new System.Drawing.Point(24, 12);
            this.label5.Margin = new System.Windows.Forms.Padding(24, 12, 24, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 25);
            this.label5.TabIndex = 0;
            this.label5.Text = "Meat Food Set";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Firebrick;
            this.label3.Location = new System.Drawing.Point(213, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(24, 10, 10, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 25);
            this.label3.TabIndex = 0;
            this.label3.Text = "Pairing of meal";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.Firebrick;
            this.label4.Location = new System.Drawing.Point(387, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(24, 10, 10, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 25);
            this.label4.TabIndex = 2;
            this.label4.Text = "Grains";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label6.Location = new System.Drawing.Point(24, 57);
            this.label6.Margin = new System.Windows.Forms.Padding(24, 10, 3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 25);
            this.label6.TabIndex = 3;
            this.label6.Text = "Seafood Set";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label9.Location = new System.Drawing.Point(172, 50);
            this.label9.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 25);
            this.label9.TabIndex = 6;
            this.label9.Text = "to seafood";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.ForeColor = System.Drawing.Color.ForestGreen;
            this.label7.Location = new System.Drawing.Point(302, 57);
            this.label7.Margin = new System.Windows.Forms.Padding(24, 10, 3, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(163, 25);
            this.label7.TabIndex = 4;
            this.label7.Text = "Set of vegetables";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label8.Location = new System.Drawing.Point(24, 95);
            this.label8.Margin = new System.Windows.Forms.Padding(24, 10, 3, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 32);
            this.label8.TabIndex = 5;
            this.label8.Text = "Light food set";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label10.Location = new System.Drawing.Point(179, 95);
            this.label10.Margin = new System.Windows.Forms.Padding(24, 10, 10, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(111, 25);
            this.label10.TabIndex = 7;
            this.label10.Text = "to light food";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(324, 95);
            this.label11.Margin = new System.Windows.Forms.Padding(24, 10, 10, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 25);
            this.label11.TabIndex = 8;
            this.label11.Text = "Assortment";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(24, 140);
            this.label12.Margin = new System.Windows.Forms.Padding(24, 10, 10, 10);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(233, 25);
            this.label12.TabIndex = 9;
            this.label12.Text = "combine from Assortment";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(24, 185);
            this.label13.Margin = new System.Windows.Forms.Padding(24, 10, 10, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(192, 25);
            this.label13.TabIndex = 10;
            this.label13.Text = "add from Assortment";
            // 
            // FoodPurchasingOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(800, 878);
            this.Controls.Add(this.flowLayoutPanel4);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(818, 925);
            this.Name = "FoodPurchasingOptionsForm";
            this.Text = "Food purchasing Options";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox FoodNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AmountFoodTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button showOptionsButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button fromFoodAssortmentButton;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label14;
    }
}