using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fall2020_CSC403_Project.code
{
    public interface DungeonRoom
    {
        bool northDoor { get; set; }
        bool eastDoor { get; set; }
        bool southDoor { get; set; }
        bool westDoor { get; set; }
        List<IDungeonEnemyData> Enemies { get; set; }
        List<IDungeonCoin> Coins { get; set; }
        bool visited { get; set; }
        IDungeonPositionData TopLeft { get; set; }
        IDungeonPositionData TopRight { get; set; }
        IDungeonPositionData BottomLeft { get; set; }
        IDungeonPositionData BottomRight { get; set; }
    }

    public interface IDungeonCoin
    {
        float Amount { get; set; }
        string Image { get; set; }
        Guid ID { get; set; }
        IDungeonPositionData Position { get; set; }
    }

    public interface IDungeonEnemyData
    {
        string displayName { get; set; }
        bool defeated { get; set; }
        int MaxHealth { get; set; }
        float strength { get; set; }
        int Health { get; set; }
        string image { get; set; }
        Guid ID { get; set; }
        List<IEnemyDialogue> chatHistory { get; set; }
        IDungeonPositionData Position { get; set; }
    }

    public interface IDungeonPositionData
    {
        float x { get; set; }
        float y { get; set; }
    }

    public interface IEnemyDialogue
    {
        string UserName { get; set; }
        string Text { get; set; }
    }
}
