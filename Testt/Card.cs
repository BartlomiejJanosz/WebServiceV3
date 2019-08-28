using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testt
{
    public class Card
    {
        public Card(int id, int level, string plWord, string angWord, string langVersion)
        {
            CardId = id;
            Level = level;
            PlWord = plWord;
            AngWord = angWord;
            //Untill now "PL" and "ANG" available
            LangVersion = langVersion;
        }

        [JsonProperty("cardId")]
        public int CardId { get; }

        [JsonProperty("level")]
        public int Level { get; }

        [JsonProperty("attackPoints")]
        public int AttackPoints { get { return CountAttackPoints(); } }

        [JsonProperty("plWord")]
        public string PlWord { get; }

        [JsonProperty("angWord")]
        public string AngWord { get; }

        [JsonProperty("langVersion")]
        public string LangVersion { get; }


        private int CountAttackPoints()
        {
            switch(Level)
            {
                case 1:
                    return 5;
                case 2:
                    return 4;
                case 3:
                    return 3;
                case 4:
                    return 2;
                case 5:
                    return 1;
            }
            return 0;
        }
    }
}
