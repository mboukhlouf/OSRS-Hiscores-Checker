using System;
using System.Collections.Generic;
using System.Text;

namespace Runescape.OSRS
{
    public class User
    {
        public string Username { get; set; }
        public Skill[] Skills { get; set; }

        public User()
        {
            Username = "";
            Skills = new Skill[24];
        }

        public User(string username)
        {
            Username = username;
            Skills = new Skill[24];
        }

        public void ParseSkillsFromText(string text)
        {
            string[] tokens = text.Split("\n");
            if(tokens.Length < 24)
            {
                throw new ParsingException($"Expecting atleast 24 tokens, but found {tokens.Length}.");
            }

            for(int i = 0; i < 24; i++)
            {
                Skills[i] = new Skill(tokens[i]);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Username + ",");
            for(int i = 0; i < Skills.Length; i++)
            {
                builder.Append($"{Skills[i].Level},{Skills[i].Xp}");
                if (i != Skills.Length - 1)
                    builder.Append(",");
            }
            return builder.ToString();
        }
    }
}
