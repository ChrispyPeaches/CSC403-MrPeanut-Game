using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class FrmBattle : Form
    {
        public static FrmBattle instance = null;
        private Enemy enemy = null;
        private Player player = null;
        public string enemyName = "";

        private FrmBattle()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        public void Setup()
        {
            Game game = Game.Instance;
            Player player = game.player;

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
        }

        public void SetupForBossBattle()
        {
            picBossBattle.Location = Point.Empty;
            picBossBattle.Size = ClientSize;
            picBossBattle.Visible = true;

            SoundPlayer simpleSound = new SoundPlayer(Resources.final_battle);
            simpleSound.Play();

            tmrFinalBattle.Enabled = true;
        }

        public static FrmBattle GetInstance(Enemy enemy)
        {
            Boolean check = CheckFlag(enemy);
            if (instance == null && !check)
            {
                instance = new FrmBattle();
                instance.enemy = enemy;
                instance.enemyName = enemy.Name;
                instance.Setup();
            }
            return instance;
        }

        public static Boolean CheckFlag(Enemy enemy)
        {
            string enemyName = enemy.Name;
            if (enemyName.Contains("enemy_cheetos"))
            {
                return Game.Instance.IsCheetosDefeated;
            }
            else if (enemyName.Contains("enemy_koolaid"))
            {
                return Game.Instance.IsKoolAidDefeated;
            }
            else if (enemyName.Contains("enemy_poisonpacket"))
            {
                return Game.Instance.IsPoisonPacketDefeated;
            }
            else
            {
                return true;
            }
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
            FrmBattle battleForm = GetInstance(enemy);
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
                    if (this.enemyName.Contains("enemy_cheetos"))
                    {
                        Game.Instance.player.MaxHealth += 20;
                        Game.Instance.player.Health = Game.Instance.player.MaxHealth;
                        Game.Instance.player.strength += 2;
                        game.IsCheetosDefeated = true;
                    }
                    else if (this.enemyName.Contains("enemy_koolaid"))
                    {
                        game.IsKoolAidDefeated = true;
                    }
                    else if (this.enemyName.Contains("enemy_poisonpacket"))
                    {
                        Game.Instance.player.MaxHealth += 20;
                        Game.Instance.player.Health = Game.Instance.player.MaxHealth;
                        Game.Instance.player.strength += 2;
                        game.IsPoisonPacketDefeated = true;
                    }
                    SendKeys.SendWait("{ESC}");
                }
            }
            else
            {
                SendKeys.SendWait("{ESC}");
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

        }

        // may need parity here
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
                instance = null;
                Close();
            }
        }

    }
}