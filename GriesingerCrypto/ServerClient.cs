using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GriesingerCrypto
{
    /// <summary>
    /// Connection point to the server.
    /// </summary>
    public class ServerClient
    {
        private const string UrlPrefix = "https://";
        private const string Host = ".azurewebsites.net";
        public string ServerName { get; set; }

        private MobileServiceClient client;

        /// <summary>
        /// This will register a new user to the backend.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="outputDialogWriteMethod">This is used to write to the output of the MainForm.</param>
        /// <returns></returns>
        public async Task RegisterUserAsync(string username, string password, Action<string> outputDialogWriteMethod)
        {
            if (client == null)
                client = new MobileServiceClient(CreateServerUrl);

            try
            {
                outputDialogWriteMethod.Invoke(MainForm.Resources.GetString("startCreateAccount"));

                //Request body
                JToken body = JToken.FromObject(new RequestBodyObejct() { Username = username, Password = password });

                //Call api
                var response = await client.InvokeApiAsync("register", body, HttpMethod.Post, null);

                outputDialogWriteMethod.Invoke(MainForm.Resources.GetString(response["message"].ToString()));
            }
            catch(Exception ex)
            {
                outputDialogWriteMethod.Invoke(ex.Message);
            }
        }

        /// <summary>
        /// This will get the encryption/decryption key from the backend.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="outputDialogWriteMethod">This is used to write to the output of the MainForm.</param>
        /// <returns>The encryption/ decryption key or null if something went wrong.</returns>
        public async Task<string> GetEncryptionKey(string username, string password, Action<string> outputDialogWriteMethod)
        {
            if (client == null)
                client = new MobileServiceClient(CreateServerUrl);

            try
            {
                outputDialogWriteMethod.Invoke(MainForm.Resources.GetString("getKey"));

                //Request body
                JToken body = JToken.FromObject(new RequestBodyObejct() { Username = username, Password = password });

                //Call api
                var response = await client.InvokeApiAsync("retrievecryptokey", body, HttpMethod.Post, null);

                //If the response starts with "serverResponse" it means that an error occured.
                if (response["message"].ToString().StartsWith("serverResponse"))
                {
                    outputDialogWriteMethod.Invoke(MainForm.Resources.GetString(response["message"].ToString()));
                    return null;
                }
                else
                {
                    outputDialogWriteMethod.Invoke(MainForm.Resources.GetString("getKeySuccess"));
                    return response["message"].ToString();
                }
            }
            catch (Exception ex)
            {
                outputDialogWriteMethod.Invoke(ex.Message);
                return null;
            }
        }

        public void ResetClient()
        {
            if (client != null)
                client = null;
        }

        private string CreateServerUrl
        {
            get { return UrlPrefix + ServerName + Host; } 
        }
    }

    public class RequestBodyObejct
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
