using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Tetris
{
    public partial class Form2 : Form
    { 
        Shape currentShape;
        int size;
        int[,] map = new int[16, 8];
        int linesRemoved;
        int score;
        int Interval;
        public Form2()
        {
            InitializeComponent();
            this.KeyUp += new KeyEventHandler(keyFunc);
            Init();

        }

        public void Init()
        {
            size = 25;
            score = 0;
            linesRemoved = 0;
            currentShape = new Shape(3, 0);
            Interval = 400;
            label1.Text = "Score: " + score;
            label2.Text = "Lines: " + linesRemoved;

            timer1.Interval =Interval;
            timer1.Tick += new EventHandler(update);
            timer1.Start();

            Invalidate();
           
        }

        private void keyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.R:
                    if (!intersec())
                    {
                        ResetArea();
                        currentShape.rotateShape();
                        Merge();
                        Invalidate();
                    }
                        break;
                    
                case Keys.Space:
                    timer1.Interval = 50;
                    break;
                case Keys.Right:
                    if (!CollideHor(1))
                    {
                        ResetArea();
                        currentShape.MoveRight();
                        Merge();
                        Invalidate();
                      
                    }
                    break;
                case Keys.Left:
               
                    if (!CollideHor(-1))
                    {
                        ResetArea();
                        currentShape.MoveLeft();
                        Merge();
                        Invalidate();
                       
                    }
                    break;

            }
        }

        public void showNext(Graphics e)
        {
            for (int i = 0; i < currentShape.sizeNextMatrix; i++)
            {
                for (int j = 0; j < currentShape.sizeNextMatrix; j++)
                {
                    if (currentShape.nextmatrix[i, j] == 1)
                    {
                        e.FillRectangle(Brushes.Red, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                    if (currentShape.nextmatrix[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                    if (currentShape.nextmatrix[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                    if (currentShape.nextmatrix[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Purple, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                    if (currentShape.nextmatrix[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.LawnGreen, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }

                }
            }
        }

        public void update(object sender, EventArgs e)
        {
            ResetArea(); 
            if (!Collide())
            {
                currentShape.MoveDown();
            }
            else
            {
                
                Merge();
                SliceMap();
                timer1.Interval = Interval;
                currentShape.ResetShape(3,0);
                if (Collide())
                {
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            map[i, j] = 0;
                        }

                    }
                    timer1.Tick -= new EventHandler(update);
                    timer1.Stop();
                    MessageBox.Show("You lost!");
                    Init();
                    
                }
            }

            
            Merge();
            Invalidate();
           
            
        }

        public void SliceMap()
        {
            int count = 0;
            int cutRemovedLines = 0;
            for (int i = 0; i < 16; i++)
            {
                count = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (map[i, j] != 0)
                    {
                        count++;
                    }
                }
                if (count == 8)
                {
                    cutRemovedLines++;
                    for (int k = i; k >= 1; k--)
                    {
                        for (int o = 0; o <8 ; o++)
                        {
                            map[k, o] = map[k-1, o];
                        }
                    }
                }
            }
            for (int i = 0; i < cutRemovedLines; i++)
            {
                score += 10 * (i+1);
            }
            linesRemoved += cutRemovedLines;

            if (linesRemoved % 5 == 0)
            {
                if(Interval>60)
                Interval -= 10;
            }

            label1.Text = "Score: " + score;
            label2.Text = "Lines: " + linesRemoved;
        }

        public bool intersec()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (j >= 0 && j <= 7)
                    {
                        if (map[i, j] != 0 && currentShape.matrix[i - currentShape.y, j - currentShape.x] == 0)
                            return true;
                    }
                }

            }
            return false;
        }

        public void Merge()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                map[i, j] = currentShape.matrix[i - currentShape.y, j - currentShape.x]; 
                }

            }
        }

        public bool Collide()
        {
            for (int i = currentShape.y + currentShape.sizeMatrix - 1; i >=currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        
                        if (i + 1 == 16)
                            return true;
                        if (map[i + 1, j] != 0)
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        public bool CollideHor(int dir)
        {
            for (int i = currentShape.y; i < currentShape.y +  currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir < 0)
                            return true;
                        if (map[i, j + 1 * dir] != 0)
                        {
                            if (j - currentShape.x + 1 * dir >= currentShape.sizeMatrix || j - currentShape.x + 1 * dir < 0) {

                                return true;
                            }
                            if (currentShape.matrix[i - currentShape.y, j - currentShape.x + 1 * dir] == 0)
                            {
                                return true;
                            }
                        }
                       
                    }
                }

            }
            return false;
        }

        public void ResetArea()
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (i >= 0 && j >= 0 && i < 16 && j < 8)
                    {
                        if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        {
                            map[i, j] = 0;
                        }
                    }
                }

            }
        }


        public void DrawMap(Graphics e)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(map[i,j] == 1) {
                        e.FillRectangle(Brushes.Red, new Rectangle(50 + j * (size)+1, 50 + i * (size)+1, size-1, size-1));
                      
                    }
                    if (map[i, j] == 2)
                    {
                        e.FillRectangle(Brushes.Yellow, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                    if (map[i, j] == 3)
                    {
                        e.FillRectangle(Brushes.Blue, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                    if (map[i, j] == 4)
                    {
                        e.FillRectangle(Brushes.Purple, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                    if (map[i, j] == 5)
                    {
                        e.FillRectangle(Brushes.LawnGreen, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1));

                    }
                }
            }
        }

        public void DrawGrid(Graphics g)
        {




            for (int i = 0; i <= 16; i++)
             {


                  g.DrawLine(Pens.Green, new Point(50, 50 + i * size), new Point(50 + 8 * size, 50 + i * size));


             }
             for (int i = 0; i <= 8; i++)
             {

                 g.DrawLine(Pens.Black, new Point(50 + i *size , 50), new Point(50 + i * size, 50 + 16 * size));


             }


        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawMap(e.Graphics);
            showNext(e.Graphics);
        }

       
    }
}
