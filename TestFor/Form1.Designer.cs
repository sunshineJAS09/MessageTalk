namespace TestFor
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            listBox1 = new ListBox();
            timer1 = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(1, 3);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(249, 429);
            listBox1.TabIndex = 0;
            listBox1.SelectedValueChanged += listBox1_SelectedValueChanged;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // button1
            // 
            button1.Location = new Point(12, 445);
            button1.Name = "button1";
            button1.Size = new Size(261, 33);
            button1.TabIndex = 1;
            button1.Text = "出现问题?尝试点我清理缓存";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(274, 481);
            Controls.Add(button1);
            Controls.Add(listBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximumSize = new Size(290, 520);
            MinimumSize = new Size(290, 520);
            Name = "Form1";
            Text = "MessageTalk";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBox1;
        private System.Windows.Forms.Timer timer1;
        private Button button1;
    }
}
