using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Xml;
using Xunit;
using Xunit.Sdk;

namespace bcDataProtector
{
    public class testHashedData
    {
        ///Id   uid amount  fromUid txTime
        ///834	101 50.50	88	    20/03/2020
        ///835	101	100	    88	    21/03/2020
        ///836	101	80.55	88	    22/03/2020

        [Fact]
        public void Test1()
        {
         
            var hashSeed = "013e2357839a8e6886ab5951d76f411475428afc90947ee320161bbf18eb6048";
            CultureInfo MyCultureInfo = new CultureInfo("en-AU");

            HashData hashData = new HashData();
            hashData.HashTheData();

            HashData.DataHashed data1 = new HashData.DataHashed()
            { id = 834, uid = 101, amount = 50.50, fromUid = 88 , txTime = DateTime.ParseExact("01/05/2020","dd/MM/yyyy", MyCultureInfo) };
            
            data1.hashKey = HashData.HashedRecord(hashSeed, HashData.data1);

            Console.WriteLine("hashed Key for data1: " + data1.hashKey);
            Assert.Equal(data1.hashKey, HashData.data1.hashKey);

            HashData.DataHashed data2 = new HashData.DataHashed()
              { id = 835, uid = 101, amount = 100,  fromUid = 88, txTime = DateTime.ParseExact("03/05/2020", "dd/MM/yyyy", MyCultureInfo) };
            data2.hashKey = HashData.HashedRecord(data1.hashKey, data2);

            Console.WriteLine("hashed Key for data2: " + data2.hashKey);
            Assert.Equal(data2.hashKey, HashData.data2.hashKey);

            HashData.DataHashed data3 = new HashData.DataHashed()
                { id = 836, uid = 101, amount = 80.55, fromUid = 88, txTime = DateTime.ParseExact("05/05/2020", "dd/MM/yyyy", MyCultureInfo) };
            data3.hashKey = HashData.HashedRecord(data2.hashKey, data3);
            Console.WriteLine("hashed Key for data3: " + data3.hashKey);
            Assert.Equal(data3.hashKey, HashData.data3.hashKey);
        }

        [Theory]
        [InlineData(834, "013e2357839a8e6886ab5951d76f411475428afc90947ee320161bbf18eb6048", "41b3d52133a2221c310b9f722647016e9f0ef6ed5bb72ad69bd2e3a298dfc4e0")]
        public void testData1(int id, string prevHashKey, string hashKey)
        {
            HashData hashData = new HashData();
            hashData.HashTheData();

            var data1 = hashData.GetDataById(id);

            Assert.Equal(hashKey, HashData.data1.hashKey);

            data1.amount = data1.amount + 0.1;
            data1.hashKey = HashData.HashedRecord(prevHashKey, data1);
            Assert.NotEqual(hashKey, data1.hashKey);
        }

    }
}
