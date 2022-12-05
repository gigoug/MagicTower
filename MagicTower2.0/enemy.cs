using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagicTower2._0
{
    public enum Attribute { None=0,Fire=1,ice=2};
    public class Enemy : Block
    {
        public static float[,] drop = new float[define.EnemyNum, define.ItemNum];
       // public float[] drop=new float[define.ItemNum];
        public int EnemyIndex { get; set; }
        public int Hp { get; set; }
        public int Damage { get; set; }
        public int Armor { get; set; }
        public int Speed { get; set; }
        public int Experience { get; set; }
        private Attribute attri;
        private int attri_num;
        public override BlockType Type { get; set; } = BlockType.Enemy;

        public Enemy(int enemyIndex, int hp, int damage, int armor, int speed, int experience, Attribute attri, int attri_num)
        {
            EnemyIndex = enemyIndex;
            Hp = hp;
            Damage = damage;
            Armor = armor;
            Speed = speed;
            Experience = experience;
            this.attri = attri;
            this.attri_num = attri_num;
            Type = BlockType.Enemy;
        }
        public Enemy(int index)
        {
            EnemyIndex = Map.EnemyInfo[index].EnemyIndex;
            Hp = Map.EnemyInfo[index].Hp;
            Damage = Map.EnemyInfo[index].Damage;
            Armor = Map.EnemyInfo[index].Armor;
            Speed = Map.EnemyInfo[index].Speed;
            Experience = Map.EnemyInfo[index].EnemyIndex;
            this.attri = Map.EnemyInfo[index].attri;
            this.attri_num = Map.EnemyInfo[index].attri_num;
            Type = BlockType.Enemy;
        }
        Enemy() { }

    }
}
