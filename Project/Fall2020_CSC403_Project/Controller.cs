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
        public Dungeon dungeon { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public PlayerData playerData { get; set; }
    }

    public class PlayerData
    {
        public string name { get; set; }
        public int MaxHealth { get; set; }
        public float strength { get; set; }
        public int Health { get; set; }
        public PositionData Position { get; set; }
        public int coinCounter { get; set; }
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
                        // get dungeon size
                        Random random = new Random();
                        int dungeonSize = random.Next(3, 5);

                        Dungeon dungeon = new Dungeon(dungeonSize);

                        int selectedRow = random.Next(0, dungeonSize);
                        int selectedCol = random.Next(0, dungeonSize);

                        SaveData defaultSave = new SaveData
                        {
                            dungeon = dungeon,
                            width = dungeonSize,
                            height = dungeonSize,
                            //row = selectedRow,
                            //column = selectedCol,
                            row = 0,
                            column = 0,
                            playerData = new PlayerData
                            {
                                name = pathToFile,
                                MaxHealth = 100,
                                strength = 5,
                                Health = 100,
                                Position = new PositionData
                                {
                                    x = 200,
                                    y = 525
                                },
                                coinCounter = 0,
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
                            },
                            coinCounter = player.coinCounter,
                        },
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