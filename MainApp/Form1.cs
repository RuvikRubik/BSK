using iText.Kernel.Pdf;
using iText.Signatures;
using KeyGenApp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace MainApp
{
    public partial class Form1 : Form
    {
        // kody zdarzeń systemowych podłączenia i odłączenia urządzeń
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        // Stałe opisujące nazwy plików i folderów
        // Nazwy plików potrzebne do generacji kluczy
        static readonly string folder = "Klucze";
        static readonly string pubKeyName = "publicKey.bin";
        static readonly string ppkName = "privateKey.enc";
        static readonly string vectorName = "iv.bin";

        // Ścieżka do wspólnego folderu z kluczem publicznym, IV i certyfikatem
        readonly string utilPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments), "Utils");

        /// <summary>
        /// Obsługa włączenia i wyłączenia przycisków
        /// </summary>
        void CheckUnlockButton()
        {
            bool unlockA = false;
            bool unlockB = false;
            // Sprawdzenie istnienia pliku pdf
            if (File.Exists(textBox4.Text))
            {
                // Sprawdzenie istnienia pendrive'a
                if (textBox2.Text.Length > 0)
                {
                    // Sprawdzenie czy pin jest poprawny (schemat)
                    if (Regex.IsMatch(textBox1.Text, @"^\d{4}$"))
                        unlockA = true;

                }
            }

            // Sprawdzenie istnienia pliku pdf
            if (File.Exists(textBox5.Text))
                unlockB = true;

            button4.Enabled = unlockA;
            button8.Enabled = unlockB;
        }

        /// <summary>
        /// Zebranie dostępnych pendrive'ów z formatem FAT32
        /// </summary>
        /// <returns></returns>
        List<string> GetPendriveList()
        {
            List<string> list = new List<string>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                // Sprawdzenie czy są dostępne pendrive'y
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    // Sprawdzenie czy ich format jest FAT32
                    if (drive.DriveFormat.Equals("FAT32", StringComparison.OrdinalIgnoreCase))
                    {
                        // Sprawdzanie czy pendrive zwiera wymagane pliki
                        string tmp = drive.RootDirectory.FullName;
                        if (Directory.Exists(Path.Combine(tmp, folder)))
                        {
                            // Sprawdzenie, czy pendrive jest poprawny dla aktualnego użytkownika
                            if (tabControl1.SelectedIndex == 0)
                            {
                                if (File.Exists(Path.Combine(tmp, folder, ppkName)))
                                    list.Add(tmp);
                            }
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Reaguja na wykrycie właściwego pendrive'a
        /// </summary>
        /// <param name="path"></param>
        void AddDetectedDrive(string path)
        {
            string pathToPpk = Path.Combine(path, folder, ppkName);

            if (File.Exists(pathToPpk))
                textBox2.Text = path;

            CheckUnlockButton();
        }

        /// <summary>
        /// Wykrywanie już podłączonych Pendrive'ów
        /// </summary>
        void CheckForPendrives()
        {
            List<string> drives = GetPendriveList();
            if (drives.Count > 0)
            {
                var dialog = new Form2(drives);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.drive != null)
                    {
                        AddDetectedDrive(dialog.drive);
                    }
                }

                dialog.Dispose();
            }
            CheckUnlockButton();
        }

        /// <summary>
        /// Obsługa usunięcia pendrive'a
        /// </summary>
        void CheckIfDeviceMissing()
        {
            string usb = "";
            if (tabControl1.SelectedIndex == 0)
                usb = textBox2.Text;


            if (!Directory.Exists(textBox2.Text))
            {
                textBox2.Text = "";
            }
            if (!Directory.Exists(usb) && usb.Length > 0)
            {
                MessageBox.Show(
                    $"Pendrive: {usb}, został odłączony.",
                    "Urządzenie odłaczone!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            CheckUnlockButton();
        }

        /// <summary>
        /// Wykrywanie eventów podłączenia i odłączenia pendrive'ów
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_DEVICECHANGE)
            {
                switch ((int)m.WParam)
                {
                    case DBT_DEVICEARRIVAL:
                        CheckForPendrives();
                        break;

                    case DBT_DEVICEREMOVECOMPLETE:
                        CheckIfDeviceMissing();
                        break;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Podpisywanie pliku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(utilPath, vectorName)))
            {
                MessageBox.Show("Brak wektora IV potrzebnego" +
                    " do weryfikacji podpisu!", "Brak Wektora IV!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string inputPdfPath = textBox4.Text;
            string outputPdfPath = Path.ChangeExtension(inputPdfPath, ".signed.pdf");

            // Odszyfrowanie klucza prywatnego
            byte[] aesKey = SHA256.HashData(Encoding.UTF8.GetBytes(textBox1.Text));
            byte[] iv = File.ReadAllBytes(Path.Combine(utilPath, vectorName));
            byte[] encryptedPrivateKey = File.ReadAllBytes(Path.Combine(textBox2.Text, folder, ppkName));

            using Aes aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv;

            using MemoryStream ms = new(encryptedPrivateKey);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream decryptedMs = new();
            cs.CopyTo(decryptedMs);
            byte[] privateKeyBytes = decryptedMs.ToArray();

            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            // Usunięcie podpisu z PDF przed hashowaniem
            using MemoryStream msOut = new();
            using (PdfReader tmpReader = new(inputPdfPath))
            using (PdfWriter tmpWriter = new(msOut))
            using (PdfDocument tmpDoc = new(tmpReader, tmpWriter))
            {
                tmpDoc.GetDocumentInfo().SetMoreInfo("MySignature", null);
            }

            byte[] hash = SHA256.HashData(msOut.ToArray());
            byte[] signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string signatureBase64 = Convert.ToBase64String(signature);

            // Osadzenie podpisu
            using (PdfReader reader = new(new MemoryStream(msOut.ToArray())))
            using (PdfWriter writer = new(outputPdfPath))
            using (PdfDocument pdfDoc = new(reader, writer))
            {
                pdfDoc.GetDocumentInfo().SetMoreInfo("MySignature", signatureBase64);
            }

            MessageBox.Show("Plik został podpisany");
        }

        /// <summary>
        /// Wybór pliku pdf do podpisu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pdf Files|*.pdf";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (tabControl1.SelectedIndex == 0)
                    textBox4.Text = openFileDialog.FileName;
                else
                    textBox5.Text = openFileDialog.FileName;
            }
            CheckUnlockButton();
        }

        /// <summary>
        /// Inicjalna blokada przycisków
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button8.Enabled = false;
        }

        /// <summary>
        /// Sprawdzenie poprawności pinu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBox1.Text, @"^\d{4}$"))
                panel1.BackColor = Color.Gray;
            else
                panel1.BackColor = Color.Red;

            CheckUnlockButton();
        }

        /// <summary>
        /// Wykrywanie pendrive'ów po starcie aplikacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            CheckForPendrives();
        }

        /// <summary>
        /// Zabronienie wpisania znaków innych niż liczb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            CheckUnlockButton();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox5.Text))
            {
                MessageBox.Show("Wybrany plik nie istnieje!!!", "Błąd pliku!!!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(Path.Combine(utilPath, pubKeyName)))
            {
                MessageBox.Show("Nie ma klucza publicznego potrzebnego" +
                    " do weryfikacji podpisu!", "Brak klucza!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string pdfPath = textBox5.Text;

            byte[] pubKeyBytes = File.ReadAllBytes(Path.Combine(utilPath, pubKeyName));
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(pubKeyBytes, out _);

            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string base64Signature = "";

            byte[] hashToVerify;

            // Usuń podpis z PDF i oblicz hash
            using MemoryStream msOriginal = new(pdfBytes);
            using MemoryStream msHash = new();
            using (PdfReader reader = new(msOriginal))
            using (PdfWriter writer = new(msHash))
            using (PdfDocument pdfDoc = new(reader, writer))
            {
                var info = pdfDoc.GetDocumentInfo();
                base64Signature = info.GetMoreInfo("MySignature");
                info.SetMoreInfo("MySignature", null); // Usuń podpis
            }

            if (string.IsNullOrEmpty(base64Signature))
            {
                MessageBox.Show("Brak podpisu w pliku PDF!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            hashToVerify = SHA256.HashData(msHash.ToArray());

            byte[] signature = Convert.FromBase64String(base64Signature);
            bool isValid = rsa.VerifyHash(hashToVerify, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            MessageBox.Show(isValid ? "Podpis poprawny." : "Podpis NIEPOPRAWNY.");
        }
    }
}
