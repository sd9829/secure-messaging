/*
 * @author: Soumya Dayal
 * This class focuses on producing the prime numbers for the encryption and decryption purposes.
 * It runs Int32.MaxValue number of threads to find a prime number of certain bits provided by the user.
 */

using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Messenger
{
    public class PrimeNumberGenerator
    {
        private int bytes;
        private RNGCryptoServiceProvider randomNumber;
        private BigInteger _primeNumber;
        private static Object primeNumberLock = new Object();

        public PrimeNumberGenerator(int _bits)
        {
            bytes = _bits/8;
            randomNumber = new RNGCryptoServiceProvider();
            _primeNumber = 0;
        }
        
        /// <summary>
        /// This function produces a random prime number using Int32.MaxValue number of threads.
        /// </summary>
        /// <returns>_primeNumber which is of type BigInteger.</returns>
        public BigInteger RandomPrimeNumber()
        {
            Parallel.For(0, Int32.MaxValue, (index, state) =>
            {
                byte[] data = new byte[bytes];
                randomNumber.GetBytes(data);
                if (new BigInteger(data).IsProbablyPrime())
                {
                    lock (primeNumberLock)
                    {
                        _primeNumber = new BigInteger(data);
                        state.Stop();
                    }
                }
            });
            return _primeNumber;
        }
    }
    
    /// <summary>
    /// The following class checks if the number provided is prime or not. 
    /// </summary>
    static class PrimeNumberCheck
    {
        public static Boolean IsProbablyPrime(this BigInteger value, int witnesses = 10)
        {
            if (value <= 1) return false;

            if (witnesses <= 0) witnesses = 10;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            Byte[] bytes = new Byte[value.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < witnesses; i++)
            {
                do
                {
                    var Gen = new Random();
                    Gen.NextBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= value-2);
                
                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1) continue;

                for (int r = 0; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == 1) return false;
                    if (x == value - 1) break;
                }

                if (x != value - 1) return false;
            }

            return true;
        }
    }
}