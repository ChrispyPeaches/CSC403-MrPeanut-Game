using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fall2020_CSC403_Project.code
{
    /// <summary>
    /// This is the class for an enemy
    /// </summary>
    public class Enemy : BattleCharacter
    {
        /// <summary>
        /// THis is the image for an enemy
        /// </summary>
        public Image Img { get; set; }

        /// <summary>
        /// this is the background color for the fight form for this enemy
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// this is the name of the enemy
        /// </summary>
        public string Name { get; set; }
        public string displayName { get; set; }
        public bool Defeated { get; set; }
        public Guid ID { get; set; }
        public List<IEnemyDialogue> chatHistory { get; set; }
        /// <summary>
        /// this determines if the enemy has made peace
        /// </summary>
        public bool isPeaceful { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initPos">this is the initial position of the enemy</param>
        /// <param name="collider">this is the collider for the enemy</param>
        public Enemy(string name, Vector2 initPos, Collider collider, bool defeated = false, Guid id = new Guid()) : base(initPos, collider)
        {
            displayName = displayName;
            Defeated = defeated;
            ID = id;
            isPeaceful = false;
        }
    }
}
