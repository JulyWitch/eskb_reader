using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace project
{
    class Program
    {
        static void Main(string[] args)
        {
            ESKB eskb = new ESKB();


            var vals = File.ReadLines("family.eskb")
            .SkipWhile(line => !line.StartsWith("[FACTS]"))
            .Skip(1)
            .SkipWhile(line => string.IsNullOrEmpty(line))
            .TakeWhile(line => !line.Equals("[RULES]")).Select(line => line);







            foreach (var item in vals)
            {
                System.Console.WriteLine("ITEM : " + item);
                if (!string.IsNullOrEmpty(item))
                    // System.Console.WriteLine(item);
                    eskb.addItem(item);
                // eskb.addItem(item);
            }

            System.Console.WriteLine(eskb.ToString());

        }
    }
    public interface IFact
    {

    }
    class ESKB
    {
        List<IFact> facts;
        public ESKB()
        {
            facts = new List<IFact>();
        }
        public void addItem(string input)
        {
            if (input.Contains(",")) facts.Add(new RelationFact(input));
            else facts.Add(new AttributeFact(input));
        }


        public override string ToString()
        {
            string str = "";
            foreach (IFact item in facts)
            {
                str += item.ToString() + "\n";
            }
            return str;
        }
    }
    public class AttributeFact : IFact
    {
        Regex keyrx = new Regex(@".*\(",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex valuerx = new Regex(@"\(.*,?.*\)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public String key;
        public String attribute;
        public AttributeFact(String input)
        {
            key = keyrx.Match(input).Value.Replace("(", "");
            var vals = valuerx.Match(input).Value.Split(",");
            attribute = vals[0];
        }


        public override string ToString()
        {
            return key + " : " + attribute;
        }


    }
    public class RelationFact : IFact
    {
        Regex keyrx = new Regex(@".*\(",
           RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex valuerx = new Regex(@"\(.*,?.*\)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public String key;
        public String prime;
        public String second;
        public RelationFact(String input)
        {
            System.Console.WriteLine(input + " RELATION");
            key = keyrx.Match(input).Value.Replace("(", "");
            var vals = valuerx.Match(input).Value.Split(",");
            prime = vals[0];
            second = vals[1];
        }


        public override string ToString()
        {
            return key + " : prime(" + prime + ")" + " second(" + second + ")";
        }
    }

}
