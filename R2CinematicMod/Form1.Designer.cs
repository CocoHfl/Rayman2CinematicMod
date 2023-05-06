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
            this.fovBar = new System.Windows.Forms.TrackBar();
            this.fovValue = new System.Windows.Forms.Label();
            this.setDefaultFOV = new System.Windows.Forms.Button();
            this.speedBar = new System.Windows.Forms.TrackBar();
            this.cinematicSpeedLabel = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.resetCam = new System.Windows.Forms.Button();
            this.undoKey = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fovBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).BeginInit();
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
            this.launchCine.Location = new System.Drawing.Point(13, 102);
            this.launchCine.Name = "launchCine";
            this.launchCine.Size = new System.Drawing.Size(133, 23);
            this.launchCine.TabIndex = 3;
            this.launchCine.Text = "Launch cinematic";
            this.launchCine.UseVisualStyleBackColor = true;
            this.launchCine.Click += new System.EventHandler(this.launchCine_Click);
            // 
            // fovBar
            // 
            this.fovBar.Location = new System.Drawing.Point(11, 242);
            this.fovBar.Maximum = 30;
            this.fovBar.Minimum = 1;
            this.fovBar.Name = "fovBar";
            this.fovBar.Size = new System.Drawing.Size(272, 45);
            this.fovBar.TabIndex = 4;
            this.fovBar.Value = 12;
            this.fovBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // fovValue
            // 
            this.fovValue.AutoSize = true;
            this.fovValue.Location = new System.Drawing.Point(11, 226);
            this.fovValue.Name = "fovValue";
            this.fovValue.Size = new System.Drawing.Size(49, 13);
            this.fovValue.TabIndex = 5;
            this.fovValue.Text = "fovValue";
            // 
            // setDefaultFOV
            // 
            this.setDefaultFOV.Location = new System.Drawing.Point(14, 280);
            this.setDefaultFOV.Name = "setDefaultFOV";
            this.setDefaultFOV.Size = new System.Drawing.Size(75, 23);
            this.setDefaultFOV.TabIndex = 6;
            this.setDefaultFOV.Text = "Default FOV";
            this.setDefaultFOV.UseVisualStyleBackColor = true;
            this.setDefaultFOV.Click += new System.EventHandler(this.setDefaultFOV_Click);
            // 
            // speedBar
            // 
            this.speedBar.Location = new System.Drawing.Point(12, 182);
            this.speedBar.Minimum = 1;
            this.speedBar.Name = "speedBar";
            this.speedBar.Size = new System.Drawing.Size(272, 45);
            this.speedBar.TabIndex = 7;
            this.speedBar.Value = 1;
            this.speedBar.Scroll += new System.EventHandler(this.speedBar_Scroll);
            // 
            // cinematicSpeedLabel
            // 
            this.cinematicSpeedLabel.AutoSize = true;
            this.cinematicSpeedLabel.Location = new System.Drawing.Point(11, 140);
            this.cinematicSpeedLabel.Name = "cinematicSpeedLabel";
            this.cinematicSpeedLabel.Size = new System.Drawing.Size(85, 13);
            this.cinematicSpeedLabel.TabIndex = 8;
            this.cinematicSpeedLabel.Text = "Cinematic speed";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(151, 102);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(132, 23);
            this.stopButton.TabIndex = 9;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Slow";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Fast";
            // 
            // resetCam
            // 
            this.resetCam.Location = new System.Drawing.Point(150, 73);
            this.resetCam.Name = "resetCam";
            this.resetCam.Size = new System.Drawing.Size(133, 23);
            this.resetCam.TabIndex = 12;
            this.resetCam.Text = "Reset camera";
            this.resetCam.UseVisualStyleBackColor = true;
            this.resetCam.Click += new System.EventHandler(this.resetCam_Click);
            // 
            // undoKey
            // 
            this.undoKey.Location = new System.Drawing.Point(11, 73);
            this.undoKey.Name = "undoKey";
            this.undoKey.Size = new System.Drawing.Size(133, 23);
            this.undoKey.TabIndex = 13;
            this.undoKey.Text = "Undo last key point";
            this.undoKey.UseVisualStyleBackColor = true;
            this.undoKey.Click += new System.EventHandler(this.undoKey_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 314);
            this.Controls.Add(this.undoKey);
            this.Controls.Add(this.resetCam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.cinematicSpeedLabel);
            this.Controls.Add(this.speedBar);
            this.Controls.Add(this.setDefaultFOV);
            this.Controls.Add(this.fovValue);
            this.Controls.Add(this.fovBar);
            this.Controls.Add(this.launchCine);
            this.Controls.Add(this.clearKeys);
            this.Controls.Add(this.addKey);
            this.Controls.Add(this.checkBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Rayman 2 Cinematic Mod";
            ((System.ComponentModel.ISupportInitialize)(this.fovBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button addKey;
        private System.Windows.Forms.Button clearKeys;
        private System.Windows.Forms.Button launchCine;
        private System.Windows.Forms.TrackBar fovBar;
        private System.Windows.Forms.Label fovValue;
        private System.Windows.Forms.Button setDefaultFOV;
        private System.Windows.Forms.TrackBar speedBar;
        private System.Windows.Forms.Label cinematicSpeedLabel;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button resetCam;
        private System.Windows.Forms.Button undoKey;
    }
}

