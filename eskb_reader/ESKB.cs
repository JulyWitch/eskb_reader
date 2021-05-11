using System;
using System.Collections.Generic;

namespace eskb_reader.ESKB
{
    public class ESKB
    {
        List<ISequence> facts;
        List<Rule> rules;
        // List<ISequence>
        public ESKB()
        {
            facts = new List<ISequence>();
            rules = new List<Rule>();
        }
        List<string> alreadyPrinted = new List<string>();




        public Dictionary<String, List<String>> findFact(ISequence input)
        {
            var dict = new Dictionary<String, List<String>>();
            int varCheck = input.checkConstOrVar();
            bool f = false;
            if (varCheck == -1) foreach (var item in facts)
                {
                    if (item.Equals(input))
                    {
                        dict.Add("Result", new List<string>() { "yes" });
                        f = true;
                    }
                }
            else if (!f)
            {
                if (varCheck == 0)
                {
                    foreach (var item in facts)
                    {
                        if (item.GetKey() == input.GetKey() && item.GetSecondLiteral() == input.GetSecondLiteral()) f = true;
                    }
                }
            }
            return dict;
        }


        public bool IsFact(ISequence input)
        {
            foreach (var item in facts)
            {
                if (item.Equals(input)) return true;
            }
            return false;
        }
        public bool IsKeyFact(ISequence input)
        {
            foreach (var item in facts)
            {
                if (item.GetKey() == input.GetKey()) return true;
            }
            return false;
        }
        public bool isConst(ISequence input) => input.checkConstOrVar() == -1;

        public void shellQuery(string inputstr)
        {
            ISequence input = GetLiteral(inputstr);
            Dictionary<String, List<String>> dict = new Dictionary<string, List<string>>();
            if (IsKeyFact(input)) dict.UnionDict(queryFact(input));
            else
            {
                dict.UnionDict(queryRule(input));
            }
            List<string> alreadyPrinted = new List<string>();
            List<String> unknowns = new List<string>();
            if (!input.GetPrimeLiteral().Contains('"')) unknowns.Add(input.GetPrimeLiteral());
            if (!input.GetSecondLiteral().Contains('"')) unknowns.Add(input.GetSecondLiteral());
            // DebugPrintDict(dict);
            foreach (var item in dict)
            {
                if (unknowns.Contains(item.Key))
                    foreach (var value in item.Value)
                    {
                        string line = item.Key + "=" + value;
                        if (!alreadyPrinted.Contains(line)) System.Console.WriteLine(line);
                        alreadyPrinted.Add(line);
                    }
                // else if (item.Key == "R")
                // {
                //     bool yes = false;
                //     foreach (var v in item.Value)
                //     {
                //         if (v == "yes") yes = true;
                //     }
                //     System.Console.WriteLine(yes ? "yes" : "no");
                //     break;
                // }
            }
            if (alreadyPrinted.Count == 0)
            {
                // System.Console.WriteLine("FUCK");
                bool yes = false;
                foreach (var item in dict)
                {
                    if (item.Value != null && item.Value.Count > 0) yes = true;
                }
                System.Console.WriteLine(yes ? "yes" : "no");
            }
        }
        public void DebugPrintDict(Dictionary<String, List<String>> dict)
        {
            System.Console.WriteLine("PRINT DICT DEBUG");
            foreach (var item in dict)
            {
                foreach (var value in item.Value)
                {
                    string line = item.Key + "=" + value;
                    System.Console.WriteLine(line);
                }
            }
        }
        public Dictionary<String, List<String>> RecursiveQuery(ISequence input)
        {
            Dictionary<String, List<String>> dict = new Dictionary<string, List<string>>();
            if (IsKeyFact(input)) dict.UnionDict(queryFact(input));
            else
            {
                dict.UnionDict(queryRule(input));
            }
            return dict;
        }
        public Dictionary<string, List<string>> queryRule(ISequence rule)
        {
            List<Rule> candidates = FindRulesByKey(rule.GetKey());
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            foreach (Rule r in candidates)
            {
                foreach (ISequence clause in r.rightSide)
                {
                    if (clause.GetPrimeLiteral() == r.leftSide.GetPrimeLiteral()) clause.SetPrimeLiteral(rule.GetPrimeLiteral());
                    else if (clause.GetPrimeLiteral() == r.leftSide.GetSecondLiteral()) clause.SetPrimeLiteral(rule.GetSecondLiteral());

                    if (clause.GetSecondLiteral() == r.leftSide.GetSecondLiteral()) clause.SetSecondLiteral(rule.GetSecondLiteral());
                    else if (clause.GetSecondLiteral() == r.leftSide.GetPrimeLiteral()) clause.SetSecondLiteral(rule.GetPrimeLiteral());
                    // System.Console.WriteLine("FucK " + clause.ToString());
                    // System.Console.WriteLine("FucK2 " + r.leftSide.ToString());

                }
                r.leftSide.SetPrimeLiteral(rule.GetPrimeLiteral());
                r.leftSide.SetSecondLiteral(rule.GetSecondLiteral());

                dict.UnionDict(CheckRule(r, rule));
                // System.Console.WriteLine(r);
                // DebugPrintDict(dict);
            }
            // switch (rule.checkConstOrVar())
            // {
            //     case -1:
            //         break;

            //     default:
            //         throw new IndexOutOfRangeException();
            // }
            return dict;
        }
        public Dictionary<string, List<string>> CheckRule(Rule rule, ISequence left)
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            foreach (ISequence item in rule.rightSide)
            {
                Dictionary<string, List<string>> newDict = RecursiveQuery(item);
                // if(rule.rightSide.IndexOf())
                if (dict.Count == 0) dict.UnionDict(newDict);
                else dict = FilterDict(dict, newDict);
                // dict.UnionDict(newDict);
                // DebugPrintDict(RecursiveQuery(item));
            }
            return dict;
        }
        public Dictionary<string, List<string>> FilterDict(Dictionary<string, List<string>> dict, Dictionary<string, List<string>> newDict)
        {
            foreach (var d in dict)
            {
                bool has = false;
                foreach (var nd in newDict)
                {
                    if (d.Key == nd.Key)
                    {
                        has = true;
                        nd.Value.Intersect(d.Value);
                    }
                }
                if (!has) newDict.Add(d.Key, d.Value);
            }
            return newDict;

        }

        public List<Rule> FindRulesByKey(string key)
        {
            List<Rule> rs = new List<Rule>();
            foreach (var item in rules)
            {

                if (item.leftSide.GetKey() == key) rs.Add(item);
            }

            return rs;
        }
        public Dictionary<string, List<string>> queryFact(ISequence input)
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            int varsPosition = input.checkConstOrVar();

            switch (varsPosition)
            {
                case -1:
                    dict.Add("R", new List<string>() { IsFact(input) ? "yes" : "no" });
                    break;
                case 0: // prime is a var
                    List<String> primelist = new List<string>();
                    foreach (var item in facts)
                    {
                        if (input.GetKey() == item.GetKey())
                            if (input.GetSecondLiteral() == item.GetSecondLiteral())
                                primelist.Add(item.GetPrimeLiteral());
                    }
                    dict.Add(input.GetPrimeLiteral(), primelist);
                    break;

                case 1: // second is a var
                    List<String> secondlist = new List<string>();
                    foreach (var item in facts)
                    {
                        if (input.GetKey() == item.GetKey())
                            if (input.GetPrimeLiteral() == item.GetPrimeLiteral())
                                secondlist.Add(item.GetSecondLiteral());
                    }
                    dict.Add(input.GetSecondLiteral(), secondlist);
                    break;

                case 2: // both prime and second are vars
                    List<String> bothprimelist = new List<string>();
                    List<String> bothsecondlist = new List<string>();
                    foreach (var item in facts)
                    {
                        // System.Console.WriteLine("FUCK FACT: " + item + " Input: " + input);
                        if (input.GetKey() == item.GetKey())
                        {
                            bothprimelist.Add(item.GetPrimeLiteral());
                            bothsecondlist.Add(item.GetSecondLiteral());
                        }
                    }
                    dict.TryAdd(input.GetPrimeLiteral(), bothprimelist);
                    dict.TryAdd(input.GetSecondLiteral(), bothsecondlist);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
            return dict;
        }






        // public void userQuery(ISequence input)
        // {
        //     alreadyPrinted = new List<string>();

        //     Dictionary<String, List<String>> dict = new Dictionary<string, List<string>>();

        //     var unknownList = new List<String>();
        //     if (!input.GetPrimeLiteral().Contains('"')) unknownList.Add(input.GetPrimeLiteral());
        //     if (!input.GetSecondLiteral().Contains('"')) unknownList.Add(input.GetSecondLiteral());
        //     if (IsKeyFact(input)) Console.WriteLine(IsFact(input) ? "yes" : "no");
        //     else dict = query(input, unknownList, true);
        //     foreach (var item in dict)
        //     {
        //         foreach (var i in item.Value)
        //         {
        //             System.Console.WriteLine(item.Key + "=" + i);
        //         }
        //     }
        // }



        // public Dictionary<String, List<String>> query(ISequence input, List<string> unknowns, bool isFactSearch = false)
        // {
        //     int varCheck = input.checkConstOrVar();
        //     var dict = new Dictionary<String, List<String>>();
        //     bool c = IsKeyFact(input);
        //     if (c)
        //     {
        //         if (varCheck == -1)
        //         {
        //             dict.Add("Result", new List<string>() { IsFact(input) ? "yes" : "no" });
        //         }
        //         else if (varCheck == 0)
        //         {
        //             var list = new List<String>();
        //             foreach (var item in facts)
        //             {
        //                 if (item.GetKey() == input.GetKey() && item.GetSecondLiteral() == input.GetSecondLiteral())
        //                 {
        //                     if (unknowns.Contains(input.GetPrimeLiteral()) && !alreadyPrinted.Contains(item.GetPrimeLiteral()))
        //                     {
        //                         list.Add(item.GetPrimeLiteral());
        //                         alreadyPrinted.Add(item.GetPrimeLiteral());
        //                     }

        //                 }
        //             }
        //             if (list.Count > 0) dict.Add(input.GetPrimeLiteral(), list);

        //         }
        //         else if (varCheck == 1)
        //         {
        //             var list = new List<String>();
        //             foreach (var item in facts)
        //             {
        //                 if (item.GetKey() == input.GetKey() && item.GetPrimeLiteral() == input.GetPrimeLiteral())
        //                 {
        //                     if (unknowns.Contains(input.GetSecondLiteral()) && !alreadyPrinted.Contains(item.GetSecondLiteral()))
        //                     {
        //                         list.Add(item.GetSecondLiteral());
        //                         alreadyPrinted.Add(item.GetSecondLiteral());
        //                     }
        //                 }
        //                 if (list.Count > 0) dict.Add(input.GetSecondLiteral(), list);
        //             }
        //         }
        //         else if (varCheck == 2)
        //         {
        //             var list = new List<String>();
        //             var list2 = new List<String>();
        //             foreach (var item in facts)
        //             {
        //                 if (item.GetKey() == input.GetKey())
        //                 {
        //                     if (unknowns.Contains(input.GetPrimeLiteral()) && !alreadyPrinted.Contains(item.GetPrimeLiteral()))
        //                     {
        //                         list.Add(item.GetPrimeLiteral());
        //                         alreadyPrinted.Add(item.GetPrimeLiteral());
        //                     }
        //                     if (unknowns.Contains(input.GetSecondLiteral()) && !alreadyPrinted.Contains(item.GetSecondLiteral()))
        //                     {
        //                         list2.Add(item.GetSecondLiteral());
        //                         alreadyPrinted.Add(item.GetSecondLiteral());
        //                     }
        //                 }
        //             }
        //             // System.Console.WriteLine("f");
        //             if (list.Count > 0) dict.Add(input.GetPrimeLiteral(), list);
        //             if (list2.Count > 0) dict.Add(input.GetSecondLiteral(), list2);
        //         }
        //     }
        //     else
        //     {
        //         foreach (var item in rules)
        //         {
        //             if (input.GetKey() == item.leftSide.GetKey()) dict.UnionDict(item.check(input, this, unknowns));
        //         }
        //     }
        //     return dict;
        //     // else
        //     // {
        //     //     System.Console.WriteLine("No Match");
        //     // }
        // }
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
}