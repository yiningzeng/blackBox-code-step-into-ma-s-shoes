using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace newdemoall
{
    public partial class showpic : Form
    {
        bool tobig = false;
        bool isMove = false;
        Point mouseDownPoint = new Point(); //记录拖拽过程鼠标位置
        public showpic(string picname,string fullname)
        {
            InitializeComponent();
            this.Text = picname;
            pic(fullname);
            //指定窗口初始化时的位置,如果为Manual，位置由Location决定(计算机屏幕中间，如果不/2，则计算机右下角)
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 12, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
        }
        public void pic(string fullname) {
            pictureBox1.Load(fullname);


        }

        private void showpic_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!tobig)
            {
                int x;
                int y;
                x =e.X-(e.X -this.pictureBox1.Location.X) *3;
                y = e.Y - (e.Y-this.pictureBox1.Location.Y) * 3;


                this.pictureBox1.Location = new Point(x,y) ;
                this.pictureBox1.Width =this.pictureBox1.Width *3 ;

                this.pictureBox1.Height = this.pictureBox1.Height *3;                
                tobig = true;
            }
            else {

                this.pictureBox1.Location = new Point(12, 12);
                this.pictureBox1.Width =486;

                this.pictureBox1.Height = 486;
                tobig = false;

            }

        }

        //鼠标按下功能
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isMove = true;
                pictureBox1.Focus();
            }
        }
        //鼠标松开功能
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }
        //鼠标移动功能
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (isMove && tobig)
            {
                int x, y;
                int moveX, moveY;
                moveX = Cursor.Position.X - mouseDownPoint.X;
                moveY = Cursor.Position.Y - mouseDownPoint.Y;
                x = pictureBox1.Location.X + moveX;
                y = pictureBox1.Location.Y + moveY;
                pictureBox1.Location = new Point(x, y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }
    }
}
