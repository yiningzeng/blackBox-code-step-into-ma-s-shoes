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
using System.Threading;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace newdemoall
{
    public partial class Checkpicture : Form
    {
        string picpath;
        bool cancheck = false;
        bool needcheck = false;
        BackgroundWorker backgroundWorker = null;
        //bbox_t_container boxlist = new bbox_t_container();
        bool card;
        string firstname;
        Bitmap newbitmap;
        showpic showpic;
        decimal typenum;// 灵敏度
        public Checkpicture(string path, string picfirstname, bool thiscard,decimal num)
        {
            InitializeComponent();
            card = thiscard;
            firstname = picfirstname;
            typenum = num;
            //获取状态初始化
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += new DoWorkEventHandler(getstauts);
            if (thiscard)
            {
                picpath = path;
                getpic(picfirstname + "*.jpg");
                button1.Hide();
                cancheck = true;
                needcheck = true;
                if (backgroundWorker.IsBusy != true)//判断BackgroundWorker 是否正在运行异步操作。
                {
                    backgroundWorker.RunWorkerAsync("object argument");//启动异步操作，有两种重载。将触发BackgroundWorker.DoWork事件
                }
            }
            else
            {
                picpath = path;
                getpic("*.jpg");
            }
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) , (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);


        }
        public void getpic(string picname)
        {
            DirectoryInfo root = new DirectoryInfo(picpath);
            dataGridView1.Rows.Clear();

            int i = 0;
            foreach (FileInfo f in root.GetFiles(picname))
            {
                dataGridView1.Rows.Add();
                dataGridView1["Column1", i].Value = f.Name;
                dataGridView1["Column2", i].Value = f.FullName;
                //string name = f.Name;
                //string fullName = f.FullName;
                i++;
            }
            if (i == 0)
            {
                button1.Enabled = false;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            if (showpic == null || showpic.IsDisposed)
            {
                showpic = new showpic(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString());
                showpic.Show();
            }
            else
            {
                showpic.Close();
                showpic = new showpic(dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString());
                showpic.Show();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "开始")
            {
                button1.Text = "暂停";
                cancheck = true;
                needcheck = true;
                if (backgroundWorker.IsBusy != true)//判断BackgroundWorker 是否正在运行异步操作。
                {
                    backgroundWorker.RunWorkerAsync("object argument");//启动异步操作，有两种重载。将触发BackgroundWorker.DoWork事件
                }
            }
            else
            {
                button1.Text = "开始";
                cancheck = false;
                needcheck = false;

            }
            //MessageBox.Show(dataGridView1.RowCount.ToString());
        }
        private void getstauts(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                while (cancheck)
                {

                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (needcheck)
                        {
                            if (dataGridView1["Column3", i].Value == null)
                            {
                                Bitmap bitmap = new Bitmap(dataGridView1["Column2", i].Value.ToString());
                                newbitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                                Graphics draw = Graphics.FromImage(newbitmap);
                                draw.DrawImage(bitmap, 0, 0);
                                draw.Dispose();
                                bitmap.Dispose();//释放bmp文件资源
                                //bool ng = true;
                                bool ng = savepic(newbitmap, dataGridView1["Column2", i].Value.ToString());
                                if (ng)
                                {
                                    dataGridView1["Column3", i].Value = "NG";
                                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                    dataGridView1["Column2", i].Value = picpath + "\\NG\\" + dataGridView1["Column1", i].Value.ToString();
                                    bitmap.Dispose();
                                    movepic(dataGridView1["Column1", i].Value.ToString(), picpath, true);
                                }
                                else
                                {
                                    dataGridView1["Column3", i].Value = "OK";
                                    dataGridView1["Column2", i].Value = picpath + "\\OK\\" + dataGridView1["Column1", i].Value.ToString();
                                    movepic(dataGridView1["Column1", i].Value.ToString(), picpath, false);
                                }
                                Thread.Sleep(20);
                            }
                            else
                            {

                                continue;
                            }
                        }
                        else
                        {
                            return;

                        }



                    }
                    cancheck = false;
                    write(picpath, card);
                    button1.Text = "开始";

                }
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Loghelper.WriteLog("移动图片失败", ex);
            }
        }

        private void movepic(string picname, string path, bool isng)
        {
            if (isng)
            {
                if (Directory.Exists(path + "\\NG"))
                {
                    if (File.Exists(path + "\\" + picname))
                    {

                        File.Move(path + "\\" + picname, path + "\\NG\\" + picname);
                    }


                }
                else
                {
                    Directory.CreateDirectory(path + "\\NG");
                    if (File.Exists(path + "\\" + picname))
                    {

                        File.Move(path + "\\" + picname, path + "\\NG\\" + picname);
                    }

                }

            }
            else
            {
                if (Directory.Exists(path + "\\OK"))
                {
                    if (File.Exists(path + "\\" + picname))
                    {

                        File.Move(path + "\\" + picname, path + "\\OK\\" + picname);
                    }


                }
                else
                {
                    Directory.CreateDirectory(path + "\\OK");
                    if (File.Exists(path + "\\" + picname))
                    {

                        File.Move(path + "\\" + picname, path + "\\OK\\" + picname);
                    }

                }

            }

        }

        private void write(string path, bool type)
        {
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ISheet sheet = hssfworkbook.CreateSheet("sheet1");
                IRow dataRow = sheet.CreateRow(0);
                ICell newcell = dataRow.CreateCell(0);
                newcell.SetCellValue("图片名");
                newcell = dataRow.CreateCell(1);
                newcell.SetCellValue("图片地址");
                newcell = dataRow.CreateCell(2);
                newcell.SetCellValue("结果");
                newcell = dataRow.CreateCell(3);


                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataRow = sheet.CreateRow(i + 1);
                    //string[] line = txtLines[i].Split('\t');
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        ICell cell = dataRow.CreateCell(j);
                        cell.SetCellValue(dataGridView1[j, i].Value.ToString());
                    }
                    Console.Write(i);
                }
                string fullname;
                if (type)
                {
                    fullname = path + "\\单板" + firstname + DateTimeUtil.DateTimeToLongTimeStamp(DateTime.Now).ToString() + ".xls";

                }
                else
                {
                    fullname = path + "\\文件夹" + DateTimeUtil.DateTimeToLongTimeStamp(DateTime.Now).ToString() + ".xls";

                }
                using (FileStream fs = File.OpenWrite(fullname)) //打开一个xls文件，如果没有则自行								创建，如果存在myxls.xls文件则在创建是不要打开该文件！
                {
                    hssfworkbook.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
        }


        //bitmap转byte
        public static byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }
        //检测错误保存文件
        public bool savepic(Bitmap bitmap, String path)
        {

            bbox_t_container boxlist = new bbox_t_container();
            bool ng;
            byte[] byteImg = Bitmap2Byte(bitmap);
            long intime = DateTimeUtil.DateTimeToLongTimeStamp(DateTime.Now);
            int n = AITestSDK.detect_opencv_mat(byteImg, byteImg.Length, ref boxlist,(float)typenum);
            long endtime = DateTimeUtil.DateTimeToLongTimeStamp(DateTime.Now);
            long gettime = endtime - intime;
            Console.WriteLine(gettime);
            int j =0;

            if (n == -1)
            {
                Loghelper.WriteLog("调用失败，请检测目录是否包含opencv的dll");
            }

                for (int i = 0; i < boxlist.bboxlist.Length; i++)
                {
                    if (boxlist.bboxlist[i].h == 0)
                    {
                        break;
                    }
                    else {
                        DrawRectangleInPicture(bitmap, (int)boxlist.bboxlist[i].x,
                        (int)boxlist.bboxlist[i].y,
                        (int)boxlist.bboxlist[i].w,
                        (int)boxlist.bboxlist[i].h, Color.Red, 2, DashStyle.Solid);
                        j++;
                    }

                }
            if (j > 0)
            {
                ng = true;
            }
            else
            {
                ng = false;
            }
            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ng;

        }
        public static Bitmap DrawRectangleInPicture(Bitmap bmp, int x, int y, int width, int height, Color RectColor, int LineWidth, DashStyle ds)
        {
            if (bmp == null) return null;


            Graphics g = Graphics.FromImage(bmp);

            Brush brush = new SolidBrush(RectColor);
            Pen pen = new Pen(brush, LineWidth);
            pen.DashStyle = ds;

            g.DrawRectangle(pen, new Rectangle(x, y, width, height));

            g.Dispose();

            return bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Checkpicture_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (showpic != null || !showpic.IsDisposed)
                {
                    showpic.Close();
                }
            }
            catch (Exception ex) {
                Loghelper.WriteLog("失败",ex);
            }

        }
    }
}
