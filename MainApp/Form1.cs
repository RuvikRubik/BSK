using System.Security.Cryptography;
using System.Text;

namespace MainApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Podpisywanie pliku
        private void button4_Click(object sender, EventArgs e)
        {
            // utworzenie klucza aes z pinu
            byte[] aesKey = SHA256.HashData(Encoding.UTF8.GetBytes(textBox1.Text));

            // wczytanie danych
            byte[] iv = File.ReadAllBytes(label4.Text);
            byte[] pdfBytes = File.ReadAllBytes(label3.Text);
            byte[] encryptedPrivateKey = File.ReadAllBytes(label2.Text);

            // tworzenie klucza aes
            System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv;

            // odszyfrowanie klucza prywatnego
            MemoryStream ms = new MemoryStream(encryptedPrivateKey);
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream decryptedMs = new MemoryStream();
            cs.CopyTo(decryptedMs);
            byte[] privateKeyBytes = decryptedMs.ToArray();
            MessageBox.Show("Plik zosta³ podpisany");
        }

        // Wybór pliku pdf do podpisu
        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pdf Files|*.pdf";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            label3.Text = fileName;
        }

        // Wybieranie klucza
        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            label2.Text = fileName;
        }

        // Wybieranie wektora
        private void button3_Click(object sender, EventArgs e)
        {
            string fileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            label4.Text = fileName;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = string.Empty;
            label4.Text = string.Empty;
            label6.Text = string.Empty;
            label9.Text = string.Empty;
            label11.Text = string.Empty;
            label13.Text = string.Empty;
        }
    }
}
