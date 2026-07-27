namespace CSHra
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
            components = new System.ComponentModel.Container();
            button1 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            label1 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(285 - 100, 420 - 18);
            button1.Name = "button1";
            button1.Size = new Size(200, 36);
            button1.TabIndex = 0;
            button1.Text = "Start Game";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            button1.Font = new System.Drawing.Font("Courier New", 20, System.Drawing.FontStyle.Bold);
            button1.ForeColor = Color.Red;
            button1.BackColor = Color.Blue;
            // 
            //
            // label1
            //
            label1.Location = new Point(285 - 240, 120);
            label1.Name = "label1";
            label1.Size = new Size(480, 150);
            label1.TabIndex = 1;
            label1.Text = "PAC-MAN";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Font = new System.Drawing.Font("Courier New", 70, System.Drawing.FontStyle.Bold);
            label1.ForeColor = Color.Red;

            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // Form1
            // 
            ClientSize = new Size(19 * 30, 22 * 30);
            Controls.Add(button1);
            Controls.Add(label1);
            KeyPreview = true;
            Name = "Form1";
            Text = "Pac-Man";
            Icon = new Icon(System.IO.Path.Combine(AppContext.BaseDirectory, "Resources", "icon.ico"));
            KeyUp += Form1_KeyUp;
            BackColor = Color.Black;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
    }
}

