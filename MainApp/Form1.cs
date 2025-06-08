using iText.Kernel.Pdf;
using iText.Signatures;
using KeyGenApp;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MainApp
{
    /// <summary>
    /// Klasa sygnatury potrzebna do realizacji emulacji podpisu pades zgodnie z wymaganiami.
    /// </summary>
    public class CustomSignature : IExternalSignatureContainer
    {
        private readonly RSA _privKey;

        public CustomSignature(RSA privKey)
        {
            _privKey = privKey;
        }

        public byte[] Sign(Stream data)
        {
            // Tworzenie signatury na podstwie hasha zawartości pliku i podpisanie go
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(data);
            return _privKey.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public void ModifySigningDictionary(PdfDictionary signDic)
        {
            signDic.Put(PdfName.Filter, PdfName.Adobe_PPKLite);
            signDic.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_detached);
        }
    }

    /// <summary>
    /// Klasa określająca okno aplikacji głównej. 
    /// Zawiera głównie metody wywoływane przy zdarzeiach w aplikcaji.
    /// </summary>
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
        /// <returns>Zwraca listę ścieżek pendrive'ów w postaci listy wrtosći string.</returns>
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
        /// Reaguja na wykrycie właściwego pendrive'a i sprawdzenie,
        /// czy nie został usunięty pomiędzy zamknięciem dialogu a przypisaniem.
        /// </summary>
        /// <param name="path">Ścieżka wybranego pendrive'a.</param>
        void AddDetectedDrive(string path)
        {
            string pathToPpk = Path.Combine(path, folder, ppkName);

            if (File.Exists(pathToPpk))
                textBox2.Text = path;

            CheckUnlockButton();
        }

        /// <summary>
        /// Wybranie jednego z już podłączonych Pendrive'ów.
        /// </summary>
        void CheckForPendrives()
        {
            // zebranie listy ścieżek
            List<string> drives = GetPendriveList();
            if (drives.Count > 0)
            {
                // uruchomienie customowego dialogu wyboru pendrive'a
                var dialog = new Form2(drives);

                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // sprwadzenie czy został wybrany pendrive
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
        /// Obsługa usunięcia używanego pendrive'a.
        /// </summary>
        void CheckIfDeviceMissing()
        {
            if (!Directory.Exists(textBox2.Text) && textBox2.Text.Length > 0)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    MessageBox.Show(
                        $"Pendrive: {textBox2.Text}, został odłączony.",
                        "Urządzenie odłaczone!!!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                textBox2.Text = "";
            }
            CheckUnlockButton();
        }

        /// <summary>
        /// Wykrywanie eventów podłączenia i odłączenia pendrive'ów
        /// </summary>
        /// <param name="m">Wiadomość ze zdarzeniem systemowym.</param>
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
        /// Metoda Realizująca podpis zgodnie z konceptem PAdES.
        /// </summary>
        /// <param name="inPath">Ścieżka do pliku pdf.</param>
        /// <param name="outPath">Ścieżka podktórą zostanie zapisany podpisany plik.</param>
        /// <param name="rsa">Odszyfrowany klucz prywatny RSA.</param>
        public void SignPdf(string inPath, string outPath, RSA rsa)
        {
            // Sprawdzenie czy w pliku jest podpis wykonany przez aplikacje
            using var reader = new PdfReader(inPath);
            using var pdfDoc = new PdfDocument(reader);
            var signUtil = new SignatureUtil(pdfDoc);

            if (signUtil.GetSignatureNames().Contains("MySignature")) throw new Exception();

            // Prep signera do wykonania podpisu
            using var outStream = new FileStream(outPath, FileMode.Create);
            var signer = new PdfSigner(reader, outStream, new StampingProperties());

            // Przygotowanie pola do podpisu
            signer.SetFieldName("MySignature");

            // Stworzenie obiektu klasy podpisu
            var extSig = new CustomSignature(rsa);
            // Umieszczenie podpisu w pliku
            signer.SignExternalContainer(extSig, 512);
        }

        /// <summary>
        /// Metoda zajmuje sie dekrypcją klucza prywatnego.
        /// </summary>
        /// <returns>Ciąg bajtów reprezentujący odszyfrowany klucz prywatny.</returns>
        private byte[] DecryptPrivKey()
        {
            // Załądowanie zmiennych do dekrypcji klucza prywatnego
            byte[] aesKey = SHA256.HashData(Encoding.UTF8.GetBytes(textBox1.Text));
            byte[] iv = File.ReadAllBytes(Path.Combine(utilPath, vectorName));
            byte[] encrPrivKey = File.ReadAllBytes(Path.Combine(textBox2.Text, folder, ppkName));

            // Tworzenie obiektu eas do dekrypcji klucza
            using Aes aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = iv;

            // Dekrypcja klucza
            try
            {
                using MemoryStream ms = new(encrPrivKey);
                using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using MemoryStream decryptedMs = new();
                cs.CopyTo(decryptedMs);
                return decryptedMs.ToArray();
            }
            catch
            {
                return [];
            }
        }

        /// <summary>
        /// Metoda zajmująca sie pdopisywaniem pliku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            // Sprawdzenie, czy jesy wektor inicjalny do dekrypcji klucza prywatnego
            if (!File.Exists(Path.Combine(utilPath, vectorName)))
            {
                MessageBox.Show("Brak wektora IV potrzebnego do weryfikacji podpisu!",
                    "Brak Wektora IV!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(textBox4.Text))
            {
                MessageBox.Show("Wybrany plik PDF nie istnieje!", "Brak wybranego pliku PDF!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(Path.Combine(textBox2.Text, folder, ppkName)))
            {
                MessageBox.Show("Klucz prywatny został usunięty z nośnika przed podpisem!",
                    "Brak klucza prywatnego!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string inPath = textBox4.Text;
            string outPath = Path.ChangeExtension(inPath, ".signed.pdf");

            // Odszyfrowanie klucza prywatnego 
            byte[] privKeyBytes = DecryptPrivKey();

            if (privKeyBytes.Length == 0) 
            {
                // Reakcja przy błądzie dekrypcji (powodowany złym pinem)
                MessageBox.Show("Podany PIN nie pasuje do klucza prywatnego na nośniku.",
                    "Błędny PIN!!!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // wczytanie klucza prywtnego do obiektu RSA
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privKeyBytes, out _);

            // Przekazanie czynności podpisu do dedykowanej metody
            try
            {
                SignPdf(inPath, outPath, rsa);
            }
            catch
            {
                MessageBox.Show("Podany plik już został podpisany przez tą aplikację.",
                    "Plik już podpisany!!!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Plik został podpisany.");
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
        /// Weryfiakcja podpisu w pliku.
        /// </summary>
        /// <param name="path">Ścieżka do podpisanego pliku.</param>
        /// <param name="rsa">Obiekt RSA zawierający klucz publiczny.</param>
        /// <returns>true lub false, true jeżeli podpis jest poprawny.</returns>
        public bool VerifySignature(string path, RSA rsa)
        {
            using var reader = new PdfReader(path);
            using var pdfDoc = new PdfDocument(reader);
            var signUtil = new SignatureUtil(pdfDoc);

            // Sprawdzenie czy w pliku jest podpis wykonany przez aplikacje
            if (!signUtil.GetSignatureNames().Contains("MySignature")) throw new Exception();

            var signDict = signUtil.GetSignatureDictionary("MySignature");

            // Pobranie zaszyfrowanego podpisu
            var contents = signDict.GetAsString(PdfName.Contents).GetValueBytes();

            // Pobranie informacji o ByteRange do utworzenia hasha dokumentu
            var byteRangeArray = signDict.GetAsArray(PdfName.ByteRange);
            var byteRange = new int[byteRangeArray.Size()];
            for (int i = 0; i < byteRangeArray.Size(); i++)
                byteRange[i] = byteRangeArray.GetAsNumber(i).IntValue();

            // Ręczne odczytanie byteRange i tworzenie ciągu bajtów do utworzenia hasha
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var rangeStream = new MemoryStream();
            for (int i = 0; i < byteRange.Length; i += 2)
            {
                fs.Seek(byteRange[i], SeekOrigin.Begin);
                byte[] buffer = new byte[byteRange[i + 1]];
                fs.Read(buffer, 0, buffer.Length);
                rangeStream.Write(buffer, 0, buffer.Length);
            }

            // Reset pozycji strumienia przed hashem
            rangeStream.Position = 0;

            // Oblicznie hasha i jego weryfikacja z dekrypcją kluczem publicznym
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(rangeStream);

            return rsa.VerifyHash(hash, contents, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// Metoda zajmuje się weryfikacją podpisu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            // Sprawdzenie istnienia niezbędnych plików
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
            // zapis ścieżki pdf do zmiennej
            string pdfPath = textBox5.Text;

            // Wczytanie klucza publicznego
            byte[] pubKeyBytes = File.ReadAllBytes(Path.Combine(utilPath, pubKeyName));
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(pubKeyBytes, out _);

            bool isValid;
            // Sprawdzenie poprawności podpisu
            try
            {
                isValid = VerifySignature(pdfPath, rsa);
            }
            catch
            {
                MessageBox.Show("Plik nie został podpisany w aplikcaji.", "Brak podpisu!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(isValid ? "Podpis jest POPRAWNY." : "Podpis jest NIEPOPRAWNY.");
        }
    }
}
