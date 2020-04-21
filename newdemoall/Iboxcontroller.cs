using Mobot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestDevice;
using ImageControl;
using WeifenLuo.WinFormsUI.Docking;
using System.Threading;
using newdemoall.Properties;

namespace newdemoall
{
    public partial class Iboxcontroller : DockContent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Point Firstpoint { get => firstpoint; set => firstpoint = value; }
        public Point Endpoint { get => endpoint; set => endpoint = value; }
        public string Xyname { get => xyname; set => xyname = value; }
        public string Xmove { get => xmove; set => xmove = value; }
        public string Ymove { get => ymove; set => ymove = value; }

        public ITestBox TestBox = null;
        public ICamera camera = null;
        int movesize = 1;
        private Point firstpoint;
        private Point endpoint;
        private string xyname;
        private string xmove;
        private string ymove;
        private int runint = 0;
        Thread thread;

        public Iboxcontroller(ITestBox testBox,ICamera camera)
        {
            this.TestBox = testBox;
            this.camera = camera;
            InitializeComponent();
            //radioButton1.Checked = true;
            X = 0;
            Y = 0;
            RefreshXYZ();
            textBox2.Enabled = false;
            getvalue();
            xmove = Settings.Default.xmove;
            ymove = Settings.Default.ymove;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            X -= movesize;
            X = GetValue(X,TestBox.ControllerSettings.RangeX);
            this.TestBox.StylusMoveXY(X, Y);
            RefreshXYZ();
        }

        private void RefreshXYZ()
        {
            //label1.Text = "X=" + X.ToString();
            //label2.Text = "Y=" + Y.ToString();            
        }

        private void button14_Click(object sender, EventArgs e)
        {
            this.TestBox.StylusReset();
            X = 0;
            Y = 0;
            RefreshXYZ();
        }

        private int GetValue(int value, RangeD range)
        {
            double min = range.Minimum;
            double max = range.Maximum;
            if (value > max)
                value = (int)max;
            if (value < min)
                value = (int)min;
            return value;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            X += movesize;
            X = GetValue(X, TestBox.ControllerSettings.RangeX);
            this.TestBox.StylusMoveXY(X, Y);
            RefreshXYZ();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Y -=movesize;
            Y = GetValue(Y ,TestBox.ControllerSettings.RangeY);
            this.TestBox.StylusMoveXY(X, Y);
            RefreshXYZ();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Y +=movesize;
            Y = GetValue(Y, TestBox.ControllerSettings.RangeY);
            this.TestBox.StylusMoveXY(X, Y);
            RefreshXYZ();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            firstpoint = new Point(X,Y);
            label6.Text = "x=" + firstpoint.X;
            label7.Text = "y=" + firstpoint.Y;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            endpoint = new Point(X, Y);
            label9.Text = "x=" + endpoint.X;
            label8.Text = "y=" + endpoint.Y;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            xyname = textBox1.Text;
            if (xyname == "")
            {
                MessageBox.Show("命名不能为空");
            } else if (firstpoint == null || endpoint == null) {
                MessageBox.Show("请设置正确起点和终点！");

            }
            else {
                xmove = Settings.Default.xmove;
                ymove = Settings.Default.ymove;
                this.DialogResult = DialogResult.OK;
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (keyCode == Keys.Up || keyCode == Keys.Down || keyCode == Keys.Left || keyCode == Keys.Right
                || keyCode == Keys.Oemplus || keyCode == Keys.OemMinus)
            {
                switch (keyCode)
                {
                    case Keys.Left: button9.PerformClick(); break;
                    case Keys.Right: button11.PerformClick(); break;
                    case Keys.Down: button13.PerformClick(); break;
                    case Keys.Up: button10.PerformClick(); break;
                    default:
                        break;
                }
                return true;

            }

            return base.ProcessCmdKey(ref msg, keyData); ;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (runint > 0) {
                runint -= 1;
            }
            getvalue();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (runint <2)
            {
                runint += 1;
            }
            getvalue();

        }
        private void getvalue() {

            switch  (runint){
                case 0:
                    textBox2.Text = "低速";
                    movesize = 1;
                    break;
                case 1:
                    textBox2.Text = "中速";
                    movesize = 10;
                    break;
                case 2:
                    textBox2.Text = "高速";
                    movesize = 100;
                    break;


            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (button5.Text == "测试")
                {
                    if (xyname == "")
                    {
                        MessageBox.Show("命名不能为空");
                        return;
                    }
                    else if (firstpoint == null || endpoint == null)
                    {
                        MessageBox.Show("请设置正确起点和终点！");
                        return;

                    }
                    button5.Text = "暂停";
                    Settings.Default.canmove = true;
                    XYrulescs xYrulescs = new XYrulescs();
                    xYrulescs.Name = textBox1.Text;
                    xYrulescs.Leftup = firstpoint;
                    xYrulescs.Rightdown = endpoint;
                    xYrulescs.Xmove = Int32.Parse(xmove);
                    xYrulescs.Ymove = Int32.Parse(ymove);
                    Newdevice newdevice = new Newdevice
                    {
                        XYrulescs = xYrulescs,
                        TestBox = this.TestBox,
                        Camera = this.camera
                    };
                    thread = new Thread(newdevice.run);
                    thread.Start();
                    //newdevice.xymove();

                }
                else
                {
                    button5.Text = "测试";
                    Settings.Default.canmove = false;
                }
            }
            catch (Exception ex) {

                Loghelper.WriteLog("新增界面测试失败",ex);
            }

        }
    }
}
