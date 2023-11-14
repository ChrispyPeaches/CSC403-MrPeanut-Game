using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.OpenAIApi;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Fall2020_CSC403_Project
{
    public partial class FrmBattle : Form
    {
        public static FrmBattle instance = null;
        public static FrmBattle instanceForDeath { get; private set; }
        private Enemy enemy = null;
        private Player player = null;
        public Image playerImg = null;
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

        private FrmBattle(IOpenAIApi openAIApi, Image playerImage)
        {
            InitializeComponent();
            KeyPreview = true;
            _openAIApi = openAIApi;
            instanceForDeath = this;
            playerImg = playerImage;
        }

        public void Setup()
        {
            Game game = Game.Instance;
            Player player = game.player;

            // set the correct player image
            picPlayer.BackgroundImage = playerImg;

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

            // Setup OpenAI
            chats = new List<ChatMessage>()
            {
                new ChatMessage()
                {
                    Role = RoleType.System,
                    Content = $"We are in a battle to the death." +
                                $"You are playing the role of {enemy.displayName}. I am playing the role of {Game.Instance.player.Name}." +
                                $"We will each send one message at a time to create a dialogue. "
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

        public static FrmBattle GetInstance(Enemy enemy, IOpenAIApi openAIApi, Image playerImage)
        {
            Boolean check = CheckFlag(enemy);
            if (instance == null && !check)
            {
                instance = new FrmBattle(openAIApi, playerImage);
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
            FrmBattle battleForm = GetInstance(enemy, this._openAIApi, playerImg);
            if (game.player.Health > 0)
            {
                // update hp
                battleForm.PlayerDamage(Convert.ToInt32(enemy.strength));
                if (enemy.Health > 0)
                {
                    battleForm.EnemyDamage(Convert.ToInt32(game.player.strength));
                }
                battleForm.UpdateHealthBars();

                if (enemy.Health <= 0)
                {
                    Game.Instance.player.MaxHealth += 20;
                    Game.Instance.player.Health = Game.Instance.player.MaxHealth;
                    Game.Instance.player.strength += 2;
                    enemy.Defeated = true;
                    enemy.Collider = null;
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
            if (!checkedMessages)
            {
                List<string> chatLines = enemy.chatHistory
                    .Select(dialogue => $"{dialogue.UserName}: {dialogue.Text}")
                    .ToList();

                textboxChatHistory.Lines = chatLines.ToArray();
                checkedMessages = true;
            }

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

            // Send to OpenAI
            ChatCompletionResponse response = await _openAIApi
                .GetChatCompletion(new ChatCompletionQuery()
                {
                    Messages = chats
                });

            // Display enemy's response in chat history
            string enemyResponse = response.Choices.First().Message.Content;
            textboxChatHistory.AppendText($"\n{enemy.displayName}: {enemyResponse}");

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
}