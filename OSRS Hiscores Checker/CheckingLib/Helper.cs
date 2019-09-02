using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CheckingLib
{
    public static class Helper
    {
        public static String EncodeFormUrlContent(IDictionary<String, String> Parameters)
        {
            String data = "";
            KeyValuePair<String, String> Parameter;
            using (IEnumerator<KeyValuePair<String, String>> e = Parameters.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    Parameter = e.Current;
                    data += $"{Parameter.Key}={Parameter.Value}&";
                }
            }
            return data;
        }
    }
}
