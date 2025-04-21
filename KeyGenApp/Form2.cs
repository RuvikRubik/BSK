using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyGenApp
{
    public partial class Form2 : Form
    {
        public string? drive { get; private set; } = null;
        public Form2(List<string> drives)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(drives.ToArray());
        }

        /// <summary>
        /// Zatwierdza wybór lub wyświetla dialog z ostrzeżeniem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                drive = comboBox1.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Musisz wybrać pednrive, by użyć tej opcji",
                    "Błąd wyboru!!!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Rezygnacja z wyboru i zamknięcie okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
