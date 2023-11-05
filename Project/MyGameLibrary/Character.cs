using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fall2020_CSC403_Project.code
{
    public class Character
    {
        private const int GO_INC = 3;
        public Vector2 MoveSpeed { get; set; }
        public Vector2 LastPosition { get; set; }
        public Vector2 Position { get; set; }
        public Collider Collider { get; set; }
        public Character(Vector2 initPos, Collider collider)
        {
            this.Position = this.Position;
            this.Collider = collider;
            this.MoveSpeed = new Vector2(0, 0);
        }

        public void Move()
        {
            this.LastPosition = this.Position;
            this.Position = new Vector2(this.Position.x + this.MoveSpeed.x, this.Position.y + this.MoveSpeed.y);
            this.Collider.MovePosition((int)this.Position.x, (int)this.Position.y);
        }

        public void MoveBack()
        {
            this.Position = this.LastPosition;
        }

        public void GoLeft()
        {
            this.MoveSpeed = new Vector2(-GO_INC, 0);
        }
        public void GoRight()
        {
            MoveSpeed = new Vector2(+GO_INC, 0);
        }
        public void GoUp()
        {
            this.MoveSpeed = new Vector2(0, -GO_INC);
        }
        public void GoDown()
        {
            this.MoveSpeed = new Vector2(0, +GO_INC);
        }
        public void GoDownRight()
        {
            this.MoveSpeed = new Vector2(+GO_INC, +GO_INC);
        }
        public void GoUpRight()
        {
            this.MoveSpeed = new Vector2(+GO_INC, -GO_INC);
        }
        public void GoDownLeft()
        {
            this.MoveSpeed = new Vector2(-GO_INC, +GO_INC);
        }
        public void GoUpLeft()
        {
            this.MoveSpeed = new Vector2(-GO_INC, -GO_INC);
        }
        public void ResetMoveSpeed()
        {
            this.MoveSpeed = new Vector2(0, 0);
        }
    }
}
