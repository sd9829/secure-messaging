/*
 * @author: Soumya Dayal
 * This class focuses on generating the JSON string format for saving the keys.
 */
using System;

namespace Messenger
{
    /// <summary>
    /// This class formats the string for the email and keys and saves them in the JSON format.
    /// </summary>
    public class KeyContent
    {
        public String email { get; set; }
        public String content { get; set; }
        
        /// <summary>
        /// JSON formatting for email and keys
        /// </summary>
        /// <returns></returns>
        public String ToStringKey()
        {
            string jsonString = "{\"email\": \"" + email + "\",\"key\":\"" + content + "\"}";
            return jsonString;
        }
        
        /// <summary>
        /// JSON formatting for email and content
        /// </summary>
        /// <returns></returns>
        public String ToStringMessage()
        {
            string jsonString = "{\"email\": \"" + email + "\",\"content\":\"" + content + "\"}";
            return jsonString;
        }

        /// <summary>
        /// JSON formatting for saving the public keys from the server.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String jsonString = "{\"email\": \"" + email + "\",\"content\":\"" + content + "\",\"messageTime\":\"" +
                                DateTime.Now.ToString("s") + "\"}";
            return jsonString;
        }
    }
}