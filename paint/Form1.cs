using System.Drawing.Imaging;
using System.Security.Cryptography.Xml;

namespace paint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Width = 900;
            this.Height = 700;
            bm = new Bitmap(PictureBox.Width, PictureBox.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            PictureBox.Image = bm;
        }

        Bitmap bm;
        Graphics g;
        bool paint = false;
        Pen p = new Pen(Color.Black, 2);
        Pen erase = new Pen(Color.White, 10);
        int index;
        Point pX, pY;
        int x, y, sX, sY, cX, cY;    

        ColorDialog cd = new ColorDialog();
        Color new_color;

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            pY = e.Location;

            cX = e.X;
            cY = e.Y;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                if(index == 1)
                {
                    pX = e.Location;
                    g.DrawLine(p, pX, pY);
                    pY = pX;
                }

                if (index == 2)
                {
                    pX = e.Location;
                    g.DrawLine(erase, pX, pY);
                    pY = pX;
                }
            }
            PictureBox.Refresh();

            x = e.X;
            y = e.Y;
            sX = e.X - cX;
            sY = e.Y - cY;
        }  

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY = y - cY;

            if(index == 3)
            {
                g.DrawEllipse(p,cX,cY,sX,sY);
            }

            if (index == 4)
            {
                g.DrawRectangle(p,cX,cY,sX,sY);
            }

            if (index == 5)
            {
                g.DrawLine(p,cX,cY,x,y);
            }
        }   

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }      

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }

        private void btn_ellipse_Click(object sender, EventArgs e)
        {
            index = 3;
        }      

        private void btn_rectangle_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btn_line_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }

                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }

                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            PictureBox.Image = bm;
            index = 0;

        }

        private void btn_color_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            new_color = cd.Color;
            picture_color.BackColor = cd.Color;
            p.Color = cd.Color;             
        }
       
        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }

        private void choose_color_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(choose_color, e.Location);
            picture_color.BackColor = ((Bitmap)choose_color.Image).GetPixel(point.X, point.Y);
            new_color = picture_color.BackColor;
            p.Color = picture_color.BackColor;
        }

        private void validate(Bitmap bm, Stack<Point>sp, int x, int y, Color old_color, Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if(cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }

        public void Fill(Bitmap bm, int x, int y, Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);
            if (old_color == new_clr) return;

            while(pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if(pt.X > 0 && pt.Y > 0 && pt.X < bm.Width-1 && pt.Y < bm.Height - 1)
                {
                    validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);
                }
            }
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if(index == 7)
            {
                Point point = set_point(PictureBox, e.Location);
                Fill(bm, point.X, point.Y, new_color);
            }
        }

        private void btn_fill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            var save = new SaveFileDialog();
            save.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*";
            if(save.ShowDialog() == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0,0,PictureBox.Width,PictureBox.Height), bm.PixelFormat);
                btm.Save(save.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Файл сохранен");
            }
        }
    }
}