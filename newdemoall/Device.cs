using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaiKang;
namespace ImageControl
{
    public class CameraDevice_HK : ICamera
    {
        public event ImageReadyEventScreen ScreenEvent;
        private Bitmap currentBitmap=null;
        private object cameraLock = new object();
        public CameraOperator_HK cameraOperator;
        public CameraDevice_HK()
        {
            cameraOperator  = new CameraOperator_HK();
            cameraOperator.ImageReady += CameraOperator_ImageReady;
        }
        private void CameraOperator_ImageReady(BitmapInfo bitmapInfo, bool isSuccess)
        {
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
                currentBitmap = null;
            }
            currentBitmap = (Bitmap)bitmapInfo.m_Bitmap.Clone();
            if (isSuccess)  
            {
                if (ScreenEvent != null)
                    ScreenEvent(bitmapInfo);
            }
        }
    

        public bool SetTriggerMode(bool isTrigger)
        {
           return cameraOperator.AcquisitionMode(isTrigger);
           
        }
        public bool IsOpen { get { return cameraOperator.IsCameraOpen; } }

      
        public int m_hCamera { get { return cameraOperator.m_hCamera; }}
        public int GetCameraCount
        { get
            {
                if (CameraName != null && CameraName != "")
                {
                    return cameraOperator.CameraCount(CameraName);
                }
                return 0;
            }
        }
     
        public Size GetCameraROIMax
        {
            get
            {
                return cameraOperator.GetCameraROIMax();
            }
        }
        public Rectangle GetCameraROI
        {
            get { return cameraOperator.GetCameraROI(); }
        }
        public bool SetROI(Rectangle rect, int widthMax, int heightMax, int increase)
        {
            int width = rect.Width - rect.Width % increase;
            int height = rect.Height - rect.Height % increase;
            int offsetX = rect.X - rect.X % increase;
            int offsetY = rect.Y - rect.Y % increase;

            if (width + offsetX > widthMax || height + offsetY > heightMax)
            {
                return false;
            }
            cameraOperator.RectROI = new Rectangle(offsetX, offsetY, width, height);
            return true;
        }
        public float ExposureTime
        {
            get { return cameraOperator.GetExposureTime(); }

            set { cameraOperator.SetExposureTime(value); }
        }

        public float Gain
        {
            get { return cameraOperator.GetGain(); }

            set { cameraOperator.SetGain(value); }
        }
        
        
        public float InitExpourseTime { get {return cameraOperator.ExpourseTime; } set { cameraOperator.ExpourseTime = value; } }
        public float InitGain { get { return cameraOperator.Gain; }set { cameraOperator.Gain = value; } }

        public string CameraName { get; set; }

        public bool UserSetSave()
        {
           return cameraOperator.UserSetSave();
        }
        public bool CameraClose()
        {
            if (cameraOperator.IsGrabbing)
            {
                int nRet = cameraOperator.StopGrabbing();
                if (nRet != 0)
                    return false;
            }
            cameraOperator.Stop();
            if (cameraOperator.IsCameraOpen)
            {
                int nRet = cameraOperator.Close();
                if (nRet != 0)
                    return false;
            }
            return true;
        }   
        public bool CameraStop()
        {
            if (cameraOperator.IsGrabbing)
            {
                int nRet = cameraOperator.StopGrabbing();
                if (nRet != 0)
                    return false;
            }
            cameraOperator.Stop();
            return true;
        }   
        public bool ConnectCamera(uint index)
        {
            if (!cameraOperator.IsCameraOpen)
            {
                int nRet = cameraOperator.OpenMVS(index);
                if (nRet == 0)
                {
                    if (!CameraStartGrabing(cameraOperator))
                    {
                        throw new Exception("相机取流错误");
                    }
                    else
                    {
                        
                        Rectangle rectangle = GetCameraROI;
                        //相机抓拍最大尺寸
                        Size size = GetCameraROIMax;
                    }
                }
                else
                {
                    throw new Exception(string.Format("连接相机错误:Code({0})", nRet));
                }
            }
            return true;
        }

        public void ContinueShot()
        {
            cameraOperator.IsContinuousShot = true;
            if (CameraStartGrabing(cameraOperator))
                cameraOperator.ContinueShot();
            else
                throw new Exception("无法启动连续采集模式");
        }

        public void OneShot()
        {
            cameraOperator.IsContinuousShot = true;
            if (CameraStartGrabing(cameraOperator))
                cameraOperator.OneShot();
            else
                throw new Exception("无法启动单张采集模式");
        }
        private static bool CameraStartGrabing(CameraOperator_HK cameraOperator)
        {
            if (!cameraOperator.IsGrabbing)
            {
                int nRet = cameraOperator.StartGrabImage();
                System.Threading.Thread.Sleep(200);
                if (nRet != 0)
                    return false;
            }
            return true;
        }

        //采集最大像素照片
        public Bitmap GetMaxImage()
        {
            Bitmap bmp = null;
            if(!cameraOperator.IsCameraOpen)
                throw new Exception ("请先连接相机");
            bool b = false;
            b = CameraStop();
            int width = cameraOperator.SizeMax.Width;
            int height = cameraOperator.SizeMax.Height;
            b = SetROI(new Rectangle(0, 0, width, height), width, height, 4);

            if (b)
            {
                DateTime startTime = new DateTime();
                OneShot();
                while (true)
                {
                    DateTime endTime = new DateTime();
                    double usedMilliseconds = (endTime - startTime).TotalMilliseconds;
                    if (usedMilliseconds > 1000)
                    {
                        break;
                    }
                    if (currentBitmap != null)
                    {
                        bmp = (Bitmap)currentBitmap.Clone();
                        currentBitmap.Dispose();
                        currentBitmap = null;
                        break;
                    }
                }
            }        
            return bmp;
        }

        public Bitmap CurrentImage()
        {
            Bitmap bmp = null;
            DateTime startTime = DateTime.Now;
            OneShot();
            while (true)
            {
                DateTime endTime = DateTime.Now;
                double usedMilliseconds = (endTime - startTime).TotalMilliseconds;
                if (usedMilliseconds > 2000)
                {
                    break;
                }
                if (currentBitmap != null)
                {
                    bmp = (Bitmap)currentBitmap.Clone();
                    currentBitmap.Dispose();
                    currentBitmap = null;
                    break;
                }
                System.Threading.Thread.Sleep(10);
            }
            return bmp;
        }
    }
}
