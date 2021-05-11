using System.Collections.Generic;
using System.Linq;

namespace eskb_reader.ESKB
{
    public static class Utils
    {
        public static void Intersect(this List<string> first, List<string> second)
        {
            for (int i = 0; i < first.Count; i++)
            {
                bool has = false;
                foreach (var s in second)
                {
                    if (first[i] == s) has = true;
                }
                if (!has) first.Remove(first[i]);
            }
        }
        public static void UnionDict(this Dictionary<string, List<string>> first, Dictionary<string, List<string>> second)
        {
            List<KeyValuePair<string, List<string>>> pairs = second.ToList();

            pairs.ForEach(pair =>
            {
                if (first.ContainsKey(pair.Key))
                {
                    List<string> list;
                    first.TryGetValue(pair.Key, out list);
                    list.AddRange(pair.Value);
                    first.Remove(pair.Key);
                    first.Add(pair.Key, list);
                }
                else
                    first.TryAdd(pair.Key, pair.Value);
            });
            // pairs.ForEach(pair => first.ContainsKey(pair.Key) ? first.TryAdd(pair.Key, first.TryGetValue(pair.Key) : first.TryAdd(pair.Key, pair.Value));
        }
    }
    // public static class Extensions
    // {
    //     public static void Append<String, List<String, List<string>>>(this Dictionary<String, List<String, List<string>>> first, Dictionary<String, List<String, List<string>>> second)
    //     {
    //         // foreach (var f in first)
    //         // {
    //         //     bool flag = false;
    //         //     foreach (var s in second)
    //         //     {
    //         //         if (f.Key.Equals(s.Key)) flag = true;
    //         //     }
    //         // }
    //         List<KeyValuePair<K, V>> pairs = second.ToList();

    //         pairs.ForEach(pair => first.ContainsKey(pair.Key) ? first.TryAdd(pair.Key , first.TryGetValue(pair.Key)): first.TryAdd(pair.Key, pair.Value));
    //     }
    // }
}