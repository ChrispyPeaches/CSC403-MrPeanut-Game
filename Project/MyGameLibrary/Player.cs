using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fall2020_CSC403_Project.code
{
    public class Player : BattleCharacter
    {
        public string Name { get; set; }
        public int coinCounter { get; set; }
        public Player(Vector2 initPos, Collider collider, JObject playerData) : base(initPos, collider)
        {
            int health = playerData.Value<int>("Health");
            int maxHealth = playerData.Value<int>("MaxHealth");
            float strength = playerData.Value<float>("strength");
            this.Name = playerData.Value<string>("name"); 
            this.ChangeHealthAndStrength(health, maxHealth, strength);
            this.coinCounter = playerData.Value<int>("coinCounter");
        }
    }
}
