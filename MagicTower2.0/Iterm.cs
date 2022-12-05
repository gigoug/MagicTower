using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicTower2._0
{
    public class Item : Block
    {
        public static int[,] formulate = new int[define.ItemNum, define.ItemNum];
        public int[] Intensify;
        private int restore { set; get; }
        public override BlockType Type { get; set; } = BlockType.Item;
        Item(int[] intensify, int restore, BlockType type)
        {
            Intensify = intensify;
            this.restore = restore;
            Type = BlockType.Item;
        }
        Item (int index)
        {
            Intensify = Map.ItemInfo[index].Intensify;
            this.restore = Map.ItemInfo[index].restore;
            Type = BlockType.Item;
        }
    }
}
