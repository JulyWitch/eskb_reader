using System;
using System.Text.RegularExpressions;

namespace eskb_reader.ESKB
{
    public interface ISequence
    {
        public string GetKey();
        public string GetPrimeLiteral();
        public string GetSecondLiteral();
        public void SetPrimeLiteral(String literal);
        public void SetSecondLiteral(String literal);

        public int checkConstOrVar();
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

        public void SetPrimeLiteral(string literal)
        {
            attribute = literal;
        }

        public void SetSecondLiteral(string literal)
        {

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

        public void SetPrimeLiteral(string literal)
        {
            prime = literal;
        }

        public void SetSecondLiteral(string literal)
        {
            second = literal;
        }
    }

}


