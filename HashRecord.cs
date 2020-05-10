using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Xunit.Sdk;

namespace bcDataProtector
{
    public class HashData
    {
        ///Id   uid amount  fromUid txTime
        ///834	101 50.50	88	    20/03/2020
        ///835	101	100	    88	    21/03/2020
        ///836	101	80.55	88	    22/03/2020

        public struct DataHashed
        {
            public Int32 id { get; set; }
            public Int32 uid { get; set; }
            public double amount { get; set; }

            public Int32 fromUid { get; set; }

            public DateTime txTime { get; set; }

            public string hashKey { get; set; }
        }

        static CultureInfo MyCultureInfo = new CultureInfo("en-AU");

        public static DataHashed data1 = new DataHashed()
        { id = 834, uid = 101, amount = 50.50, fromUid = 88, txTime = DateTime.ParseExact("01/05/2020", "dd/MM/yyyy", MyCultureInfo) };

        public static DataHashed data2 = new DataHashed()
        { id = 835, uid = 101, amount = 100, fromUid = 88, txTime = DateTime.ParseExact("03/05/2020", "dd/MM/yyyy", MyCultureInfo) };

        public static DataHashed data3 = new DataHashed()
        { id = 836, uid = 101, amount = 80.55, fromUid = 88, txTime = DateTime.ParseExact("05/05/2020", "dd/MM/yyyy", MyCultureInfo) };


        public void HashTheData()
        {
            //A random hashKey here, in General, it should come from previous record, or a provious record in the chain.
            //as sha256 always produce 256 bits of output, would represend 64 hex characters, so can define it as varchar(64) in db 
            var hashSeed = "013e2357839a8e6886ab5951d76f411475428afc90947ee320161bbf18eb6048";
            CultureInfo MyCultureInfo = new CultureInfo("en-AU");

            //DataHashed data1 = new DataHashed()
            //{ id = 834, uid = 101, amount = 50.50, fromUid = 88, txTime = DateTime.ParseExact("01/05/2020", "dd/MM/yyyy", MyCultureInfo) };
            data1.hashKey = HashedRecord(hashSeed, data1);
            Console.WriteLine("hashed Key for data1: " + data1.hashKey);

            //DataHashed data2 = new DataHashed()
            //{ id = 835, uid = 101, amount = 100, fromUid = 88, txTime = DateTime.ParseExact("03/05/2020", "dd/MM/yyyy", MyCultureInfo) };
            data2.hashKey = HashedRecord(data1.hashKey, data2);
            Console.WriteLine("hashed Key for data2: " + data1.hashKey);

            //DataHashed data3 = new DataHashed()
            //{ id = 836, uid = 101, amount = 80.55, fromUid = 88, txTime = DateTime.ParseExact("05/05/2020", "dd/MM/yyyy", MyCultureInfo) };
            data3.hashKey = HashedRecord(data2.hashKey, data3);
            Console.WriteLine("hashed Key for data3: " + data1.hashKey);

        }


        internal DataHashed GetDataById(int id)
        {
            switch (id)
            {
                case 834: return data1;
                case 835: return data2;
                case 836: return data3;
                default:
                    return data1;                    
            }            
        }

        static public string HashedRecord(string prev_hashKey, DataHashed hashData)
        {
            //in the record, the values of uid, amount, fromUid, and txTime should NOT allow to change.
            //convert them to hex strings and connected to a hex_header, plus previous hashKey
            //in this demo we use 'hashSeed' as the first one for data1.

            //Int32 to 8 length hex (2bytes)
            var id_hex = hashData.id.ToString("x8");
            var uid_hex = hashData.uid.ToString("x8");
            var fromUid_hex = hashData.fromUid.ToString("x8");

            //convert Double to long first , then to 16 length hex (4bytes)
            var amount_long = BitConverter.DoubleToInt64Bits(hashData.amount);
            var amount_hex = amount_long.ToString("x16");
            //same for Datetime , get ticks then to 16 lenght hex
            var txTime_hex = hashData.txTime.Ticks.ToString("x16"); ;

            //connect prev_hashKey + id + uid + amount + fromUid + txTime 
            string block_header = prev_hashKey + id_hex + uid_hex + amount_hex + fromUid_hex + txTime_hex;
            byte[] block_data = ToBytes(block_header);
            byte[] block_hasheddata = SHA256.Create().ComputeHash(block_data);
            string block_hashedKey = ToString(block_hasheddata);

            return block_hashedKey;
        }

        static public byte[] ToBytes(string value)
        {
            byte[] bytes = new byte[(value.Length + 1) / 2];
            for (int i = 0, j = 0; i < value.Length; j++, i += 2)
                bytes[j] = byte.Parse(value.Substring(i, value.Length >= i + 2 ? 2 : 1), System.Globalization.NumberStyles.HexNumber);
            return bytes;
        }

        static public string ToString(byte[] value)
        {
            string result = "";
            foreach (byte b in value)
                result += b.ToString("x2");
            return result;
        }
    }

    

}
