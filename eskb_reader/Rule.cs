using System;
using System.Collections.Generic;

namespace eskb_reader.ESKB
{
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
        public Rule()
        {
        }

        // public Dictionary<string, List<String>> check(ISequence input, ESKB eskb, List<String> unknowns)
        // {
        //     String x = input.GetPrimeLiteral();
        //     String y = input.GetSecondLiteral();
        //     // var unknownList = new List<String>();
        //     // if (!isRecursive && !x.Contains('"')) unknowns.Add(x);
        //     // if (!isRecursive && !y.Contains('"')) unknowns.Add(y);

        //     Dictionary<string, List<String>> dict = new Dictionary<string, List<string>>();
        //     Rule r = this.Clone();
        //     foreach (var item in r.rightSide)
        //     {
        //         if (r.leftSide.GetPrimeLiteral() == item.GetPrimeLiteral()) item.SetPrimeLiteral(x);
        //         if (r.leftSide.GetSecondLiteral() == item.GetSecondLiteral()) item.SetSecondLiteral(y);
        //         dict.Append(eskb.query(item, unknowns));
        //     }

        //     // r.leftSide.SetPrimeLiteral(x);
        //     // r.leftSide.SetSecondLiteral(y);
        //     String result = "";
        //     bool all = false;
        //     foreach (var item in dict)
        //     {
        //         // foreach (var i in item.Value)
        //         // {
        //         //     System.Console.WriteLine((item.Key + " haha =") + i);
        //         // }

        //         if (item.Key != "Result") all = true;
        //         if (!all && result != "yes") result = item.Value[0];
        //     }
        //     if (all && result == "no") System.Console.WriteLine("no");
        //     else
        //         foreach (var item in dict)
        //         {
        //             foreach (var i in item.Value)
        //             {
        //                 System.Console.WriteLine((item.Key + "=") + i);
        //             }
        //         }
        //     return dict;
        // }
        public Rule Clone()
        {
            Rule r = new Rule();
            r.leftSide = leftSide;
            r.rightSide = rightSide;
            return r;
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


}