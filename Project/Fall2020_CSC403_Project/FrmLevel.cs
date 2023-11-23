﻿using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Media;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Vector2 = Fall2020_CSC403_Project.code.Vector2;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections;

namespace Fall2020_CSC403_Project
{
    public partial class FrmLevel : Form
    {
        private List<Character> walls;
        private List<Character> doors;

        private DateTime timeBegin;
        private FrmBattle frmBattle;

        private Enemy currentEnemy = null;

        public static FrmLevel instanceForDeath { get; private set; }

        public IOpenAIApi _openAIApi;

        public static FrmLevel instance;


        private bool isUpPressed = false;
        private bool isDownPressed = false;
        private bool isLeftPressed = false;
        private bool isRightPressed = false;

        private Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom;
        private List<List<System.Windows.Forms.Panel>> roomPanels = new List<List<System.Windows.Forms.Panel>>();
        private List<List<List<Enemy>>> characterList;
        private List<List<List<Coin>>> coinList;
        private List<List<List<Character>>> dungeonWalls;
        private List<Enemy> currentRoomEnemies;
        private List<Coin> currentRoomCoins;
        private List<Control> roomControls = new List<Control>();

        public int currentRow;
        public int currentCol;

        public FrmLevel(IOpenAIApi openAIApi)
        {
            InitializeComponent();
            SetPlayerImage();
            _openAIApi = openAIApi;
            instanceForDeath = this;
            if (instance == null)
            {
                instance = this;
            }

            // Remove borders and scroll bars
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScroll = false;
        }

        // Return current instance of window
        public static FrmLevel Instance
        {
            get
            {
                return instance;
            }
        }

        public void ResetMovementBooleans()
        {
            isUpPressed = false;
            isDownPressed = false;
            isRightPressed = false;
            isLeftPressed = false;
        }

        public int getRow()
        {
            return this.currentRow;
        }

        public int getColumn()
        {
            return this.currentCol;
        }

        private void FrmLevel_Load(object sender, EventArgs e)
        {
            const int PADDING = 7;

            Game game = Game.Instance;
            this.currentRow = game.getRowForLevel();
            this.currentCol = game.getColForLevel();
            this.DoubleBuffered = true;
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

            this.currentRoom = Game.Instance.Dungeon[currentRow, currentCol];
            LoadRoomElements(currentRoom);

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
        }
        private void tmrPlayerMove_Tick(object sender, EventArgs e)
        {
            Game game = Game.Instance;
            // move player
            game.player.Move();

            // check collision with walls
            if (HitAWall(game.player, this.walls))
            {
                game.player.MoveBack();
            }

            if (HitAChar(game.player, this.currentRoomEnemies))
            {
                if (game.player.Health > 0)
                {
                    Fight(currentEnemy);
                }
                else
                {
                    game.player.MoveBack();
                }
            }

            if (HitACoin(game.player, currentRoomCoins))
            {
                lblCoins.Text = "Coins: " + Game.Instance.player.coinCounter.ToString();
            }

            if (HitADoor(game.player, doors))
            {
            }

            this.UpdatePlayerPictureBox();

        }

        private bool HitAWall(Character c, List<Character> wallList)
        {
            foreach (Character wall in wallList)
            {
                if (c.Collider.Intersects(wall.Collider))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HitAChar(Character you, List<Enemy> enemyList)
        {
            foreach (Enemy enemy in enemyList)
            {
                if (you.Collider.Intersects(enemy.Collider))
                {
                    currentEnemy = enemy;
                    return true;
                }
            }
            return false;
        }

        private bool HitACoin(Character character, List<Coin> coinsList)
        {
            foreach (Coin coin in coinsList)
            {
                if (character.Collider.Intersects(coin.Collider))
                {
                    Game.Instance.player.coinCounter += coin.Amount;
                    RemoveCoinControl(coin.ID);

                    coinsList.RemoveAll(c => c.ID == coin.ID);

                    return true;
                }
            }
            return false;
        }

        private void RemoveCoinControl(Guid coinId)
        {
            this.roomControls.RemoveAll(control =>
            {
                if (control is PictureBox pictureBox && pictureBox.Tag is string tag && tag.StartsWith("Coin") && Guid.TryParse(tag.Substring(4), out Guid guid))
                {
                    if (guid == coinId)
                    {
                        control.Parent.Controls.Remove(control);
                        control.Dispose();
                        return true;
                    }
                }
                return false;
            });
        }

        private bool HitADoor(Character character, List<Character> doors)
        {
            foreach (Character door in doors)
            {
                if (door != null && character != null && character.Collider != null && door.Collider != null)
                {
                    bool collisionMade = character.Collider.Intersects(door.Collider);

                    if (collisionMade)
                    {
                        string doorDirection = door.Tag as string;

                        for (int i = roomControls.Count - 1; i >= 0; i--)
                        {
                            try
                            {
                                this.Controls.Remove(roomControls[i]);
                                roomControls[i].Dispose();
                                roomControls.RemoveAt(i);
                            }
                            catch
                            {
                            }
                        }

                        roomControls.Clear();

                        this.UpdateRoomData();

                        Game game = Game.Instance;
                        Player player = game.player;
                        int oldRow = currentRow;
                        int oldCol = currentCol;

                        switch (doorDirection)
                        {
                            case "North":
                                currentRow -= 1;
                                character.Position = new Vector2((int)character.Position.x, ClientSize.Height-150);
                                break;

                            case "East":
                                currentCol += 1;
                                character.Position = new Vector2(ClientSize.Width/10 - 50, (int)character.Position.y);
                                break;

                            case "South":
                                currentRow += 1;
                                character.Position = new Vector2((int)character.Position.x, ClientSize.Height/10);
                                break;

                            case "West":
                                currentCol -= 1;
                                character.Position = new Vector2(ClientSize.Width-150, (int)character.Position.y);
                                break;
                        }

                        try
                        {
                            currentRoom = (Fall2020_CSC403_Project.code.Game.DungeonRoom)Game.Instance.Dungeon[currentRow, currentCol];
                        }
                        catch
                        {
                            currentRow = oldRow;
                            currentCol = oldCol;
                            player.MoveBack();
                        }

                        LoadRoomElements(currentRoom);

                        return collisionMade;
                    }
                }
            }
            return false;
        }

        private void UpdatePlayerPictureBox()
        {
            picPlayer.Location = new Point((int)Game.Instance.player.Position.x, (int)Game.Instance.player.Position.y);
        }

        private void Fight(Enemy enemy)
        {
            Game game = Game.Instance;
            Player player = game.player;
            player.MoveBack();
            try
            {
                frmBattle = FrmBattle.GetInstance(enemy, _openAIApi);

                if (!(frmBattle == null) && enemy.Defeated != true)
                {
                    if (enemy.Name.Contains("koolaid")) 
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
                QuickStartMenu quickStartMenu = new QuickStartMenu(this);
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
            player.ResetMoveSpeed();
            if (isUpPressed && isRightPressed)
            {
                player.GoUpRight();
            }
            else if (isUpPressed && isLeftPressed)
            {
                player.GoUpLeft();
            }
            else if (isDownPressed && isRightPressed)
            {
                player.GoDownRight();
            }
            else if (isDownPressed && isLeftPressed)
            {
                player.GoDownLeft();
            }
            else if (isUpPressed)
            {
                player.GoUp();
            }
            else if (isDownPressed)
            {
                player.GoDown();
            }
            else if (isLeftPressed)
            {
                player.GoLeft();
            }
            else if (isRightPressed)
            {
                player.GoRight();
            }
            else
            {
                player.ResetMoveSpeed();
            }
        }

        private void lblInGameTime_Click(object sender, EventArgs e)
        {

        }

        private void picBossKoolAid_Click(object sender, EventArgs e)
        {

        }

        private void LoadRoomElements(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom)
        {
            const int PADDING = 7;

            this.currentRoomEnemies = new List<Enemy>();
            this.currentRoomCoins = new List<Coin>();
            this.roomControls = new List<Control>();

            foreach (IDungeonEnemyData enemyData in currentRoom.Enemies)
            {
                if(enemyData.defeated != true)
                {
                    int enemyX = (int)enemyData.Position.x;
                    int enemyY = (int)enemyData.Position.y;

                    Enemy enemy = new Enemy(null, new Vector2(enemyX, enemyY), null, enemyData.defeated, enemyData.ID);
                    enemy.Img = GetImageFromResources(enemyData.image);

                    PictureBox enemyPictureBox = new PictureBox();
                    enemyPictureBox.Size = new Size(70, 70);
                    enemyPictureBox.Location = new Point(enemyX, enemyY);
                    enemyPictureBox.BackgroundImage = enemy.Img;
                    enemyPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                    enemyPictureBox.Tag = "Enemy" + enemy.ID.ToString();
                    enemyPictureBox.Visible = true;

                    enemy.Position = CreatePosition(enemyPictureBox, false);
                    enemy.Collider = CreateCollider(enemyPictureBox, PADDING);
                    enemy.Name = enemyData.image;
                    enemy.displayName = enemyData.displayName;
                    enemy.chatHistory = enemyData.chatHistory;
                    enemy.ChangeHealthAndStrength(enemyData.Health, enemyData.MaxHealth, enemyData.strength);

                    System.Windows.Forms.Label enemyLabel = new System.Windows.Forms.Label();
                    enemyLabel.Text = enemyData.displayName;
                    enemyLabel.Location = new Point(enemyX - 20, enemyY - 20);
                    enemyLabel.BackColor = Color.Transparent;
                    enemyLabel.Tag = "Enemy" + enemy.ID.ToString();

                    this.Controls.Add(enemyPictureBox);
                    this.Controls.Add(enemyLabel);
                    this.roomControls.Add(enemyPictureBox);
                    this.roomControls.Add(enemyLabel);
                    currentRoomEnemies.Add(enemy);
                    Point location = PointToScreen(enemyPictureBox.Location);
                }
            }

            foreach (IDungeonCoin coinData in currentRoom.Coins)
            {
                int coinX = (int)(currentRoom.BottomLeft.x + coinData.Position.x);
                int coinY = (int)(currentRoom.TopRight.y + coinData.Position.y);

                int coinValue = (int)Math.Round(coinData.Amount);
                Guid coinID = coinData.ID;
                Coin coin = new Coin(new Vector2(coinX, coinY), null, coinValue, coinID);
                coin.Img = Resources.coin1;

                PictureBox coinPictureBox = new PictureBox();
                coinPictureBox.Size = new Size(30, 30);
                coinPictureBox.Location = new Point(coinX, coinY);
                coinPictureBox.BackgroundImage = coin.Img;
                coinPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                coinPictureBox.Tag = "Coin" + coinID;
                coinPictureBox.Visible = true;

                coin.Position = CreatePosition(coinPictureBox, false);
                coin.Collider = CreateCollider(coinPictureBox, PADDING);

                System.Windows.Forms.Label coinLabel = new System.Windows.Forms.Label();
                coinLabel.Text = coinData.Amount.ToString();
                coinLabel.Location = new Point(coinX, coinY);
                coinLabel.BackColor = Color.Transparent;
                coinLabel.BringToFront();

                this.Controls.Add(coinPictureBox);
                this.Controls.Add(coinLabel);
                this.roomControls.Add(coinPictureBox);
                this.roomControls.Add(coinLabel);
                currentRoomCoins.Add(coin);
                Point location = PointToScreen(coinPictureBox.Location);
            }

            GenerateWalls(currentRoom);
        }

        private void GenerateWalls(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom)
        {
            Bitmap wallImage = Resources.wall;

            int wallWidth = 50;
            this.walls = new List<Character>();
            this.doors = new List<Character>();

            int roomHeight = ClientRectangle.Height;
            int roomWidth = ClientRectangle.Width;

            #region North Wall

            if (!currentRoom.northDoor)
            {
                PictureBox topWall = new PictureBox();
                topWall.Size = new Size(roomWidth, wallWidth);
                topWall.Location = new Point(0, 0);
                topWall.BackgroundImage = wallImage;
                topWall.BackgroundImageLayout = ImageLayout.Tile;
                topWall.Visible = true;
                this.Controls.Add(topWall);
                this.roomControls.Add(topWall);

                Character topWallCharacter = new Character(CreatePosition(topWall, false), CreateCollider(topWall, 0));
                this.walls.Add(topWallCharacter);
            }
            else
            {
                PictureBox topWall = new PictureBox();
                topWall.Size = new Size(roomWidth / 3, wallWidth);
                topWall.Location = new Point(0, 0);
                topWall.BackgroundImage = wallImage;
                topWall.BackgroundImageLayout = ImageLayout.Tile;
                topWall.Visible = true;
                this.Controls.Add(topWall);
                this.roomControls.Add(topWall);

                Character topWallCharacter = new Character(CreatePosition(topWall, false), CreateCollider(topWall, 0));
                this.walls.Add(topWallCharacter);

                PictureBox door = new PictureBox();
                door.Size =new Size(roomWidth / 3, wallWidth);
                door.Location = new Point(roomWidth /3, 0);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Tile;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, 0))
                {
                    Tag = "North"
                };
                this.doors.Add(doorCharacter);

                PictureBox topWall2 = new PictureBox();
                topWall2.Size = new Size(roomWidth / 3, wallWidth);
                topWall2.Location = new Point(roomWidth / 3 * 2, 0);
                topWall2.BackgroundImage = wallImage;
                topWall2.BackgroundImageLayout = ImageLayout.Tile;
                topWall2.Visible = true;
                this.Controls.Add(topWall2);
                this.roomControls.Add(topWall2);

                Character topWallCharacter2 = new Character(CreatePosition(topWall2, false), CreateCollider(topWall2, 0));
                this.walls.Add(topWallCharacter2);
            }

            #endregion

            #region East Wall

            if (!currentRoom.eastDoor)
            {
                PictureBox rightWall = new PictureBox();
                rightWall.Size = new Size(wallWidth, roomHeight);
                rightWall.Location = new Point(roomWidth - wallWidth, 0);
                rightWall.BackgroundImage = wallImage;
                rightWall.BackgroundImageLayout = ImageLayout.Tile;
                rightWall.Visible = true;
                this.Controls.Add(rightWall);
                this.roomControls.Add(rightWall);

                Character rightWallCharacter = new Character(CreatePosition(rightWall, false), CreateCollider(rightWall, 0));
                this.walls.Add(rightWallCharacter);
            }
            else
            {
                PictureBox rightWall = new PictureBox();
                rightWall.Size = new Size(wallWidth, roomHeight / 3);
                rightWall.Location = new Point(roomWidth - wallWidth, 0);
                rightWall.BackgroundImage = wallImage;
                rightWall.BackgroundImageLayout = ImageLayout.Tile;
                rightWall.Visible = true;
                this.Controls.Add(rightWall);
                this.roomControls.Add(rightWall);

                Character rightWallCharacter = new Character(CreatePosition(rightWall, false), CreateCollider(rightWall, 0));
                this.walls.Add(rightWallCharacter);

                PictureBox door = new PictureBox();
                door.Size = new Size(wallWidth, roomHeight / 3);
                door.Location = new Point(roomWidth - wallWidth, roomHeight / 3);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Tile;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, 0))
                {
                    Tag = "East"
                };
                this.doors.Add(doorCharacter);

                PictureBox rightWall2 = new PictureBox();
                rightWall2.Size = new Size(wallWidth, roomHeight / 3);
                rightWall2.Location = new Point(roomWidth - wallWidth, roomHeight / 3 * 2);
                rightWall2.BackgroundImage = wallImage;
                rightWall2.BackgroundImageLayout = ImageLayout.Tile;
                rightWall2.Visible = true;
                this.Controls.Add(rightWall2);
                this.roomControls.Add(rightWall2);

                Character rightWallCharacter2 = new Character(CreatePosition(rightWall2, false), CreateCollider(rightWall2, 0));
                this.walls.Add(rightWallCharacter2);
            }

            #endregion

            #region South Wall

            if (!currentRoom.southDoor)
            {
                PictureBox bottomWall = new PictureBox();
                bottomWall.Size = new Size(roomWidth, wallWidth);
                bottomWall.Location = new Point(0, roomHeight - wallWidth);
                bottomWall.BackgroundImage = wallImage;
                bottomWall.BackgroundImageLayout = ImageLayout.Tile;
                bottomWall.Visible = true;
                this.Controls.Add(bottomWall);
                this.roomControls.Add(bottomWall);

                Character bottomWallCharacter = new Character(CreatePosition(bottomWall, false), CreateCollider(bottomWall, 0));
                this.walls.Add(bottomWallCharacter);
            }
            else
            {
                PictureBox bottomWall = new PictureBox();
                bottomWall.Size = new Size(roomWidth / 3, wallWidth);
                bottomWall.Location = new Point(0, roomHeight - wallWidth - 0);
                bottomWall.BackgroundImage = wallImage;
                bottomWall.BackgroundImageLayout = ImageLayout.Tile;
                bottomWall.Visible = true;
                this.Controls.Add(bottomWall);
                this.roomControls.Add(bottomWall);

                Character bottomWallCharacter = new Character(CreatePosition(bottomWall, false), CreateCollider(bottomWall, 0));
                this.walls.Add(bottomWallCharacter);

                PictureBox door = new PictureBox();
                door.Size = new Size(roomWidth / 3, wallWidth);
                door.Location = new Point(roomWidth / 3, roomHeight - wallWidth);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Tile;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, 0))
                {
                    Tag = "South"
                };
                this.doors.Add(doorCharacter);

                PictureBox bottomWall2 = new PictureBox();
                bottomWall2.Size = new Size(roomWidth / 3, wallWidth);
                bottomWall2.Location = new Point(roomWidth / 3 * 2, roomHeight - wallWidth);
                bottomWall2.BackgroundImage = wallImage;
                bottomWall2.BackgroundImageLayout = ImageLayout.Tile;
                bottomWall2.Visible = true;
                this.Controls.Add(bottomWall2);
                this.roomControls.Add(bottomWall2);

                Character bottomWallCharacter2 = new Character(CreatePosition(bottomWall2, false), CreateCollider(bottomWall2, 0));
                this.walls.Add(bottomWallCharacter2);
            }

            #endregion

            #region West Wall

            if (!currentRoom.westDoor)
            {                PictureBox leftWall = new PictureBox();
                leftWall.Size = new Size(wallWidth, roomHeight);
                leftWall.Location = new Point(0, 0);
                leftWall.BackgroundImage = wallImage;
                leftWall.BackgroundImageLayout = ImageLayout.Tile;
                leftWall.Visible = true;
                this.Controls.Add(leftWall);
                this.roomControls.Add(leftWall);

                Character leftWallCharacter = new Character(CreatePosition(leftWall, false), CreateCollider(leftWall, 0));
                this.walls.Add(leftWallCharacter);
            }
            else
            {
                PictureBox leftWall = new PictureBox();
                leftWall.Size = new Size(wallWidth, roomHeight / 3);
                leftWall.Location = new Point(0, 0);
                leftWall.BackgroundImage = wallImage;
                leftWall.BackgroundImageLayout = ImageLayout.Tile;
                leftWall.Visible = true;
                this.Controls.Add(leftWall);
                this.roomControls.Add(leftWall);

                Character leftWallCharacter = new Character(CreatePosition(leftWall, false), CreateCollider(leftWall, 0));
                this.walls.Add(leftWallCharacter);

                PictureBox door = new PictureBox();
                door.Size = new Size(wallWidth, roomHeight / 3);
                door.Location = new Point(0, roomHeight / 3);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Tile;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, 0))
                {
                    Tag = "West"
                };
                this.doors.Add(doorCharacter);

                PictureBox leftWall2 = new PictureBox();
                leftWall2.Size = new Size(wallWidth, roomHeight / 3);
                leftWall2.Location = new Point(0, roomHeight / 3 * 2);
                leftWall2.BackgroundImage = wallImage;
                leftWall2.BackgroundImageLayout = ImageLayout.Tile;
                leftWall2.Visible = true;
                this.Controls.Add(leftWall2);
                this.roomControls.Add(leftWall2);

                Character leftWallCharacter2 = new Character(CreatePosition(leftWall2, false), CreateCollider(leftWall2, 0));
                this.walls.Add(leftWallCharacter2);
            }

            #endregion
        }

        public void UpdateEnemyData(List<Enemy> enemyList)
        {
            foreach (Enemy enemy in enemyList)
            { 
                if (enemy.ID == currentEnemy.ID)
                {
                    enemy.Health = enemy.Health;
                    enemy.Defeated = true;
                }
            }
        }

        public void UpdateRoomData()
        {
            foreach (IDungeonEnemyData currentEnemy in Game.Instance.Dungeon[currentRow, currentCol].Enemies)
            {
                foreach (Enemy enemy in currentRoomEnemies)
                {
                    if (enemy.ID == currentEnemy.ID)
                    {
                        currentEnemy.Health = enemy.Health;
                        currentEnemy.defeated = enemy.Defeated;
                        currentEnemy.chatHistory = enemy.chatHistory;
                        break;
                    }
                }
            }

            foreach (IDungeonCoin currentCoin in Game.Instance.Dungeon[currentRow, currentCol].Coins)
            {
                foreach (Coin coin in currentRoomCoins)
                {
                    if (coin.ID == currentCoin.ID)
                    {
                        if (coin.Collider == null)
                        {
                            Game.Instance.Dungeon[currentRow, currentCol].Coins.Remove(currentCoin);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            this.currentRoomCoins.Clear();
            this.currentRoomEnemies.Clear();
            this.doors.Clear();
            this.walls.Clear();
            this.roomControls.Clear();
        }

        public void SetPlayerImage()
        {
            string playerImagePath = Path.Combine(Settings.Default.AppDataDirectory, "playerImage.png");

            if (File.Exists(playerImagePath))
            {
                picPlayer.BackgroundImage = System.Drawing.Image.FromFile(playerImagePath);
            }
        }

        // Upon defeating an enemy, hide image and label
        public void DefeatEnemy(string Id)
        {
            // Get picture box then hide
            PictureBox enemyPictureBox = Controls.OfType<PictureBox>().FirstOrDefault(pb => (string)pb.Tag == "Enemy" + Id);
            enemyPictureBox.BackgroundImage = Resources.transparent;
            enemyPictureBox.Hide();

            // Get label then hide
            System.Windows.Forms.Label enemyLabel = Controls.OfType<System.Windows.Forms.Label>().FirstOrDefault(l => (string)l.Tag == "Enemy" + Id);
            enemyLabel.Hide();
        }

        public static Bitmap GetImageFromResources(string resourceName)
        {
            return (Bitmap) Resources.ResourceManager
                .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>()
                .Where(resource => resource.Value.GetType().Equals(typeof(Bitmap)) &&
                                   resource.Key.ToString() == resourceName)
                .Select(resource => resource.Value)
                .First();
        }
    }
}
