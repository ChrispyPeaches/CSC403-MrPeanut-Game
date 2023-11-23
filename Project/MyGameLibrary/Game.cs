using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Fall2020_CSC403_Project.code
{
    public class Game
    {
        public class DungeonRoom : code.DungeonRoom
        {
            public bool northDoor { get; set; }
            public bool eastDoor { get; set; }
            public bool southDoor { get; set; }
            public bool westDoor { get; set; }
            public List<IDungeonEnemyData> Enemies { get; set; }
            public List<IDungeonCoin> Coins { get; set; }
            public bool visited { get; set; }
            public IDungeonPositionData TopLeft { get; set; }
            public IDungeonPositionData TopRight { get; set; }
            public IDungeonPositionData BottomLeft { get; set; }
            public IDungeonPositionData BottomRight { get; set; }
        }

        public class DungeonCoin : IDungeonCoin
        {
            public float Amount { get; set; }
            public string Image { get; set; }
            public Guid ID { get; set; }
            public IDungeonPositionData Position { get; set; }
        }

        public class DungeonEnemyData : IDungeonEnemyData
        {
            public string displayName { get; set; }
            public bool defeated { get; set; }
            public int MaxHealth { get; set; }
            public float strength { get; set; }
            public int Health { get; set; }
            public string image { get; set; }
            public Guid ID { get; set; }
            public List<IEnemyDialogue> chatHistory { get; set; }
            public IDungeonPositionData Position { get; set; }
        }

        public class DungeonPositionData : IDungeonPositionData
        {
            public float x { get; set; }
            public float y { get; set; }
        }

        public class EnemyDialogue : IEnemyDialogue
        {
            public string UserName { get; set; }
            public string Text { get; set; }
        }


        public static Game instance;
        public Player player { get; set; }
        public Enemy bossKoolaid { get; set; }
        public Enemy enemyPoisonPacket { get; set; }
        public Enemy enemyCheeto { get; set; }
        public bool IsKoolAidDefeated { get; set; }
        public bool IsPoisonPacketDefeated { get; set; }
        public bool IsCheetosDefeated { get; set; }
        public DungeonRoom[,] Dungeon { get; set; }
        public int row { get; set; } = 0;
        public int column { get; set; } = 0;

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

        public int getRowForLevel()
        {
            return this.row;
        }

        public int getColForLevel()
        {
            return this.column;
        }

        public class EnemyDialogueConverter : JsonConverter<IEnemyDialogue>
        {
            public override IEnemyDialogue ReadJson(JsonReader reader, Type objectType, IEnemyDialogue existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);
                return jsonObject.ToObject<EnemyDialogue>();
            }

            public override void WriteJson(JsonWriter writer, IEnemyDialogue value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        public void InitializeGameEntities(Dictionary<string, object> save)
        {
            JObject playerData = (JObject)save["playerData"];
            player = new Player(new Vector2(0, 0), null, playerData);

            JObject dungeonData = (JObject)save["dungeon"];
            int dungeonWidth = Convert.ToInt32(save["width"]);
            int dungeonHeight = Convert.ToInt32(save["height"]);

            this.row = Convert.ToInt32(save["row"]);
            this.column = Convert.ToInt32(save["column"]);

            Dungeon = new DungeonRoom[dungeonWidth, dungeonHeight];

            for (int j = 0; j < dungeonHeight; j++)
            {
                for (int i = 0; i < dungeonWidth; i++)
                {
                    JObject roomData = (JObject)dungeonData["DungeonRooms"][j][i];

                    List<IDungeonEnemyData> enemies = new List<IDungeonEnemyData>();
                    JArray enemiesArray = (JArray)roomData["Enemies"];
                    foreach (JObject enemyData in enemiesArray)
                    {
                        DungeonEnemyData enemy = new DungeonEnemyData
                        {
                            displayName = enemyData["displayName"].Value<string>(),
                            defeated = enemyData["defeated"].Value<bool>(),
                            MaxHealth = enemyData["MaxHealth"].Value<int>(),
                            strength = enemyData["strength"].Value<float>(),
                            Health = enemyData["Health"].Value<int>(),
                            image = enemyData["image"].Value<string>(),
                            ID = Guid.Parse(enemyData["ID"].Value<string>()),
                            Position = enemyData["Position"].ToObject<DungeonPositionData>(),
                        };

                        JArray chatHistoryArray = (JArray)enemyData["chatHistory"];
                        List<IEnemyDialogue> chatHistoryList = chatHistoryArray.ToObject<List<IEnemyDialogue>>(new JsonSerializer
                        {
                            Converters = { new EnemyDialogueConverter() }
                        });
                        enemy.chatHistory = chatHistoryList;

                        enemies.Add(enemy);
                    }

                    List<IDungeonCoin> coins = new List<IDungeonCoin>();
                    JArray coinsArray = (JArray)roomData["Coins"];
                    foreach (JObject coinData in coinsArray)
                    {
                        DungeonCoin coin = new DungeonCoin
                        {
                            Amount = coinData["Amount"].Value<float>(),
                            Image = coinData["Image"].Value<string>(),
                            ID = Guid.Parse(coinData["ID"].Value<string>()),
                            Position = coinData["Position"].ToObject<DungeonPositionData>()
                        };
                        coins.Add(coin);
                    }

                    Dictionary<string, object> roomInfo = new Dictionary<string, object>
                    {
                        { "Room at", $"({i}, {j})" },
                        { "North Door", roomData["northDoor"].Value<bool>() },
                        { "East Door", roomData["eastDoor"].Value<bool>() },
                        { "South Door", roomData["southDoor"].Value<bool>() },
                        { "West Door", roomData["westDoor"].Value<bool>() },
                        { "Enemies", enemies },
                        { "Coins", coins },
                        { "Visited", roomData["visited"].Value<bool>() },
                        { "TopLeft", roomData["TopLeft"].ToObject<DungeonPositionData>() },
                        { "TopRight", roomData["TopRight"].ToObject<DungeonPositionData>() },
                        { "BottomLeft", roomData["BottomLeft"].ToObject<DungeonPositionData>() },
                        { "BottomRight", roomData["BottomRight"].ToObject<DungeonPositionData>() }
                    };

                    Dungeon[i, j] = new DungeonRoom
                    {
                        northDoor = (bool)roomInfo["North Door"],
                        eastDoor = (bool)roomInfo["East Door"],
                        southDoor = (bool)roomInfo["South Door"],
                        westDoor = (bool)roomInfo["West Door"],
                        Enemies = (List<IDungeonEnemyData>)roomInfo["Enemies"],
                        Coins = (List<IDungeonCoin>)roomInfo["Coins"],
                        visited = (bool)roomInfo["Visited"],
                        TopLeft = (DungeonPositionData)roomInfo["TopLeft"],
                        TopRight = (DungeonPositionData)roomInfo["TopRight"],
                        BottomLeft = (DungeonPositionData)roomInfo["BottomLeft"],
                        BottomRight = (DungeonPositionData)roomInfo["BottomRight"]
                    };
                }
            }
        }
    }
}
