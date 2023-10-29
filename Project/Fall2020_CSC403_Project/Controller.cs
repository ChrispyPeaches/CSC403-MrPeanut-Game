// imports
using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.OpenAIApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using static Fall2020_CSC403_Project.OpenAIApi.ChatCompletionQuery;
using Vector2 = Fall2020_CSC403_Project.code.Vector2;

namespace Fall2020_CSC403_Project
{
    // LOCAL models and other variables -- use
    // models to format any jsons from requests, etc
    public class SaveData
    {
        public PlayerData playerData { get; set; }
        public EnemyData enemy_koolaidData { get; set; }
        public EnemyData enemy_poisonpacketData { get; set; }
        public EnemyData enemy_cheetosData { get; set; }
    }

    public class PlayerData
    {
        public string name { get; set; }
        public int MaxHealth { get; set; }
        public float strength { get; set; }
        public int Health { get; set; }
        public PositionData Position { get; set; }
    }

    public class EnemyData
    {
        public string displayName { get; set; }
        public bool defeated { get; set; }
        public int MaxHealth { get; set; }
        public float strength { get; set; }
        public int Health { get; set; }

    }

    public class PositionData
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public partial class Controller
    {
        public class GameData : Controller
        {
            public void SaveData(string pathToFile = "Save Data Name Here")
            {
                try
                {
                    string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string savesDirectoryPath = Path.Combine(appDirectory, "..", "..", "Saves");
                    if (!Directory.Exists(savesDirectoryPath))
                    {
                        Directory.CreateDirectory(savesDirectoryPath);
                    }
                    string saveFilePath = Path.Combine(savesDirectoryPath, pathToFile + ".json");
                    if (!File.Exists(saveFilePath))
                    {
                        SaveData defaultSave = new SaveData
                        {
                            playerData = new PlayerData
                            {
                                name = pathToFile,
                                MaxHealth = 100,
                                strength = 5,
                                Health = 100,
                                Position = new PositionData
                                {
                                    x = 300,
                                    y = 600
                                }
                            },
                            enemy_koolaidData = new EnemyData
                            {
                                displayName = "Kool Aid Man",
                                defeated = false,
                                MaxHealth = 150,
                                strength = 15,
                                Health = 150,
                            },

                            enemy_poisonpacketData = new EnemyData
                            {
                                displayName = "Kool Aid Poison Packet",
                                defeated = false,
                                MaxHealth = 75,
                                strength = 10,
                                Health = 75,
                            },

                            enemy_cheetosData = new EnemyData
                            {
                                displayName = "Violent Cheeto",
                                defeated = false,
                                MaxHealth = 50,
                                strength = 8,
                                Health = 50,
                            }
                        };

                        string jsonSaveData = JsonConvert.SerializeObject(defaultSave);
                        File.WriteAllText(saveFilePath, jsonSaveData);

                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving JSON file: " + ex.Message);
                }
            }

            public void UpdateData(string pathToFile = "Save Data Name Here")
            {
                string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string saveDirectory = Path.Combine(appDirectory, "..", "..", "Saves", pathToFile + ".json");
                try
                {
                    Game game = Game.Instance;
                    Player player = game.player;
                    CharacterState characterState = BattleCharacter.GetCharacterState(player);
                    SaveData updatedSave = new SaveData
                    {
                        playerData = new PlayerData
                        {
                            name = player.Name,
                            MaxHealth = player.MaxHealth,
                            strength = player.strength,
                            Health = player.Health,
                            Position = new PositionData
                            {
                                x = player.Position.x,
                                y = player.Position.y
                            }
                        },

                        enemy_koolaidData = new EnemyData
                        {
                            displayName = game.bossKoolaid.displayName,
                            defeated = game.IsKoolAidDefeated,
                            MaxHealth = game.bossKoolaid.MaxHealth,
                            strength = game.bossKoolaid.strength,
                            Health = game.bossKoolaid.Health,
                        },

                        enemy_poisonpacketData = new EnemyData
                        {
                            displayName = game.enemyPoisonPacket.displayName,
                            defeated = game.IsPoisonPacketDefeated,
                            MaxHealth = game.enemyPoisonPacket.MaxHealth,
                            strength = game.enemyPoisonPacket.strength,
                            Health = game.enemyPoisonPacket.Health,
                        },

                        enemy_cheetosData = new EnemyData
                        {
                            displayName = game.enemyCheeto.displayName,
                            defeated = game.IsCheetosDefeated,
                            MaxHealth = game.enemyCheeto.MaxHealth,
                            strength = game.enemyCheeto.strength,
                            Health = game.enemyCheeto.Health,
                        }
                    };
                    string jsonSaveData = JsonConvert.SerializeObject(updatedSave);
                    File.WriteAllText(saveDirectory, jsonSaveData, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating JSON file: " + ex.Message);
                }
            }

            public static Dictionary<string, object> RetrieveData(string pathToFile = "Save Data Name Here")
            {
                try
                {
                    string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string saveDirectory = Path.Combine(appDirectory, "..", "..", "Saves", pathToFile + ".json");
                    string jsonString = File.ReadAllText(saveDirectory);
                    var savedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                    return savedData;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading JSON file: " + ex.Message);
                }

                return null;
            }

        }
    }
}