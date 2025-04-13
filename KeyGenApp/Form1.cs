using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace KeyGenApp
{
    public partial class Form1 : Form
    {
        // Podstawowa œcie¿ka generacji kluczy
        string defaultPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments), "Klucze");

        public Form1()
        {
            InitializeComponent();
        }

        // wybranie œcie¿ki zapisu 
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = Path.Combine(folderBrowserDialog1.SelectedPath, "Klucze");
            }
        }

        // wygenerowanie kluczy i zapis
        private void button2_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(textBox2.Text);
            // wygenerowanie kluczy o d³ugoœci 4096
            RSA rsa = RSA.Create(4096);
            byte[] privateKeyBytes = rsa.ExportRSAPrivateKey();
            byte[] publicKeyBytes = rsa.ExportRSAPublicKey();

            // Utworzenie klucza z Pinu
            byte[] aesKey = SHA256.HashData(Encoding.UTF8.GetBytes(textBox1.Text));

            // zaszyfrowanie klucza prywatnego
            Aes aes = Aes.Create();
            aes.Key = aesKey;
            aes.GenerateIV();
            byte[] iv = aes.IV;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(privateKeyBytes, 0, privateKeyBytes.Length);
            cs.FlushFinalBlock();
            byte[] encryptedPrivateKey = ms.ToArray();

            // zapis do pliku
            File.WriteAllBytes(textBox2.Text + "\\privateKey.enc", encryptedPrivateKey);
            File.WriteAllBytes(textBox2.Text + "\\publicKey.bin", publicKeyBytes);
            File.WriteAllBytes(textBox2.Text + "\\iv.bin", iv);

            MessageBox.Show("Klucze zapisane poprawnie!");
        }

        // Wizualny feedback co do wpisanego pinu
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string text = textBox1.Text;

            if (Regex.IsMatch(text, @"^\d{4}$"))
            {
                panel1.BackColor = Color.Gray;
                button2.Enabled = true;
            }
            else
            {
                panel1.BackColor = Color.Red;
                button2.Enabled = false;
            }
        }

        // Blokowanie generacji kluczy od razu po w³¹czeniu aplikacji
        // i wyœwietlenie podstawowej œcie¿ki zapisu
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = defaultPath;
            button2.Enabled = false;
        }

        // Powrót do podstawowej œcie¿ki zapisu
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = defaultPath;
        }

        // Blokowaniu mo¿liwoœci wpisanie znaków innych ni¿ liczb w pole pinu
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
