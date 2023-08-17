/*
 * @author: Soumya Dayal
 * This class focuses on connecting to the server and sending the public key for the certain email to the server.
 */
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Messenger
{
    /// <summary>
    /// This class send the key for the certain email to the server. The server in this case is:
    /// http://kayrun.cs.rit.edu:5000/
    /// </summary>
    public class SendKey
    {
        private String publicPath = "public.key";
        private String email;
        private HttpClient client;

        public SendKey(String _email)
        {
            email = _email;
            client = new HttpClient();
        }
        
        /// <summary>
        /// Checks if the public.key file exists and if it exists then gets the public key from it.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>public key of type String</returns>
        public String getPublicPrivateKey(String path)
        {
            Boolean fileExist = File.Exists(path);
            if (fileExist)
            {
                String key = File.ReadAllText(path);
                return key;
            }

            return null;
        }
        
        /// <summary>
        /// The function sends the PUT request to the server.
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="content"></param>
        public async Task putRequest(String URL, String content)
        {
            var data = new StringContent(content, Encoding.Default, "application/json");
            var response = await client.PutAsync(URL, data);
            response.EnsureSuccessStatusCode();
        }
        
        /// <summary>
        /// Send the key to the server for the certain email.
        /// </summary>
        public void sendKeyToServer()
        {
            try
            {
                KeyContent keyContent = new KeyContent();
                keyContent.email = email;
                keyContent.content = getPublicPrivateKey(publicPath);
                if (keyContent.content != null)
                {
                    String URL = "http://kayrun.cs.rit.edu:5000/Key/" + email;
                    Task.WaitAll(putRequest(URL, keyContent.ToStringKey()));
                    Console.WriteLine("Key saved");
                }
            }
            catch (HttpRequestException){}
        }
    }
}