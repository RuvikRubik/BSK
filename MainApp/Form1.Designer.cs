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
            label3 = new Label();
            label7 = new Label();
            textBox1 = new TextBox();
            button4 = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel1 = new Panel();
            textBox4 = new TextBox();
            textBox2 = new TextBox();
            tabPage2 = new TabPage();
            textBox5 = new TextBox();
            button8 = new Button();
            button5 = new Button();
            label8 = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 9);
            label1.Name = "label1";
            label1.Size = new Size(92, 15);
            label1.TabIndex = 0;
            label1.Text = "Plik do podpisu:";
            // 
            // button1
            // 
            button1.Location = new Point(241, 50);
            button1.Name = "button1";
            button1.Size = new Size(103, 23);
            button1.TabIndex = 1;
            button1.Text = "Wybierz plik";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 84);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 3;
            label3.Text = "Pendrive:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(8, 145);
            label7.Name = "label7";
            label7.Size = new Size(73, 15);
            label7.TabIndex = 9;
            label7.Text = "Pin (4 cyfry):";
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(2, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(75, 23);
            textBox1.TabIndex = 10;
            textBox1.TextChanged += textBox1_TextChanged;
            textBox1.KeyPress += textBox1_KeyPress;
            // 
            // button4
            // 
            button4.Location = new Point(3, 182);
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
            tabControl1.Size = new Size(360, 241);
            tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(panel1);
            tabPage1.Controls.Add(textBox4);
            tabPage1.Controls.Add(textBox2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(button4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label7);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(352, 213);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Użytkownik A";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBox1);
            panel1.Location = new Point(87, 140);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(2);
            panel1.Size = new Size(79, 27);
            panel1.TabIndex = 14;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(106, 6);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.ScrollBars = ScrollBars.Horizontal;
            textBox4.Size = new Size(238, 38);
            textBox4.TabIndex = 13;
            textBox4.WordWrap = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(68, 81);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.ScrollBars = ScrollBars.Horizontal;
            textBox2.Size = new Size(114, 37);
            textBox2.TabIndex = 12;
            textBox2.WordWrap = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(textBox5);
            tabPage2.Controls.Add(button8);
            tabPage2.Controls.Add(button5);
            tabPage2.Controls.Add(label8);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(352, 213);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Użytkownik B";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(117, 6);
            textBox5.Multiline = true;
            textBox5.Name = "textBox5";
            textBox5.ReadOnly = true;
            textBox5.ScrollBars = ScrollBars.Horizontal;
            textBox5.Size = new Size(227, 37);
            textBox5.TabIndex = 14;
            textBox5.WordWrap = false;
            // 
            // button8
            // 
            button8.Location = new Point(3, 182);
            button8.Name = "button8";
            button8.Size = new Size(109, 23);
            button8.TabIndex = 9;
            button8.Text = "Zweryfikuj podpis";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button5
            // 
            button5.Location = new Point(246, 49);
            button5.Name = "button5";
            button5.Size = new Size(103, 23);
            button5.TabIndex = 1;
            button5.Text = "Wybierz plik";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button1_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(6, 9);
            label8.Name = "label8";
            label8.Size = new Size(105, 15);
            label8.TabIndex = 0;
            label8.Text = "Plik do weryfikacji:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(360, 241);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            Shown += Form1_Shown;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button button1;
        private Label label3;
        private Label label7;
        private TextBox textBox1;
        private Button button4;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label8;
        private Button button5;
        private Button button8;
        private TextBox textBox2;
        private TextBox textBox4;
        private TextBox textBox5;
        private Panel panel1;
    }
}
