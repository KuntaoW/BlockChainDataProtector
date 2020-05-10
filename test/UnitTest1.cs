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
        public struct DataHashed
        {
            public Int32 id { get; set; }
            public Int32 uid { get; set; }
            public double amount { get; set; }

            public Int32 fromUid { get; set; }

            public DateTime txTime { get; set; }

            public string hashKey { get; set; }
        }

        [Fact]
        public void Test1()
        {
         
            var hashSeed = "013e2357839a8e6886ab5951d76f411475428afc90947ee320161bbf18eb6048";
            CultureInfo MyCultureInfo = new CultureInfo("en-AU");

            DataHashed data1 = new DataHashed()
            { id = 834, uid = 101, amount = 50.50, fromUid = 88 , txTime = DateTime.ParseExact("01/05/2020","dd/MM/yyyy", MyCultureInfo) };
            
            data1.hashKey = HashData.HashedRecord(hashSeed, HashData.data1);

            Console.WriteLine("hashed Key for data1: " + HashData.data1.hashKey);
            Assert.Equal("41b3d52133a2221c310b9f722647016e9f0ef6ed5bb72ad69bd2e3a298dfc4e0", HashData.data1.hashKey);

            DataHashed data2 = new DataHashed()
              { id = 835, uid = 101, amount = 100,  fromUid = 88, txTime = DateTime.ParseExact("03/05/2020", "dd/MM/yyyy", MyCultureInfo) };
            data2.hashKey = HashData.HashedRecord(data1.hashKey, data2);
            Console.WriteLine("hashed Key for data2: " + data1.hashKey);
            Assert.Equal("f6a7b4d40dda8dd65099b80e1b0059d353e23cfe99dd6589f5ff86767b450725", data2.hashKey);

            DataHashed data3 = new DataHashed()
                { id = 836, uid = 101, amount = 80.55, fromUid = 88, txTime = DateTime.ParseExact("05/05/2020", "dd/MM/yyyy", MyCultureInfo) };
            data3.hashKey = HashData.HashedRecord(data2.hashKey, data3);
            Console.WriteLine("hashed Key for data3: " + data1.hashKey);
            Assert.Equal("473e537329acfaca1945fb5283aabf754313dd8108087f502584046e1531d04e", data3.hashKey);
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
