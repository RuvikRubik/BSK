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
            string inputPdfPath = textBox4.Text;
            string outputPdfPath = Path.ChangeExtension(inputPdfPath, ".signed.pdf");
            // utworzenie klucza aes z pinu
            byte[] aesKey = SHA256.HashData(Encoding.UTF8.GetBytes(textBox1.Text));

            // wczytanie danych
            byte[] iv = File.ReadAllBytes(Path.Combine(utilPath, vectorName));
            byte[] pdfBytes = File.ReadAllBytes(textBox4.Text);
            byte[] encryptedPrivateKey = File.ReadAllBytes(Path.Combine(textBox2.Text, folder, ppkName));

            // tworzenie klucza aes
            using Aes aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv;

            using MemoryStream ms = new(encryptedPrivateKey);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream decryptedMs = new();
            try
            {
                cs.CopyTo(decryptedMs);
            }
            catch
            {
                MessageBox.Show("Nieprawidłowy PIN!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] privateKeyBytes = decryptedMs.ToArray();

            // utwórz RSA z odszyfrowanego klucza prywatnego
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            // oblicz hash
            byte[] hash = SHA256.HashData(pdfBytes);

            // podpisz hash kluczem prywatnym (emulacja PAdES)
            byte[] signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string signatureBase64 = Convert.ToBase64String(signature);

            // osadź podpis w metadanych PDF
            using PdfReader reader = new(inputPdfPath);
            using PdfWriter writer = new(outputPdfPath);
            using PdfDocument pdfDoc = new(reader, writer);

            pdfDoc.GetDocumentInfo().SetMoreInfo("MySignature", signatureBase64);

            pdfDoc.Close();

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
                MessageBox.Show("Nie ma klucza publicznego potrzebnego do weryfikacji podpisu!", "Brak klucza!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string pdfPath = textBox5.Text;

            // Wczytaj podpis z metadanych PDF
            using var pdfReader = new PdfReader(pdfPath);
            using var pdfDoc = new PdfDocument(pdfReader);
            var info = pdfDoc.GetDocumentInfo();

            string base64Signature = info.GetMoreInfo("MySignature");
            if (string.IsNullOrEmpty(base64Signature))
            {
                MessageBox.Show("Brak podpisu w pliku PDF!", "Brak podpisu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] signature = Convert.FromBase64String(base64Signature);
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);

            // Oblicz hash dokumentu
            using SHA256 sha = SHA256.Create();
            byte[] currentHash = sha.ComputeHash(pdfBytes);

            // Wczytaj klucz publiczny
            byte[] pubKeyBytes = File.ReadAllBytes(Path.Combine(utilPath, pubKeyName));
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(pubKeyBytes, out _);

            // Weryfikuj podpis (działa z kluczem publicznym)
            bool isValid = rsa.VerifyHash(currentHash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            MessageBox.Show(isValid ? "Podpis poprawny." : "Podpis NIEPOPRAWNY.");
        }
    }
}
