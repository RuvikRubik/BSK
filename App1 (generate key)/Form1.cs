using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BSK
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        // wybranie œcie¿ki zapisu
        private void button3_Click(object sender, EventArgs e)
        {
            string folderPath = "";
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
            }
            label1.Text = folderPath;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        
        // wygenerowanie kluczy i zapis
        private void button1_Click(object sender, EventArgs e)
        {
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
            File.WriteAllBytes(label1.Text+"\\privateKey.enc", encryptedPrivateKey);
            File.WriteAllBytes(label1.Text+"\\publicKey.bin", publicKeyBytes);
            File.WriteAllBytes(label1.Text+"\\iv.bin", iv);

            MessageBox.Show("Klucze zapisane poprawnie!");
        }
    }
}
