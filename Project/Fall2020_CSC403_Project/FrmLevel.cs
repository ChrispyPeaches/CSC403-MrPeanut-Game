﻿using Fall2020_CSC403_Project.code;
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
        private List<Character> walls;
        private List<Character> doors;

        private DateTime timeBegin;
        private FrmBattle frmBattle;

        private Enemy currentEnemy = null;
        private Coin currentCoin = null;

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
        private List<Enemy> currentRoomEnemies;
        private List<Coin> currentRoomCoins;
        private List<Control> roomControls = new List<Control>();

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
            this.currentRow = game.row;
            this.currentCol = game.column;
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
                game.player.coinCounter += currentCoin.Amount;
            }

            if (HitADoor(game.player, doors))
            {
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

        private bool HitAChar(Character you, List<Enemy> enemyList)
        {
            bool hitAChar = false;
            foreach (Enemy enemy in enemyList)
            {
                if (you.Collider.Intersects(enemy.Collider))
                {
                    currentEnemy = enemy;
                    hitAChar = true;
                    break;
                }
            }
            return hitAChar;
        }

        private bool HitACoin(Character character, List<Coin> coinsList)
        {
            foreach (Coin coin in coinsList)
            {
                if (coin != null && character != null && character.Collider != null && coin.Collider != null)
                {
                        currentCoin = coin;
                        return character.Collider.Intersects(coin.Collider);
                }
            }
            return false;
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

                        foreach (Control control in roomControls)
                        {
                            if (this.Controls.Contains(control))
                            {
                                this.Controls.Remove(control);
                            }

                        }

                        switch (doorDirection)
                        {
                            case "North":
                                currentRow -= 1;
                                character.Position = new Vector2((int)character.Position.x, 700);
                                try
                                {
                                    currentRoom = (Fall2020_CSC403_Project.code.Game.DungeonRoom)Game.Instance.Dungeon[currentRow, currentCol];
                                }
                                catch
                                {
                                    currentRow += 1;
                                }
                                LoadRoomElements(currentRoom);
                                break;

                            case "East":
                                currentCol += 1;
                                character.Position = new Vector2(100, (int)character.Position.y);
                                try
                                {
                                    currentRoom = (Fall2020_CSC403_Project.code.Game.DungeonRoom)Game.Instance.Dungeon[currentRow, currentCol];
                                }
                                catch
                                {
                                    currentCol -= 1;
                                }
                                LoadRoomElements(currentRoom);
                                break;

                            case "South":
                                currentRow += 1;
                                character.Position = new Vector2((int)character.Position.x, 100);
                                try
                                {
                                    currentRoom = (Fall2020_CSC403_Project.code.Game.DungeonRoom)Game.Instance.Dungeon[currentRow, currentCol];
                                }
                                catch
                                {
                                    currentRow -= 1;
                                }
                                LoadRoomElements(currentRoom);
                                break;

                            case "West":
                                currentCol -= 1;
                                character.Position = new Vector2(700, (int)character.Position.y);
                                try
                                {
                                    currentRoom = (Fall2020_CSC403_Project.code.Game.DungeonRoom)Game.Instance.Dungeon[currentRow, currentCol];
                                }
                                catch
                                {
                                    currentCol += 1;
                                }
                                LoadRoomElements(currentRoom);
                                break;
                        }
                    }
                    return collisionMade;
                }
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

                if (!(frmBattle == null) && enemy.Defeated != true)
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

        private void LoadRoomElements(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom)
        {
            const int PADDING = 7;

            this.currentRoomEnemies = new List<Enemy>();
            this.currentRoomCoins = new List<Coin>();
            this.roomControls = new List<Control>();

            foreach (IDungeonEnemyData enemyData in currentRoom.Enemies)
            {
                int enemyX = (int)enemyData.Position.x % (this.Width -50);
                int enemyY = (int)enemyData.Position.y % (this.Height -50);

                Enemy enemy = new Enemy(null, new Vector2(50, 50), null, enemyData.defeated, enemyData.ID);
                enemy.Img = LoadImage(enemyData.image);

                PictureBox enemyPictureBox = new PictureBox();
                enemyPictureBox.Size = new Size(70, 70);
                enemyPictureBox.Location = new Point(enemyX, enemyY);
                enemyPictureBox.BackgroundImage = enemy.Img;
                enemyPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                enemyPictureBox.Tag = "Enemy";
                enemyPictureBox.Visible = true;

                enemy.Position = CreatePosition(enemyPictureBox, false);
                enemy.Collider = CreateCollider(enemyPictureBox, PADDING);

                System.Windows.Forms.Label enemyLabel = new System.Windows.Forms.Label();
                enemyLabel.Text = enemyData.displayName;
                enemyLabel.Location = new Point(enemyX, enemyY);
                enemyLabel.BackColor = Color.Transparent;

                this.Controls.Add(enemyPictureBox);
                this.Controls.Add(enemyLabel);
                this.roomControls.Add(enemyPictureBox);
                this.roomControls.Add(enemyLabel);
                currentRoomEnemies.Add(enemy);
            }

            foreach (IDungeonCoin coinData in currentRoom.Coins)
            {
                int coinX = (int)coinData.Position.x % (this.Width - 50);
                int coinY = (int)coinData.Position.y % (this.Height - 50);

                int coinValue = (int)Math.Round(coinData.Amount);
                Coin coin = new Coin(new Vector2(20, 20), null, coinValue);
                coin.Img = LoadImage(coinData.Image);

                PictureBox coinPictureBox = new PictureBox();
                coinPictureBox.Size = new Size(30, 30);
                coinPictureBox.Location = new Point(coinX, coinY);
                coinPictureBox.BackgroundImage = coin.Img;
                coinPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                coinPictureBox.Tag = "Coin";
                coinPictureBox.Visible = true;

                coin.Position = CreatePosition(coinPictureBox, false);
                coin.Collider = CreateCollider(coinPictureBox, PADDING);

                System.Windows.Forms.Label coinLabel = new System.Windows.Forms.Label();
                coinLabel.Text = coinData.Amount.ToString();
                coinLabel.Location = new Point(coinX, coinY);
                coinLabel.BackColor = Color.Transparent;

                this.Controls.Add(coinPictureBox);
                this.Controls.Add(coinLabel);
                this.roomControls.Add(coinPictureBox);
                this.roomControls.Add(coinLabel);
                currentRoomCoins.Add(coin);
            }

            GenerateWalls(currentRoom);
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

        private void GenerateWalls(Fall2020_CSC403_Project.code.Game.DungeonRoom currentRoom)
        {
            const int PADDING = 4;
            int wallWidth = 25;
            this.walls = new List<Character>();
            this.doors = new List<Character>();

            if (!currentRoom.northDoor)
            {
                PictureBox topWall = new PictureBox();
                topWall.Size = new Size(this.Width, wallWidth);
                topWall.Location = new Point(0, 0);
                topWall.BackgroundImage = LoadImage("wall.jpg");
                topWall.BackgroundImageLayout = ImageLayout.Stretch;
                topWall.Visible = true;
                this.Controls.Add(topWall);
                this.roomControls.Add(topWall);

                Character topWallCharacter = new Character(CreatePosition(topWall, false), CreateCollider(topWall, PADDING));
                this.walls.Add(topWallCharacter);
            }
            else
            {
                PictureBox topWall = new PictureBox();
                topWall.Size = new Size(this.Width / 3, wallWidth);
                topWall.Location = new Point(this.Width / 3, 0);
                topWall.BackgroundImage = LoadImage("wall.jpg");
                topWall.BackgroundImageLayout = ImageLayout.Stretch;
                topWall.Visible = true;
                this.Controls.Add(topWall);
                this.roomControls.Add(topWall);

                Character topWallCharacter = new Character(CreatePosition(topWall, false), CreateCollider(topWall, PADDING));
                this.walls.Add(topWallCharacter);

                PictureBox door = new PictureBox();
                door.Size =new Size(this.Width / 3, wallWidth);
                door.Location = new Point(this.Width - wallWidth, this.Height / 3);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Stretch;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, PADDING));
                doorCharacter.Tag = "North";
                this.doors.Add(doorCharacter);

                PictureBox topWall2 = new PictureBox();
                topWall2.Size = new Size(this.Width / 3, wallWidth);
                topWall2.Location = new Point(this.Width / 3 * 2, 0);
                topWall2.BackgroundImage = LoadImage("wall.jpg");
                topWall2.BackgroundImageLayout = ImageLayout.Stretch;
                topWall2.Visible = true;
                this.Controls.Add(topWall2);
                this.roomControls.Add(topWall2);

                Character topWallCharacter2 = new Character(CreatePosition(topWall2, false), CreateCollider(topWall2, PADDING));
                this.walls.Add(topWallCharacter2);
            }

            if (!currentRoom.eastDoor)
            {
                PictureBox rightWall = new PictureBox();
                rightWall.Size = new Size(wallWidth, this.Height);
                rightWall.Location = new Point(this.Width - wallWidth, 0);
                rightWall.BackgroundImage = LoadImage("wall.jpg");
                rightWall.BackgroundImageLayout = ImageLayout.Stretch;
                rightWall.Visible = true;
                this.Controls.Add(rightWall);
                this.roomControls.Add(rightWall);

                Character rightWallCharacter = new Character(CreatePosition(rightWall, false), CreateCollider(rightWall, PADDING));
                this.walls.Add(rightWallCharacter);
            }
            else
            {
                PictureBox rightWall = new PictureBox();
                rightWall.Size = new Size(wallWidth, this.Height / 3);
                rightWall.Location = new Point(this.Width - wallWidth, 0);
                rightWall.BackgroundImage = LoadImage("wall.jpg");
                rightWall.BackgroundImageLayout = ImageLayout.Stretch;
                rightWall.Visible = true;
                this.Controls.Add(rightWall);
                this.roomControls.Add(rightWall);

                Character rightWallCharacter = new Character(CreatePosition(rightWall, false), CreateCollider(rightWall, PADDING));
                this.walls.Add(rightWallCharacter);

                PictureBox door = new PictureBox();
                door.Size = new Size(wallWidth, this.Height / 3);
                door.Location = new Point(this.Width - wallWidth, this.Height / 3);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Stretch;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, PADDING));
                doorCharacter.Tag = "East";
                this.doors.Add(doorCharacter);

                PictureBox rightWall2 = new PictureBox();
                rightWall2.Size = new Size(wallWidth, this.Height / 3);
                rightWall2.Location = new Point(this.Width - wallWidth, this.Height / 3 * 2);
                rightWall2.BackgroundImage = LoadImage("wall.jpg");
                rightWall2.BackgroundImageLayout = ImageLayout.Stretch;
                rightWall2.Visible = true;
                this.Controls.Add(rightWall2);
                this.roomControls.Add(rightWall2);

                Character rightWallCharacter2 = new Character(CreatePosition(rightWall2, false), CreateCollider(rightWall2, PADDING));
                this.walls.Add(rightWallCharacter2);
            }
            if (!currentRoom.southDoor)
            {
                PictureBox bottomWall = new PictureBox();
                bottomWall.Size = new Size(this.Width, wallWidth);
                bottomWall.Location = new Point(0, this.Height - wallWidth);
                bottomWall.BackgroundImage = LoadImage("wall.jpg");
                bottomWall.BackgroundImageLayout = ImageLayout.Stretch;
                bottomWall.Visible = true;
                this.Controls.Add(bottomWall);
                this.roomControls.Add(bottomWall);

                Character bottomWallCharacter = new Character(CreatePosition(bottomWall, false), CreateCollider(bottomWall, PADDING));
                this.walls.Add(bottomWallCharacter);
            }
            else
            {
                PictureBox bottomWall = new PictureBox();
                bottomWall.Size = new Size(this.Width / 3, wallWidth);
                bottomWall.Location = new Point(0, this.Height - wallWidth);
                bottomWall.BackgroundImage = LoadImage("wall.jpg");
                bottomWall.BackgroundImageLayout = ImageLayout.Stretch;
                bottomWall.Visible = true;
                this.Controls.Add(bottomWall);
                this.roomControls.Add(bottomWall);

                Character bottomWallCharacter = new Character(CreatePosition(bottomWall, false), CreateCollider(bottomWall, PADDING));
                this.walls.Add(bottomWallCharacter);

                PictureBox door = new PictureBox();
                door.Size = new Size(this.Width / 3, wallWidth);
                door.Location = new Point(this.Width / 3, this.Height - wallWidth);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Stretch;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, PADDING));
                doorCharacter.Tag = "South";
                this.doors.Add(doorCharacter);

                PictureBox bottomWall2 = new PictureBox();
                bottomWall2.Size = new Size(this.Width / 3, wallWidth);
                bottomWall2.Location = new Point(this.Width / 3 * 2, this.Height - wallWidth);
                bottomWall2.BackgroundImage = LoadImage("wall.jpg");
                bottomWall2.BackgroundImageLayout = ImageLayout.Stretch;
                bottomWall2.Visible = true;
                this.Controls.Add(bottomWall2);
                this.roomControls.Add(bottomWall2);

                Character bottomWallCharacter2 = new Character(CreatePosition(bottomWall2, false), CreateCollider(bottomWall2, PADDING));
                this.walls.Add(bottomWallCharacter2);
            }

            if (!currentRoom.westDoor)
            {                PictureBox leftWall = new PictureBox();
                leftWall.Size = new Size(wallWidth, this.Height);
                leftWall.Location = new Point(0, 0);
                leftWall.BackgroundImage = LoadImage("wall.jpg");
                leftWall.BackgroundImageLayout = ImageLayout.Stretch;
                leftWall.Visible = true;
                this.Controls.Add(leftWall);
                this.roomControls.Add(leftWall);

                Character leftWallCharacter = new Character(CreatePosition(leftWall, false), CreateCollider(leftWall, PADDING));
                this.walls.Add(leftWallCharacter);
            }
            else
            {
                PictureBox leftWall = new PictureBox();
                leftWall.Size = new Size(wallWidth, this.Height / 3);
                leftWall.Location = new Point(0, 0);
                leftWall.BackgroundImage = LoadImage("wall.jpg");
                leftWall.BackgroundImageLayout = ImageLayout.Stretch;
                leftWall.Visible = true;
                this.Controls.Add(leftWall);
                this.roomControls.Add(leftWall);

                Character leftWallCharacter = new Character(CreatePosition(leftWall, false), CreateCollider(leftWall, PADDING));
                this.walls.Add(leftWallCharacter);

                PictureBox door = new PictureBox();
                door.Size = new Size(wallWidth, this.Height / 3);
                door.Location = new Point(0, this.Height / 3);
                door.BackColor = Color.Brown;
                door.BackgroundImageLayout = ImageLayout.Stretch;
                door.Visible = true;
                this.Controls.Add(door);
                this.roomControls.Add(door);

                Character doorCharacter = new Character(CreatePosition(door, false), CreateCollider(door, PADDING));
                doorCharacter.Tag = "West";
                this.doors.Add(doorCharacter);

                PictureBox leftWall2 = new PictureBox();
                leftWall2.Size = new Size(wallWidth, this.Height / 3);
                leftWall2.Location = new Point(0, this.Height / 3 * 2);
                leftWall2.BackgroundImage = LoadImage("wall.jpg");
                leftWall2.BackgroundImageLayout = ImageLayout.Stretch;
                leftWall2.Visible = true;
                this.Controls.Add(leftWall2);
                this.roomControls.Add(leftWall2);

                Character leftWallCharacter2 = new Character(CreatePosition(leftWall2, false), CreateCollider(leftWall2, PADDING));
                this.walls.Add(leftWallCharacter2);
            }
        }

        public void UpdateEnemyData(List<Enemy> enemyList)
        {
            foreach (Enemy enemy in enemyList)
            { 
                if (enemy.ID == currentEnemy.ID)
                {
                    enemy.Defeated = true;
                }
            }
        }
    }
}
