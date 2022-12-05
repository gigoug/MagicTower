using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace MagicTower2._0
{
    public partial class Form1 : Form
    {
        public static Map mymap = new Map();

        public static Form1 form1;
        public Form1()
        {
            InitializeComponent();
            form1 = this;
            mymap.SetMap();
            mymap.Paint();
            //Label test=new Label();
            //test.Text = "test";
            //this.panel1.Controls.Add(test);
        }

        int direction = 0; // 表示方向
        //处理键盘消息
        //要注意的是:由于按钮等元素的存在,窗体得不到KeyDown事件,所以在覆盖ProcessCmdKey
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool bMoved = false;
            switch (keyData)
            {
                case Keys.D:
                    bMoved = true;
                    direction = 0;
                    break;
                case Keys.S:
                    bMoved = true;
                    direction = 1;
                    break;
                case Keys.A:
                    bMoved = true;
                    direction = 2;
                    break;
                case Keys.W:
                    bMoved = true;
                    direction = 3;
                    break;
                case Keys.J:
                    mymap.Detect(direction);
                    break;
            }
            if (bMoved)
            {
                mymap.Move(direction);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

   

    #region User类，Block类，BlockType枚举类

    public enum BlockType
    {
        Wall = 0,   
        Enemy = 1,
        Item = 2,
        Doors = 3,
        Empty = 4,
        Goods=5,
        Stair=6
    }

    public abstract class Block
    {
        public abstract BlockType Type { get; set; }
    }

    public class Wall : Block
    {
        public override BlockType Type { get; set; } = BlockType.Wall;
    }
    public class stair:Block {
        public override BlockType Type { get; set; } = BlockType.Stair;
    }

    public class Goods : Block
    {
        public override BlockType Type { get; set; } = BlockType.Goods;
    }

    public class KeysOfGame : Goods
    {
        public int keyCode;
    }
    public class YellowKey : KeysOfGame
    {
        public YellowKey()
        {
            keyCode = 0;
        }
    }
    public class BlueKey : KeysOfGame
    {
        public BlueKey()
        {
            keyCode = 1;
        }
    }
    public class RedKey : KeysOfGame
    {
        public RedKey()
        {
            keyCode = 2;
        }
    }
    public class HealthPots : Goods { }

    public class Doors : Block
    {
        public int keyCode;
        public override BlockType Type { get; set; } = BlockType.Doors;
    }
    public class YellowDoor : Doors
    {
        public YellowDoor()
        {
            keyCode = 0;
        }
    }
    public class BlueDoor : Doors
    {
        public BlueDoor()
        {
            keyCode = 1;
        }
    }
    public class RedDoor : Doors
    {
        public RedDoor()
        {
            keyCode = 2;
        }
    }

    public class Empty : Block
    {
        public override BlockType Type { get; set; } = BlockType.Empty;
    }
    #endregion

}
