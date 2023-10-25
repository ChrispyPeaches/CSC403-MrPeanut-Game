using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project {
  public partial class FrmLevel : Form {
    private Player player;

    private Enemy enemyPoisonPacket;
    private Enemy bossKoolaid;
    private Enemy enemyCheeto;
    private Character[] walls;

    private Coin coin1;
    private Coin coin2;
    private Coin coin3;
    private Coin coin4;
    private Coin coin5;

    private int coinCounter;

    private DateTime timeBegin;
    private FrmBattle frmBattle;

    public FrmLevel() {
      InitializeComponent();
    }

    private void FrmLevel_Load(object sender, EventArgs e) {
      const int PADDING = 7;
      const int NUM_WALLS = 13;

      player = new Player(CreatePosition(picPlayer), CreateCollider(picPlayer, PADDING));
      bossKoolaid = new Enemy(CreatePosition(picBossKoolAid), CreateCollider(picBossKoolAid, PADDING));
      enemyPoisonPacket = new Enemy(CreatePosition(picEnemyPoisonPacket), CreateCollider(picEnemyPoisonPacket, PADDING));
      enemyCheeto = new Enemy(CreatePosition(picEnemyCheeto), CreateCollider(picEnemyCheeto, PADDING));

      coin1 = new Coin(CreatePosition(picCoin1), CreateCollider(picCoin1, PADDING));
      coin2 = new Coin(CreatePosition(picCoin2), CreateCollider(picCoin2, PADDING));
      coin3 = new Coin(CreatePosition(picCoin3), CreateCollider(picCoin3, PADDING));
      coin4 = new Coin(CreatePosition(picCoin4), CreateCollider(picCoin4, PADDING));
      coin5 = new Coin(CreatePosition(picCoin5), CreateCollider(picCoin5, PADDING));

      bossKoolaid.Img = picBossKoolAid.BackgroundImage;
      enemyPoisonPacket.Img = picEnemyPoisonPacket.BackgroundImage;
      enemyCheeto.Img = picEnemyCheeto.BackgroundImage;

      coin1.Img = picCoin1.BackgroundImage;
      coin2.Img = picCoin2.BackgroundImage;
      coin3.Img = picCoin3.BackgroundImage;
      coin4.Img = picCoin4.BackgroundImage;
      coin5.Img = picCoin5.BackgroundImage;

      bossKoolaid.Color = Color.Red;
      enemyPoisonPacket.Color = Color.Green;
      enemyCheeto.Color = Color.FromArgb(255, 245, 161);

      walls = new Character[NUM_WALLS];
      for (int w = 0; w < NUM_WALLS; w++) {
        PictureBox pic = Controls.Find("picWall" + w.ToString(), true)[0] as PictureBox;
        walls[w] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
      }

      Game.player = player;
      timeBegin = DateTime.Now;
    }

    private Vector2 CreatePosition(PictureBox pic) {
      return new Vector2(pic.Location.X, pic.Location.Y);
    }

    private Collider CreateCollider(PictureBox pic, int padding) {
      Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
      return new Collider(rect);
    }

    private void FrmLevel_KeyUp(object sender, KeyEventArgs e) {
      player.ResetMoveSpeed();
    }

    private void tmrUpdateInGameTime_Tick(object sender, EventArgs e) {
      TimeSpan span = DateTime.Now - timeBegin;
      string time = span.ToString(@"hh\:mm\:ss");
      lblInGameTime.Text = "Time: " + time.ToString();
    }

    private void tmrPlayerMove_Tick(object sender, EventArgs e) {
      // move player
      player.Move();

      // check collision with walls
      if (HitAWall(player)) {
        player.MoveBack();
      }

      // check collision with enemies
      if (HitAChar(player, enemyPoisonPacket)) {
        Fight(enemyPoisonPacket);
      }
      else if (HitAChar(player, enemyCheeto)) {
        Fight(enemyCheeto);
      }
      if (HitAChar(player, bossKoolaid)) {
        Fight(bossKoolaid);
      }

      if (HitACoin(player, coin1)) {
        picCoin1.BackgroundImage = Resources.transparent;
        coin1.Collider = null;
        coinCounter++;
        lblCoins.Text = "Coins: " + coinCounter.ToString();
      }
      if (HitACoin(player, coin2)) {
        picCoin2.BackgroundImage = Resources.transparent;
        coin2.Collider = null;
        coinCounter++;
        lblCoins.Text = "Coins: " + coinCounter.ToString();
      }
      if (HitACoin(player, coin3)) {
        picCoin3.BackgroundImage = Resources.transparent;
        coin3.Collider = null;
        coinCounter++;
        lblCoins.Text = "Coins: " + coinCounter.ToString();
      }
      if (HitACoin(player, coin4)) {
        picCoin4.BackgroundImage = Resources.transparent;
        coin4.Collider = null;
        coinCounter++;
        lblCoins.Text = "Coins: " + coinCounter.ToString();
      }
      if (HitACoin(player, coin5)) {
        picCoin5.BackgroundImage = Resources.transparent;
        coin5.Collider = null;
        coinCounter++;
        lblCoins.Text = "Coins: " + coinCounter.ToString();
      }

     // update player's picture box
     picPlayer.Location = new Point((int)player.Position.x, (int)player.Position.y);
    }

    private bool HitAWall(Character c) {
      bool hitAWall = false;
      for (int w = 0; w < walls.Length; w++) {
        if (c.Collider.Intersects(walls[w].Collider)) {
          hitAWall = true;
          break;
        }
      }
      return hitAWall;
    }

    private bool HitAChar(Character you, Character other) {
      return you.Collider.Intersects(other.Collider);
    }

    private bool HitACoin(Character character, Coin coin)
        {
            return character.Collider.Intersects(coin.Collider);
        }

    private void Fight(Enemy enemy) {
      player.ResetMoveSpeed();
      player.MoveBack();
      frmBattle = FrmBattle.GetInstance(enemy);
      frmBattle.Show();

      if (enemy == bossKoolaid) {
        frmBattle.SetupForBossBattle();
      }
    }

    private void FrmLevel_KeyDown(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
        case Keys.Left:
          player.GoLeft();
          break;

        case Keys.Right:
          player.GoRight();
          break;

        case Keys.Up:
          player.GoUp();
          break;

        case Keys.Down:
          player.GoDown();
          break;

        default:
          player.ResetMoveSpeed();
          break;
      }
    }

    private void lblInGameTime_Click(object sender, EventArgs e) {

    }
    }
}
