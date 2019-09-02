using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CheckingLib
{
    public class Variables : Dictionary<string, string>
    {

        public Variables()
        {
        }

        public String Transform(String Input)
        {
            String output = Input;
            Regex regex = new Regex("%([A-Za-z0-9]+)%");
            MatchCollection matches = regex.Matches(Input);
            foreach (Match match in matches)
            {
                if (ContainsKey(match.Groups[1].Value))
                {
                    output = Regex.Replace(output, match.Groups[0].Value, this[match.Groups[1].Value]);
                }
            }

            return output;
        }
    }
}
