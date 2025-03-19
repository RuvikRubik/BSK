using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Reflection.PortableExecutable;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Operators;

namespace App2__sign_and_verificate_document_
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

        private void button5_Click(object sender, EventArgs e)
        {
            string fileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            label4.Text = fileName;
        }


        

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

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
