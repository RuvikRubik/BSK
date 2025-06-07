using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;


namespace KeyGenApp
{
    public partial class Form1 : Form
    {
        // kody zdarzeń systemowych podłączenia i odłączenia urządzeń
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        // Nazwy plików potrzebne do generacji kluczy
        static readonly string folder = "Klucze";
        static readonly string pubKeyName = "publicKey.bin";
        static readonly string ppkName = "privateKey.enc";
        static readonly string vectorName = "iv.bin";
        static readonly string certName = "cert1.cer";

        // Ścieżki
        string usbPath = "";
        readonly string defaultPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments), folder);
        readonly string utilPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments), "Utils");

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
                        list.Add(drive.RootDirectory.FullName);
                    }
                }
            }

            return list;
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
                        usbPath += dialog.drive;
                        textBox2.Text = Path.Combine(usbPath, folder);
                    }
                }

                dialog.Dispose();
            }
        }

        /// <summary>
        /// Obsługa usunięcia pendrive'a
        /// </summary>
        void CheckIfDeviceMissing()
        {
            if (!Directory.Exists(usbPath))
            {
                MessageBox.Show(
                    $"Pendrive: {usbPath}, został odłączony.",
                    "Urządzenie odłaczone!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                usbPath = "";
                textBox2.Text = defaultPath;
            }
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
        /// wybranie ścieżki zapisu 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = Path.Combine(folderBrowserDialog1.SelectedPath, folder);
            }
        }

        /// <summary>
        /// wygenerowanie kluczy i zapis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(textBox2.Text);
            Directory.CreateDirectory(utilPath);
            // wygenerowanie kluczy o długości 4096
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

            //generacja certyfikatu
            var request = new CertificateRequest(
                "CN=USER",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            var cert = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            // zapis do pliku
            File.WriteAllBytes(Path.Combine(textBox2.Text, ppkName), encryptedPrivateKey);
            File.WriteAllBytes(Path.Combine(utilPath, pubKeyName), publicKeyBytes);
            File.WriteAllBytes(Path.Combine(utilPath, certName), cert.Export(X509ContentType.Cert));
            File.WriteAllBytes(Path.Combine(utilPath, vectorName), iv);

            MessageBox.Show("Klucze zapisane poprawnie!");
        }

        /// <summary>
        /// Wizualny feedback co do wpisanego pinu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Blokowanie generacji kluczy od razu po włączeniu aplikacji
        /// i wyświetlenie podstawowej ścieżki zapisu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = defaultPath;
            button2.Enabled = false;
        }

        /// <summary>
        /// Powrót do podstawowej ścieżki zapisu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = defaultPath;
        }

        /// <summary>
        /// Blokowaniu możliwości wpisania znaków innych niż liczb w pole pinu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Detekcja pendrive'ów przy sracie aplikacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            CheckForPendrives();
        }
    }
}
