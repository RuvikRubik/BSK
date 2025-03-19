namespace BSK
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
            textBox1 = new TextBox();
            button1 = new Button();
            button3 = new Button();
            label2 = new Label();
            label4 = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(23, 36);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(167, 23);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(23, 364);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "Wygeneruj";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.Location = new Point(23, 80);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 3;
            button3.Text = "Button";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 9);
            label2.Name = "label2";
            label2.Size = new Size(24, 15);
            label2.TabIndex = 5;
            label2.Text = "Pin";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(23, 62);
            label4.Name = "label4";
            label4.Size = new Size(164, 15);
            label4.TabIndex = 7;
            label4.Text = "Wybierz miejsce zapisu kluczy";
            label4.Click += label4_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 106);
            label1.Name = "label1";
            label1.Size = new Size(0, 15);
            label1.TabIndex = 8;
            label1.Click += label1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(216, 450);
            Controls.Add(label1);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "App1 (Generate Key)";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private Button button3;
        private Label label2;
        private Label label4;
        private Label label1;
    }
}
