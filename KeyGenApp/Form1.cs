using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.DirectoryServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace KeyGenApp
{
    public partial class Form1 : Form
    {
        // kody zdarzeñ systemowych pod³¹czenia i od³¹czenia urz¹dzeñ
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        // Podstawowa œcie¿ka generacji kluczy
        static string folder = "Klucze";
        static string pubKeyName = "publicKey.bin";
        static string ppkName = "privateKey.enc";
        static string vectorName = "iv.bin";
        string usbPath = "";
        string defaultPath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments), folder);

        /// <summary>
        /// Zebranie dostêpnych pendrive'ów z formatem FAT32
        /// </summary>
        /// <returns></returns>
        List<string> GetPendriveList()
        {
            List<string> list = new List<string>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                // Sprawdzenie czy s¹ dostêpne pendrive'y
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
        /// Wykrywanie ju¿ pod³¹czonych Pendrive'ów
        /// </summary>
        void CheckForPendrives()
        {
            List<string> drives = GetPendriveList();

            if (drives.Count == 1)
            {
                var result = MessageBox.Show(
                    $"Wykryto pendrive: {drives[0]} \nCzy chcesz w nim zapisaæ wygenerowane klucze?",
                    "Wykryto pendrive",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );

                if (result == DialogResult.Yes)
                {
                    usbPath += drives[0];
                    textBox2.Text = Path.Combine(usbPath, folder);
                }
            }
            else if (drives.Count > 1)
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
        /// Obs³uga usuniêcia pendrive'a
        /// </summary>
        void CheckIfDeviceMissing()
        {
            if (!Directory.Exists(usbPath))
            {
                MessageBox.Show(
                    $"Pendrive: {usbPath}, zosta³ od³¹czony.",
                    "Urz¹dzenie od³aczone!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                usbPath = "";
                textBox2.Text = defaultPath;
            }
        }

        /// <summary>
        /// Wykrywanie eventów pod³¹czenia i od³¹czenia pendrive'ów
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
        /// wybranie œcie¿ki zapisu 
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
            File.WriteAllBytes(Path.Combine(textBox2.Text, ppkName), encryptedPrivateKey);
            File.WriteAllBytes(Path.Combine(textBox2.Text, pubKeyName), publicKeyBytes);
            File.WriteAllBytes(Path.Combine(textBox2.Text, vectorName), iv);

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
        /// Blokowanie generacji kluczy od razu po w³¹czeniu aplikacji
        /// i wyœwietlenie podstawowej œcie¿ki zapisu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = defaultPath;
            button2.Enabled = false;
        }

        /// <summary>
        /// Powrót do podstawowej œcie¿ki zapisu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = defaultPath;
        }

        /// <summary>
        /// Blokowaniu mo¿liwoœci wpisanie znaków innych ni¿ liczb w pole pinu
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
