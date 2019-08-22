using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testt
{
    public class Player
    {
        [JsonProperty("name")]
        public string PlayerName { get; set; }

        [JsonProperty("id")]
        public int PlayerId { get; set; } = -1;

        [JsonProperty("option")]
        public bool JoinToExistingGame { get; set; }
    }
}
