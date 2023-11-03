using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public class DungeonRoom
    {
        public bool northDoor { get; set; }
        public bool eastDoor { get; set; }
        public bool southDoor { get; set; }
        public bool westDoor { get; set; }
        public List<DungeonEnemyData> Enemies { get; set; }
        public List<DungeonCoin> Coins { get; set; }
        public bool visited { get; set; }
        public DungeonPositionData TopLeft { get; set; }
        public DungeonPositionData TopRight { get; set; }
        public DungeonPositionData BottomLeft { get; set; }
        public DungeonPositionData BottomRight { get; set; }
    }

    public class DungeonCoin
    {
        public float Amount { get; set; }
        public string Image { get; set; }
        public DungeonPositionData Position { get; set; }
    }

    public class DungeonEnemyData
    {
        public string displayName { get; set; }
        public bool defeated { get; set; }
        public int MaxHealth { get; set; }
        public float strength { get; set; }
        public int Health { get; set; }
        public string image { get; set; }
        public DungeonPositionData Position { get; set; }
    }

    public class DungeonPositionData
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public class Dungeon
    {
        public DungeonRoom[,] DungeonRooms { get; set; }
        private int N;
        private int roomWidth = 600;
        private int roomHeight = 800;
        private int padding = 7;
        List<string> enemyImages = new List<string>
        {
            "enemy_cheetos.fw.png",
            "enemy_koolaid.png",
            "enemy_poisonpacket.fw.png"
        };

        public Dungeon(int N)
        {
            this.N = N;
            DungeonRooms = new DungeonRoom[N, N];

            // create a 2D matrix to represent the dungeon
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    DungeonRoom room = new DungeonRoom
                    {
                        northDoor = false,
                        eastDoor = false,
                        southDoor = false,
                        westDoor = false,
                        Enemies = new List<DungeonEnemyData>(),
                        Coins = new List<DungeonCoin>(),
                        visited = false,
                    };
                    float roomLeft = i * (roomWidth + padding);
                    float roomTop = j * (roomHeight + padding);
                    float roomRight = roomLeft + roomWidth;
                    float roomBottom = roomTop + roomHeight;

                    room.TopLeft = new DungeonPositionData { x = roomLeft, y = roomTop };
                    room.TopRight = new DungeonPositionData { x = roomRight, y = roomTop };
                    room.BottomLeft = new DungeonPositionData { x = roomLeft, y = roomBottom };
                    room.BottomRight = new DungeonPositionData { x = roomRight, y = roomBottom };

                    GenerateRandomEnemies(room);
                    GenerateRandomCoins(room);

                    // add room to dungeon
                    DungeonRooms[i, j] = room;
                }
            }

            // generate a random starting room for the player;
            // this room should be on the outskirts of the maze itself
            int startX, startY;
            if (new Random().NextDouble() < 0.5)
            {
                // get a random row on the top or bottom wall
                startY = new Random().Next(0, 2) == 0 ? 0 : N - 1;
                startX = new Random().Next(0, N);
            }
            else
            {
                // get a random column on the left or right wall
                startX = new Random().Next(0, 2) == 0 ? 0 : N - 1;
                startY = new Random().Next(0, N);
            }
            
            // create the maze using DFS
            DepthFirstSearch(startX, startY);
        }

        private void DepthFirstSearch(int x, int y)
        {
            DungeonRoom room = DungeonRooms[x, y];
            room.visited = true;

            List<int> directions = new List<int> { 0, 1, 2, 3 };
            Shuffle(directions);

            foreach (int direction in directions)
            {
                int newX = x;
                int newY = y;

                if (direction == 0)
                    newY--;
                else if (direction == 1)
                    newX++;
                else if (direction == 2)
                    newY++;
                else if (direction == 3)
                    newX--;

                // check if the new position is even valid
                if (newX >= 0 && newX < N && newY >= 0 && newY < N)
                {
                    DungeonRoom nextRoom = DungeonRooms[newX, newY];

                    if (!nextRoom.visited)
                    {
                        // set doors for the room
                        if (direction == 0) // north
                        {
                            room.northDoor = true;
                            nextRoom.southDoor = true;
                        }
                        else if (direction == 1) // east
                        {
                            room.eastDoor = true;
                            nextRoom.westDoor = true;
                        }
                        else if (direction == 2) // south
                        {
                            room.southDoor = true;
                            nextRoom.northDoor = true;
                        }
                        else if (direction == 3) // west
                        {
                            room.westDoor = true;
                            nextRoom.eastDoor = true;
                        }
                        DepthFirstSearch(newX, newY);
                    }
                }
            }
        }

        private void Shuffle(List<int> list)
        {
            int n = list.Count;
            Random random = new Random();

            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                int temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        private void GenerateRandomEnemies(DungeonRoom room)
        {
            int numEnemies = new Random().Next(0, 3);
            for (int i = 0; i < numEnemies; i++)
            {
                // for random health
                int randomHealth = new Random().Next(3, 21);
                int roundedHealth = (int)(Math.Floor(randomHealth / 10.0) * 10);

                // for random strength
                int randomStrength = new Random().Next(3, 8);
                int roundedStrength = (int)(Math.Floor(randomStrength / 10.0) * 10);

                // for random indexing
                int randomIndex = new Random().Next(enemyImages.Count);

                DungeonEnemyData enemy = new DungeonEnemyData
                {
                    displayName = "Enemy " + (i+1),
                    defeated = false,
                    MaxHealth = roundedHealth,
                    strength = roundedStrength,
                    Health = roundedHealth,
                    image = enemyImages[randomIndex],
            };

                enemy.Position = CalculateRandomPositionInRoom(room);

                room.Enemies.Add(enemy);
            }
        }

        private void GenerateRandomCoins(DungeonRoom room)
        {
            int numCoins = new Random().Next(3, 8);
            for (int i = 0; i < numCoins; i++)
            {
                DungeonCoin coin = new DungeonCoin
                {
                    Amount = new Random().Next(1, 5),
                    Image = "coin.png"
                };

                coin.Position = CalculateRandomPositionInRoom(room);

                room.Coins.Add(coin);
            }
        }

        private DungeonPositionData CalculateRandomPositionInRoom(DungeonRoom room)
        {
            float minX = room.TopLeft.x;
            float maxX = room.TopRight.x;
            float minY = room.TopLeft.y;
            float maxY = room.BottomLeft.y;

            // generate random position in room
            float randomX = (float)(minX + (maxX - minX) * new Random().NextDouble());
            float randomY = (float)(minY + (maxY - minY) * new Random().NextDouble());

            return new DungeonPositionData { x = randomX, y = randomY };
        }

        public DungeonRoom GetCurrentRoom(DungeonPositionData playerPosition)
        {
            // using floor div here, given borders of rooms
            int roomX = (int)(playerPosition.x / (roomWidth + padding));
            int roomY = (int)(playerPosition.y / (roomHeight + padding));

            if (roomX >= 0 && roomX < N && roomY >= 0 && roomY < N)
            {
                return DungeonRooms[roomX, roomY];
            }
            
            return null;
        }

        // DEBUGGING
        public void PrintDungeon()
        {
            for (int j = 0; j < N; j++)
            {

                for (int i = 0; i < N; i++)
                {
                    DungeonRoom room = DungeonRooms[i, j];
                    Console.Write("+");
                    Console.Write(room.northDoor ? " " : "-");
                    Console.Write("+");
                }
                Console.WriteLine();

                for (int i = 0; i < N; i++)
                {
                    DungeonRoom room = DungeonRooms[i, j];
                    Console.Write(room.westDoor ? " " : "|");
                    Console.Write(" ");
                    Console.Write(room.eastDoor ? " " : "|");
                }
                Console.WriteLine();

                for (int i = 0; i < N; i++)
                {
                    DungeonRoom room = DungeonRooms[i, j];
                    Console.Write("+");
                    Console.Write(room.southDoor ? " " : "-");
                    Console.Write("+");
                }
                Console.WriteLine();
            }
        }

    }

}