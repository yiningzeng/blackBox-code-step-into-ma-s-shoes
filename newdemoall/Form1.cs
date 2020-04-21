using Driver;
using HaiKang;
using Motor;
using MvCamCtrl.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using TestDevice;
using ImageControl;
using newdemoall.Properties;
using System.IO;
using newdemoall.Tools;
using System.Drawing.Imaging;
using newdemoall.Model;
using Newtonsoft.Json;

namespace newdemoall
{
    public partial class Form1 : Form
    {
        public delegate int StitchCallBack(bool isEnd, OneStitchSidePcb oneStitchSidePcb, OneStitchSidePcb.BitmapInfo bitmapInfo, RectangleF scaleRect);
        public StitchCallBack onStitchCallBack;

        private bool ismoveative;
        private bool iscangetstatus = true;
        private Object obj = new object();
        CameraDevice_HK cameraDevice = new CameraDevice_HK();
        XYrulescs xYrulescs;
        BackgroundWorker backgroundWorker = null;
        BackgroundWorker newbackgroundWorker = null;
        public ICamera Camera;
        private Bitmap bitmap;
        private bool needsave = false;
        private string path;
        private bool thiscard = false;
        public Queue<Bitmap> bitmaps = new Queue<Bitmap>();
        private int fuckDetectNum = 0;
        private int fuckSaveNum = 0;
        private object detectKey = new object();
        Pcb nowPcb;
        private int allDetectNum = 0;
        private bool isBabyYiNing = false; //不跑小马的代码
        /// <summary>
        /// 拼图回调函数，用于执行界面更新
        /// </summary>
        /// <param name="end"></param>
        /// <param name="isFront"></param>
        /// <param name="bitmap"></param>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public int doStitchCallBack(bool isEnd, OneStitchSidePcb oneStitchSidePcb, OneStitchSidePcb.BitmapInfo bitmapInfo, RectangleF scaleRect)
        {
            Console.WriteLine("doStitchCallBack");
            //bool end, bool isFront, Bitmap bitmap, RectangleF rect
            //if (InvokeRequired)
            //{
            //    // If called from a different thread, we must use the Invoke method to marshal the call to the proper GUI thread.
            //    // The grab result will be disposed after the event call. Clone the event arguments for marshaling to the GUI thread.
            //    BeginInvoke(new StitchCallBack(doStitchCallBack), isEnd, oneStitchSidePcb, bitmapInfo, scaleRect);
            //    return 0;
            //}
            Rectangle scaleR = new Rectangle(Convert.ToInt32(scaleRect.X),
                        Convert.ToInt32(scaleRect.Y),
                        Convert.ToInt32(scaleRect.Width),
                        Convert.ToInt32(scaleRect.Height));

                    aiDetect(oneStitchSidePcb.zTrajectory,
                        bitmapInfo,
                        scaleR,
                        oneStitchSidePcb.scale,
                        oneStitchSidePcb.confidence,
                        oneStitchSidePcb.savePath);

            return 1;
        }
        /// <summary>
        /// AI大图检测
        /// </summary>
        /// <param name="isFront">是否是正面</param>
        /// <param name="bitmapInfo">图片信息</param>
        /// <param name="scaleRect">已经缩放的矩形框在缩放大图的位置</param>
        /// <param name="scale">缩放的尺度</param>
        /// <param name="confidence">置信度</param>
        /// <param name="savePath">图像保存地址</param>
        public void aiDetect(bool isFront, OneStitchSidePcb.BitmapInfo bitmapInfo, Rectangle scaleRect, double scale, float confidence, string savePath)
        {
            //Console.WriteLine(allDetectNum);
            MySmartThreadPool.InstanceSmall().QueueWorkItem((name, bmp) =>
            {
                try
                {
                    bbox_t_container boxlist = new bbox_t_container();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bmp.Save(stream, ImageFormat.Jpeg);
                        byte[] byteImg = new byte[stream.Length];
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.Read(byteImg, 0, Convert.ToInt32(stream.Length));
                        int n = -1;
                        lock (detectKey)
                        {
                            n = AITestSDK.detect_opencv_mat(byteImg, byteImg.Length, ref boxlist, confidence);
                        }
                        if (n == -1) Console.WriteLine("AI调用失败");
                        else
                        {
                            resultJoin(name, isFront, scale, scaleRect, boxlist, new List<string>() { "NG"}, new Point(0, 0));
                        }
                    }
                }
                catch (Exception er) {  }
                finally
                {
                    bmp.Dispose();
                    allDetectNum++;
                    aiDone(savePath);
                }
                //最总检测的结果还是放在这里发送吧

            }, bitmapInfo.name, (Bitmap)bitmapInfo.bitmap.Clone());
        }
        /// <summary>
        /// Json格式化输出
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }
        /// <summary>
        /// AI检测结束共用函数
        /// </summary>
        /// <param name="oneStitchSidePcb"></param>
        public void aiDone(string savePath)
        {
            Console.WriteLine(allDetectNum);
            if (allDetectNum == nowPcb.AllPhotoNum)
            {
                this.BeginInvoke((Action)(() =>
                {
                    fuckDetectNum = 0;
                    try
                    {
                        MySmartThreadPool.InstanceSmall().WaitForIdle();
                        Console.WriteLine("InstanceSmall: " + MySmartThreadPool.InstanceSmall().InUseThreads);
                        Console.WriteLine("Instance: " + MySmartThreadPool.Instance().InUseThreads);
                        //这里可以直接发送了！！！！！！
                        //结束计时  
                        //MessageBox.Show("执行查询总共使用了, total :" + times + "s 秒");
                        try
                        {
                            JsonData<Pcb> jsonData = new JsonData<Pcb>();
                            jsonData.data = nowPcb;
                            jsonData.executionTime = 11;
                            jsonData.ngNum = nowPcb.results.Count;
                            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                            string[] props = { "FrontPcb", "BackPcb" }; //排除掉，不能让前端看到的字段。为true的话就是只保留这些字段
                            jSetting.ContractResolver = new LimitPropsContractResolver(props, false);

                            string res = JsonConvert.SerializeObject(jsonData, jSetting);
                            RabbitMQClientHandler.GetInstance()
                            .TopicExchangePublishMessageToServerAndWaitConfirm("", "work", "work", res);

                            File.WriteAllText(Path.Combine(savePath, "result.json"), ConvertJsonString(res));
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show(" 发送队列出错" + er.Message);
                            //LogHelper.WriteLog("连接队列失败!!!", er);
                        }
                    }
                    catch (Exception er)
                    {

                    }
                    finally
                    {
                        allDetectNum = 0;
                    }
                }));
            }
        }


        public void resultJoin(string imgName, bool isFront, double scale, Rectangle scaleRect, bbox_t_container boxlist, List<string> names, Point startPoint)
        {
            Snowflake snowflake = new Snowflake(2);
            if (boxlist.bboxlist.Length > 0)
            {
                for (int i = 0; i < boxlist.bboxlist.Length; i++)
                {
                    if (boxlist.bboxlist[i].h == 0) break;
                    else
                    {
                        string id = imgName.Replace(".jpg", "") + "(" + snowflake.nextId().ToString() + ")";
                        bbox_t bbox = boxlist.bboxlist[i];
                        bbox.x = (uint)((bbox.x + startPoint.X) * scale) + (uint)scaleRect.X; // + (uint)scaleRect.X;
                        bbox.y = (uint)((bbox.y + startPoint.Y) * scale) + (uint)scaleRect.Y; // + (uint)scaleRect.Y;
                        bbox.w = (uint)(bbox.w * scale);
                        bbox.h = (uint)(bbox.h * scale);
                        Result result = new Result()
                        {
                            Id = id,
                            IsBack = Convert.ToInt32(!isFront),
                            score = bbox.prob,
                            PcbId = nowPcb.Id,
                            Area = "",
                            Region = bbox.x + "," + bbox.y + "," + bbox.w + "," + bbox.h,
                            NgType = names[(int)bbox.obj_id],
                            PartImagePath = id + ".jpg",
                            CreateTime = DateTime.Now,
                        };
                        lock (nowPcb.results)
                        {
                            nowPcb.results.Add(result);
                        }
                    }
                }
            }
        }


        public Form1()
        {
            InitializeComponent();
            #region 初始化委托
            onStitchCallBack = new StitchCallBack(doStitchCallBack);
            #endregion
            comboxgetrules();
            Camera = cameraDevice;
            tbSavePath.ForeColor = ColorTranslator.FromHtml("#999999");
            Camera.CameraName = Settings.Default.cameraname;
            //获取状态初始化
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += new DoWorkEventHandler(getstauts);
            Loghelper.WriteLog("初始化获取箱体按钮状态成功");
            //存照片线程初始化
            newbackgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            newbackgroundWorker.DoWork += new DoWorkEventHandler(picevent);
            Loghelper.WriteLog("初始化保存图片线程成功");
            Camera.ScreenEvent += ScreenEvent;
            //button10.Enabled = false;
            button5.Enabled = false;
            button11.Enabled = false;
            this.pictureBox1.Load(@"qtsj.jpg");
            sdkin();

        }

        private void conncamera()
        {
            try
            {
                Camera.InitExpourseTime = Settings.Default.InitExpourseTime;
                int number = Camera.GetCameraCount;
                if (Camera.m_hCamera >= 0)
                {
                    Camera.ConnectCamera(0);
                }
            }
            catch (Exception e)
            {

                Loghelper.WriteLog("连接相机失败", e);
            }




        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Camera.CameraClose();
                pictureBox1.Image = null;
            }
            catch (Exception ex)
            {
                Loghelper.WriteLog("连接相机失败", ex);

            }

        }

        private void btnFuckStart_Click(object sender, EventArgs e)
        {
            fuckSaveNum = 0;
            fuckDetectNum = 0;
            nowPcb = Pcb.CreatePcb();

            nowPcb.FrontPcb.allCols = 24;
            nowPcb.FrontPcb.allRows = 10;
            nowPcb.FrontPcb.allNum = 240;
            nowPcb.FrontPcb.or_hl = 0.3;
            nowPcb.FrontPcb.or_hu = 0.4;
            nowPcb.FrontPcb.or_vl = 0.1;
            nowPcb.FrontPcb.or_vu = 0.5;
            nowPcb.FrontPcb.dr_hu = 0.02;
            nowPcb.FrontPcb.dr_vu = 0.02;
            if (fuckType.SelectedValue.ToString() == "请添加新的拍摄方案")
            {

                MessageBox.Show("请去添加页面添加新的拍摄方案");
                return;

            }
            Settings.Default.num = 0;
            conncamera();
            tbSavePath.Enabled = false;
            button6_Click_1();
            button9.Enabled = false;
            btnFuckDetectThisPcb.Enabled = true;
            ismoveative = false;
            this.xYrulescs = fuckType.SelectedValue as XYrulescs;
            Settings.Default.canmove = true;
            if (tbSavePath.Text != "图片保存地址,不填为不保存")
            {
                path = tbSavePath.Text;
                try
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch
                {
                    MessageBox.Show("请输入正确路径");
                    return;

                }

                needsave = true;
                if (newbackgroundWorker.IsBusy != true)//判断BackgroundWorker 是否正在运行异步操作。
                {
                    newbackgroundWorker.RunWorkerAsync("object argument");//启动异步操作，有两种重载。将触发BackgroundWorker.DoWork事件
                }
            }
            if (this.testDevice != null && this.testDevice.IsOpen && this.Camera.IsOpen)
            {
                iscangetstatus = true;
                fuckType.Enabled = false;
                button4.Enabled = false;
                btnFuckStart.Enabled = false;
                button5.Enabled = true;
                button11.Enabled = true;
                if (backgroundWorker.IsBusy != true)//判断BackgroundWorker 是否正在运行异步操作。
                {
                    backgroundWorker.RunWorkerAsync("object argument");//启动异步操作，有两种重载。将触发BackgroundWorker.DoWork事件
                }

            }
            else
            {
                MessageBox.Show("请连接设备");
            }

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
            comboxgetrules();

        }

        private void comboxgetrules()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"xyrule.xml");
            XmlNodeList no = doc.SelectSingleNode("//Rules").ChildNodes;
            ArrayList arrayList = new ArrayList();
            foreach (XmlElement node in no)
            {
                string[] stringarray = null;
                XYrulescs xYrulescs = new XYrulescs();
                xYrulescs.Name = node.GetElementsByTagName("name")[0].InnerText;
                stringarray = node.GetElementsByTagName("leftup")[0].InnerText.Split(',');
                xYrulescs.Leftup = new Point(Int32.Parse(stringarray[0]), Int32.Parse(stringarray[1]));
                stringarray = node.GetElementsByTagName("rightdown")[0].InnerText.Split(',');
                xYrulescs.Rightdown = new Point(Int32.Parse(stringarray[0]), Int32.Parse(stringarray[1]));
                xYrulescs.Xmove = Int32.Parse(node.GetElementsByTagName("xmove")[0].InnerText);
                xYrulescs.Ymove = Int32.Parse(node.GetElementsByTagName("ymove")[0].InnerText);
                arrayList.Add(new DictionaryEntry(node.GetElementsByTagName("name")[0].InnerText, xYrulescs));
            }
            if (arrayList.Count > 0)
            {
                fuckType.DataSource = arrayList;
                fuckType.DisplayMember = "Key";
                fuckType.ValueMember = "Value";
            }
            else
            {
                ArrayList list = new ArrayList();
                list.Add("请添加新的拍摄方案");
                fuckType.DataSource = list;

            }

        }

        private void button6_Click_1()
        {
            try
            {
                if (testDevice == null)
                {
                    testDevice = new TestBoxDriver(Settings.Default.comname);
                    device = testDevice.Driver as MotorDriver;
                    testDevice.IsReset = false;
                    testDevice.DebugMode = false;
                    device.RequestTimeout = 30 * 1000;
                }
                if (testDevice.IsOpen)
                {
                    testDevice.Close();
                }
                else
                {
                    testDevice.LeftUpPoint = new Point(0, 0);
                    testDevice.RigntDownPoint = new Point(0, 0);
                    testDevice.Open();
                }
            }
            catch (Exception e)
            {
                Loghelper.WriteLog("连接移动电机失败", e);

            }

        }

        private bool getevent()
        {
            Newdevice newdevice = new Newdevice
            {
                XYrulescs = this.xYrulescs,
                TestBox = this.testDevice,
                Camera = this.Camera
            };
            return newdevice.xymove();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cameraDevice.IsOpen)
            {
                Camera.CameraClose();
            }
        }
        //backgroudwork
        private void getstauts(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                while (iscangetstatus)
                {
                    if (this.testDevice.StylusStatue() && ismoveative == false)
                    {
                        if (thiscard)
                        {
                            Settings.Default.num--;
                            this.ismoveative = getevent();
                            if (!isBabyYiNing)
                            {
                                Checkpicture checkpicture = new Checkpicture(tbSavePath.Text, Settings.Default.num + xYrulescs.Name, true, numericUpDown1.Value);
                                checkpicture.ShowDialog();
                            }
                            thiscard = false;
                            Settings.Default.num++;
                        }
                        else
                        {
                            this.ismoveative = getevent();
                            Settings.Default.num++;

                        }

                    }
                    Thread.Sleep(200);
                }
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Loghelper.WriteLog("获取箱体按钮状态失败", ex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (button5.Text == "暂停")
                {
                    Settings.Default.canmove = false;
                    //button5.Enabled = false;
                    button5.Text = "继续";
                }
                else if (button5.Text == "继续")
                {
                    Settings.Default.canmove = true;
                    //button5.Enabled = false;
                    button5.Text = "暂停";

                }
            }
            catch (Exception ex)
            {
                Loghelper.WriteLog("暂停失败", ex);

            }


            //button10.Enabled = true;
        }

        private void ScreenEvent(BitmapInfo bitmapInfo)
        {
            lock (obj)
            {
                if (this.InvokeRequired)
                {
                    BeginInvoke(new ImageReadyEventScreen(ScreenEvent), bitmapInfo);
                    return;
                }
                else
                {
                    try
                    {
                 
                        bitmap = bitmapInfo.m_Bitmap;

                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);  //指定不进行翻转的 180 度旋转  （垂直翻转+水平翻转）
                        nowPcb.FrontPcb.bitmaps.Enqueue(new OneStitchSidePcb.BitmapInfo() { bitmap = BitmapScaleHelper.ScaleToSize(bitmap, (float)0.5), name = "/F" + fuckDetectNum + ".jpg" });
                        fuckDetectNum++;

                        //Aoi.StitchMain(nowPcb.FrontPcb, onStitchCallBack);
                        MySmartThreadPool.Instance().QueueWorkItem(() =>
                        {
                            lock (nowPcb.FrontPcb)
                            {
                                Aoi.StitchMain(nowPcb.FrontPcb, onStitchCallBack);
                            }
                        });
                        bitmap.Save(nowPcb.FrontPcb.savePath + "F" + fuckDetectNum + ".jpg");
                        //this.pictureBox1.Image = bitmap;
                        //if (needsave)
                        //{

                        //    bitmaps.Enqueue(bitmap);
                        //}
                    }
                    catch (Exception e)
                    {
                        Loghelper.WriteLog("获取照片失败", e);
                        throw e;

                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Camera.ContinueShot();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            try
            {
                conncamera();
                button6_Click_1();
                needsave = false;
                this.Camera.ContinueShot();
                Iboxcontroller iboxcontroller = new Iboxcontroller(this.testDevice, this.Camera);
                iboxcontroller.ShowDialog();
                if (iboxcontroller.DialogResult == DialogResult.OK)
                {
                    string name = iboxcontroller.Xyname;
                    string xmove = iboxcontroller.Xmove;
                    string ymove = iboxcontroller.Ymove;
                    string firstpont = iboxcontroller.Firstpoint.X.ToString() + "," + iboxcontroller.Firstpoint.Y.ToString();
                    string endponit = iboxcontroller.Endpoint.X.ToString() + "," + iboxcontroller.Endpoint.Y.ToString();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@"xyrule.xml");
                    //XmlNodeList no = doc.SelectSingleNode("//Rules").ChildNodes;
                    XmlNode xmlElementpark = doc.SelectSingleNode("//Rules");
                    XmlElement xmlElementall = doc.CreateElement("rule");
                    XmlElement xmlElement_name = doc.CreateElement("name");
                    xmlElement_name.InnerText = name;
                    xmlElementall.AppendChild(xmlElement_name);
                    XmlElement xmlElement_cardnum = doc.CreateElement("leftup");
                    xmlElement_cardnum.InnerText = firstpont;
                    xmlElementall.AppendChild(xmlElement_cardnum);
                    XmlElement xmlElement_kname = doc.CreateElement("rightdown");
                    xmlElement_kname.InnerText = endponit;
                    xmlElementall.AppendChild(xmlElement_kname);
                    XmlElement xmlElement_khangbie = doc.CreateElement("xmove");
                    xmlElement_khangbie.InnerText = xmove;
                    xmlElementall.AppendChild(xmlElement_khangbie);
                    XmlElement xmlElement_kainame = doc.CreateElement("ymove");
                    xmlElement_kainame.InnerText = ymove;
                    xmlElementall.AppendChild(xmlElement_kainame);
                    xmlElementpark.AppendChild(xmlElementall);
                    doc.Save(@"xyrule.xml");
                }
                comboxgetrules();
                Camera.CameraStop();
                testDevice.StylusReset();
                this.testDevice.Close();
            }
            catch (Exception ex)
            {

                Loghelper.WriteLog("创建运行规则失败", ex);
            }


        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            Settings.Default.canmove = true;
            button5.Enabled = true;
            //button10.Enabled = false;
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            try
            {
                iscangetstatus = false;
                Settings.Default.canmove = false;
                if (backgroundWorker.IsBusy)
                    backgroundWorker.CancelAsync();
                btnFuckStart.Enabled = true;
                //button10.Enabled = false;
                button5.Enabled = false;
                button11.Enabled = false;
                fuckType.Enabled = true;
                button4.Enabled = true;
                button9.Enabled = true;
                tbSavePath.Enabled = true;
                btnFuckDetectThisPcb.Enabled = false;
                Camera.CameraStop();
                Thread.Sleep(1000);
                testDevice.StylusReset();
                testDevice.Close();

            }
            catch (Exception ex)
            {
                Loghelper.WriteLog("停止失败", ex);

            }

        }


        private void picevent(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    if (bitmaps.Count > 0)
                    {
                        Bitmap savebitmap = bitmaps.Dequeue();
                        savebitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        path = tbSavePath.Text + "F" + fuckSaveNum + ".jpg";
                        savebitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                        fuckSaveNum++;



                        //Bitmap fBitmap = new Bitmap(fuckpath + "/F" + i + ".jpg");
                        //fBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);  //指定不进行翻转的 180 度旋转  （垂直翻转+水平翻转）
                        //nowPcb.FrontPcb.bitmaps.Enqueue(new OneStitchSidePcb.BitmapInfo() { bitmap = fBitmap, name = "/F" + i + ".jpg" });


                        ////Aoi.StitchMain(nowPcb.FrontPcb, onStitchCallBack);
                        //MySmartThreadPool.Instance().QueueWorkItem(() =>
                        //{
                        //    lock (nowPcb.FrontPcb)
                        //    {
                        //        Aoi.StitchMain(nowPcb.FrontPcb, onStitchCallBack);
                        //    }
                        //});
                    }
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                Loghelper.WriteLog("保存图片失败", ex);

            }

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (tbSavePath.Text.Trim() == "图片保存地址,不填为不保存")
            {
                tbSavePath.ForeColor = ColorTranslator.FromHtml("#333333");
                tbSavePath.Text = "";
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (tbSavePath.Text.Trim().Length == 0)
            {
                tbSavePath.ForeColor = ColorTranslator.FromHtml("#999999");
                tbSavePath.PasswordChar = '\0';
                tbSavePath.Text = "图片保存地址,不填为不保存";
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择保存图片文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                tbSavePath.Text = dialog.SelectedPath + "\\";
            }
            else
            {
                this.ActiveControl = btnFuckStart;
                tbSavePath.Text = "";
                textBox1_Leave(null, null);
            }
        }

        private void btnDetectPath_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }

                Checkpicture checkpicture = new Checkpicture(dialog.SelectedPath, null, false, numericUpDown1.Value);
                checkpicture.ShowDialog();
            }
        }

        private void btnFuckDetectThisPcb_Click(object sender, EventArgs e)
        {
            thiscard = true;
        }

        private int sdkin()
        {

            // VS要用管理员权限打开
            // VS要用管理员权限打开
            // VS要用管理员权限打开

            //初始化检测器
            //return AITestSDK.init(@"C:\Users\Administrator\Desktop\suomi-test-img\aimodel\config.data",
            //     // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
            //     // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
            //     // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
            //     "C://Users//Administrator//Desktop//suomi-test-img//aimodel//voc.weights",
            //     0);
            return AITestSDK.init(@"E:\BaiduNetdiskDownload\newdemoall\newdemoall\newdemoall\bin\x64\Debug\suomi\data\config.data",
                // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
                // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
                // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
                "E:/BaiduNetdiskDownload/newdemoall/newdemoall/newdemoall/bin/x64/Debug/suomi/data/voc.weights",
                0);

        }

        private void btnFuckTest_Click(object sender, EventArgs e)
        {

            //thiscard = true;
            //isBabyYiNing = true;
            nowPcb = Pcb.CreatePcb();

            try
            {
                JsonData<Pcb> jsonData = new JsonData<Pcb>();
                jsonData.data = nowPcb;
                jsonData.executionTime = 11;
                jsonData.ngNum = nowPcb.results.Count;
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                string[] props = { "FrontPcb", "BackPcb" }; //排除掉，不能让前端看到的字段。为true的话就是只保留这些字段
                jSetting.ContractResolver = new LimitPropsContractResolver(props, false);

                string res = JsonConvert.SerializeObject(jsonData, jSetting);
                RabbitMQClientHandler.GetInstance()
                .TopicExchangePublishMessageToServerAndWaitConfirm("", "work", "work", res);

                //File.WriteAllText(Path.Combine(savePath, "result.json"), ConvertJsonString(res));
            }
            catch (Exception er)
            {
                //LogHelper.WriteLog("连接队列失败!!!", er);
            }
            //string imgPath = "testpic.png";
            //OpenFileDialog ofd = new OpenFileDialog();
            //DialogResult fdr = ofd.ShowDialog();
            //if (fdr == DialogResult.OK)
            //{
            //    imgPath = ofd.FileName;
            //}
            //DirectoryInfo info = new DirectoryInfo(imgPath);
            //string fuckpath = info.Parent.FullName;

            //nowPcb.FrontPcb.allCols = 24;
            //nowPcb.FrontPcb.allRows = 10;
            //nowPcb.FrontPcb.allNum = 240;
            //nowPcb.FrontPcb.or_hl = 0.3;
            //nowPcb.FrontPcb.or_hu = 0.4;
            //nowPcb.FrontPcb.or_vl = 0.1;
            //nowPcb.FrontPcb.or_vu = 0.5;
            //nowPcb.FrontPcb.dr_hu = 0.02;
            //nowPcb.FrontPcb.dr_vu = 0.02;
            //MySmartThreadPool.Instance().QueueWorkItem(() =>
            //{
            //    DirectoryInfo root = new DirectoryInfo(fuckpath);
            //    //int i = 0;
            //    for (int i = 1; i <= nowPcb.FrontPcb.allNum; i++)
            //    {
            //        Bitmap fBitmap = new Bitmap(fuckpath + "/F" + i + ".jpg");
            //        fBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);  //指定不进行翻转的 180 度旋转  （垂直翻转+水平翻转）
            //        nowPcb.FrontPcb.bitmaps.Enqueue(new OneStitchSidePcb.BitmapInfo() { bitmap = (Bitmap)BitmapScaleHelper.ScaleToSize(fBitmap, (float)0.4).Clone(), name = "/F" + i + ".jpg" });
            //        fBitmap.Dispose();

            //        //Aoi.StitchMain(nowPcb.FrontPcb, onStitchCallBack);
            //        MySmartThreadPool.Instance().QueueWorkItem(() =>
            //        {
            //            lock (nowPcb.FrontPcb)
            //            {
            //                Aoi.StitchMain(nowPcb.FrontPcb, onStitchCallBack);
            //            }
            //        });
            //    }
            //});

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }

}
