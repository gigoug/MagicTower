using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTower2._0
{
    public class User
    {
        public int X { get; set; }
        public int Y { get; set; }
        //user自身属性
        public int Hp { get; set; } = 1000;
        public int Damage { get; set; } = 10;
        public int Armor { get; set; } = 10;

        public int[] Keys = { 0, 0, 0 };

        public string Deal(Block block)
        {
            if (!IsMoved(block))
            {
                return Detect(block);
            }
            switch (block.Type)
            {
                case BlockType.Enemy:
                    return Attack(block);
                case BlockType.Goods:
                    return PickUp(block);
                case BlockType.Doors:
                    return Unlock(block);
                case BlockType.Empty:
                    return "You moved forward";
                case BlockType.Stair:
                    return GoUpstair(block);
            }
            return "";
        }

        public string Attack(Block block)
        {
            Enemy enemy = (Enemy)block;
            int damageOfUser = this.Damage - enemy.Armor;
            int numberOfAttacks = (enemy.Hp + damageOfUser - 1) / damageOfUser;
            this.Hp -= numberOfAttacks * (enemy.Damage - this.Armor);
            return "You defeat it.";
        }

        public string PickUp(Block block)
        {
            switch (block.Type)
            {
                case BlockType.Item:
                    if (block is KeysOfGame tempkeys)
                    {
                        this.Keys[tempkeys.keyCode]++;
                    }
                    break;
            }
            return "You pick up it";
        }

        public string Unlock(Block block)
        {
            Doors doors = (Doors)block;
            this.Keys[doors.keyCode]--;
            return "You open the door.";
        }

        public string Detect(Block block)
        {
            switch (block.Type)
            {
                case BlockType.Wall:
                    return "Cannot move. This is wall.";
                case BlockType.Enemy:
                    if (IsMoved(block))
                    {
                        return "This is enemy. You could attack it.";
                    }
                    else
                    {
                        return "You can not attack it.";
                    }
                case BlockType.Item:
                    return "You may have found some useful goods.";
                case BlockType.Doors:
                    return "This is a door. You need a key to open it.";
                case BlockType.Empty:
                    return "There's nothing here. You could move forward.";
            }
            return "";
        }

        public bool IsMoved(Block block)
        {
            switch (block.Type)
            {
                case BlockType.Wall:
                    return false;
                case BlockType.Enemy:
                    return IsAttacked(block);
                case BlockType.Doors:
                    return IsUnlocked(block);
                case BlockType.Goods:
                case BlockType.Empty:
                    return true;
            }
            return true;
        }

        public bool IsAttacked(Block block)
        {
            Enemy enemy = (Enemy)block;
            if (enemy.Armor >= this.Damage)
            {
                return false;
            }
            int damageOfUser = this.Damage - enemy.Armor;
            int numberOfAttacks = (enemy.Hp + damageOfUser - 1) / damageOfUser;
            if (numberOfAttacks * (enemy.Damage - this.Armor) >= this.Hp)
            {
                return false;
            }
            return true;
        }

        public bool IsUnlocked(Block block)
        {
            Doors door = (Doors)block;
            return this.Keys[door.keyCode] > 0;
        }
        public string GoUpstair(Block block)
        {
            return "Congratulation!\nyou went upstair";
        }
    }
}
