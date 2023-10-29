using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.OpenAIApi;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Media;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Fall2020_CSC403_Project.OpenAIApi.ChatCompletionQuery;
using Fall2020_CSC403_Project.Properties;
using System.Numerics;
using Vector2 = Fall2020_CSC403_Project.code.Vector2;

namespace Fall2020_CSC403_Project
{
    public partial class FrmLevel : Form
    {
        private Character[] walls;

        private Coin coin1;
        private Coin coin2;
        private Coin coin3;
        private Coin coin4;
        private Coin coin5;

        private DateTime timeBegin;
        private FrmBattle frmBattle;

        public static FrmLevel instanceForDeath { get; private set; }

        public IOpenAIApi _openAIApi;

        private bool isUpPressed = false;
        private bool isDownPressed = false;
        private bool isLeftPressed = false;
        private bool isRightPressed = false;

        public FrmLevel(IOpenAIApi openAIApi)
        {
            InitializeComponent();
            _openAIApi = openAIApi;
            instanceForDeath = this;
        }

        public void ResetMovementBooleans()
        {
            isUpPressed = false;
            isDownPressed = false;
            isRightPressed = false;
            isLeftPressed = false;
        }

        private void FrmLevel_Load(object sender, EventArgs e)
        {
            const int PADDING = 7;
            const int NUM_WALLS = 13;

            Game game = Game.Instance;
            SoundPlayer overworldTheme = new SoundPlayer(Resources.overworld_theme);
            overworldTheme.PlayLooping();

            coin1 = new Coin(CreatePosition(picCoin1), CreateCollider(picCoin1, PADDING));
            coin2 = new Coin(CreatePosition(picCoin2), CreateCollider(picCoin2, PADDING));
            coin3 = new Coin(CreatePosition(picCoin3), CreateCollider(picCoin3, PADDING));
            coin4 = new Coin(CreatePosition(picCoin4), CreateCollider(picCoin4, PADDING));
            coin5 = new Coin(CreatePosition(picCoin5), CreateCollider(picCoin5, PADDING));

            coin1.Img = picCoin1.BackgroundImage;
            coin2.Img = picCoin2.BackgroundImage;
            coin3.Img = picCoin3.BackgroundImage;
            coin4.Img = picCoin4.BackgroundImage;
            coin5.Img = picCoin5.BackgroundImage;

            if (game == null)
            {
                System.Environment.Exit(0);
            }

            lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();

            game.player.Position = CreatePosition(picPlayer);
            game.player.Collider = CreateCollider(picPlayer, PADDING);

            game.bossKoolaid.Position = CreatePosition(picBossKoolAid);
            game.bossKoolaid.Collider = CreateCollider(picBossKoolAid, PADDING);

            game.enemyPoisonPacket.Position = CreatePosition(picEnemyPoisonPacket);
            game.enemyPoisonPacket.Collider = CreateCollider(picEnemyPoisonPacket, PADDING);

            game.enemyCheeto.Position = CreatePosition(picEnemyCheeto);
            game.enemyCheeto.Collider = CreateCollider(picEnemyCheeto, PADDING);

            game.bossKoolaid.Img = picBossKoolAid.Image;
            game.enemyPoisonPacket.Img = picEnemyPoisonPacket.BackgroundImage;
            game.enemyCheeto.Img = picEnemyCheeto.BackgroundImage;

            game.bossKoolaid.Color = Color.Red;
            game.enemyPoisonPacket.Color = Color.Green;
            game.enemyCheeto.Color = Color.FromArgb(255, 245, 161);

            walls = new Character[NUM_WALLS];
            for (int w = 0; w < NUM_WALLS; w++)
            {
                PictureBox pic = Controls.Find("picWall" + w.ToString(), true)[0] as PictureBox;
                walls[w] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
            }
            timeBegin = DateTime.Now;
        }

        private static Vector2 CreatePosition(PictureBox pic)
        {
            return new Vector2(pic.Location.X, pic.Location.Y);
        }

        private Collider CreateCollider(PictureBox pic, int padding)
        {
            Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
            return new Collider(rect);
        }

        private void tmrUpdateInGameTime_Tick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - timeBegin;
            string time = span.ToString(@"hh\:mm\:ss");
            lblInGameTime.Text = "Time: " + time.ToString();
        }

        private void tmrPlayerMove_Tick(object sender, EventArgs e)
        {
            Game game = Game.Instance;
            // move player
            game.player.Move();

            // check collision with walls
            if (HitAWall(game.player))
            {
                game.player.MoveBack();
            }

            // check collision with enemies
            if (HitAChar(game.player, game.enemyPoisonPacket))
            {
                if (game.player.Health > 0)
                {
                    Fight(game.enemyPoisonPacket);
                }
                else
                {
                    game.player.MoveBack();
                }
            }
            else if (HitAChar(game.player, game.enemyCheeto))
            {
                if(game.player.Health > 0)
                {
                    Fight(game.enemyCheeto);
                }
                else
                {
                    game.player.MoveBack();
                }
            }
            if (HitAChar(game.player, game.bossKoolaid))
            {
                if (game.player.Health > 0)
                {
                    Fight(game.bossKoolaid);
                }
                else
                {
                    game.player.MoveBack();
                }
            }

            if (HitACoin(game.player, coin1))
            {
                picCoin1.BackgroundImage = Resources.transparent;
                coin1.Collider = null;
                Game.Instance.player.coinCounter++;
                lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();
            }
            if (HitACoin(game.player, coin2))
            {
                picCoin2.BackgroundImage = Resources.transparent;
                coin2.Collider = null;
                Game.Instance.player.coinCounter++;
                lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();
            }
            if (HitACoin(game.player, coin3))
            {
                picCoin3.BackgroundImage = Resources.transparent;
                coin3.Collider = null;
                Game.Instance.player.coinCounter++;
                lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();
            }
            if (HitACoin(game.player, coin4))
            {
                picCoin4.BackgroundImage = Resources.transparent;
                coin4.Collider = null;
                Game.Instance.player.coinCounter++;
                lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();
            }
            if (HitACoin(game.player, coin5))
            {
                picCoin5.BackgroundImage = Resources.transparent;
                coin5.Collider = null;
                Game.Instance.player.coinCounter++;
                lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();
            }

            // update player's picture box
            picPlayer.Location = new Point((int)game.player.Position.x, (int)game.player.Position.y);
        }

        private bool HitAWall(Character c)
        {
            bool hitAWall = false;
            for (int w = 0; w < walls.Length; w++)
            {
                if (c.Collider.Intersects(walls[w].Collider))
                {
                    hitAWall = true;
                    break;
                }
            }
            return hitAWall;
        }

        private bool HitAChar(Character you, Character other)
        {
            return you.Collider.Intersects(other.Collider);
        }

        private bool HitACoin(Character character, Coin coin)
        {
            return character.Collider.Intersects(coin.Collider);
        }

        private void Fight(Enemy enemy)
        {
            Game game = Game.Instance;
            Player player = game.player;
            player.MoveBack();
            try
            {
                frmBattle = FrmBattle.GetInstance(enemy, _openAIApi);
                if (!(frmBattle == null))
                {
                    if (enemy == game.bossKoolaid)
                    {
                        frmBattle.SetupForBossBattle();
                    }
                    frmBattle.StartPosition = FormStartPosition.Manual;
                    frmBattle.Left = this.Left + (this.Width - frmBattle.Width) / 2;
                    frmBattle.Top = this.Top + (this.Height - frmBattle.Height) / 2;
                    game.player.ResetMoveSpeed();
                    isUpPressed = false;
                    isRightPressed = false;
                    isDownPressed = false;
                    isLeftPressed = false;
                    frmBattle.Show();
                }
                else{
                }
            }
            catch
            {
            }
        }

        // detect input
        private void FrmLevel_KeyDown(object sender, KeyEventArgs e)
        {
            Game.Instance.player.ResetMoveSpeed();
            if (e.KeyCode == Keys.Up)
            {
                isUpPressed = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                isDownPressed = true;
            }
            else if (e.KeyCode == Keys.Left)
            {
                isLeftPressed = true;
            }
            else if (e.KeyCode == Keys.Right)
            {
                isRightPressed = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Environment.Exit(0);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                QuickStartMenu quickStartMenu = new QuickStartMenu();
                quickStartMenu.StartPosition = FormStartPosition.Manual;
                quickStartMenu.Left = this.Left + (this.Width - quickStartMenu.Width) / 2;
                quickStartMenu.Top = this.Top + (this.Height - quickStartMenu.Height) / 2;
                quickStartMenu.Show();
            }

            HandleMovement(); // handle pressed input
        }

        // detect release of input
        private void FrmLevel_KeyUp(object sender, KeyEventArgs e)
        {
            Game.Instance.player.ResetMoveSpeed();
            if (e.KeyCode == Keys.Up)
            {
                isUpPressed = false;
            }
            else if (e.KeyCode == Keys.Down)
            {
                isDownPressed = false;
            }
            else if (e.KeyCode == Keys.Left)
            {
                isLeftPressed = false;
            }
            else if (e.KeyCode == Keys.Right)
            {
                isRightPressed = false;
            }

            HandleMovement(); // handle released input
        }

        // handle directional movement
        private void HandleMovement()
        {
            Game game = Game.Instance;
            Player player = game.player;
            Game.Instance.player.ResetMoveSpeed();
            if (isUpPressed && isRightPressed)
            {
                game.player.GoUpRight();
            }
            else if (isUpPressed && isLeftPressed)
            {
                game.player.GoUpLeft();
            }
            else if (isDownPressed && isRightPressed)
            {
                game.player.GoDownRight();
            }
            else if (isDownPressed && isLeftPressed)
            {
                game.player.GoDownLeft();
            }
            else if (isUpPressed)
            {
                game.player.GoUp();
            }
            else if (isDownPressed)
            {
                game.player.GoDown();
            }
            else if (isLeftPressed)
            {
                game.player.GoLeft();
            }
            else if (isRightPressed)
            {
                game.player.GoRight();
            }
            else
            {
                game.player.ResetMoveSpeed();
            }
        }
        private void lblInGameTime_Click(object sender, EventArgs e)
        {

        }

        private void picBossKoolAid_Click(object sender, EventArgs e)
        {

        }
    }
}
