using System.Security.Cryptography;
using System.Text;

namespace KeyGenApp
{
    public partial class Form1 : Form
    {
        string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Klucze");

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
                label1.Text = Path.Combine(folderBrowserDialog1.SelectedPath, "Klucze");
            }
        }

        // wygenerowanie kluczy i zapis
        private void button2_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(label2.Text);
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
            File.WriteAllBytes(label2.Text + "\\privateKey.enc", encryptedPrivateKey);
            File.WriteAllBytes(label2.Text + "\\publicKey.bin", publicKeyBytes);
            File.WriteAllBytes(label2.Text + "\\iv.bin", iv);

            MessageBox.Show("Klucze zapisane poprawnie!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = defaultPath;
        }

        // Powrót do podstawowej œcie¿ki zapisu
        private void button3_Click(object sender, EventArgs e)
        {
            label2.Text = defaultPath;
        }
    }
}
