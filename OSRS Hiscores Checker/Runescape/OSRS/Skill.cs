using System;

namespace Runescape.OSRS
{
    public class Skill
    {
        public int Rank { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }

        public Skill()
        {
        }

        public Skill(int rank, int level, int xp)
        {
            Rank = rank;
            Level = level;
            Xp = xp;
        }

        public Skill(string text)
        {
            ParseFromString(text);
        }

        public void ParseFromString(string text)
        {
            String[] tokens = text.Split(",");
            if(tokens.Length != 3)
            {
                throw new ParsingException($"Expecting 3 tokens in {text}, found {tokens.Length}.");
            }

            try
            {
                Rank = int.Parse(tokens[0]);
                Level = int.Parse(tokens[1]);
                Xp = int.Parse(tokens[2]);
            }
            catch(Exception e)
            {
                throw new ParsingException($"Exception at parsing values from \"{text}\".", e);
            }
        }
    }
}
