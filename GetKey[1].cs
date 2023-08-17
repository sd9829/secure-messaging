/*
 * @author: Soumya Dayal
 * This class focuses on getting the public key for the certain email provided
 * by the user. 
 */
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Messenger
{
    /// <summary>
    /// This class gets the public key for the certain email provided by the user.
    /// </summary>
    public class GetKey
    {
        private String email;
        private HttpClient client;

        public GetKey(String _email)
        {
            email = _email;
            client = new HttpClient();
        }

        /// <summary>
        /// The function sends the GET request to the server.
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public async Task<String> getRequest(String URL)
        {
            var result = await client.GetAsync(URL);
            String response = await result.Content.ReadAsStringAsync();
            return response;
        }

        /// <summary>
        /// The function gets the public key of the certain email from the server.
        /// </summary>
        /// <returns>public key of type String</returns>
        public String getPublicKey()
        {
            String URL = "http://kayrun.cs.rit.edu:5000/Key/" + email;
            Task<String> jsonString = getRequest(URL);
            Task.WaitAll(jsonString);
            String temp = jsonString.Result.Substring(1, jsonString.Result.Length - 2);
            String[] split1 = temp.Split(',');
            String[] split2 = split1[1].Split(':');
            String publicKey = split2[1].Substring(1, split2[1].Length - 2);
            return publicKey;
        }

        /// <summary>
        /// Stores the public key in the certain file locally with the file name of the format:
        /// email+".key"
        /// </summary>
        public void storePublicKey()
        {
            KeyContent keyContent = new KeyContent();
            keyContent.email = email;
            if (getPublicKey() != null)
            {
                keyContent.content = getPublicKey();
                StreamWriter writer = new StreamWriter(email+".key");
                writer.Write(keyContent);
                writer.Close();
            }
        }
    }
}