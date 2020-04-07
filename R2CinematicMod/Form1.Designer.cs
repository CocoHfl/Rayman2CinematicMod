namespace R2CinematicMod
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.addKey = new System.Windows.Forms.Button();
            this.clearKeys = new System.Windows.Forms.Button();
            this.launchCine = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.fovValue = new System.Windows.Forms.Label();
            this.setDefaultFOV = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(13, 13);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(146, 24);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Cinematic mod";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // addKey
            // 
            this.addKey.Location = new System.Drawing.Point(13, 44);
            this.addKey.Name = "addKey";
            this.addKey.Size = new System.Drawing.Size(133, 23);
            this.addKey.TabIndex = 1;
            this.addKey.Text = "Add key point";
            this.addKey.UseVisualStyleBackColor = true;
            this.addKey.Click += new System.EventHandler(this.addKey_Click);
            // 
            // clearKeys
            // 
            this.clearKeys.Location = new System.Drawing.Point(152, 44);
            this.clearKeys.Name = "clearKeys";
            this.clearKeys.Size = new System.Drawing.Size(133, 23);
            this.clearKeys.TabIndex = 2;
            this.clearKeys.Text = "Clear keys";
            this.clearKeys.UseVisualStyleBackColor = true;
            this.clearKeys.Click += new System.EventHandler(this.clearKeys_Click);
            // 
            // launchCine
            // 
            this.launchCine.Location = new System.Drawing.Point(13, 73);
            this.launchCine.Name = "launchCine";
            this.launchCine.Size = new System.Drawing.Size(133, 23);
            this.launchCine.TabIndex = 3;
            this.launchCine.Text = "Launch cinematic";
            this.launchCine.UseVisualStyleBackColor = true;
            this.launchCine.Click += new System.EventHandler(this.launchCine_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(13, 123);
            this.trackBar1.Maximum = 30;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(272, 45);
            this.trackBar1.TabIndex = 4;
            this.trackBar1.Value = 12;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // fovValue
            // 
            this.fovValue.AutoSize = true;
            this.fovValue.Location = new System.Drawing.Point(13, 107);
            this.fovValue.Name = "fovValue";
            this.fovValue.Size = new System.Drawing.Size(49, 13);
            this.fovValue.TabIndex = 5;
            this.fovValue.Text = "fovValue";
            // 
            // setDefaultFOV
            // 
            this.setDefaultFOV.Location = new System.Drawing.Point(16, 161);
            this.setDefaultFOV.Name = "setDefaultFOV";
            this.setDefaultFOV.Size = new System.Drawing.Size(75, 23);
            this.setDefaultFOV.TabIndex = 6;
            this.setDefaultFOV.Text = "Default FOV";
            this.setDefaultFOV.UseVisualStyleBackColor = true;
            this.setDefaultFOV.Click += new System.EventHandler(this.setDefaultFOV_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 204);
            this.Controls.Add(this.setDefaultFOV);
            this.Controls.Add(this.fovValue);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.launchCine);
            this.Controls.Add(this.clearKeys);
            this.Controls.Add(this.addKey);
            this.Controls.Add(this.checkBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Rayman 2 Cinematic Mod";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button addKey;
        private System.Windows.Forms.Button clearKeys;
        private System.Windows.Forms.Button launchCine;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label fovValue;
        private System.Windows.Forms.Button setDefaultFOV;
    }
}

