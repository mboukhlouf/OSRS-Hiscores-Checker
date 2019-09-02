using System;
using System.Collections.Generic;
using System.Text;

namespace CheckingLib
{
    public class Response
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public Dictionary<string, string[]> Headers { get; set; }
        public string Body { get; set; }

        public Response()
        {
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"HTTP/{Version} {StatusCode} {ReasonPhrase}");
            foreach (KeyValuePair<string, string[]> header in Headers)
            {
                foreach (var value in header.Value)
                    builder.AppendLine($"{header.Key}: {value}");
            }
            builder.AppendLine();
            builder.AppendLine(Body);
            return builder.ToString();
        }
    }
}
