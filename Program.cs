using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using eskb_reader.ESKB;

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
                eskb.shellQuery(q);
            }

        }
    }

}