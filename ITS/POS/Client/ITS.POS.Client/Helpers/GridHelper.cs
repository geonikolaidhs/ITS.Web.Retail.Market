using ITS.POS.Model.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Helpers
{
    public static class GridHelper
    {
        public static int GetNextRow(int previous, int next, IEnumerable<DocumentDetail> list)
        {
            long motion = next - (long)previous;
            if (motion == 0)
            {
                motion = 1;
            }
            motion /= Math.Abs(motion);
            var l = list.ToList();
            Dictionary<int, DocumentDetail> dict = list.Select(g => new { Key = l.IndexOf(g), Value = g }).Where(x => x.Value.IsCanceled == false && x.Value.IsLinkedLine == false).ToDictionary(i => i.Key, i => i.Value);
            if (dict.Count() == 0)
                return -1;
            int minKey = dict.Min(g => g.Key), maxKey = dict.Max(g => g.Key);
            if (minKey == maxKey)
            {
                return minKey;
            }

            while (!dict.ContainsKey(next))
            {
                next += (int)motion;
                if (next > maxKey)
                    return previous;
                if (next < minKey)
                    return previous;
            }

            return next;
        }
    }
}
