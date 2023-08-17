/*
 * @author: Soumya Dayal
 * The main purpose of this class is to generate the public and the private key for the user and store them in their
 * respective files. The code uses RSA and generates the key for that cryptosystem.
 */
using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace Messenger
{
    /// <summary>
    /// This class generates the public and the private key.
    /// </summary>
    public class KeyGenerator
    {
        private static BigInteger E;
        private static BigInteger N;
        private static BigInteger D;
        private int keySize;
        
        public KeyGenerator(int _keysize)
        {
            keySize = _keysize;
        }
        
        /// <summary>
        /// Calls the prime number generator for generating the E and D value for producing the private and public keys.
        /// </summary>
        public void keyGenerate()
        {
            var pqSize = keySize / 2;
            PrimeNumberGenerator pqGenerator = new PrimeNumberGenerator(pqSize);
            var p = pqGenerator.RandomPrimeNumber();
            var q = pqGenerator.RandomPrimeNumber();
            N = p * q;
            var r = (p - 1) * (q - 1);
            PrimeNumberGenerator EGenerator = new PrimeNumberGenerator(16);
            E = EGenerator.RandomPrimeNumber();
            D = modInverse(E, r);
        }
        
        /// <summary>
        /// Generates the private key for the user and returns it to be stored in the PrivateKeyEmails class.
        /// </summary>
        /// <returns>privateKey of type String</returns>
        public String privateKey()
        {
            byte[] DBytesArray = D.ToByteArray();
            byte[] NBytesArray = N.ToByteArray();
            
            var d = BitConverter.GetBytes(DBytesArray.Length);
            var n = BitConverter.GetBytes(NBytesArray.Length);
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(d);
                Array.Reverse(n);
            }

            var priKey = new Byte[4 + DBytesArray.Length + 4 + NBytesArray.Length];
            d.CopyTo(priKey, 0);
            DBytesArray.CopyTo(priKey, 4);
            n.CopyTo(priKey, 4 + DBytesArray.Length);
            NBytesArray.CopyTo(priKey, 4 + DBytesArray.Length + 4);
            var encodePriKey = Convert.ToBase64String(priKey);
            return encodePriKey;
        }
        
        /// <summary>
        /// Generates the public key for the user and stores it in the public.key file
        /// </summary>
        public void publicKey()
        {
            byte[] EBytesArray = E.ToByteArray();
            byte[] NBytesArray = N.ToByteArray();

            var e = BitConverter.GetBytes(EBytesArray.Length);
            var n = BitConverter.GetBytes(NBytesArray.Length);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(e);
                Array.Reverse(n);
            }

            var pubKey = new Byte[4 + EBytesArray.Length + 4 + NBytesArray.Length];
            e.CopyTo(pubKey, 0);
            EBytesArray.CopyTo(pubKey, 4);
            n.CopyTo(pubKey, 4 + EBytesArray.Length);
            NBytesArray.CopyTo(pubKey, 4 + EBytesArray.Length + 4);
            var encodePubKey = Convert.ToBase64String(pubKey);
            File.WriteAllText("public.key", encodePubKey);
        }
        
        /// <summary>
        /// Support function for calculating the inverse of a modulus. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns>The inverse value of type BigInteger</returns>
        static BigInteger modInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a>0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }

            v %= n;
            if (v<0)
            {
                v = (v + n) % n;
            }

            return v;
        }
    }
}