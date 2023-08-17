/*
 * @author: Soumya Dayal
 * The class focuses on sending the plaintext message to the email specified by the user.
 */
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Messenger
{
    /// <summary>
    /// This class send the message to the email provided by the user. The message is
    /// encrypted and then sent to the server where the receiver can get the message and
    /// decrypt it using their own key.
    /// </summary>
    public class SendMessage
    {
        private String email;
        private String message;
        private HttpClient client;
        private static BigInteger E;
        private static BigInteger N;

        public SendMessage(String _email, String _message)
        {
            email = _email;
            message = _message;
            client = new HttpClient();
        }

        /// <summary>
        /// The function sends the PUT request to the server.
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="content"></param>
        private async Task putRequest(String URL, String content)
        {
            var data = new StringContent(content, Encoding.Default, "application/json");
            var response = await client.PutAsync(URL, data);
            response.EnsureSuccessStatusCode();
        }
        
        /// <summary>
        /// opens the public key file for that certain email and gets the public key.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>the public key of type String</returns>
        private String findPublicKey(String path)
        {
            Boolean fileExists = File.Exists(path);
            if (fileExists)
            {
                String publicKey = File.ReadAllText(path);
                String temp = publicKey.Substring(1, publicKey.Length - 2);
                String[] split1 = temp.Split(',');
                String[] split2 = split1[1].Split(':');
                String key = split2[1].Substring(1, split2[1].Length - 2);
                return key;
            }
            else
            {
                Console.WriteLine("Key does not exist for " + email);
                return null;
            }
        }

        /// <summary>
        /// This function decodes the public key for that email so that the message to be sent
        /// can be encrypted using the value of E and N.
        /// </summary>
        /// <param name="key"></param>
        private void decodePublicKey(String key)
        {
            // E:
            byte[] data = Convert.FromBase64String(key);
            byte[] e = data.Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(e);
            }
        
            var eInt = BitConverter.ToInt32(e,0);            
            var byteE = data.Skip(4).Take(eInt).ToArray();
            E = new BigInteger(byteE);
            
            //N:
            byte[] n = data.Skip(eInt + 4).Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(n);
            }
        
            var nInt = BitConverter.ToInt32(n);
            var byteN = data.Skip(8 + eInt).Take(nInt).ToArray();
            N = new BigInteger(byteN);
        }

        /// <summary>
        /// Message is encoded properly with the E and N values and then converted to base 64
        /// and then the encoded message is returned to the sendMessage funtion.
        /// </summary>
        /// <returns>Encoded message of type String</returns>
        private String messageEncoding()
        {
            var publicKeyPath = email + ".key";
            String publicKey = findPublicKey(publicKeyPath);
            if (publicKey != null)
            {
                byte[] byteMessage = Encoding.Default.GetBytes(message);
                decodePublicKey(publicKey);
                BigInteger encryptedMessage = BigInteger.ModPow(new BigInteger(byteMessage), E, N);
                var base64EncodedEncryptedText = Convert.ToBase64String(encryptedMessage.ToByteArray());
                return base64EncodedEncryptedText;
            }

            return null;
        }

        /// <summary>
        /// This function connects to the server and sends the message to the server for the certain email.
        /// </summary>
        public void sendMessage()
        {
            KeyContent keyContent = new KeyContent();
            keyContent.email = email;
            keyContent.content = messageEncoding();
            if (keyContent.content != null)
            {
                String URL = "http://kayrun.cs.rit.edu:5000/Message/" + email;
                Task.WaitAll(putRequest(URL, keyContent.ToStringMessage()));
                Console.WriteLine("Message written");
            }
        }
    }
}