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
        System.Windows.Forms.Panel mainPanel;


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

            this.mainPanel = new System.Windows.Forms.Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
            };

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
            //this.Controls.Add(mainPanel);
            this.currentRoom = this.GetCurrentRoom();
            roomPanels[currentRow][currentCol].Visible = true;
            roomPanels[currentRow][currentCol].Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(roomPanels[currentRow][currentCol]);

            lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();

            game.player.Position = CreatePosition(picPlayer, true);
            game.player.Collider = CreateCollider(picPlayer, PADDING);
            timeBegin = DateTime.Now;
        }

        private static Vector2 CreatePosition(PictureBox pic, bool PlayerPos)
        {
            if (PlayerPos)
            {
                return new Vector2(Game.instance.player.Position.x, Game.instance.player.Position.y);
            }
            else
            {
                return new Vector2(pic.Location.X, pic.Location.Y);
            }
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
            GetCurrentRoom();
            Console.WriteLine(currentCol);
            Console.WriteLine(currentRow);
            Console.WriteLine(currentRoom);
            Console.WriteLine(Game.Instance.player.Position.x);
            Console.WriteLine(Game.Instance.player.Position.y);
        }

        private void tmrPlayerMove_Tick(object sender, EventArgs e)
        {
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
                    game.player.coinCounter += coin.Amount;
                    //this.Controls.Remove(coin);
                }
            }

            // update player's picture box
            picPlayer.Location = new Point((int)game.player.Position.x, (int)game.player.Position.y);

            // TESTING
            mainPanel.AutoScrollPosition = picPlayer.Location;
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
            if (coin != null && character != null && character.Collider != null && coin.Collider != null)
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
            RenderDungeon();
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

        private void SetCurrentRoom(Fall2020_CSC403_Project.code.Game.DungeonRoom room)
        {
            currentRoom = room;
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
                    if (playerIsInRoom(room))
                    {
                        currentRow = i;
                        currentCol = j;
                        SetCurrentRoom(room);
                        return room;
                    }
                }
            }

            return null;
        }

        private bool playerIsInRoom(Fall2020_CSC403_Project.code.Game.DungeonRoom room)
        {
            return Game.Instance.player.Position.x >= room.TopLeft.x &&
                   Game.Instance.player.Position.x <= room.BottomRight.x &&
                   Game.Instance.player.Position.y >= room.TopLeft.y &&
                   Game.Instance.player.Position.y <= room.BottomRight.y;
        }

        private void LoadRoomElements(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom, System.Windows.Forms.Panel roomPanel)
        {
            const int PADDING = 7;
            int panelWidth = roomPanel.Width;
            int panelHeight = roomPanel.Height;

            List<Enemy> currentRoomEnemies = characterList[currentRow][currentCol];
            List<Coin> currentRoomCoins = coinList[currentRow][currentCol];

            foreach (IDungeonEnemyData enemyData in currentRoom.Enemies)
            {
                // Adjust enemy coordinates to fit within the panel's dimensions
                int enemyX = (int)enemyData.Position.x;
                int enemyY = (int)enemyData.Position.y;
                
                while (enemyX >= panelWidth) enemyX -= panelWidth;
                while (enemyY >= panelHeight) enemyY -= panelHeight;

                // Create the enemy, load image, and set position as before
                Enemy enemy = new Enemy(null, new Vector2(40, 40), null, null);
                enemy.Img = LoadImage(enemyData.image);
                currentRoomEnemies.Add(enemy);

                PictureBox enemyPictureBox = new PictureBox();
                enemyPictureBox.Size = new Size(40, 40);
                enemyPictureBox.Location = new Point(enemyX, enemyY);
                enemyPictureBox.BackgroundImage = enemy.Img;
                enemyPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                enemyPictureBox.Tag = "Enemy";
                enemyPictureBox.Visible = true;

                enemy.Position = CreatePosition(enemyPictureBox, false);
                enemy.Collider = CreateCollider(enemyPictureBox, PADDING);

                System.Windows.Forms.Label enemyLabel = new System.Windows.Forms.Label();
                enemyLabel.Text = enemyData.displayName;
                enemyLabel.Location = new Point(enemyX, enemyY - 20);
                enemyLabel.BackColor = Color.Transparent;

                roomPanel.Controls.Add(enemyPictureBox);
                roomPanel.Controls.Add(enemyLabel);

                enemyPictureBox.BringToFront();
                enemyLabel.BringToFront();
            }

            foreach (IDungeonCoin coinData in currentRoom.Coins)
            {
                // Adjust coin coordinates to fit within the panel's dimensions
                int coinX = (int)coinData.Position.x;
                int coinY = (int)coinData.Position.y;

                while (coinX >= panelWidth) coinX -= panelWidth;
                while (coinY >= panelHeight) coinY -= panelHeight;

                // Create the coin, load image, and set position as before
                int coinValue = (int)Math.Round(coinData.Amount);
                Coin coin = new Coin(new Vector2(20, 20), null, coinValue);
                coin.Img = LoadImage(coinData.Image);
                currentRoomCoins.Add(coin);

                PictureBox coinPictureBox = new PictureBox();
                coinPictureBox.Size = new Size(40, 40);
                coinPictureBox.Location = new Point(coinX, coinY);
                coinPictureBox.BackgroundImage = coin.Img;
                coinPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                coinPictureBox.Tag = "Coin";
                coinPictureBox.Visible = true;

                coin.Position = CreatePosition(coinPictureBox, false);
                coin.Collider = CreateCollider(coinPictureBox, PADDING);

                System.Windows.Forms.Label coinLabel = new System.Windows.Forms.Label();
                coinLabel.Text = coinData.Amount.ToString();
                coinLabel.Location = new Point(coinX, coinY - 20);
                coinLabel.BackColor = Color.Transparent;

                roomPanel.Controls.Add(coinPictureBox);
                roomPanel.Controls.Add(coinLabel);

                coinPictureBox.BringToFront();
                coinLabel.BringToFront();
            }

            GenerateWalls(currentRoom, roomPanel);
        }


        private System.Drawing.Image LoadImage(string imageName)
        {
            try
            {
                string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string imagePath = Path.Combine(appDirectory, "..", "..", "data", imageName);

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
            const int PADDING = 4;
            int wallWidth = 25;
            List<Character> currentRoomWalls = dungeonWalls[currentRow][currentCol];

            // Generate the north wall
            if (!currentRoom.northDoor)
            {
                // Create a full-width top wall
                PictureBox topWall = new PictureBox();
                topWall.Size = new Size(roomPanel.Width, wallWidth);
                topWall.Location = new Point(0, 0);
                topWall.BackgroundImage = LoadImage("wall.jpg");
                topWall.BackgroundImageLayout = ImageLayout.Stretch;
                topWall.Visible = true;
                roomPanel.Controls.Add(topWall);

                Character topWallCharacter = new Character(CreatePosition(topWall, false), CreateCollider(topWall, PADDING));
                currentRoomWalls.Add(topWallCharacter);
            }
            else
            {
                // Create a 1/3 width top wall
                PictureBox topWall = new PictureBox();
                topWall.Size = new Size(roomPanel.Width / 3, wallWidth);
                topWall.Location = new Point(0, 0);
                topWall.BackgroundImage = LoadImage("wall.jpg");
                topWall.BackgroundImageLayout = ImageLayout.Stretch;
                topWall.Visible = true;
                roomPanel.Controls.Add(topWall);

                Character topWallCharacter = new Character(CreatePosition(topWall, false), CreateCollider(topWall, PADDING));
                currentRoomWalls.Add(topWallCharacter);

                // Start from 2/3 down the wall for the other end of the wall.
                PictureBox topWall2 = new PictureBox();
                topWall2.Size = new Size(roomPanel.Width / 3, wallWidth);
                topWall2.Location = new Point(roomPanel.Width / 3 * 2, 0);
                topWall2.BackgroundImage = LoadImage("wall.jpg");
                topWall2.BackgroundImageLayout = ImageLayout.Stretch;
                topWall2.Visible = true;
                roomPanel.Controls.Add(topWall2);

                Character topWallCharacter2 = new Character(CreatePosition(topWall2, false), CreateCollider(topWall2, PADDING));
                currentRoomWalls.Add(topWallCharacter2);
            }

            // Generate the east wall
            if (!currentRoom.eastDoor)
            {
                // Create a full-height right wall
                PictureBox rightWall = new PictureBox();
                rightWall.Size = new Size(wallWidth, roomPanel.Height);
                rightWall.Location = new Point(roomPanel.Width - wallWidth, 0);
                rightWall.BackgroundImage = LoadImage("wall.jpg");
                rightWall.BackgroundImageLayout = ImageLayout.Stretch;
                rightWall.Visible = true;
                roomPanel.Controls.Add(rightWall);

                Character rightWallCharacter = new Character(CreatePosition(rightWall, false), CreateCollider(rightWall, PADDING));
                currentRoomWalls.Add(rightWallCharacter);
            }
            else
            {
                // Create a 1/3 height right wall
                PictureBox rightWall = new PictureBox();
                rightWall.Size = new Size(wallWidth, roomPanel.Height / 3);
                rightWall.Location = new Point(roomPanel.Width - wallWidth, 0);
                rightWall.BackgroundImage = LoadImage("wall.jpg");
                rightWall.BackgroundImageLayout = ImageLayout.Stretch;
                rightWall.Visible = true;
                roomPanel.Controls.Add(rightWall);

                Character rightWallCharacter = new Character(CreatePosition(rightWall, false), CreateCollider(rightWall, PADDING));
                currentRoomWalls.Add(rightWallCharacter);

                // Start from 2/3 across the wall for the other end of the wall.
                PictureBox rightWall2 = new PictureBox();
                rightWall2.Size = new Size(wallWidth, roomPanel.Height / 3);
                rightWall2.Location = new Point(roomPanel.Width - wallWidth, roomPanel.Height / 3 * 2);
                rightWall2.BackgroundImage = LoadImage("wall.jpg");
                rightWall2.BackgroundImageLayout = ImageLayout.Stretch;
                rightWall2.Visible = true;
                roomPanel.Controls.Add(rightWall2);

                Character rightWallCharacter2 = new Character(CreatePosition(rightWall2, false), CreateCollider(rightWall2, PADDING));
                currentRoomWalls.Add(rightWallCharacter2);
            }

            // Generate the south wall
            if (!currentRoom.southDoor)
            {
                // Create a full-width bottom wall
                PictureBox bottomWall = new PictureBox();
                bottomWall.Size = new Size(roomPanel.Width, wallWidth);
                bottomWall.Location = new Point(0, roomPanel.Height - wallWidth);
                bottomWall.BackgroundImage = LoadImage("wall.jpg");
                bottomWall.BackgroundImageLayout = ImageLayout.Stretch;
                bottomWall.Visible = true;
                roomPanel.Controls.Add(bottomWall);

                Character bottomWallCharacter = new Character(CreatePosition(bottomWall, false), CreateCollider(bottomWall, PADDING));
                currentRoomWalls.Add(bottomWallCharacter);
            }
            else
            {
                // Create a 1/3 width bottom wall
                PictureBox bottomWall = new PictureBox();
                bottomWall.Size = new Size(roomPanel.Width / 3, wallWidth);
                bottomWall.Location = new Point(0, roomPanel.Height - wallWidth);
                bottomWall.BackgroundImage = LoadImage("wall.jpg");
                bottomWall.BackgroundImageLayout = ImageLayout.Stretch;
                bottomWall.Visible = true;
                roomPanel.Controls.Add(bottomWall);

                Character bottomWallCharacter = new Character(CreatePosition(bottomWall, false), CreateCollider(bottomWall, PADDING));
                currentRoomWalls.Add(bottomWallCharacter);

                // Start from 2/3 down the wall for the other end of the wall.
                PictureBox bottomWall2 = new PictureBox();
                bottomWall2.Size = new Size(roomPanel.Width / 3, wallWidth);
                bottomWall2.Location = new Point(roomPanel.Width / 3 * 2, roomPanel.Height - wallWidth);
                bottomWall2.BackgroundImage = LoadImage("wall.jpg");
                bottomWall2.BackgroundImageLayout = ImageLayout.Stretch;
                bottomWall2.Visible = true;
                roomPanel.Controls.Add(bottomWall2);

                Character bottomWallCharacter2 = new Character(CreatePosition(bottomWall2, false), CreateCollider(bottomWall2, PADDING));
                currentRoomWalls.Add(bottomWallCharacter2);
            }

            // Generate the west wall
            if (!currentRoom.westDoor)
            {
                // Create a full-height left wall
                PictureBox leftWall = new PictureBox();
                leftWall.Size = new Size(wallWidth, roomPanel.Height);
                leftWall.Location = new Point(0, 0);
                leftWall.BackgroundImage = LoadImage("wall.jpg");
                leftWall.BackgroundImageLayout = ImageLayout.Stretch;
                leftWall.Visible = true;
                roomPanel.Controls.Add(leftWall);

                Character leftWallCharacter = new Character(CreatePosition(leftWall, false), CreateCollider(leftWall, PADDING));
                currentRoomWalls.Add(leftWallCharacter);
            }
            else
            {
                // Create a 1/3 height left wall
                PictureBox leftWall = new PictureBox();
                leftWall.Size = new Size(wallWidth, roomPanel.Height / 3);
                leftWall.Location = new Point(0, 0);
                leftWall.BackgroundImage = LoadImage("wall.jpg");
                leftWall.BackgroundImageLayout = ImageLayout.Stretch;
                leftWall.Visible = true;
                roomPanel.Controls.Add(leftWall);

                Character leftWallCharacter = new Character(CreatePosition(leftWall, false), CreateCollider(leftWall, PADDING));
                currentRoomWalls.Add(leftWallCharacter);

                // Start from 2/3 across the wall for the other end of the wall.
                PictureBox leftWall2 = new PictureBox();
                leftWall2.Size = new Size(wallWidth, roomPanel.Height / 3);
                leftWall2.Location = new Point(0, roomPanel.Height / 3 * 2);
                leftWall2.BackgroundImage = LoadImage("wall.jpg");
                leftWall2.BackgroundImageLayout = ImageLayout.Stretch;
                leftWall2.Visible = true;
                roomPanel.Controls.Add(leftWall2);

                Character leftWallCharacter2 = new Character(CreatePosition(leftWall2, false), CreateCollider(leftWall2, PADDING));
                currentRoomWalls.Add(leftWallCharacter2);
            }
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
                    int roomPanelX = (int)(room.TopLeft.x);
                    int roomPanelY = (int)(room.TopLeft.y);
                    int roomPanelWidth = 800;
                    int roomPanelHeight = 1000;

                    System.Windows.Forms.Panel roomPanel = new System.Windows.Forms.Panel();
                    roomPanel.Size = new Size(roomPanelWidth, roomPanelHeight);
                    roomPanel.Location = new Point(roomPanelX, roomPanelY);
                    roomPanel.BackColor = Color.Gray;
                    roomPanel.Visible = true;
                    rowPanelsList.Add(roomPanel);
                    LoadRoomElements(room, roomPanel);
                    mainPanel.Controls.Add(roomPanel);
                }
                roomPanels.Add(rowPanelsList);
            }
        }

        private void RenderDungeon()
        {
            int playerX = currentCol;
            int playerY = currentRow;
            int visibilityRadius = 2;

            for (int i = 0; i < Game.Instance.Dungeon.GetLength(0); i++)
            {
                for (int j = 0; j < Game.Instance.Dungeon.GetLength(1); j++)
                {
                    int distance = Math.Abs(i - playerY) + Math.Abs(j - playerX);


                    if (distance <= visibilityRadius)
                    {
                        roomPanels[i][j].Visible = true;
                    }
                    else
                    {
                        roomPanels[i][j].Visible = false;
                    }
                }
            }
        }


    }
}
