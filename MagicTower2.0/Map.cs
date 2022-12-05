using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MagicTower2._0
{
    public class Map
    {
        private string MapPath = "../../map/map.txt";
        private string SourcePath = "../../source/";//souce的相对地址
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
        public int Floor=0;
        public static Enemy[] EnemyInfo = new Enemy[define.EnemyNum];
        public static Item[] ItemInfo=new Item[define.ItemNum];

        User user = new User(); // User类，便于调用User
        int[] directionsX = { 0, 1, 0, -1 }; // 行坐标X的变化
        int[] directionsY = { 1, 0, -1, 0 }; // 列坐标Y的变化
        Block[,] blocks;
        public Map() { }
        public void ReadPainMap()
        {
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
                                //blocks[i, j] = new Enemy();
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
        }
        private void SetEnemy()
        {
            StreamReader fs = new StreamReader("../../source/enemy/怪物信息表2.txt");
            string line;
            int index = 0;
            do
            {
                line = fs.ReadLine();
                if (line == null)
                    break;
                string[] res = Regex.Split(line, "\\|");
                string ImgName = res[1];
                //加入图片集合
                Image tmp = Image.FromFile("../../source/enemy/" + ImgName);
                imagelist.Images.Add("enemy" + index.ToString(), tmp);
                int hp = int.Parse(res[3]);
                int damage = int.Parse(res[4]);
                int armor = int.Parse(res[5]);
                int speed = int.Parse(res[6]);
                int Exp = int.Parse(res[7]);
                
                ///写入掉落矩阵
                string[] drops = Regex.Split(res[8], "\\+");
                for (int i = 0; i < define.ItemNum; i++)
                    Enemy.drop[index, i] = 0;
                foreach (string drop in drops)
                {
                    string[] num = Regex.Split(drop, "\\*");
                    int itemIdex = int.Parse(num[0]);
                    string[] rate = Regex.Split(num[1], "/");
                    float Rate;
                    if (rate.Length == 2)
                        Rate = int.Parse(rate[0]) / int.Parse(rate[1]);
                    else
                        Rate = 1;
                    Enemy.drop[index, itemIdex] = Rate;
                }
                ///
                Attribute attri;
                int attriteNum = -1;
                if (res[9] == "None")
                    attri = Attribute.None;
                else
                {
                    Match mc=Regex.Match(res[9], @"[0-9]+");
                    //Console.WriteLine(res[9][mc.Groups[0].Value.Length]);
                    //Console.WriteLine("冰");
                    if (res[9][mc.Groups[0].Value.Length] == '冰')
                    {
                        attri = Attribute.ice;
                    }
                    else
                    {
                        attri = Attribute.Fire;
                    }
                    attriteNum = int.Parse(mc.Groups[0].Value);

                }
                EnemyInfo[index] = new Enemy(index, hp, damage, armor, speed, Exp, attri, attriteNum);
                index++;
            } while (line != null);

        }
        private void SetItem()
        {
            StreamReader fs = new StreamReader("../../source/items/物品信息表.txt");
            string line;
            int index = 0;
            for(int i = 0; i < define.ItemNum; i++)
            {
                for(int j = 0; j < define.ItemNum; j++)
                {
                    Item.formulate[i, j] = 0;
                }
            }
            Item.formulate[6, 5] = 10;
            Item.formulate[6, 20] = 2;
            Item.formulate[6, 21] = 10; 
            Item.formulate[6, 0] = 5;
            Item.formulate[7, 5] = 25;
            Item.formulate[7, 2] = 2;
            Item.formulate[7, 23] = 15;
            Item.formulate[7, 0] = 10;
            Item.formulate[24, 5] = 50;
            do
            {
                line = fs.ReadLine();
                if (line == null)
                    break;
                string[] res = Regex.Split(line, "\\|");
                string ImgName = res[1];
                //加入图片集合
                Image tmp = Image.FromFile("../../source/items/" + ImgName);
                imagelist.Images.Add("item" + index.ToString(), tmp);
                string[] intensify = Regex.Split(res[3], ",");
                int[] Intensify=new int[intensify.Length];
                if(intensify.Length==5)
                    for (int i = 0; i < Intensify.Length; i++)
                      Intensify[i]=int.Parse(intensify[i]);
                int restore =0;
                if (res[4] != "/")
                    restore = int.Parse(res[4]);

                index++;
            } while (line != null);

        }
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
            SetEnemy();
            SetItem();
            Image wall = Image.FromFile(SourcePath + "tiles/wall0.png");
            imagelist.Images.Add("wall", wall);
            //Image enemy = Image.FromFile(SourcePath + "enemy1.png");
            //imagelist.Images.Add("enemy", enemy);
            //Image key = Image.FromFile(SourcePath + "key.png");
            //imagelist.Images.Add("key", key);
            Image door = Image.FromFile(SourcePath + "tiles/door0.png");
            imagelist.Images.Add("door", door);
            //Image empty = Image.FromFile(SourcePath + "empty.png");
            //imagelist.Images.Add("empty", empty);
            Image hero = Image.FromFile(SourcePath + "hero.png");
            imagelist.Images.Add("hero", hero);
            ///读取map,并更新block
            ReadPainMap();
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
            message.Location = new Point(5 + width * blockWidth, /*mapLeftTop_Y*/  110 + 40 * 4);
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
                        pictureMap[j, i] = new PictureBox();
                        pictureMap[j, i].Location = new Point(/*mapLeftTop_X+*/j * blockWidth,/*mapLeftTop_Y+*/i * blockHeight);
                        pictureMap[j, i].Height = blockHeight;
                        pictureMap[j, i].Width = blockWidth;
                        switch (map[i, j])
                        {
                            case 0: //wall
                                {
                                    pictureMap[j, i].Image = imagelist.Images["wall"];
                                    break;
                                }
                            case 1: //enemy
                                {
                                    pictureMap[j, i].Image = imagelist.Images["enemy"];
                                    break;
                                }
                            case 2: //key
                                {
                                    pictureMap[j, i].Image = imagelist.Images["key"];
                                    break;
                                }
                            case 3: //door
                                {
                                    pictureMap[j, i].Image = imagelist.Images["door"];
                                    break;
                                }
                            case 4: //empty
                                {
                                    pictureMap[j, i].Image = imagelist.Images["empty"];
                                    break;
                                }
                            case 5://hero
                                {
                                    pictureMap[j, i].Image = imagelist.Images["hero"];
                                    user.X = i;
                                    user.Y = j;
                                    break;
                                }
                        }
                        output.Controls.Add(pictureMap[j, i]);
                        //pictureMap[j,i].BringToFront();
                        pictureMap[j, i].BackColor = Color.Transparent;
                        //Console.WriteLine("test");
                    }
                }
                PictureBox tmp = new PictureBox();
                tmp.BackColor = Color.Purple;
                tmp.Location = new Point(width * blockWidth, 0);
                tmp.Width = outputWidth;
                tmp.Height = height * blockHeight;
                output.Controls.Add(tmp);

                //for(int i=0;i<)

            }
            catch (Exception e)
            {
                Console.WriteLine("error0001:" + e.ToString());
            }
        }

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
                message.Text = ("can not move.\ntry to detect it(press J).");
                return;
            }
            //如果是上楼,更新地图
            if (block.Type == BlockType.Stair)
            {
                ReadPainMap();
            }
            // 3. 如果发生移动，更新当前地图，目标地点变为主角，出发地点变为空
            map[user.X, user.Y] = 4;
            map[endX, endY] = 5;
            blocks[user.X, user.Y] = new Empty();
            //打印消息
            string answer = user.Deal(block);
            message.Text = (answer);
            pictureMap[endY, endX].Image = imagelist.Images["hero"];
            pictureMap[user.Y, user.X].Image = imagelist.Images["empty"];
            user.X = endX;
            user.Y = endY;
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
}
