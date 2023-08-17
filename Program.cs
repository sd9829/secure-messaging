/*
 * @author: Soumya Dayal
 * This class is the main class that performs all the functions that are to be provided by the user.
 * The functions to help the user:
 * dotnet run <option> <other arguments>
 * There are several options:
 * keyGen <keysize>
 * sendKey <email>
 * getKey <email>
 * sendMsg <email> <plaintext>
 * getMsg <email>
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Messenger
{
    /// <summary>
    /// This class consists of the data to be stored for the private key
    /// It contains the List of emails and the private key for all those emails.
    /// </summary>
    public class PrivateKeyEmails
    {
        public List<String> emails { get; set; }
        public String keys { get; set; }

        public PrivateKeyEmails()
        {
            emails = new List<string>();
        }
    }
    
    /// <summary>
    /// This is the main class that will help the user call the functions and their arguments
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Invalid Argument
            if (args.Length == 1)
            {
                Console.WriteLine("dotnet run <option> <other arguments>");
                return;
            }
            
            // option = keyGen <keysize>
            // keysize - size of the key to be generated
            // This will produce the private and the public key for the user.
            if (args[0] == "keyGen")
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("dotnet run <option> <bitsize>");
                    return;
                }

                if (Int32.Parse(args[1])%8 != 0)
                {
                    Console.WriteLine("key size should be a multiple of 8");
                    return;
                }
                KeyGenerator kg = new KeyGenerator(Int32.Parse(args[1]));
                kg.keyGenerate();
                kg.publicKey();
                var privateKey = kg.privateKey();
                try
                {
                    PrivateKeyEmails privateKeyEmails = new PrivateKeyEmails();
                    privateKeyEmails.keys = privateKey;
                    File.WriteAllText("private.key", JsonSerializer.Serialize(privateKeyEmails));
                }
                catch (UnauthorizedAccessException) {}
                catch (DirectoryNotFoundException) {}
            }
            
            // option = sendKey <email>
            // email - the email for which the public key was generated
            // This will send the public key for that certain email to the server.
            else if (args[0] == "sendKey")
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("email is needed");
                }
                else
                {
                    try
                    {
                        SendKey sendKey = new SendKey(args[1]);
                        sendKey.sendKeyToServer();
                        var email = JsonSerializer.Deserialize<PrivateKeyEmails>(
                            File.ReadAllText("private.key"));
                        email.emails.Add(args[1]);
                        File.WriteAllText("private.key", JsonSerializer.Serialize(email));
                    }
                    catch (NullReferenceException) {}
                    catch (UnauthorizedAccessException) {}
                    catch (DirectoryNotFoundException) {}
                }
            }
            
            // option = getKey <email>
            // email - the email for which the public key is to be retrieved.
            // This will get the public key for the certain email provided by the user from the server.
            else if (args[0] == "getKey")
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("email is needed");
                }
                else
                {
                    GetKey getKey = new GetKey(args[1]);
                    getKey.storePublicKey();
                }
            }
            
            // option = sendMsg <email> <plaintext>
            // email - the email to which the message is to be sent
            // plaintext - the message to be sent
            // This will send the message to the email specified by the user after encryption.
            else if (args[0] == "sendMsg")
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("email and the message is needed");
                }
                else
                {
                    SendMessage sendMessage = new SendMessage(args[1], args[2]);
                    sendMessage.sendMessage();
                }
            }
            
            // option = getMsg <email>
            // email - the email to which the message is to be sent
            // This will get the message for the email specified by the user. 
            else if (args[0] == "getMsg")
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("email is missing");
                }
                else
                {
                    var priKey = JsonSerializer.Deserialize<PrivateKeyEmails>(
                        File.ReadAllText("private.key"));
                    var emailExists = priKey.emails.Contains(args[1]);
                    if (emailExists)
                    {
                        GetMessage getMessage = new GetMessage(args[1]);
                        getMessage.messageDecoding(priKey.keys);
                    }
                    else
                    {
                        Console.WriteLine("The message cannot be decoded.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Your command line arguments should be of the form: ");
                Console.WriteLine("dotnet run <option> <other arguments>");
                Console.WriteLine("option: ");
                Console.WriteLine("keyGen <keysize> -- (Generate private and public key)" +
                                  "\nsendKey <email> -- (Send the public key to the server for the email)" +
                                  "\ngetKey <email> -- (Get the public key from the server for the email)" +
                                  "\nsendMsg <email> <plaintext> -- (Send the message to the certain email)" +
                                  "\ngetMsg <email> -- (Get message from the server for that certain email)");
                Console.WriteLine("The keys need to be generated before any other command.");
            }
        }
    }
}