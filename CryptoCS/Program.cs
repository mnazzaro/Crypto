using System;
using System.Security.Cryptography;
using System.Text;
using System.Buffers.Binary;


namespace CryptoCS
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SHA256 sha = SHA256.Create()) {
                Block b = new Block(17, sha.ComputeHash(Encoding.ASCII.GetBytes("hello")));
                (byte[] temp, ulong nonce) = b.Mine(0);
                Console.WriteLine(BitConverter.ToString(temp));
                Console.WriteLine(nonce);
            }
        }
    }
}
