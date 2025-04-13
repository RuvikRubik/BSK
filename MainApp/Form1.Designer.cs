namespace MainApp
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
            label1 = new Label();
            button1 = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            button2 = new Button();
            label5 = new Label();
            label6 = new Label();
            button3 = new Button();
            label7 = new Label();
            textBox1 = new TextBox();
            button4 = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            button8 = new Button();
            button7 = new Button();
            label13 = new Label();
            label12 = new Label();
            button6 = new Button();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            button5 = new Button();
            label8 = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 3);
            label1.Name = "label1";
            label1.Size = new Size(92, 15);
            label1.TabIndex = 0;
            label1.Text = "Plik do podpisu:";
            // 
            // button1
            // 
            button1.Location = new Point(6, 21);
            button1.Name = "button1";
            button1.Size = new Size(103, 23);
            button1.TabIndex = 1;
            button1.Text = "Wybierz plik";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(104, 3);
            label2.Name = "label2";
            label2.Size = new Size(26, 15);
            label2.TabIndex = 2;
            label2.Text = "plik";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 62);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 3;
            label3.Text = "Klucz:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(50, 62);
            label4.Name = "label4";
            label4.Size = new Size(34, 15);
            label4.TabIndex = 4;
            label4.Text = "klucz";
            // 
            // button2
            // 
            button2.Location = new Point(6, 80);
            button2.Name = "button2";
            button2.Size = new Size(98, 23);
            button2.TabIndex = 5;
            button2.Text = "Wybierz klucz";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 121);
            label5.Name = "label5";
            label5.Size = new Size(48, 15);
            label5.TabIndex = 6;
            label5.Text = "Wektor:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(60, 121);
            label6.Name = "label6";
            label6.Size = new Size(43, 15);
            label6.TabIndex = 7;
            label6.Text = "wektor";
            // 
            // button3
            // 
            button3.Location = new Point(6, 139);
            button3.Name = "button3";
            button3.Size = new Size(98, 23);
            button3.TabIndex = 8;
            button3.Text = "Wybierz wektor";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 179);
            label7.Name = "label7";
            label7.Size = new Size(73, 15);
            label7.TabIndex = 9;
            label7.Text = "Pin (4 cyfry):";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(85, 176);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 10;
            // 
            // button4
            // 
            button4.Location = new Point(6, 225);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 11;
            button4.Text = "Podpisz";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(360, 288);
            tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(button4);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(button3);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(label5);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(352, 260);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Użytkownik A";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button8);
            tabPage2.Controls.Add(button7);
            tabPage2.Controls.Add(label13);
            tabPage2.Controls.Add(label12);
            tabPage2.Controls.Add(button6);
            tabPage2.Controls.Add(label11);
            tabPage2.Controls.Add(label10);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(button5);
            tabPage2.Controls.Add(label8);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(352, 260);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Użytkownik B";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            button8.Location = new Point(6, 231);
            button8.Name = "button8";
            button8.Size = new Size(109, 23);
            button8.TabIndex = 9;
            button8.Text = "Zweryfikuj podpis";
            button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Location = new Point(8, 141);
            button7.Name = "button7";
            button7.Size = new Size(103, 23);
            button7.TabIndex = 8;
            button7.Text = "Wybierz wektor";
            button7.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(62, 123);
            label13.Name = "label13";
            label13.Size = new Size(43, 15);
            label13.TabIndex = 7;
            label13.Text = "wektor";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(8, 123);
            label12.Name = "label12";
            label12.Size = new Size(48, 15);
            label12.TabIndex = 6;
            label12.Text = "Wektor:";
            // 
            // button6
            // 
            button6.Location = new Point(8, 81);
            button6.Name = "button6";
            button6.Size = new Size(103, 23);
            button6.TabIndex = 5;
            button6.Text = "Wybierz klucz";
            button6.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(103, 63);
            label11.Name = "label11";
            label11.Size = new Size(34, 15);
            label11.TabIndex = 4;
            label11.Text = "klucz";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(8, 63);
            label10.Name = "label10";
            label10.Size = new Size(92, 15);
            label10.TabIndex = 3;
            label10.Text = "Klucz publiczny:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(117, 3);
            label9.Name = "label9";
            label9.Size = new Size(26, 15);
            label9.TabIndex = 2;
            label9.Text = "plik";
            // 
            // button5
            // 
            button5.Location = new Point(8, 21);
            button5.Name = "button5";
            button5.Size = new Size(103, 23);
            button5.TabIndex = 1;
            button5.Text = "Wybierz plik";
            button5.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 3);
            label8.Name = "label8";
            label8.Size = new Size(105, 15);
            label8.TabIndex = 0;
            label8.Text = "Plik do weryfikacji:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(360, 288);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button button1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button button2;
        private Label label5;
        private Label label6;
        private Button button3;
        private Label label7;
        private TextBox textBox1;
        private Button button4;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label8;
        private Label label9;
        private Button button5;
        private Label label11;
        private Label label10;
        private Button button6;
        private Button button8;
        private Button button7;
        private Label label13;
        private Label label12;
    }
}
