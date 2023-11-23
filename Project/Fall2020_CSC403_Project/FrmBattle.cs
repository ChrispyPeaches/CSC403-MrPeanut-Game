using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.OpenAIApi;
using Fall2020_CSC403_Project.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class FrmBattle : Form
    {
        public static FrmBattle instance = null;
        public static FrmBattle instanceForDeath { get; private set; }
        private Enemy enemy = null;
        private Player player = null;
        public string enemyName = "";
        private IOpenAIApi _openAIApi;
        private IList<ChatMessage> chats;
        private bool checkedMessages = false;

        public static DeathScreen deathScreen = null;

        SoundPlayer bossMusic = new SoundPlayer(Resources.boss_music);
        SoundPlayer finalBattleClip = new SoundPlayer(Resources.final_battle);
        SoundPlayer overworldTheme = new SoundPlayer(Resources.overworld_theme);
        SoundPlayer battleMusic = new SoundPlayer(Resources.battle_music);
        SoundPlayer gameOverTheme = new SoundPlayer(Resources.game_over_theme);

        private FrmBattle(IOpenAIApi openAIApi)
        {
            InitializeComponent();
            KeyPreview = true;
            _openAIApi = openAIApi;
            instanceForDeath = this;
        }

        public void Setup()
        {
            Game game = Game.Instance;
            Player player = game.player;

            // set the player image
            SetPlayerImage();

            // play battle music
            battleMusic.PlayLooping();

            // update for this enemy
            picEnemy.BackgroundImage = enemy.Img;
            picEnemy.Refresh();
            BackColor = enemy.Color;
            picBossBattle.Visible = false;

            // Observer pattern
            enemy.AttackEvent += PlayerDamage;
            player.AttackEvent += EnemyDamage;

            // show health
            UpdateHealthBars();

            if (!checkedMessages)
            {
                List<string> chatLines = enemy.chatHistory
                    .Select(dialogue => $"{dialogue.Text}")
                    .ToList();

                textboxChatHistory.Lines = chatLines.ToArray();
                checkedMessages = true;
            }

            // Setup OpenAI
            chats = new List<ChatMessage>()
            {
                new ChatMessage()
                {
                    Role = RoleType.System,
                    Content = $"We are in a battle to the death." +
                                $"You are playing the role of {enemy.displayName}. I am playing the role of {Game.Instance.player.Name}." +
                                $"We will each send one message at a time to create a dialogue. You will only send messages as the role of {enemy.displayName}. If attempts at peace are made, you are open to negotiation."
                }
            };
        }

        public void SetupForBossBattle()
        {
            picBossBattle.Location = Point.Empty;
            picBossBattle.Size = ClientSize;
            picBossBattle.Visible = true;
            finalBattleClip.Play();
            tmrFinalBattle.Enabled = true;
        }

        public static FrmBattle GetInstance(Enemy enemy, IOpenAIApi openAIApi)
        {
            Boolean check = CheckFlag(enemy);
            if (instance == null && !check)
            {
                instance = new FrmBattle(openAIApi);
                instance.enemy = enemy;
                instance.enemyName = enemy.Name;
                instance.Setup();
            }

            return instance;
        }

        public static Boolean CheckFlag(Enemy enemy)
        {
            return enemy.Defeated;
        }

        public void SetPlayerHealth(int health)
        {
            player.Health = health;
        }

        public void SetEnemyHealth(int health)
        {
            enemy.Health = health;
        }

        private void UpdateHealthBars()
        {
            Game game = Game.Instance;
            Player player = game.player;
            float playerHealthPer = player.Health / (float)player.MaxHealth;
            float enemyHealthPer = enemy.Health / (float)enemy.MaxHealth;

            const int MAX_HEALTHBAR_WIDTH = 226;
            lblPlayerHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * playerHealthPer);
            lblEnemyHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * enemyHealthPer);

            lblPlayerHealthFull.Text = player.Health.ToString();
            lblEnemyHealthFull.Text = enemy.Health.ToString();
        }

        private void btnAttack_Click(object sender, EventArgs e)
        {
            Game game = Game.Instance;
            FrmBattle battleForm = GetInstance(enemy, this._openAIApi);
            if (game.player.Health > 0)
            {
                // update hp
                battleForm.PlayerDamage(Convert.ToInt32(enemy.strength));
                if (enemy.Health > 0)
                {
                    battleForm.EnemyDamage(Convert.ToInt32(game.player.strength));
                }
                battleForm.UpdateHealthBars();

                // Defeat enemy upon health hitting 0
                if (enemy.Health <= 0)
                {
                    Game.Instance.player.MaxHealth += 20;
                    Game.Instance.player.Health = Game.Instance.player.MaxHealth;
                    Game.Instance.player.strength += 2;
                    enemy.Defeated = true;
                    enemy.Collider = null;
                    FrmLevel.instance.DefeatEnemy(enemy.ID.ToString());
                    SendKeys.SendWait("{ESC}");
                }
                if (game.player.Health <= 0)
                {
                    FrmLevel frmLevel = FrmLevel.instanceForDeath;
                    frmLevel.Opacity = .01;
                    this.Opacity = 0;
                    gameOverTheme.Play();
                    deathScreen = new DeathScreen();
                    deathScreen.ShowDialog();
                }
            }
        }

        private void PlayerDamage(int amount)
        {
            Game game = Game.Instance;
            Player player = game.player;
            player.AlterHealth(amount);
        }

        private void EnemyDamage(int amount)
        {
            this.enemy.AlterHealth(amount);
        }

        private void tmrFinalBattle_Tick(object sender, EventArgs e)
        {
            picBossBattle.Visible = false;
            tmrFinalBattle.Enabled = false;
            bossMusic.PlayLooping();
        }

        private void btnFlee_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{ESC}");
        }

        private async void FrmBattle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Game game = Game.Instance;
                Player player = game.player;
                Vector2 Position = player.Position;

                if (Position.y < 350)
                {
                    player.GoDown();
                }
                else
                {
                    player.GoUp();
                }
                await Task.Delay(50);
                player.ResetMoveSpeed();
                // battle instance
                overworldTheme.PlayLooping();
                instance = null;
                Close();
            }
        }

        /// <summary>
        /// When the chat button is clicked:
        /// <list type="number">
        ///     <item>Display the user's message</item>
        ///     <item>Log user prompt into conversation history</item>
        ///     <item>Get response from OpenAI</item>
        ///     <item>Log response into conversation history</item>
        /// </list>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnChat_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textboxChatInput.Text))
            {
                return;
            }

            // Disable chat button while retrieving message
            btnChat.Enabled = false;

            // initialize game instance
            Game game = Game.Instance;
            Player player = game.player;

            // Display user message in chat history
            string userMessage = $"{player.Name}: {textboxChatInput.Text}";
            textboxChatHistory.AppendText($"\n{userMessage}");
            textboxChatInput.Text = string.Empty;

            // Format message for OpenAI
            chats.Add(new ChatMessage()
            {
                Role = RoleType.User,
                Content = userMessage
            });

            enemy.chatHistory.Add(new EnemyDialogue()
            {
                UserName = player.Name,
                Text = userMessage,
            });

            // Create completion query
            ChatCompletionQuery query = new ChatCompletionQuery()
            {
                Messages = chats,
                Tools = new List<ChatCompletionQuery.Tool>
                {
                    new ChatCompletionQuery.Tool
                    {
                        Type = "function",
                        Function = new ChatCompletionQuery.Tool.FunctionModel
                        {
                            Name = "make_peace",
                            Description = $"Execute this function if {player.Name} expresses satisfaction after a new truce",
                            Parameter = new ChatCompletionQuery.Tool.FunctionModel.ParameterModel
                            {
                                Type = "object",
                                Properties = new ChatCompletionQuery.Tool.FunctionModel.ParameterModel.Property
                                {
                                    Response = new ChatCompletionQuery.Tool.FunctionModel.ParameterModel.Property.PropertyStuff
                                    {
                                        Type = "string",
                                        Description = $"Final speech from you about the new truce made between fighters."
                                    }
                                },
                                Required = new List<string> {"response"}
                            }
                        }
                    }
                }
            };

            // Send to OpenAI
            ChatCompletionResponse response = await _openAIApi
                .GetChatCompletion(query);

            // Check if AI wants to make peace
            if (response.Choices.First().Message.ToolChoice != null)
            {
                // Grab response from tool response
                string enemyResponse = response.Choices.First().Message.ToolChoice.First().Function.Argument;

                // Deserialize JSON
                JObject jsonResponse = JObject.Parse(enemyResponse);

                // Extract the "response" property
                enemyResponse = $"\n{enemy.displayName}: " + jsonResponse["response"].ToString() + "\n";

                makePeace();

                // Display enemy's response in chat history
                chats.Add(new ChatMessage()
                {
                    Role = RoleType.Assistant,
                    Content = enemyResponse
                });

                // Display enemy message in chat history and add peace message
                textboxChatHistory.AppendText(enemyResponse);
                textboxChatHistory.AppendText("\nPEACE ESTABLISHED. YOU MAY LEAVE AT ANYTIME");

                // Change player stats
                Game.Instance.player.MaxHealth += 20;
                Game.Instance.player.Health = Game.Instance.player.MaxHealth;
                Game.Instance.player.strength += 2;
                enemy.Defeated = true;
                enemy.Collider = null;

                // Disable attack button and change leave text
                btnAttack.Enabled = false;
                btnFlee.Text = "Leave";
            }
            else // If battle continues
            {
                // Remove parts where AI creates user text
                if (response.Choices.First().Message.Content.Contains("Goliath:"))
                {
                    response.Choices.First().Message.Content = response.Choices.First().Message.Content
                        .Remove(response.Choices.First().Message.Content.IndexOf("Goliath:") - 1);
                }

                string enemyResponse = response.Choices.First().Message.Content;
                textboxChatHistory.AppendText($"\n{enemyResponse}");
                chats.Add(new ChatMessage()
                {
                    Role = RoleType.Assistant,
                    Content = enemyResponse
                });

                enemy.chatHistory.Add(new EnemyDialogue()
                {
                    UserName = enemy.displayName,
                    Text = enemyResponse,
                });

                // Enable chat button
                btnChat.Enabled = true;
            }
        }

        public void SetPlayerImage()
        {
            string playerImagePath = Path.Combine(Settings.Default.AppDataDirectory, "playerImage.png");

            if (File.Exists(playerImagePath))
            {
                picPlayer.BackgroundImage = Image.FromFile(playerImagePath);
            }
            else
            {
                picPlayer.BackgroundImage = Resources.mrPeanut;
            }
        }

        // "Defeat enemy" when peace is made
        public void makePeace()
        {
            this.enemy.isPeaceful = true;
            FrmLevel.instance.DefeatEnemy(enemy.ID.ToString());
        }
    }
}