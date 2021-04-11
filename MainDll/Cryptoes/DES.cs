using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Main.Logs;

namespace Main.Cryptoes
{
    internal class DES
    {
        private TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();

        private byte[] TruncateHash(byte[] chiave, int lunghezza) {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] hash = sha1.ComputeHash(chiave);

            // Truncate or pad the hash.
            Array.Resize(ref hash, lunghezza);
            return hash;
        }

        public DES(string chiave, bool usaChiavePredef = false) {
            // Initialize the crypto provider.
            byte[] chiaveInBytes;

            if (usaChiavePredef == false) {
                chiaveInBytes = Encoding.Unicode.GetBytes(chiave);
                TripleDes.Key = TruncateHash(chiaveInBytes, TripleDes.KeySize / 8);
            } else {
                chiaveInBytes = new byte[39];
                for (byte i=0; i<=38; i++) {
                    chiaveInBytes[i] = Convert.ToByte(i * 6);
                }
                TripleDes.Key = TruncateHash(chiaveInBytes, TripleDes.KeySize / 8);
            }

            chiaveInBytes = Encoding.Unicode.GetBytes("");
            TripleDes.IV = TruncateHash(chiaveInBytes, TripleDes.BlockSize / 8);
        }

        public void CriptaDati(byte[] flussoDecript, out byte[] flussoCript) {
            flussoCript = new byte[0];

            if (flussoDecript == null)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto flussoDecript a null"));
                return;
            }

            MemoryStream flussoInRam = new MemoryStream();
            CryptoStream oggCript = new CryptoStream(flussoInRam, TripleDes.CreateEncryptor(), CryptoStreamMode.Write);

            oggCript.Write(flussoDecript, 0, flussoDecript.Length);
            oggCript.FlushFinalBlock();

            flussoCript = flussoInRam.ToArray();
        }

        public bool DecriptaDati(byte[] flussoCript, out byte[] flussoDecript) {
            flussoDecript = new byte[0];

            if (flussoCript == null)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto flussoCript a null"));
                return false;
            }

            MemoryStream flussoInRam = new MemoryStream();
            CryptoStream oggDecript = new CryptoStream(flussoInRam, TripleDes.CreateDecryptor(), CryptoStreamMode.Write);

            try {
                oggDecript.Write(flussoCript, 0, flussoCript.Length);
                oggDecript.FlushFinalBlock();
            } catch (Exception ex) {
                Log.main.Add(new Mess(LogType.ERR, "", "Eccezione flussoCript:<" + flussoCript.ToString() + "> ex.mess:<" + ex.Message + ">"));
                return false;
            }

            flussoDecript = flussoInRam.ToArray();
            return true;
        }

        public bool DaTestoADatiCript(string testoDecript, out byte[] flussoCript) {
            flussoCript = new byte[0];

            if (testoDecript == null)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto testoDecript a null"));
                return false;
            }

            if (testoDecript.Length == 0) {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto testoDecript vuoto"));
                return false;
            }

            byte[] flussoDecript = new byte[testoDecript.Length];

            for (int i=0; i<testoDecript.Length; i++) {
                flussoDecript[i] = (byte)(Convert.ToChar(testoDecript.Substring(i, 1))); 
            }

            CriptaDati(flussoDecript, out flussoCript);

            return true;
        }

        public bool DaDatiCriptATesto(byte[] flussoCript, out string testoDecript) {
            testoDecript = "";

            if (flussoCript == null)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto flussoCript a null"));
                return false;
            }

            if (flussoCript.Length == 0) {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto flussoCript vuoto"));
                return false;
            }

            byte[] flussoDecript;

            if (DecriptaDati(flussoCript, out flussoDecript) == false) return false;

            for (Int64 i = 0; i < flussoDecript.Length; i++) {
                testoDecript = testoDecript + (char)flussoDecript[i];
            }

            return true;
        }

        public bool DaTestoAFileCript(string testoDecript, string percorsoENomeFile, bool sovraScrivi = true) {
            byte[] flussoCript;

            if (DaTestoADatiCript(testoDecript, out flussoCript) == false) return false;

            if (Crypto.ScriviFlussoInFile(flussoCript, percorsoENomeFile, sovraScrivi) == false) return false;

            return true;
        }

        public bool DaFileCriptATesto(string percorsoENomeFile, out string testoDecript) {
            testoDecript = "";

            if (File.Exists(percorsoENomeFile) == false) {
                Log.main.Add(new Mess(LogType.ERR, "", "Il file in percorsoENomeFile:<" + percorsoENomeFile + "> non esiste"));
                return false;
            }

            FileStream file = new FileStream(percorsoENomeFile, FileMode.Open);
            byte[] flussoCript = new byte[file.Length]; //(file.Length - 1) può valere anche -1 

            if (file.Length > 0) file.Read(flussoCript, 0, Convert.ToInt32(file.Length));

            file.Close();

            if (DaDatiCriptATesto(flussoCript, out testoDecript) == false) return false;

            return true;
        }

        public bool DaFileCryptAData(string percorsoENomeFile, out byte[] flussoDecript) {
            flussoDecript = new byte[0];

            if (File.Exists(percorsoENomeFile) == false) {
                Log.main.Add(new Mess(LogType.ERR, "", "Il percorsoENomeFile:<" + percorsoENomeFile + "> non esistente"));
                return false;
            }

            FileStream file = new FileStream(percorsoENomeFile, FileMode.Open);
            byte[] flussoCript = new byte[file.Length];  //(file.Length - 1) può valere anche -1 

            if (file.Length > 0) file.Read(flussoCript, 0, Convert.ToInt32(file.Length));

            file.Close();

            if (DecriptaDati(flussoCript, out flussoDecript) == false) return false;

            return true;
        }

        public bool DaFileADatiCript(string percorsoENomeFile, out byte[] flussoCript) {
            flussoCript = new byte[0];

            if (File.Exists(percorsoENomeFile) == false) {
                Log.main.Add(new Mess(LogType.ERR, "", "Il percorsoENomeFile:<" + percorsoENomeFile + "> non esistente"));
                return false;
            }

            FileStream file = new FileStream(percorsoENomeFile, FileMode.Open);
            byte[] flussoDecript = new byte[file.Length]; //(file.Length - 1) può valere anche -1 

            if (file.Length > 0) file.Read(flussoDecript, 0, Convert.ToInt32(file.Length));

            file.Close();

            CriptaDati(flussoDecript, out flussoCript);

            return true;
        }

        public bool DaDatiCriptADatiInFile(byte[] flussoCript, string percorsoENomeFile, bool sovraScrivi = true) {

            if (flussoCript == null)
            {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto flussoCript a null"));
                return false;
            }

            if (flussoCript.Length == 0) {
                Log.main.Add(new Mess(LogType.ERR, "", "Ricevuto flussoCript vuoto"));
                return false;
            }

            byte[] DecryptStream;

            if (DecriptaDati(flussoCript, out DecryptStream) == false) return false;

            if (Crypto.ScriviFlussoInFile(DecryptStream, percorsoENomeFile, sovraScrivi) == false) return false;

            return true;
        }

    }
}
