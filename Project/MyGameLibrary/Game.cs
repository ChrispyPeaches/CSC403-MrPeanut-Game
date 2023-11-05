using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fall2020_CSC403_Project.code;
using System.Linq.Expressions;
using System.Media;
using System.IO;
using System.Xml.Linq;
using System.Drawing.Printing;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Fall2020_CSC403_Project.code
{
    public class Game
    {
        public static Game instance;
        public Player player { get; set; }
        public Enemy bossKoolaid { get; set; }
        public Enemy enemyPoisonPacket { get; set; }
        public Enemy enemyCheeto { get; set; }
        public bool IsKoolAidDefeated { get; set; }
        public bool IsPoisonPacketDefeated { get; set; }
        public bool IsCheetosDefeated { get; set; }

        public Game(Dictionary<string, object> save)
        {
            if (instance == null)
            {
                instance = this;
            }
            InitializeGameEntities(save);
        }

        public static Game Instance
        {
            get
            {
                return instance;
            }
        }

        public void InitializeGameEntities(Dictionary<string, object> save)
        {
            // convert save dictionary into a JObject -- don't know why this is finnicky
            JObject playerData = (JObject)save["playerData"];
            JObject koolAidData = (JObject)save["enemy_koolaidData"];
            JObject poisonPacketData = (JObject)save["enemy_poisonpacketData"];
            JObject cheetosData = (JObject)save["enemy_cheetosData"];

            this.IsKoolAidDefeated = (bool)koolAidData["defeated"];
            this.IsPoisonPacketDefeated = (bool)poisonPacketData["defeated"];
            this.IsCheetosDefeated = (bool)cheetosData["defeated"];
            
            player = new Player(new Vector2(0, 0), null, playerData);
            bossKoolaid = new Enemy("enemy_koolaid", new Vector2(0, 0), null, koolAidData);
            enemyPoisonPacket = new Enemy("enemy_poisonpacket", new Vector2(0, 0), null, poisonPacketData);
            enemyCheeto = new Enemy("enemy_cheetos", new Vector2(0, 0), null, cheetosData);
        }
    }

}
