using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitterStatistics
{
    /// <summary>
    /// Determining the top emojis, hashtags, or domains is tricky because given the volume of data, eventually you will run out of memory
    /// to store all the values and rank them.  So we must make a trade-off with an algorithm that is o(1) rather than o(n) memory.
    /// So we will just store top values over a fixed or rolling time period.
    /// A fixed time period like hourly is easy for a user to understand but will have volatile results at the beginning of hours
    /// A moving time period like trailing 60 minutes solves that issue but gets complex storaging and processing timestamps of everything
    /// This algorithm will store tops by 15 minute intervals and combine to produce trailing hour results, which will be 
    /// </summary>
    public class Top
    {
        private int length;
        private int topLength;

        // queue holds last x values
        private Queue<string> values = new Queue<string>();

        // dictionary can track counts of each with O(1) lookup
        private Dictionary<string, int> counts = new Dictionary<string, int>();

        // 0 is most frequent key
        private List<string>rankedKeys = new List<string>();


        public Top(int _valuesLength, int _topLength)
        {
            length = _valuesLength;
            topLength = _topLength;

            if (topLength > length)
                throw new Exception("First parameter must greater than second");

            if (length <= 0)
                throw new Exception("Length must be greater than 0");
        }

        public void Track(IEnumerable<string> newItems)
        {
            if (newItems == null)
                return;

            foreach (var item in newItems)
                Track(item);
        }

        public void Clear()
        {
            // v2 test threadsafety add locks
            values.Clear();
            counts.Clear();
            rankedKeys.Clear();
        }

        /// <summary>
        /// Count number of times this string occured
        /// </summary>
        public void Track(string newItem)
        {
            if (string.IsNullOrWhiteSpace(newItem))
                return;

            string removedItem = UpdateValues(newItem);

            if (newItem == removedItem)
                return;

            UpdateCounts(newItem, removedItem);

            Rank(newItem, removedItem);
        }

        public List<string> ToStrings()
        {
            // v2 add counts & pcts to each ranked item
            return rankedKeys.ToList(); // threadsafe copy
        }

        private string UpdateValues(string newItem)
        {
            // cannot optimize for case where newItem == oldItem as the sequence in the queue matters
            values.Enqueue(newItem);

            string removed = null;
            if (values.Count > length)
                removed = values.Dequeue();

            return removed;
        }

        private void UpdateCounts(string newItem, string removedItem)
        {
            if (counts.ContainsKey(newItem))
                counts[newItem]++;
            else
                counts.Add(newItem, 1);

            if (removedItem == null)
                return;

            // key must be present
            if (counts[removedItem] <= 1)
                counts.Remove(removedItem);
        }

        private void Rank(string newItem, string removedItem)
        {
            if (RemoveOldRanking(removedItem))
                Sort();

            if (AddNewRanking(newItem))
                Sort();
                
        }

        private void Sort()
        {
            rankedKeys = rankedKeys.OrderByDescending(k => counts[k]).ToList();
        }

        /// <summary>
        /// Does the newItem join top items (or move up/down that list)?
        /// Does the deleteItem drop out of top items (or move up/down)?
        /// </summary>
        private bool RemoveOldRanking(string removedItem)
        {
            if (removedItem == null)
                return false;

            int removedRanking = rankedKeys.IndexOf(removedItem);

            // case: need to possibly remove or lower rank of removedItem
            if (removedRanking >= 0)
            {
                // case: no count for item, so remove from ranks
                if (!counts.ContainsKey(removedItem))
                    rankedKeys.RemoveAt(removedRanking);

                return true;
            }

            return false;
        }

        private bool AddNewRanking(string newItem) 
        {
            int curRankedLength = rankedKeys.Count;
            int oldRanking = rankedKeys.IndexOf(newItem);

            // case: not enough ranked items, if this item is not on the list, just add it
            if (curRankedLength < topLength && oldRanking < 0) 
            { 
                rankedKeys.Add(newItem);
                return true;
            }

            int lowestRankedCount = counts[rankedKeys[curRankedLength - 1]];

            // case: new item already at top so nothing more to do
            if (oldRanking == 0)
                return false;

            // case: new item already on list, so resort in case
            if (oldRanking > 0)
                return true;

            // case: newItem not ranked yet, see if should be added to the list
            if (counts[newItem] > lowestRankedCount)
            {
                // add it to the end of the list
                rankedKeys.Insert(curRankedLength - 1, newItem);
                if (rankedKeys.Count > topLength)
                {
                    // remove last item and exit
                    rankedKeys.RemoveAt(rankedKeys.Count - 1);
                }
                return true;
            }

            return false;
        }
    }
}
