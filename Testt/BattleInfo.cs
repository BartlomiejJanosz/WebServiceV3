using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testt
{
    public class BattleInfo
    {
        public BattleInfo(int battleInfoId, int playerId, int health, Card firstCard, Card secondCard)
        {
            PlayerId = playerId;
            Health = health;
            FirstCard = firstCard;
            SecondCard = secondCard;
            BattleInfoId = battleInfoId;
        }

        [JsonProperty("battleInfoId")]
        public int BattleInfoId { get; } 

        [JsonProperty("playerId")]
        public int PlayerId { get; }

        [JsonProperty("health")]
        public int Health { get; }

        [JsonProperty("firstCard")]
        public Card FirstCard { get; }

        [JsonProperty("secondCard")]
        public Card SecondCard { get; }

    }
}
