using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1591 // use this to disable comment warnings

namespace Fall2020_CSC403_Project.code
{

    public class CharacterState
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public float strength { get; set; }
    }

    public class BattleCharacter : Character
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public float strength { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<int> AttackEvent;
        public BattleCharacter(Vector2 initPos, Collider collider) : base(initPos, collider)
        {

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ChangeHealthAndStrength(int newHealth, int newMaxHealth, float newStrength)
        {
            this.MaxHealth = newMaxHealth;
            this.strength = newStrength;
            this.Health = newHealth;
            OnPropertyChanged(nameof(Health));
            OnPropertyChanged(nameof(MaxHealth));
            OnPropertyChanged(nameof(strength));
        }

        public void OnAttack(int amount)
        {
            AttackEvent((int)(-amount * this.strength));
        }

        public void AlterHealth(int amount)
        {
            this.Health -= amount;
        }

        public static CharacterState GetCharacterState(BattleCharacter character)
        {
            return new CharacterState
            {
                Health = character.Health,
                MaxHealth = character.MaxHealth,
                strength = character.strength,
            };
        }
    }
}
