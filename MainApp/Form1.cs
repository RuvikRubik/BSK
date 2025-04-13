using KeyGenApp;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MainApp
{
    public partial class Form1 : Form
    {
        // kody zdarzeñ systemowych pod³¹czenia i od³¹czenia urz¹dzeñ
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        // Sta³e opisuj¹ce nazwy plików i folderów
        static string folder = "Klucze";
        static string pubKeyName = "publicKey.bin";
        static string ppkName = "privateKey.enc";
        static string vectorName = "iv.bin";

        // Obs³uga w³¹czenia i wy³¹czenia przycisków
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
            {
                // Sprawdzenie istnienia pendrive'a
                if (textBox3.Text.Length > 0)
                    unlockB = true;
            }

            button4.Enabled = unlockA;
            button8.Enabled = unlockB;
        }

        // Zebranie dostêpnych pendrive'ów z formatem FAT32
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
                        // Sprawdzanie czy pendrive zwiera wymagane pliki
                        string tmp = drive.RootDirectory.FullName;
                        if (Directory.Exists(Path.Combine(tmp, folder)))
                        {
                            // Sprawdzenie, czy pendrive jest poprawny dla aktualnego u¿ytkownika
                            if (tabControl1.SelectedIndex == 0)
                            {
                                string pathToPpk = Path.Combine(tmp, folder, ppkName);
                                string pathToVec = Path.Combine(tmp, folder, vectorName);

                                if (File.Exists(pathToPpk) && File.Exists(pathToVec))
                                    list.Add(tmp);
                            }
                            else
                            {
                                string pathToPubk = Path.Combine(tmp, folder, pubKeyName);
                                if (File.Exists(pathToPubk))
                                    list.Add(tmp);
                            }
                        }
                    }
                }
            }

            return list;
        }

        // Reaguja na wykrycie w³aœciwego pendrive'a
        void AddDetectedDrive(string path)
        {
            string pathToPpk = Path.Combine(path, folder, ppkName);
            string pathToVec = Path.Combine(path, folder, vectorName);
            string pathToPubk = Path.Combine(path, folder, pubKeyName);

            if (File.Exists(pathToPpk) && File.Exists(pathToVec))
                textBox2.Text = path;

            if (File.Exists(pathToPubk))
                textBox3.Text = path;

            CheckUnlockButton();
        }

        // Wykrywanie ju¿ pod³¹czonych Pendrive'ów
        void CheckForPendrives()
        {
            List<string> drives = GetPendriveList();

            if (drives.Count == 1)
            {
                var result = MessageBox.Show(
                    $"Wykryto pendrive: {drives[0]} \nCzy chcesz skorzystaæ z zawartych w nim kluczy?",
                    "Wykryto pendrive",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );

                if (result == DialogResult.Yes)
                {
                    AddDetectedDrive(drives[0]);
                }
            }
            else if (drives.Count > 1)
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

        // Obs³uga usuniêcia pendrive'a
        void CheckIfDeviceMissing()
        {
            string usb = "";
            if (tabControl1.SelectedIndex == 0)
                usb = textBox2.Text;
            else
                usb = textBox3.Text;


            if (!Directory.Exists(textBox2.Text))
            {
                textBox2.Text = "";
            }
            if (!Directory.Exists(textBox3.Text))
            {
                textBox3.Text = "";
            }
            if (!Directory.Exists(usb) && usb.Length > 0)
            {
                MessageBox.Show(
                    $"Pendrive: {usb}, zosta³ od³¹czony.",
                    "Urz¹dzenie od³aczone!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            CheckUnlockButton();
        }

        // Wykrywanie eventów pod³¹czenia i od³¹czenia pendrive'ów
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

        // Podpisywanie pliku
        private void button4_Click(object sender, EventArgs e)
        {
            // utworzenie klucza aes z pinu
            byte[] aesKey = SHA256.HashData(Encoding.UTF8.GetBytes(textBox1.Text));

            // wczytanie danych
            byte[] iv = File.ReadAllBytes(Path.Combine(textBox2.Text, folder, vectorName));
            byte[] pdfBytes = File.ReadAllBytes(textBox4.Text);
            byte[] encryptedPrivateKey = File.ReadAllBytes(Path.Combine(textBox2.Text, folder, ppkName));

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

        // Inicjalna blokada przycisków
        private void Form1_Load(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button8.Enabled = false;
        }

        // Sprawdzenie poprawnoœci pinu
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBox1.Text, @"^\d{4}$"))
                panel1.BackColor = Color.Gray;
            else
                panel1.BackColor = Color.Red;

            CheckUnlockButton();
        }

        // Wykrywanie pendrive'ów po starcie aplikacji
        private void Form1_Shown(object sender, EventArgs e)
        {
            CheckForPendrives();
        }

        // Zabronienie wpisania znaków innych ni¿ liczb
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            CheckUnlockButton();
        }
    }
}
