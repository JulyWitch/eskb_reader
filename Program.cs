using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace project
{
    class Program
    {
        static void Main(string[] args)
        {

            var vals = File.ReadLines("family.eskb")
            .SkipWhile(line => !line.StartsWith("[FACTS]"))
            .Skip(1)
            .TakeWhile(line => !line.Equals("[RULES]"))
            .Select(line => new Fact(line));
            foreach (var item in vals)
            {
                System.Console.WriteLine(item.ToString());
            }

        }
    }

    class ESKB
    {
    }
    public class Fact
    {
        Regex keyrx = new Regex(@".*\(",
           RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex valuerx = new Regex(@"\(.*,?.*\)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public String key;
        public List<String> values = new List<String>();
        public Fact(String input)
        {

            key = keyrx.Match(input).Value.Replace("(", "");
            foreach (var item in valuerx.Match(input).Value.Split(","))
            {
                values.Add(item);
            }
        }


        public override string ToString()
        {
            String v = "";
            foreach (var item in values)    
            {
                v += item + "  ";
            }
            return key + " : " + v ;
        }
    }

}
