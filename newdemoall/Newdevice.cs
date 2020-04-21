using newdemoall.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TestDevice;
using HaiKang;
using System.Windows.Forms;
using ImageControl;
using System.Drawing;

namespace newdemoall
{
    public class Newdevice
    {
        private ITestBox testBox = null;
        //private newshowpoint newshowpointtake = null;
        private XYrulescs xYrulescs;
        private ImageControl.ICamera camera;
        internal XYrulescs XYrulescs { get => xYrulescs; set => xYrulescs = value; }
        public ITestBox TestBox { get => testBox; set => testBox = value; }
        public ICamera Camera { get => camera; set => camera = value; }
        public void run() {
            xymove();
        }

        public bool xymove()
            {
            try
            {
                //this.newshowpointtake = showpoint;
                if (this.testBox.IsOpen && this.testBox != null)
                {
                    Mobot.Point3 point3 = new Mobot.Point3(Settings.Default.xspeed, Settings.Default.yspeed, 100);
                    testBox.MotorSpeed = point3;
                    int firsty = xYrulescs.Leftup.Y;
                    int firstx = xYrulescs.Leftup.X;
                    int x = firstx;
                    int y = firsty;
                    int lasty = y;
                    testBox.StylusMoveXY(x, y);

                    Thread.Sleep(200);
                    showpoint(x, y);
                    //Loghelper.WriteLog("x=" + x.ToString() + "  y=" + y.ToString());
                    while (y <= xYrulescs.Rightdown.Y)
                    {
                        if (!Settings.Default.canmove)
                        {
                            break;

                        }
                        int lastx = x;
                        int endx = x;
                        while (lastx <= xYrulescs.Rightdown.X)
                        {
                            if (!Settings.Default.canmove)
                            {
                                break;

                            }
                            if (endx < lastx)
                            {
                                if (x == xYrulescs.Rightdown.X)
                                {
                                    endx = x;
                                    break;

                                }
                                else if (x + xYrulescs.Xmove > xYrulescs.Rightdown.X)
                                {
                                    x = xYrulescs.Rightdown.X;
                                    lastx = x;
                                    testBox.StylusMoveXY(x, y);
                                    showpoint(x, y);
                                    endx = x;
                                    break;

                                }
                                else
                                {
                                    x = x + xYrulescs.Xmove;
                                    lastx = x;
                                    testBox.StylusMoveXY(x, y);
                                    showpoint(x, y);

                                }
                            }
                            else if (endx > lastx)
                            {
                                if (x == firstx)
                                {
                                    lastx = firstx;
                                    break;

                                }
                                else if (x - xYrulescs.Xmove < firstx)
                                {
                                    x = firstx;
                                    lastx = x;
                                    testBox.StylusMoveXY(x, y);

                                    showpoint(x, y);
                                    break;
                                }
                                else
                                {
                                    x = x - xYrulescs.Xmove;
                                    lastx = x;
                                    testBox.StylusMoveXY(x, y);
                                    showpoint(x, y);

                                }
                            }
                            else
                            {
                                if (endx == xYrulescs.Rightdown.X)
                                {
                                    if (x - xYrulescs.Xmove < xYrulescs.Leftup.X)
                                    {
                                        if (y > lasty)
                                        {
                                            x = firstx;
                                            lastx = x;
                                            testBox.StylusMoveXY(x, y);
                                            showpoint(x, y);
                                            endx = x;
                                            break;
                                        }
                                        else
                                        {
                                            x = firstx;
                                            lastx = x;
                                            endx = x;
                                            break;

                                        }
                                    }
                                    else
                                    {
                                        x = x - xYrulescs.Xmove;
                                        lastx = x;
                                        testBox.StylusMoveXY(x, y);
                                        showpoint(x, y);

                                    }
                                }
                                else if (endx == xYrulescs.Leftup.X)
                                {
                                    if (x + xYrulescs.Xmove > xYrulescs.Rightdown.X)
                                    {
                                        if (y > lasty)
                                        {
                                            x = xYrulescs.Rightdown.X;
                                            lastx = x;
                                            endx = x;
                                            testBox.StylusMoveXY(x, y);
                                            showpoint(x, y);
                                            break;
                                        }
                                        else
                                        {
                                            x = xYrulescs.Rightdown.X;
                                            lastx = x;
                                            endx = x;
                                            break;

                                        }


                                    }
                                    else
                                    {
                                        x = x + xYrulescs.Xmove;
                                        lastx = x;
                                        testBox.StylusMoveXY(x, y);
                                        showpoint(x, y);

                                    }
                                }
                            }

                        }
                        if (y + xYrulescs.Ymove <= xYrulescs.Rightdown.Y)
                        {
                            lasty = y;
                            y = y + xYrulescs.Ymove;
                            testBox.StylusMoveXY(x, y);
                            showpoint(x, y);

                        }
                        else
                        {
                            testBox.StylusReset();
                            break;
                        }
                    }
                    return false;
                }
                return false;
            }
            catch (Exception e) {
                Loghelper.WriteLog("移动错误",e);
                return false;


            }

            }

        public void showpoint(int x,int y)
        {
            try
            {
                Thread.Sleep(Settings.Default.waittme);
                this.camera.OneShot();
            }
            catch(Exception e) {
                Loghelper.WriteLog("拍照错误",e);
            }
        }

    }


    
}
