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
using System.Web.UI.WebControls;
using System.IO;
using System.Linq;

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

        private Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom;
        private List<List<System.Windows.Forms.Panel>> roomPanels = new List<List<System.Windows.Forms.Panel>>();
        private List<List<List<Enemy>>> characterList;
        private List<List<List<Coin>>> coinList;
        private List<List<List<Character>>> dungeonWalls;


        private int currentRow;
        private int currentCol;


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

            Game game = Game.Instance;
            SoundPlayer overworldTheme = new SoundPlayer(Resources.overworld_theme);
            overworldTheme.PlayLooping();

            if (game == null)
            {
                System.Environment.Exit(0);
            }

            int dungeonWidth = Game.Instance.Dungeon.GetLength(0);
            int dungeonHeight = Game.Instance.Dungeon.GetLength(1);

            characterList = new List<List<List<Enemy>>>(dungeonWidth);
            coinList = new List<List<List<Coin>>>(dungeonWidth);
            dungeonWalls = new List<List<List<Character>>>(dungeonWidth);

            for (int x = 0; x < dungeonWidth; x++)
            {
                characterList.Add(new List<List<Enemy>>());
                coinList.Add(new List<List<Coin>>());
                dungeonWalls.Add(new List<List<Character>>());

                for (int y = 0; y < dungeonHeight; y++)
                {
                    characterList[x].Add(new List<Enemy>());
                    coinList[x].Add(new List<Coin>());
                    dungeonWalls[x].Add(new List<Character>());
                }
            }

            MakeRoomPanels();
            this.GetCurrentRoom();
            ToggleRoomPanelVisibility(currentRoom);

            lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();

            game.player.Position = CreatePosition(picPlayer);
            game.player.Collider = CreateCollider(picPlayer, PADDING);

            timeBegin = DateTime.Now;
        }


        private static Vector2 CreatePosition(PictureBox pic)
        {
            return new Vector2(Game.instance.player.Position.x, Game.instance.player.Position.y);
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
            Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom = GetCurrentRoom();
            Game game = Game.Instance;
            // move player
            game.player.Move();

            // check collision with walls
            if (HitAWall(game.player, dungeonWalls[currentRow][currentCol]))
            {
                game.player.MoveBack();
            }

            // check collision with enemies
            foreach (Enemy enemy in characterList[currentRow][currentCol])
            {
                if (HitAChar(game.player, enemy))
                {
                    if (game.player.Health > 0)
                    {
                        Fight(enemy);
                    }
                    else
                    {
                        game.player.MoveBack();
                    }
                }
            }

            // check collision with coins
            foreach (var coin in coinList[currentRow][currentCol])
            {
                if (HitACoin(game.player, coin))
                {
                }
            }

            // update player's picture box
            picPlayer.Location = new Point((int)game.player.Position.x, (int)game.player.Position.y);
        }


        private bool HitAWall(Character c, List<Character> wallList)
        {
            bool hitAWall = false;
            foreach (Character wall in wallList)
            {
                if (c.Collider.Intersects(wall.Collider))
                {
                    hitAWall = true;
                    break;
                }
            }
            return hitAWall;
        }

        private bool HitAChar(Character you, Character other)
        {
            if (you != null && other != null && you.Collider != null && other.Collider != null)
            {
                return you.Collider.Intersects(other.Collider);
            }
            return false;
        }

        private bool HitACoin(Character character, Coin coin)
        {
            if (coin != null && character.Collider != null && coin.Collider != null)
            {
                return character.Collider.Intersects(coin.Collider);
            }
            return false;
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

        private void SetCurrentRoom(int row, int col)
        {
            currentRow = row;
            currentCol = col;
        }
        private Fall2020_CSC403_Project.code.Game.DungeonRoom GetCurrentRoom()
        {
            int dungeonWidth = Game.Instance.Dungeon.GetLength(0);
            int dungeonHeight = Game.Instance.Dungeon.GetLength(1);

            for (int j = 0; j < dungeonHeight; j++)
            {
                for (int i = 0; i < dungeonWidth; i++)
                {
                    Fall2020_CSC403_Project.code.Game.DungeonRoom room = (Fall2020_CSC403_Project.code.Game.DungeonRoom)Game.Instance.Dungeon[i, j];
                    if (Game.Instance.player.Position.x >= room.TopLeft.x && Game.Instance.player.Position.x <= room.TopRight.x
                        && Game.Instance.player.Position.y >= room.TopLeft.y && Game.Instance.player.Position.y <= room.BottomLeft.y)
                    {
                        SetCurrentRoom(i, j);
                        return room;
                    }
                }
            }

            return null;
        }

        private void LoadRoomElements(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom, System.Windows.Forms.Panel roomPanel)
        {
            List<Enemy> currentRoomEnemies = characterList[currentRow][currentCol];
            List<Coin> currentRoomCoins = coinList[currentRow][currentCol];

            foreach (IDungeonEnemyData enemyData in currentRoom.Enemies)
            {
                Enemy enemy = new Enemy(null, new Vector2(0,0), null, null);
                enemy.Img = LoadImage(enemyData.image);
                currentRoomEnemies.Add(enemy);

                PictureBox enemyPictureBox = new PictureBox();
                enemyPictureBox.Size = new Size(40, 40);
                enemyPictureBox.Location = new Point((int)enemyData.Position.x, (int)enemyData.Position.y);
                enemyPictureBox.BackgroundImage = LoadImage(enemyData.image);
                enemyPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                enemyPictureBox.Tag = "Enemy";

                System.Windows.Forms.Label enemyLabel = new System.Windows.Forms.Label();
                enemyLabel.Text = enemyData.displayName;
                enemyLabel.Location = new Point((int)enemyData.Position.x, (int)enemyData.Position.y - 20);
                enemyLabel.BackColor = Color.Transparent;

                roomPanel.Controls.Add(enemyPictureBox);
                roomPanel.Controls.Add(enemyLabel);
            }

            foreach (IDungeonCoin coinData in currentRoom.Coins)
            {
                Coin coin = new Coin(new Vector2(0, 0), null);
                coin.Img = LoadImage(coinData.Image);
                currentRoomCoins.Add(coin);

                PictureBox coinPictureBox = new PictureBox();
                coinPictureBox.Size = new Size(40, 40);
                coinPictureBox.Location = new Point((int)coinData.Position.x, (int)coinData.Position.y);
                coinPictureBox.BackgroundImage = LoadImage(coinData.Image);
                coinPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                coinPictureBox.Tag = "Coin";

                System.Windows.Forms.Label coinLabel = new System.Windows.Forms.Label();
                coinLabel.Text = coinData.Amount.ToString();
                coinLabel.Location = new Point((int)coinData.Position.x, (int)coinData.Position.y - 20);
                coinLabel.BackColor = Color.Transparent;

                roomPanel.Controls.Add(coinPictureBox);
                roomPanel.Controls.Add(coinLabel);
            }

            GenerateWalls(currentRoom, roomPanel); 
        }

        private System.Drawing.Image LoadImage(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    return System.Drawing.Image.FromFile(imagePath);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void GenerateWalls(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom, System.Windows.Forms.Panel roomPanel)
        {
            const int NUM_WALLS = 16;
            const int PADDING = 7;

            Character[] walls = new Character[NUM_WALLS];
            int wallIndex = 0;
            List<Character> currentRoomWalls = dungeonWalls[currentRow][currentCol];

            // top wall
            if (!currentRoom.northDoor)
            {
                for (int x = 0; x < NUM_WALLS; x++)
                {
                    if (wallIndex < walls.Length)
                    {
                        PictureBox pic = new PictureBox();
                        pic.Size = new Size(40, 40);
                        pic.Location = new Point(x * 40, 0);
                        pic.BackgroundImage = LoadImage("wall.jpg");
                        pic.BackgroundImageLayout = ImageLayout.Stretch;

                        walls[wallIndex] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
                        roomPanel.Controls.Add(pic);
                        wallIndex++;
                    }
                }
            }

            // right wall
            for (int y = 0; y < NUM_WALLS; y++)
            {
                if (wallIndex < walls.Length)
                {
                    PictureBox pic = new PictureBox();
                    pic.Size = new Size(40, 40);
                    pic.Location = new Point(roomPanel.Width - 40, y * 40);
                    pic.BackgroundImage = LoadImage("wall.jpg");
                    pic.BackgroundImageLayout = ImageLayout.Stretch;

                    walls[wallIndex] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
                    roomPanel.Controls.Add(pic);
                    wallIndex++;
                }
            }

            // bottom wall
            for (int x = 0; x < NUM_WALLS; x++)
            {
                if (wallIndex < walls.Length)
                {
                    PictureBox pic = new PictureBox();
                    pic.Size = new Size(40, 40);
                    pic.Location = new Point(x * 40, roomPanel.Height - 40);
                    pic.BackgroundImage = LoadImage("wall.jpg");
                    pic.BackgroundImageLayout = ImageLayout.Stretch;

                    walls[wallIndex] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
                    roomPanel.Controls.Add(pic);
                    wallIndex++;
                }
            }

            // left wall
            for (int y = 0; y < NUM_WALLS; y++)
            {
                if (wallIndex < walls.Length)
                {
                    PictureBox pic = new PictureBox();
                    pic.Size = new Size(40, 40);
                    pic.Location = new Point(0, y * 40);
                    pic.BackgroundImage = LoadImage("wall.jpg");
                    pic.BackgroundImageLayout = ImageLayout.Stretch;

                    walls[wallIndex] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
                    roomPanel.Controls.Add(pic);
                    wallIndex++;
                }
            }

            // Add all generated walls to the current room's wall list
            currentRoomWalls.AddRange(walls);
            dungeonWalls[currentRow][currentCol] = currentRoomWalls;
        }

        public void MakeRoomPanels()
        {
            for (int row = 0; row < Game.Instance.Dungeon.GetLength(0); row++)
            {
                List<System.Windows.Forms.Panel> rowPanelsList = new List<System.Windows.Forms.Panel>();
                for (int col = 0; col < Game.Instance.Dungeon.GetLength(1); col++)
                {
                    Fall2020_CSC403_Project.code.Game.DungeonRoom room = Game.Instance.Dungeon[row, col];
                    this.currentRoom = room;
                    this.currentRow = row;
                    this.currentCol = col;
                    System.Windows.Forms.Panel roomPanel = new System.Windows.Forms.Panel();
                    roomPanel.Size = new Size((int)(room.TopRight.x - room.TopLeft.x), (int)(room.BottomLeft.y - room.TopLeft.y));
                    roomPanel.Location = new Point((int)room.TopLeft.x, (int)room.TopLeft.y);
                    roomPanel.BackColor = Color.Black;
                    roomPanel.Visible = (room == currentRoom);
                    this.Controls.Add(roomPanel);
                    rowPanelsList.Add(roomPanel);
                    LoadRoomElements(room, roomPanel);
                }
                roomPanels.Add(rowPanelsList);
            }
        }

        private void ToggleRoomPanelVisibility(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom)
        {
            for (int row = 0; row < roomPanels.Count; row++)
            {
                for (int col = 0; col < roomPanels[row].Count; col++)
                {
                    roomPanels[row][col].Visible = (Game.Instance.Dungeon[row, col] == currentRoom);
                }
            }
        }

    }
}
