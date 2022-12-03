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

////11.26迭代说明_lxy:改变empty图片,文件路径改为相对路径,实现了说明界面显示和界面更新
///todo:1 说明界面比较粗糙,需要更改 2 从txt中读取map信息和储存 3 写的比较丑陋
///

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

    public class Map {
        private string MapPath = "../../map/map.txt";
        private string SourcePath = "../../source/";//souce的绝对地址
        //地图大小 以block数量记
        private int width = 11;
        private int height = 11;
        //每个block的像素宽高
        private int blockWidth = 40;
        private int blockHeight = 40;
        //地图左上点坐标
        private int mapLeftTop_X = 100;
        private int mapLeftTop_Y = 100;
        /// <summary>
        /// 各个显示的控件
        /// </summary>
        static Panel output;
        static PictureBox photo;
        static Label level;
        static Label Hp;
        static Label Damage;
        static Label armor;
        static Label message;
        private int outputWidth = 500;
        static int[,] map; //0:wall 1:enemy 2:key 3:door 4:empty
        static PictureBox[,] pictureMap;
        static ImageList imagelist;
        public Map() { }
        public void SetMap()
        {
            map = new int[11, 11]{          { 1,0,4,0,4,4,4,0,4,0,0},
                                            { 0,0,4,0,4,4,4,0,4,0,0},
                                            { 4,4,4,0,4,4,4,0,4,4,4},
                                            { 0,0,0,0,0,0,0,0,4,4,4},
                                            { 4,4,4,0,4,4,4,0,4,4,4},
                                            { 4,4,4,0,4,4,4,0,4,4,4},
                                            { 4,4,3,0,4,3,4,0,4,4,4},
                                            { 4,4,3,0,4,0,1,0,4,4,4},
                                            { 4,4,2,0,4,4,0,0,4,4,4},
                                            { 4,4,1,0,4,4,4,4,4,4,4},
                                            { 4,4,1,0,5,4,4,4,4,4,4},
                                          };
            blocks = new Block[height, width];
            //添加素材
            imagelist = new ImageList();
            pictureMap = new PictureBox[width, height];
            imagelist.ImageSize = new Size(blockWidth, blockHeight);
            Image wall = Image.FromFile(SourcePath + "wall.png");
            imagelist.Images.Add("wall", wall);
            Image enemy = Image.FromFile(SourcePath + "enemy1.png");
            imagelist.Images.Add("enemy", enemy);
            Image key = Image.FromFile(SourcePath + "key.png");
            imagelist.Images.Add("key", key);
            Image door = Image.FromFile(SourcePath + "door.png");
            imagelist.Images.Add("door", door);
            Image empty = Image.FromFile(SourcePath + "empty.png");
            imagelist.Images.Add("empty", empty);
            Image hero = Image.FromFile(SourcePath + "hero.png");
            imagelist.Images.Add("hero", hero);
            ///[todo]根据txt文件获得map
            ///
            ///
            ///
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    switch (map[i, j])
                    {
                        case 0: //wall
                            {
                                blocks[i, j] = new Wall();
                                break;
                            }
                        case 1: //enemy
                            {
                                blocks[i, j] = new Enemy();
                                break;
                            }
                        case 2: //key
                            {
                                blocks[i, j] = new Goods();
                                break;
                            }
                        case 3: //door
                            {
                                blocks[i, j] = new Doors();
                                break;
                            }
                        case 4: //empty
                            {
                                blocks[i, j] = new Empty();
                                break;
                            }
                    }
                }
            }
            ////
            ///添加控件
            ///
            output = new Panel();
            output.Location = new Point(mapLeftTop_X, mapLeftTop_Y);
            //output.BackColor = Color.Purple;
            output.BackgroundImage = imagelist.Images["empty"];
            output.Visible = true;
            output.Width = outputWidth + width * blockWidth;
            output.Height = height * blockHeight;
            //
            PictureBox photo = new PictureBox();
            photo.Location = new Point(outputWidth / 2 - 50 + width * blockWidth, /*mapLeftTop_Y*/  10);
            photo.Image = imagelist.Images["hero"];
            photo.Height = blockHeight * 2;
            photo.Width = blockWidth * 2;
            photo.SizeMode = PictureBoxSizeMode.StretchImage;
            photo.BringToFront();
            //
            level = new Label();
            level.Text = "等级  1";
            level.Location = new Point(20 + width * blockWidth, /*mapLeftTop_Y*/  110);
            level.Font = new Font("宋体", 20);
            level.AutoSize = true;
            level.ForeColor = Color.Black;
            output.Controls.Add(level);
            level.BackColor = Color.Transparent;
            ///
            Hp = new Label();
            Hp.Text = "生命  1000";
            Hp.Location = new Point(20 + width * blockWidth, /*mapLeftTop_Y*/  110 + 40);
            Hp.Font = new Font("宋体", 20);
            Hp.ForeColor = Color.Black;
            output.Controls.Add(Hp);
            Hp.AutoSize = true;
            Hp.BackColor = Color.Transparent;
            ////
            ///  
            Damage = new Label();
            Damage.Text = "攻击  10";
            Damage.Location = new Point(20 + width * blockWidth, /*mapLeftTop_Y*/  110 + 40 * 2);
            Damage.Font = new Font("宋体", 20);
            output.Controls.Add(Damage);
            Damage.AutoSize = true;
            Damage.ForeColor = Color.Black;
            Damage.BackColor = Color.Transparent;
            ////
            armor = new Label();
            armor.Text = "防御  10";
            armor.Location = new Point(20 + width * blockWidth, /*mapLeftTop_Y*/  110 + 40 * 3);
            armor.Font = new Font("宋体", 20);
            output.Controls.Add(armor);
            armor.AutoSize = true;
            armor.ForeColor = Color.Black;
            armor.BackColor = Color.Transparent;
            ///
            message = new Label();
            message.Text = "";
            message.Location = new Point(5+ width * blockWidth, /*mapLeftTop_Y*/  110 + 40 * 4);
            message.Font = new Font("宋体", 13);
            output.Controls.Add(message);
            message.AutoSize = true;
            message.ForeColor = Color.Red;
            message.BackColor = Color.Transparent;

            output.Controls.Add(photo);
            photo.Visible = true;
            photo.BackColor = Color.Transparent;
            Form1.form1.Controls.Add(output);

        }
        public void SaveMap()
        {

        }
        public void Paint()
        {

            try
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        pictureMap[j,i] = new PictureBox();
                        pictureMap[j,i].Location = new Point(/*mapLeftTop_X+*/j * blockWidth,/*mapLeftTop_Y+*/i * blockHeight);
                        pictureMap[j,i].Height = blockHeight;
                        pictureMap[j,i].Width = blockWidth;
                        switch (map[i, j])
                        {
                            case 0: //wall
                                {
                                    pictureMap[j,i].Image = imagelist.Images["wall"];
                                    break;
                                }
                            case 1: //enemy
                                {
                                    pictureMap[j,i].Image = imagelist.Images["enemy"];
                                    break;
                                }
                            case 2: //key
                                {
                                    pictureMap[j,i].Image = imagelist.Images["key"];
                                    break;
                                }
                            case 3: //door
                                {
                                    pictureMap[j,i].Image = imagelist.Images["door"];
                                    break;
                                }
                            case 4: //empty
                                {
                                    pictureMap[j,i].Image = imagelist.Images["empty"];
                                    break;
                                }
                            case 5://hero
                                {
                                    pictureMap[j,i].Image = imagelist.Images["hero"];
                                    user.X = i;
                                    user.Y = j;
                                    break;
                                }
                        }
                        output.Controls.Add(pictureMap[j,i]);
                        //pictureMap[j,i].BringToFront();
                        pictureMap[j,i].BackColor = Color.Transparent;
                        //Console.WriteLine("test");
                    }
                }
                PictureBox tmp = new PictureBox();
                tmp.BackColor = Color.Purple;
                tmp.Location = new Point(width * blockWidth,0);
                tmp.Width = outputWidth;
                tmp.Height = height*blockHeight;
                output.Controls.Add(tmp);

                //for(int i=0;i<)

            }
            catch (Exception e)
            {
                Console.WriteLine("error0001:" + e.ToString());
            }
        }

        User user = new User(); // User类，便于调用User
        int[] directionsX = { 0, 1, 0, -1 }; // 行坐标X的变化
        int[] directionsY = { 1, 0, -1, 0 }; // 列坐标Y的变化
        Block[,] blocks;
        /*public void SetTestMap()
        {
            map = new int[width, height];
            blocks = new Block[height, width];
        }*/
        private void showmessage()
        {
            Hp.Text = String.Format("生命  {0}", user.Hp.ToString());
            armor.Text = String.Format("防御  {0}", user.Armor.ToString());
            Damage.Text = String.Format("攻击  {0}", user.Damage.ToString());
        }
        // 移动函数
        // 需要添加控件显示信息，更新地图
        public void Move(int direction)
        {
            int endX = user.X + directionsX[direction];
            int endY = user.Y + directionsY[direction];
            if (endX < 0 || endY < 0)
            {
                return;
            }
            Block block = blocks[endX, endY];
            // 1. 添加banner或者title之类的提示信息，放在地图中间上方
            // 显示当前遇到了什么地形，或者做了什么事情
            if (!user.IsMoved(block))
            {
                message.Text=("can not move.\ntry to detect it(press J).");
                return;
            }
            // 2. 如果发生移动，更新当前地图，目标地点变为主角，出发地点变为空
            map[user.X, user.Y] = 4;
            map[endX, endY] = 5;
            blocks[user.X, user.Y] = new Empty();
            string answer = user.Deal(block);
            message.Text = (answer);
            pictureMap[endY, endX].Image = imagelist.Images["hero"];
            pictureMap[user.Y, user.X].Image = imagelist.Images["empty"];
            user.X = endX;
            user.Y= endY;
            showmessage();
        }

        // 侦测函数，不发生移动，但是需要显示提示信息，以及侦测到的敌人信息
        public void Detect(int direction)
        {
            int endX = user.X + directionsX[direction];
            int endY = user.Y + directionsY[direction];
            string answer = user.Detect(blocks[endX, endY]);
            message.Text = (answer);
            PaintEnemy(direction);
        }

        // 显示侦测到的敌人信息
        public void PaintEnemy(int direction)
        {

        }
    }

    #region User类，Block类，BlockType枚举类
    public class User {
        public int X { get; set; }
        public int Y { get; set; }
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
                case BlockType.Goods:
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
                case BlockType.Goods:
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
    }

    public enum BlockType
    {
        Wall = 0,
        Enemy = 1,
        Goods = 2,
        Doors = 3,
        Empty = 4
    }

    public abstract class Block
    {
        public abstract BlockType Type { get; set; }
    }

    public class Wall : Block
    {
        public override BlockType Type { get; set; } = BlockType.Wall;
    }

    public class Enemy : Block
    {
        public int Hp { get; set; } = 60;
        public int Damage { get; set; } = 20;
        public int Armor { get; set; } = 5;

        public override BlockType Type { get; set; } = BlockType.Enemy;
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
