using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace GriesingerCrypto
{
    /// <summary>
    /// Contains methods for encryption and decryption of files. Both processes are performed in an own thread.
    /// </summary>
    public class Crypto
    {
        //TODO add support for cancellation of the encrption and decryption process


        /// <summary>
        /// Extension of the encrypted files.
        /// </summary>
        public const string CryptedFileExtension = ".jgc";

        public Crypto()
        {
            //Setup background workers. We wan't them to report progress.
            //Cancellation is not yet supported by the UI
            EncryptBackgroundWorker = new BackgroundWorker();
            EncryptBackgroundWorker.WorkerReportsProgress = true;
            EncryptBackgroundWorker.WorkerSupportsCancellation = true;

            DecryptBackgroundWorker = new BackgroundWorker();
            DecryptBackgroundWorker.WorkerReportsProgress = true;
            DecryptBackgroundWorker.WorkerSupportsCancellation = true;
        }

        public BackgroundWorker EncryptBackgroundWorker { get; private set; }
        public BackgroundWorker DecryptBackgroundWorker { get; private set; }

        /// <summary>
        /// Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                // Fill the buffer with the generated data
                rng.GetBytes(data);
            }

            return data;
        }

        /// <summary>
        /// Encrypts a file from its path and a plain password.
        /// </summary>
        /// <param name="inputPath">The path to the file to encrypt.</param>
        /// <param name="outputPath">The path where the encrypted file should be created and saved.</param>
        /// <param name="password"></param>
        /// <param name="outputDialogWriteMethod">This is used to write to the output of the MainForm.</param>
        public void Encrypt(string inputPath, string outputPath, string password, Action<string> outputDialogWriteMethod)
        {
            if (EncryptBackgroundWorker.IsBusy)
                return;

            bool hadIssues = false;

            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            outputDialogWriteMethod.Invoke(MainForm.Resources.GetString("startEncryption"));

            //generate random salt
            byte[] salt = GenerateRandomSalt();

            //create output file name
            FileStream fsCrypt = new FileStream(outputPath + CryptedFileExtension, FileMode.Create);

            //convert password string to byte arrray
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
            AES.Mode = CipherMode.CFB;

            // write salt to the begining of the output file, so in this case can be random every time
            fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputPath, FileMode.Open);

            //create a buffer (1mb) so only this amount will be allocated in memory and not the whole file
            byte[] buffer = new byte[1048576];
            int read;

            DoWorkEventHandler doWorkHandler = null;
            doWorkHandler = delegate(object sender, DoWorkEventArgs e)
            {
                try
                {
                    while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cs.Write(buffer, 0, read);
                        EncryptBackgroundWorker.ReportProgress((int)(((double)fsIn.Position / (double)fsIn.Length) * 100));
                    }

                    // Close up
                    fsIn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);

                    outputDialogWriteMethod.Invoke(ex.Message);

                    hadIssues = true;
                }
                finally
                {
                    cs.Close();
                    fsCrypt.Close();
                    EncryptBackgroundWorker.DoWork -= doWorkHandler;

                    //We have to throw a unhandled Exception if the encryption process had issues
                    //so that the listener of BackgroundWorker.RunWorkerCompleted gets informed that something went wrong.
                    if (hadIssues)
                    {
                        hadIssues = false;
                        throw new Exception();
                    }
                }
            };

            EncryptBackgroundWorker.DoWork += doWorkHandler;

            EncryptBackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Decrypts an encrypted file with the FileEncrypt method through its path and the plain password.
        /// </summary>
        /// <param name="inputPath">The path to the <see cref="CryptedFileExtension"/> file to decrypt.</param>
        /// <param name="outputPath">The path where the decrypted file should be created and saved.</param>
        /// <param name="password"></param>
        /// <param name="outputDialogWriteMethod">This is used to write to the output of the MainForm.</param>
        public void Decrypt(string inputPath, string outputPath, string password, Action<string> outputDialogWriteMethod)
        {
            if (DecryptBackgroundWorker.IsBusy)
                return;

            bool hadIssues = false;

            outputDialogWriteMethod.Invoke(MainForm.Resources.GetString("startDecryption"));

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];

            FileStream fsCrypt = new FileStream(inputPath, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputPath, FileMode.Create);

            int read;
            byte[] buffer = new byte[1048576];

            DoWorkEventHandler doWorkHandler = null;
            doWorkHandler = delegate (object sender, DoWorkEventArgs e)
            {
                try
                {
                    while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fsOut.Write(buffer, 0, read);
                        DecryptBackgroundWorker.ReportProgress((int)(((double)fsCrypt.Position / (double)fsCrypt.Length) * 100));
                    }
                }
                catch (CryptographicException ex_CryptographicException)
                {
                    Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
                    outputDialogWriteMethod.Invoke(ex_CryptographicException.Message);
                    hadIssues = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    outputDialogWriteMethod.Invoke(ex.Message);
                    hadIssues = true;
                }

                try
                {
                    cs.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
                    outputDialogWriteMethod.Invoke(ex.Message);
                }
                finally
                {
                    fsOut.Close();
                    fsCrypt.Close();
                    DecryptBackgroundWorker.DoWork -= doWorkHandler;

                    //We have to throw a unhandled Exception if the encryption process had issues
                    //so that the listener of BackgroundWorker.RunWorkerCompleted gets informed that something went wrong.
                    if (hadIssues)
                    {
                        hadIssues = false;
                        throw new Exception();
                    }
                }
            };

            DecryptBackgroundWorker.DoWork += doWorkHandler;

            DecryptBackgroundWorker.RunWorkerAsync();
        }
    }
}
