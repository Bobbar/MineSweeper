namespace MineSweeper
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            resetButton = new Button();
            widthTextBox = new TextBox();
            heightTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            numMinesTextBox = new TextBox();
            label3 = new Label();
            applyButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(130, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(759, 807);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseEnter += pictureBox1_MouseEnter;
            pictureBox1.MouseLeave += pictureBox1_MouseLeave;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // resetButton
            // 
            resetButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            resetButton.Location = new Point(932, 743);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(87, 27);
            resetButton.TabIndex = 1;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            // 
            // widthTextBox
            // 
            widthTextBox.Location = new Point(33, 60);
            widthTextBox.Name = "widthTextBox";
            widthTextBox.Size = new Size(51, 23);
            widthTextBox.TabIndex = 2;
            // 
            // heightTextBox
            // 
            heightTextBox.Location = new Point(33, 110);
            heightTextBox.Name = "heightTextBox";
            heightTextBox.Size = new Size(51, 23);
            heightTextBox.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 42);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 4;
            label1.Text = "Width";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 92);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 5;
            label2.Text = "Height";
            // 
            // numMinesTextBox
            // 
            numMinesTextBox.Location = new Point(33, 162);
            numMinesTextBox.Name = "numMinesTextBox";
            numMinesTextBox.Size = new Size(51, 23);
            numMinesTextBox.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(33, 144);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 7;
            label3.Text = "# Mines";
            // 
            // applyButton
            // 
            applyButton.Location = new Point(33, 203);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(51, 28);
            applyButton.TabIndex = 8;
            applyButton.Text = "Apply";
            applyButton.UseVisualStyleBackColor = true;
            applyButton.Click += applyButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1057, 831);
            Controls.Add(applyButton);
            Controls.Add(label3);
            Controls.Add(numMinesTextBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(heightTextBox);
            Controls.Add(widthTextBox);
            Controls.Add(resetButton);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            Shown += Form1_Shown;
            ResizeEnd += Form1_ResizeEnd;
            Paint += Form1_Paint;
            Resize += Form1_Resize;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
		private Button resetButton;
		private TextBox widthTextBox;
		private TextBox heightTextBox;
		private Label label1;
		private Label label2;
		private TextBox numMinesTextBox;
		private Label label3;
		private Button applyButton;
	}
}