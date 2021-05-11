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

            var lines = File.ReadLines("family.eskb");
            var facts = lines
                            .SkipWhile(line => !line.StartsWith("[FACTS]"))
                            .Skip(1)
                            .SkipWhile(line => string.IsNullOrEmpty(line))
                            .TakeWhile(line => !line.Equals("[RULES]")).Select(line => line);


            var rules = lines
                            .SkipWhile(line => !line.StartsWith("[RULES]"))
                            .Skip(1)
                            .SkipWhile(line => string.IsNullOrEmpty(line))
                            // .TakeWhile(line => !line.Equals("[RULES]"))
                            .Select(line => line);


            foreach (var item in facts)
            {
                if (!string.IsNullOrEmpty(item))
                    eskb.AddFact(item);
            }

            foreach (var item in rules)
            {
                if (!string.IsNullOrEmpty(item))
                    eskb.AddRule(item);
            }

            // System.Console.WriteLine(eskb.ToString());

            while (true)
            {
                string q = Console.ReadLine();
                eskb.query(q);
            }

        }
    }
    public interface ISequence
    {
        public string GetKey();
        public string GetPrimeLiteral();
        public string GetSecondLiteral();
        public int checkConstOrVar();
    }
    class ESKB
    {
        List<ISequence> facts;
        List<Rule> rules;
        // List<ISequence>
        public ESKB()
        {
            facts = new List<ISequence>();
            rules = new List<Rule>();
        }
        public void query(string str)
        {
            ISequence input = GetLiteral(str);
            int varCheck = input.checkConstOrVar();
            if (varCheck == -1)
            {
                bool f = false;
                foreach (var item in facts)
                {
                    if (item.Equals(input)) f = true;
                }
                System.Console.WriteLine(f ? "yes" : "no");
            }
            else if (varCheck == 0)
            {
                foreach (var item in facts)
                {
                    if (item.GetKey() == input.GetKey() && item.GetSecondLiteral() == input.GetSecondLiteral())
                        System.Console.WriteLine(input.GetPrimeLiteral() + "=" + item.GetPrimeLiteral());
                }
            }
            else if (varCheck == 1)
            {
                foreach (var item in facts)
                {
                    if (item.GetKey() == input.GetKey() && item.GetPrimeLiteral() == input.GetPrimeLiteral())
                        System.Console.WriteLine(input.GetSecondLiteral() + "=" + item.GetSecondLiteral());
                }
            }
            else if (varCheck == 2)
            {
                foreach (var item in facts)
                {
                    if (item.GetKey() == input.GetKey())
                    {
                        System.Console.WriteLine(input.GetPrimeLiteral() + "=" + item.GetPrimeLiteral());
                        System.Console.WriteLine(input.GetSecondLiteral() + "=" + item.GetSecondLiteral());
                    }
                }
            }
            else
            {
                System.Console.WriteLine("No Match");
            }
        }
        // public int checkConstOrVar(ISequence input)
        // {
        //     if (input.checkConstOrVar() == -1) return -1;
        // }
        public void AddFact(string input)
        {
            facts.Add(GetLiteral(input));
        }

        public void AddRule(string input)
        {
            // System.Console.WriteLine("rule is "+input);
            rules.Add(new Rule(input));
        }
        public static ISequence GetLiteral(String input)
        {
            return input.Contains(",") ? new TwoLiteral(input) : new OneLiteral(input);
        }

        public override string ToString()
        {
            string str = "";
            foreach (ISequence item in facts)
            {
                str += item.ToString() + "\n";
            }
            foreach (var item in rules)
            {
                str += item.ToString() + "\n";
            }
            return str;
        }
    }
    public class Rule
    {
        public ISequence leftSide;
        public List<ISequence> rightSide = new List<ISequence>();

        public Rule(string input)
        {
            var left = input.Split('=')[0];
            var rights = input.Split('=')[1].Split(", ");
            leftSide = ESKB.GetLiteral(left);
            foreach (var item in rights)
            {
                rightSide.Add(ESKB.GetLiteral(item));
            }
        }

        public override string ToString()
        {
            string ans = "left side : " + leftSide.ToString() + " .. right side is ";
            foreach (var item in rightSide)
            {
                ans += item.ToString() + " and ";
            }
            return ans;
        }
    }
    public class OneLiteral : ISequence
    {
        Regex keyrx = new Regex(@".*\(",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex valuerx = new Regex(@"\(.*,?.*\)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public String key;
        public String attribute;
        public OneLiteral(String input)
        {
            key = keyrx.Match(input).Value.Replace("(", "").Replace(" ", ""); ;
            var vals = valuerx.Match(input).Value.Split(",");
            attribute = vals[0].Replace("(", "").Replace(")", "").Replace(" ", "");
        }


        public override string ToString()
        {
            return key + " : " + attribute;
        }

        public override bool Equals(object obj)
        {
            if (obj is TwoLiteral) return false;
            return key == (obj as OneLiteral).key &&
                   attribute == (obj as OneLiteral).attribute ? true : false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(keyrx, valuerx, key, attribute);
        }

        public int checkConstOrVar()
        {
            if (!attribute.Contains('"')) return 0;
            return -1;
        }

        public string GetKey()
        {
            return key;
        }

        public string GetPrimeLiteral()
        {
            return attribute;
        }

        public string GetSecondLiteral()
        {
            return null;
        }
    }
    public class TwoLiteral : ISequence
    {
        Regex keyrx = new Regex(@".*\(",
           RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex valuerx = new Regex(@"\(.*,?.*\)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public String key;
        public String prime;
        public String second;
        public TwoLiteral(String input)
        {
            // System.Console.WriteLine(input );
            key = keyrx.Match(input).Value.Replace("(", "").Replace(" ", ""); ;
            var vals = valuerx.Match(input).Value.Split(",");
            prime = vals[0].Replace("(", "").Replace(")", "").Replace(" ", "");
            second = vals[1].Replace("(", "").Replace(")", "").Replace(" ", "");
        }


        public override string ToString()
        {
            return key + " : prime(" + prime + ")" + " second(" + second + ")";
        }

        public override bool Equals(object obj)
        {
            if (obj is OneLiteral) return false;

            return
                   key == (obj as TwoLiteral).key &&
                   prime == (obj as TwoLiteral).prime &&
                   second == (obj as TwoLiteral).second;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(keyrx, valuerx, key, prime, second);
        }

        public int checkConstOrVar()
        {
            if (!prime.Contains('"') && !second.Contains('"')) return 2;
            if (!second.Contains('"')) return 1;
            if (!prime.Contains('"')) return 0;
            return -1;
        }

        public string GetKey()
        {
            return key;
        }

        public string GetPrimeLiteral()
        {
            return prime;
        }

        public string GetSecondLiteral()
        {
            return second;
        }
    }

}
