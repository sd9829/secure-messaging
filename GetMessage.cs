/*
 * @author: Soumya Dayal
 * The class focuses on getting the message from the server for the email specified and decrypting it.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Messenger
{
    /// <summary>
    /// This class connects to the server and gets the message for the certain email.
    /// After getting the message it is checked if we have the private key for that email.
    /// If we don't then the message cannot be decoded else the message is decoded and printed.
    /// </summary>
    public class GetMessage
    {
        private String email;
        private HttpClient client;
        private static BigInteger D;
        private static BigInteger N;

        public GetMessage(String _email)
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
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private String findPrivateKey(String path)
        {
            Boolean fileExists = File.Exists(path);
            if (fileExists)
            {
                String privateKey = File.ReadAllText(path);
                String temp = privateKey.Substring(1, privateKey.Length - 2);
                String[] split1 = temp.Split(',');
                String[] split2 = split1[^1].Split(':');
                String key = split2[1].Substring(1, split2[1].Length - 2);
                Console.WriteLine("Key: " + key.GetType());
                return key;
            }

            return null;
        }
        
        /// <summary>
        /// This function decodes the private key for that email so that the message received
        /// can be decrypted using the value of D and N.
        /// </summary>
        /// <param name="key"></param>
        private void decodePrivateKey(String key)
        {
            // D:
            byte[] data = Convert.FromBase64String(key);
            byte[] d = data.Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(d);
            }
        
            var dInt = BitConverter.ToInt32(d,0);
            var byteD = data.Skip(4).Take(dInt).ToArray();
            D = new BigInteger(byteD);
            
            //N:
            byte[] n = data.Skip(dInt + 4).Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(n);
            }
        
            var nInt = BitConverter.ToInt32(n);
            var byteN = data.Skip(8 + dInt).Take(nInt).ToArray();
            N = new BigInteger(byteN);            
        }

        /// <summary>
        /// This function gets the encoded message and decodes it and prints it.
        /// </summary>
        /// <param name="privateKey"></param>
        public void messageDecoding(String privateKey)
        {
            String encodedMessage = getMessage();
            var byteMessage = Convert.FromBase64String(encodedMessage);
            decodePrivateKey(privateKey);
            BigInteger decryptedMessage = BigInteger.ModPow(new BigInteger(byteMessage), D, N);
            var message = Encoding.Default.GetString(decryptedMessage.ToByteArray());
            Console.WriteLine(message);
        }

        /// <summary>
        /// This function connects with the server to get the encoded message for the certain email.
        /// </summary>
        /// <returns></returns>
        private String getMessage()
        {
            String URL = "http://kayrun.cs.rit.edu:5000/Message/" + email;
            Task<String> jsonString = getRequest(URL);
            Task.WaitAll(jsonString);
            String temp = jsonString.Result.Substring(1, jsonString.Result.Length - 2);
            String[] split1 = temp.Split(',');
            String[] split2 = split1[1].Split(':');
            String encodedMessage = split2[1].Substring(1, split2[1].Length - 2);
            return encodedMessage;
        }
    }
}