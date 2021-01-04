using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TwitterStatistics;

namespace TwitterStatiticsTests
{
    public class TopTests
    {
        /// <summary>
        /// values of 0-99 all occur once, so expected ranking is 0-9 
        /// </summary>
        [Test]
        public void TestEqualCountsAscending()
        {
            const int valuesLength = 100;
            const int topLength = 10;

            var top = new Top(valuesLength, topLength);

            for (int i = 0; i < valuesLength; i++)
                top.Track(i.ToString());

            var expected = new List<string>();
            for (int i = 0; i < topLength; i++)
                expected.Add(i.ToString());

            Assert.AreEqual(expected, top.ToStrings());
        }


        [Test]
        public void TestEqualCountsDescending()
        {
            const int valuesLength = 100;
            const int topLength = 10;

            var top = new Top(valuesLength, topLength);

            for (int i = valuesLength; i > 0; i--)
                top.Track(i.ToString());

            var expected = new List<string>();
            for (int i = valuesLength; i > valuesLength - topLength; i--)
                expected.Add(i.ToString());

            Assert.AreEqual(expected, top.ToStrings());
        }

        [Test]
        public void TestLastItemChangesRank()
        {
            var inputs = new List<string> { "3", "1", "4", "1", "2", "2", "1", "2", "1" };
            var expected = new List<string> { "1", "2", "3" };
            Test(inputs, expected, inputs.Count, expected.Count);
        }

        [Test]
        public void TestLastJumpsToFirst()
        {
            var inputs = new List<string> { "3", "2", "1", "1" };
            var expected = new List<string> { "1", "3" };
            Test(inputs, expected, inputs.Count, expected.Count);
        }

        [Test]
        public void TestChangeRankLengthChangesResults()
        {
            // we want to show the change from this test and DRY
            TestLastJumpsToFirst();

            var inputs = new List<string> { "3", "2", "1", "1" };
            var expected = new List<string> { "1", "2" };
            Test(inputs, expected, inputs.Count-1, expected.Count);
        }

        [Test]
        public void TestBlanksIgnored()
        {
            var inputs = new List<string> { "a", "b", "c", "b", "z", "b", "", "b", "z", "", "" };
            var expected = new List<string> { "b", "z", "a" };
            Test(inputs, expected, inputs.Count - 3, expected.Count);
        }

        [Test]
        public void TestNoDuplicateRankedKeys()
        {
            var inputs = new List<string> { "a", "b", "c", "b" };
            var expected = new List<string> { "b", "c" };
            Test(inputs, expected, inputs.Count - 1, expected.Count + 1);
        }


        [Test]
        public void TestPreviousHighRankRemoved()
        {
            var inputs = new List<string> { "a", "b", "b"};
            var expected = new List<string> { "b" };
            Test(inputs, expected, inputs.Count - 1, expected.Count);
        }

        [Test]
        public void TestStoreUnfilled()
        {
            var inputs = new List<string> { "a", "b", "c" };
            Test(inputs, inputs.ToList(), 100, 99);
        }

        [Test]
        public void TestLastChangesRanking()
        {
            var inputs = new List<string> { "a", "b", "c", "b", "c", "c" };
            var expected = new List<string> { "c", "b" };
            Test(inputs, expected, inputs.Count-1, expected.Count);
        }

        [Test]
        public void TestRankNeverChanges()
        {
            var inputs = new List<string> { "a", "b", "a", "a", "b", "c", "d" };
            var expected = new List<string> { "a", "b" };
            Test(inputs, expected, inputs.Count - 1, expected.Count);
        }

        [Test]
        public void TestEmpties()
        {
            var inputs = new List<string> { null, string.Empty, null, null };
            var expected = new List<string>();
            Test(inputs, expected, inputs.Count-1, expected.Count);
        }

        private static void Test(List<string> inputs, List<string> expected, int valuesTracked, int ranksTracked)
        {
            var top = new Top(valuesTracked, ranksTracked);

            for (int i = 0; i < inputs.Count; i++)
                top.Track(inputs[i]);

            Assert.AreEqual(expected, top.ToStrings());
        }

    }
}
