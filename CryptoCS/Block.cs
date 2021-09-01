using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Buffers.Binary;

namespace CryptoCS
{
    class Block
    {

        protected int transaction;
        protected byte[] prevHash;
        protected long timestamp;
        private static DateTime centuryBegin = new DateTime(2000, 1, 1);

        public Block(int transaction, byte[] prevHash)
        {
            this.timestamp = Block.getTime();
            this.transaction = transaction;
            try
            {
                this.prevHash = prevHash.ToArray();
            }
            catch (NullReferenceException e)
            {
                //Prob do something better here
                Console.WriteLine(e.Message);
            }
        }

        public static long getTime ()
        {
            return DateTime.Now.Ticks - centuryBegin.Ticks;
        }

        //[DllImport("QuickFuncs.dll", EntryPoint="combine")]
        //protected static extern String combine (int transaction, String prevHash, int l);

        protected byte[] combine (ulong nonce)
        {

            byte[] temp = new byte[52]; //prevHash is 32 bytes, transaction is 4, datetime is 8, nonce is 8
            prevHash.CopyTo(temp, 0);
            Span<byte> tempSpan = temp;
            BinaryPrimitives.TryWriteInt32BigEndian(tempSpan[32..36], transaction);
            BinaryPrimitives.TryWriteInt64BigEndian(tempSpan[36..44], timestamp);
            BinaryPrimitives.TryWriteUInt64BigEndian(tempSpan[44..], nonce);
            return temp;
        }

        //Rewrite in C later lol, this is trash
        public (byte[] solution, ulong nonce) Mine (int initialSeed)
        {
            using (SHA256 sha = SHA256.Create())
            {
                int i = 0;
                ulong adder = 0;
                bool flag = false;
                byte[] temp = new byte[32];
                while (!flag) {
                    adder++;
                    temp = sha.ComputeHash(combine(adder));
                    //Console.WriteLine(BitConverter.ToString(temp));
                    for (i = 0; i < 3; i++)
                    {
                        if (temp[i] == 0)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                return (temp, adder);
            }
        }
    }
}
