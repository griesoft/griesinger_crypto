using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GriesingerCrypto
{
    public partial class MainForm : Form
    {
        //  Call this function to remove the key from memory after use for security
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        private string ResourcePath { get; set; }
        private ServerClient ServerClient { get; set; }
        private Crypto GCrypto { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        /// <summary>
        /// This property is mainly used by <see cref="WriteProgressDotsToOutput"/>.
        /// </summary>
        private bool HeavyProcessInProgress { get; set; }

        /// <summary>
        /// Access to localized strings.
        /// </summary>
        public static ResourceManager Resources { get; private set; }

        public MainForm()
        {
            InitializeComponent();

            ServerClient = new ServerClient();
            GCrypto = new Crypto();
            HeavyProcessInProgress = false;

            //Get the resource manager for access to localized strings.
            Resources = Properties.Resources.ResourceManager;

            WriteToOutput(String.Format(Resources.GetString("welcomeMessage"), Crypto.CryptedFileExtension));
        }

        #region Textbox TextChanged Events
        private void servername_txt_TextChanged(object sender, EventArgs e)
        {
            ServerClient.ServerName = servername_txt.Text;

            //Reset the client always when the user changes the servername.
            ServerClient.ResetClient();

            EnableButtonsOnInformationExists();
        }
        private void username_txt_TextChanged(object sender, EventArgs e)
        {
            Username = username_txt.Text;
            EnableButtonsOnInformationExists();
        }
        private void password_txt_TextChanged(object sender, EventArgs e)
        {
            Password = password_txt.Text;
            EnableButtonsOnInformationExists();
        }
        #endregion

        #region Button Click Events
        private void findresource_btn_Click(object sender, EventArgs e)
        {
            //Get the path to a file or an folder
            if (file_radiobtn.Checked)
            {
                DialogResult result = openFileDialog1.ShowDialog();

                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    ResourcePath = openFileDialog1.FileName;
                    resourcePath_txt.Text = ResourcePath;

                    EnableButtonsOnInformationExists();
                }
            }
            else
            {
                DialogResult result = folderBrowserDialog1.ShowDialog();

                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    ResourcePath = folderBrowserDialog1.SelectedPath;
                    resourcePath_txt.Text = ResourcePath;

                    EnableButtonsOnInformationExists();
                }
            }
        }
        private async void createaccount_btn_Click(object sender, EventArgs e)
        {
            WriteProgressDotsToOutput();
            await ServerClient.RegisterUserAsync(Username, Password, WriteToOutput);
            HeavyProcessInProgress = false;
        }
        private async void encrypt_btn_Click(object sender, EventArgs e)
        {
            LockButtons();

            WriteToOutput(String.Format(Resources.GetString("startingEncryptionProcessMessage"), 
                file_radiobtn.Checked ? Resources.GetString("file") : Resources.GetString("folder"), ResourcePath));

            WriteProgressDotsToOutput();

            string key = await ServerClient.GetEncryptionKey(Username, Password, WriteToOutput);

            HeavyProcessInProgress = false;

            if (!String.IsNullOrEmpty(key))
            {
                // For additional security Pin the password
                GCHandle gch = GCHandle.Alloc(key, GCHandleType.Pinned);

                if (file_radiobtn.Checked)
                {
                    EncryptFile(key);
                }
                else
                {
                    await EncryptFolder(key);
                }

                // To increase the security of the decryption, delete the used password from the memory !
                ZeroMemory(gch.AddrOfPinnedObject(), key.Length * 2);
                gch.Free();
            }
            else
            {
                //Reset and cancel the process upon error
                if (file_radiobtn.Checked)
                    RollbackFileCryptoProcess(true);
                else
                    RollbackFolderCryptoProcess(true, null);
            }
        }
        private async void decrypt_btn_Click(object sender, EventArgs e)
        {
            LockButtons();

            WriteToOutput(String.Format(Resources.GetString("startingDecryptionProcessMessage"), 
                file_radiobtn.Checked ? Resources.GetString("file") : Resources.GetString("folder"), ResourcePath));

            WriteProgressDotsToOutput();

            string key = await ServerClient.GetEncryptionKey(Username, Password, WriteToOutput);

            HeavyProcessInProgress = false;

            if (!String.IsNullOrEmpty(key))
            {
                // For additional security Pin the password of your files
                GCHandle gch = GCHandle.Alloc(key, GCHandleType.Pinned);

                if (file_radiobtn.Checked && !ResourcePath.Contains(".zip"))
                {
                    DecryptFile(key);
                }
                else
                {
                    DecryptFolder(key);
                }

                // To increase the security of the decryption, delete the used password from the memory !
                ZeroMemory(gch.AddrOfPinnedObject(), key.Length * 2);
                gch.Free();
            }
            else
            {
                //Reset and cancel the process upon error
                if (file_radiobtn.Checked)
                    RollbackFileCryptoProcess(false);
                else
                    RollbackFolderCryptoProcess(false, null);
            }
        }
        #endregion

        #region Output Dialog Methods
        /// <summary>
        /// Writes an message to the output of this form.
        /// </summary>
        /// <param name="message"></param>
        private void WriteToOutput(string message)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => output_txt.AppendText(Environment.NewLine)));
                this.Invoke(new Action(() => output_txt.AppendText(Environment.NewLine)));
                this.Invoke(new Action(() => output_txt.AppendText(DateTime.Now.ToLongTimeString() + " ----> " + message)));
            }
            else
            {
                output_txt.AppendText(Environment.NewLine);
                output_txt.AppendText(Environment.NewLine);
                output_txt.AppendText(DateTime.Now.ToLongTimeString() + " ----> " + message);
            }
        }
        /// <summary>
        /// Writes dots and removes them to the output of this Form. This is to show the user that the 
        /// program is still doing work.
        /// CAUTION: You need to set <see cref="HeavyProcessInProgress"/> to false after a heavy process has finnished.
        /// Else this method will keep looping forever.
        /// </summary>
        private async void WriteProgressDotsToOutput()
        {
            HeavyProcessInProgress = true;

            //Start a task that will keep printing 10 dots to the output
            //and after that removes them and repeats this until canceled
            await Task.Run( async () =>
            {
                int count = 0;

                while (HeavyProcessInProgress)
                {
                    count++;

                    if (count < 10)
                    {
                        //Add a dot to the output
                        if (InvokeRequired)
                            this.Invoke(new Action(() => output_txt.AppendText(".")));
                        else
                            output_txt.AppendText(".");
                    }
                    else
                    {
                        //Remove all dots from the output
                        if (InvokeRequired)
                            this.Invoke(new Action(() => output_txt.Text = output_txt.Text.Remove(output_txt.Text.Length - count, count - 1)));
                        else
                            output_txt.Text = output_txt.Text.Remove(output_txt.Text.Length - count, count - 1);

                        count = 0;
                    }

                    await Task.Delay(1000);
                }
            });
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// This will enable or unable buttons based on some conditions that have to 
        /// be fulfilled before a usr can use the buttons. 
        /// </summary>
        private void EnableButtonsOnInformationExists()
        {
            //Unable all buttons.
            createaccount_btn.Enabled = false;
            decrypt_btn.Enabled = false;
            encrypt_btn.Enabled = false;

            if (String.IsNullOrEmpty(ServerClient.ServerName))
                return;
            if (String.IsNullOrEmpty(Username))
                return;
            if (String.IsNullOrEmpty(Password))
                return;

            //A user may create an account if the conditions above are met.
            createaccount_btn.Enabled = true;

            if (String.IsNullOrEmpty(ResourcePath))
                return;

            //The user may encrypt or decrypt (based on the file extension) when all
            //conditions above are met.
            if (ResourcePath.EndsWith(Crypto.CryptedFileExtension))
                decrypt_btn.Enabled = true;
            else
                encrypt_btn.Enabled = true;
        }

        /// <summary>
        /// Reset the resource path that was selected by the user
        /// </summary>
        private void ResetResourcePath()
        {
            resourcePath_txt.Text = String.Empty;
            ResourcePath = String.Empty;
            EnableButtonsOnInformationExists();
        }

        /// <summary>
        /// Lock all buttons while a encryption or decryption process is in progress.
        /// </summary>
        private void LockButtons()
        {
            encrypt_btn.Enabled = false;
            decrypt_btn.Enabled = false;
            file_radiobtn.Enabled = false;
            folder_radiobtn.Enabled = false;
            findresource_btn.Enabled = false;
            createaccount_btn.Enabled = false;
        }

        /// <summary>
        /// Unlock buttons after a encryption or decryption process has finnished.
        /// </summary>
        private void UnlockButtons()
        {
            file_radiobtn.Enabled = true;
            folder_radiobtn.Enabled = true;
            findresource_btn.Enabled = true;
        }
        #endregion

        #region Folder Zip Methods
        /// <summary>
        /// Zip a folder from the resource path that was seleced by the user.
        /// Folders are ziped so they can be encrypted without iterating through every
        /// folder and file in the selected folder.
        /// </summary>
        /// <param name="outputPath">A empty ouput path string. The method will set the value
        /// to the path where the ziped folder was saved.</param>
        /// <returns>Success status.</returns>
        private bool ZipFolder(out string outputPath)
        {
            outputPath = "";

            try
            {
                WriteToOutput(Resources.GetString("zipFolderForEncrypt"));

                DirectoryInfo folderInfo = new DirectoryInfo(ResourcePath);
                string folderName = folderInfo.Name;
                DirectoryInfo parentFolder = folderInfo.Parent;

                //Save the zip to the temp folder of the machine
                outputPath = Path.GetTempPath();

                if (File.Exists(Path.Combine(outputPath, folderName + ".zip")))
                    File.Delete(Path.Combine(outputPath, folderName + ".zip"));

                WriteProgressDotsToOutput();

                ZipFile.CreateFromDirectory(ResourcePath, outputPath + folderName + ".zip");

                HeavyProcessInProgress = false;

                outputPath = outputPath + folderName + ".zip";

                WriteToOutput(Resources.GetString("zipFolderSuccessfull"));

                return true;
            }
            catch (Exception ex)
            {
                WriteToOutput(ex.Message);
                WriteToOutput(Resources.GetString("zipFolderError"));
                HeavyProcessInProgress = false;
                return false;
            }
        }

        /// <summary>
        /// Unzips a folder after decryption.
        /// </summary>
        /// <param name="inputPath">The path to the decrypted zip folder.</param>
        /// <param name="outputPath">The path and name containing the .zip extension where the unziped file should be saved to.</param>
        /// <returns>Success status.</returns>
        private bool UnzipFolder(string inputPath, string outputPath)
        {
            try
            {
                WriteToOutput(Resources.GetString("unzipFolderStart"));

                WriteProgressDotsToOutput();

                ZipFile.ExtractToDirectory(inputPath, outputPath.Remove(outputPath.Length - ".zip".Length, ".zip".Length));

                HeavyProcessInProgress = false;

                WriteToOutput(Resources.GetString("unzipFolderSuccess"));
                return true;
            }
            catch (Exception ex)
            {
                WriteToOutput(ex.Message);
                WriteToOutput(Resources.GetString("unzipFolderFailed"));
                HeavyProcessInProgress = false;
                return false;
            }
        }
        #endregion

        #region Methods that handle success or failure of encryption/ decryption process
        /// <summary>
        /// Completes an encryption or decryption process of a file.
        /// </summary>
        /// <param name="encrypting">Was the calling process encrypting?</param>
        private void CompleteFileCryptoProcess(bool encrypting)
        {
            //Delete the original file
            File.Delete(ResourcePath);

            UnlockButtons();
            ResetResourcePath();
            WriteToOutput(encrypting ? Resources.GetString("encryptSuccess") : Resources.GetString("encryptSuccess"));
            cryptoProgressBar.Value = 0;
        }

        /// <summary>
        /// Completes an encryption or decryption process of a folder.
        /// </summary>
        /// <param name="encrypting">Was the calling process encrypting?</param>
        /// <param name="tempFilePath">The path to the temp file that was created.</param>
        private void CompleteFolderCryptoProcess(bool encrypting, string tempFilePath)
        {
            //Delete original file
            if (encrypting)
            {
                //Remove all ReadOnly folder/file attributes so that we can delete them
                //without problems.
                var di = new DirectoryInfo(ResourcePath);

                foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                    if (file.Attributes.HasFlag(FileAttributes.ReadOnly))
                        file.Attributes &= ~FileAttributes.ReadOnly;

                if (di.Attributes.HasFlag(FileAttributes.ReadOnly))
                    di.Attributes &= ~FileAttributes.ReadOnly;

                Directory.Delete(ResourcePath, true);
            }
            else
                File.Delete(ResourcePath);

            //Delete the temp file if one was created.
            if (!String.IsNullOrEmpty(tempFilePath))
                File.Delete(tempFilePath);

            UnlockButtons();
            ResetResourcePath();
            WriteToOutput(encrypting ? Resources.GetString("encryptSuccess") : Resources.GetString("decryptSuccess"));
            cryptoProgressBar.Value = 0;
        }

        /// <summary>
        /// Undo all changes to original files if something went wrong
        /// on file encryption of decryption.
        /// </summary>
        /// <param name="encryption">Was the calling process encrypting?</param>
        private void RollbackFileCryptoProcess(bool encryption)
        {
            //Unhide the original file if it was hidden
            if (File.GetAttributes(ResourcePath).HasFlag(FileAttributes.Hidden))
                File.SetAttributes(ResourcePath, File.GetAttributes(ResourcePath) & ~FileAttributes.Hidden);

            WriteToOutput(encryption ? Resources.GetString("encryptFailed") : Resources.GetString("decryptFailed"));

            UnlockButtons();
            EnableButtonsOnInformationExists();
            cryptoProgressBar.Value = 0;
        }

        /// <summary>
        /// Undo all changes to original folders if something went wrong
        /// on folder encryption of decryption.
        /// </summary>
        /// <param name="encryption">Was the calling process encrypting?</param>
        /// <param name="tempFilePath">The path to the temp file that was created.</param>
        private void RollbackFolderCryptoProcess(bool encrypting, string tempFilePath)
        {
            //Delete temp file if one was created
            if (!String.IsNullOrEmpty(tempFilePath))
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);

            //Unhide the original folder if it was hidden
            if (File.GetAttributes(ResourcePath).HasFlag(FileAttributes.Hidden))
                File.SetAttributes(ResourcePath, File.GetAttributes(ResourcePath) & ~FileAttributes.Hidden);

            WriteToOutput(encrypting ? Resources.GetString("encryptFailed") : Resources.GetString("decryptFailed"));

            UnlockButtons();
            EnableButtonsOnInformationExists();
            cryptoProgressBar.Value = 0;
        }
        #endregion

        #region Encryption and decryption methods
        /// <summary>
        /// Encrypts a file
        /// </summary>
        /// <param name="key"></param>
        private void EncryptFile(string key)
        {
            //Hide the original, just so that the user can see that the file is in use or in best case can't see it at all
            File.SetAttributes(ResourcePath, File.GetAttributes(ResourcePath) | FileAttributes.Hidden);

            RunWorkerCompletedEventHandler completeHandler = null;
            completeHandler = delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                if ((e.Cancelled == true))
                {
                    RollbackFileCryptoProcess(true);
                }
                else if (!(e.Error == null))
                {
                    RollbackFileCryptoProcess(true);
                }
                else
                {
                    CompleteFileCryptoProcess(true);
                }

                GCrypto.EncryptBackgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                GCrypto.EncryptBackgroundWorker.RunWorkerCompleted -= completeHandler;
            };

            GCrypto.EncryptBackgroundWorker.RunWorkerCompleted += completeHandler;
            GCrypto.EncryptBackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            //Start encryption on a background worker
            GCrypto.Encrypt(ResourcePath, ResourcePath, key, WriteToOutput);
        }

        /// <summary>
        /// Encrypts a folder
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task EncryptFolder(string key)
        {
            //Hide the original, just so that the user can see that the folder is in use or in best case can't see it at all
            File.SetAttributes(ResourcePath, File.GetAttributes(ResourcePath) | FileAttributes.Hidden);

            string zipedFolderPath = "";

            if (!(await Task.Run(() => { return ZipFolder(out zipedFolderPath); })))
            {
                RollbackFileCryptoProcess(true);
                return;
            }

            RunWorkerCompletedEventHandler completeHandler = null;
            completeHandler = delegate(object sender, RunWorkerCompletedEventArgs e)
            {
                if ((e.Cancelled == true))
                {
                    RollbackFolderCryptoProcess(true, zipedFolderPath);
                }
                else if (!(e.Error == null))
                {
                    RollbackFolderCryptoProcess(true, zipedFolderPath);
                }
                else
                {
                    CompleteFolderCryptoProcess(true, zipedFolderPath);
                }
                GCrypto.EncryptBackgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                GCrypto.EncryptBackgroundWorker.RunWorkerCompleted -= completeHandler;
            };

            GCrypto.EncryptBackgroundWorker.RunWorkerCompleted += completeHandler;
            GCrypto.EncryptBackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            //Start encryption on a background worker
            GCrypto.Encrypt(zipedFolderPath, ResourcePath + ".zip", key, WriteToOutput);
        }

        /// <summary>
        /// Decrypts a file
        /// </summary>
        /// <param name="key"></param>
        private void DecryptFile(string key)
        {
            //Hide the original, just so that the user can see that the file is in use or in best case can't see it at all
            File.SetAttributes(ResourcePath, File.GetAttributes(ResourcePath) | FileAttributes.Hidden);

            RunWorkerCompletedEventHandler completeHandler = null;
            completeHandler = delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                if ((e.Cancelled == true))
                {
                    RollbackFileCryptoProcess(false);
                }
                else if (!(e.Error == null))
                {
                    RollbackFileCryptoProcess(false);
                }
                else
                {
                    CompleteFileCryptoProcess(false);
                }
                GCrypto.DecryptBackgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                GCrypto.DecryptBackgroundWorker.RunWorkerCompleted -= completeHandler;
            };

            GCrypto.DecryptBackgroundWorker.RunWorkerCompleted += completeHandler;
            GCrypto.DecryptBackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            //Start decryption on a background worker
            GCrypto.Decrypt(ResourcePath,
                ResourcePath.Remove(ResourcePath.Length - Crypto.CryptedFileExtension.Length, Crypto.CryptedFileExtension.Length),
                key, WriteToOutput);
        }

        /// <summary>
        /// Decrypts a folder
        /// </summary>
        /// <param name="key"></param>
        private void DecryptFolder(string key)
        {
            //Hide the original, just so that the user can see that the file is in use or in best case can't see it at all
            File.SetAttributes(ResourcePath, File.GetAttributes(ResourcePath) | FileAttributes.Hidden);

            DirectoryInfo folderInfo = new DirectoryInfo(ResourcePath);
            string folderName = folderInfo.Name;
            folderName = folderName.Remove(folderName.Length - Crypto.CryptedFileExtension.Length, Crypto.CryptedFileExtension.Length);

            string zipedFolderPath = Path.GetTempPath();

            if (File.Exists(Path.Combine(zipedFolderPath, folderName)))
                File.Delete(Path.Combine(zipedFolderPath, folderName));

            RunWorkerCompletedEventHandler completeHandler = null;
            completeHandler = async delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                if ((e.Cancelled == true))
                {
                    RollbackFolderCryptoProcess(false, zipedFolderPath + folderName);
                }
                else if (!(e.Error == null))
                {
                    RollbackFolderCryptoProcess(false, zipedFolderPath + folderName);
                }
                else
                {
                    if (!(await Task.Run(() =>
                    {
                        return UnzipFolder(Path.Combine(zipedFolderPath, folderName),
                            ResourcePath.Remove(ResourcePath.Length - Crypto.CryptedFileExtension.Length, Crypto.CryptedFileExtension.Length));
                    })))
                    {
                        RollbackFolderCryptoProcess(false, zipedFolderPath + folderName);
                        return;
                    }

                    CompleteFolderCryptoProcess(false, zipedFolderPath + folderName);
                }
                GCrypto.DecryptBackgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                GCrypto.DecryptBackgroundWorker.RunWorkerCompleted -= completeHandler;
            };

            GCrypto.DecryptBackgroundWorker.RunWorkerCompleted += completeHandler;
            GCrypto.DecryptBackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            //Start decryption on a background worker
            GCrypto.Decrypt(ResourcePath, Path.Combine(zipedFolderPath, folderName), key, WriteToOutput);
        }
        #endregion

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            cryptoProgressBar.Value = e.ProgressPercentage;
        }
    }
}
