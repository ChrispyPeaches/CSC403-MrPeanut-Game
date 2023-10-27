using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.OpenAIApi;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class FrmBattle : Form
    {
        public static FrmBattle instance = null;
        private Enemy enemy;
        private Player player;
        private IOpenAIApi _openAIApi;
        private IList<ChatCompletionQuery.ChatMessage> chats;

        SoundPlayer bossMusic = new SoundPlayer(Resources.boss_music);
        SoundPlayer finalBattleClip = new SoundPlayer(Resources.final_battle);
        SoundPlayer overworldTheme = new SoundPlayer(Resources.overworld_theme);
        SoundPlayer battleMusic = new SoundPlayer(Resources.battle_music);

        private FrmBattle(IOpenAIApi openAIApi)
        {
            InitializeComponent();
            player = Game.player;
            _openAIApi = openAIApi;
        }

        public void Setup()
        {
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
            chats = new List<ChatCompletionQuery.ChatMessage>()
            {
                new ChatCompletionQuery.ChatMessage()
                {
                    Role = ChatCompletionQuery.RoleType.System,
                    Content = $"We are in a battle to the death." +
                                $"You are playing the role of {enemy.Name}. I am playing the role of {player.Name}." +
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

        public static FrmBattle GetInstance(Enemy enemy, IOpenAIApi openAIApi)
        {
            if (instance == null)
            {
                instance = new FrmBattle(openAIApi);
                instance.enemy = enemy;
                instance.Setup();
            }
            return instance;
        }

        private void UpdateHealthBars()
        {
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
            player.OnAttack(-4);
            if (enemy.Health > 0)
            {
                enemy.OnAttack(-2);
            }

            UpdateHealthBars();
            if (player.Health <= 0 || enemy.Health <= 0)
            {
                overworldTheme.PlayLooping();
                instance = null;
                Close();
            }
        }

        private void EnemyDamage(int amount)
        {
            enemy.AlterHealth(amount);
        }

        private void PlayerDamage(int amount)
        {
            player.AlterHealth(amount);
        }

        private void tmrFinalBattle_Tick(object sender, EventArgs e)
        {
            picBossBattle.Visible = false;
            tmrFinalBattle.Enabled = false;
            bossMusic.PlayLooping();
        }

        private async void btnChat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textboxChatInput.Text))
            {
                return;
            }

            // Disable chat button while retrieving message
            btnChat.Enabled = false;

            // Display chat in chat history
            List<string> chatHistory = textboxChatHistory.Lines.ToList();
            chatHistory.Add($"\n{player.Name}:");
            chatHistory.AddRange(textboxChatInput.Lines);
            textboxChatHistory.Lines = chatHistory.ToArray();
            textboxChatInput.Text = String.Empty;

            // Format message for OpenAI
            string message = "";
            message = chatHistory.Aggregate(
                (combinedString, currentString) => 
                    combinedString = $"{combinedString}\n{currentString}");

            chats.Add(new ChatCompletionQuery.ChatMessage()
            {
                Role = ChatCompletionQuery.RoleType.User,
                Content = message
            });

            // Send to OpenAI
            ChatCompletionResponse response = await _openAIApi.GetChatCompletion(new ChatCompletionQuery()
            {
                Messages = chats
            });

            // Display enemy's response in chat history
            chats.Add(new ChatCompletionQuery.ChatMessage()
            {
                Role = ChatCompletionQuery.RoleType.Assistant,
                Content = response.Choices.First().Message.Content
            });

            // Display enemy name and message content
            chatHistory.Add($"\n{enemy.Name}:");
            chatHistory.Add(chats.Last().Content
                .Substring(chats.Last().Content.IndexOf(':') + 1)
                .TrimStart('\n'));
            textboxChatHistory.Lines = chatHistory.ToArray();

            // Enable chat button
            btnChat.Enabled = true;
        }

        private void btnFlee_Click(object sender, EventArgs e)
        {
            overworldTheme.PlayLooping();
            instance = null;
            Close();
        }
    }
}
