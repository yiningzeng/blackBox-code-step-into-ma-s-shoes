/***************************************************************************************************
* 文件名称：CameraOperator.cs
* 摘    要：相机操作类
* 当前版本：1.0.0.0
* 日    期：2016-07-07
***************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace HaiKang
{
    using ImageCallBack = MyCamera.cbOutputdelegate;
    using ExceptionCallBack = MyCamera.cbExceptiondelegate;
    public delegate void ImageReadyEvent(BitmapInfo map, bool isSuccess);
    public class BitmapInfo
    {
        public Bitmap m_Bitmap { get; set; }
        public string CameraName { get; set; }
        public BitmapInfo()
        {

        }
        public BitmapInfo(Bitmap bmp,string cameraName)
        {
            m_Bitmap = bmp;
            CameraName = cameraName;
        }
    }

    public class CameraOperator_HK
    {
        private BitmapInfo bitmapInfo;
        private int cameraIndex = -1;
        public Bitmap m_bitmap = null;    
        public event ImageReadyEvent ImageReady=null;
        public object obj = new object();
        public const int CO_FAIL = -1;
        public const int CO_OK = 0;
        private MyCamera m_pCSI;    
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        public Size SizeMax = new Size(5500, 5500);    
        public bool IsCameraOpen { get; set; }
        public bool IsGrabbing { get; set; }
        public Rectangle RectROI { get; set; }
        public float ExpourseTime { get; set; }
        public float Gain { get; set; }
        public bool IsContinuousShot { get; set; }

        //private delegate void ImageCallBack(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser);
        public CameraOperator_HK()
        {
            bitmapInfo = new BitmapInfo();
            // m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            ImageReadyEvent += new ImageReadyEventHandler(OnMVImageReadyEventCallback);
            m_pCSI = new MyCamera();
            m_grabThread = new Thread(Grab);
        }
        
        public int m_hCamera { get; set; }
        // 获取设备名 为 name 的设备 返回数量
        public int CameraCount(string name)
        {
            m_hCamera = -1;
            int count = 0;
            int nRet;
            /*创建设备列表*/
            System.GC.Collect();
            CameraOperator_HK.EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            //在窗体列表中显示设备名
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    //if (name == gigeInfo.chModelName)
                    //{
                        bitmapInfo.CameraName = name;
                        m_hCamera = i;
                    //}
                }
            }
            count = (int)m_pDeviceList.nDeviceNum;
            return count;
        }
        //打开相机设置 工作模式 triggerSource为触发模式（若为软触发时使用）
        public int OpenMVS(uint triggerSource)
        {
            if(m_bitmap!=null)
            {
                m_bitmap.Dispose();
                m_bitmap = null;
              
            }
            if (m_pDeviceList.nDeviceNum == 0)
            {              
                return -1;  //没有相机
            }
            int nRet = -1;

            //获取选择的设备信息
            MyCamera.MV_CC_DEVICE_INFO device =
                (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[m_hCamera],
                                                              typeof(MyCamera.MV_CC_DEVICE_INFO));
            //打开设备
            nRet = Open(ref device);
            if (MyCamera.MV_OK != nRet)
            { 
                return -2; //打开相机失败
            }
            //设置采集连续模式
            SetEnumValue("AcquisitionMode", 2);// 工作在连续模式
            SetEnumValue("TriggerMode", 0);   
                                                // 连续模式     
                                               //触发源选择:0 - Line0;
                                               //           1 - Line1;
                                               //           2 - Line2;
                                               //           3 - Line3;
                                               //           4 - Counter;
                                               //           7 - Software;
           // SetEnumValue("TriggerSource", triggerSource);    //触发源   
                                                             //return cameraOperator.SetROI(rect, widthMax, heightMax, increase);
                                                             //Size maxSize = GetCameraROIMax();
                                                             //SetROI(RectROI, maxSize.Width, maxSize.Height, 4);
            SetExposureTime(ExpourseTime);
            //SetGain(Gain);
            IsCameraOpen = true;
            return 0;
        }

        //抓拍图片 识别(调用 ImageReady方法)
       public MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo;
        //public MyCamera.MV_FRAME_OUT_INFO stFrameInfo;
        public void OnMVImageReadyEventCallback()
        {
            lock (obj)
            {
                try
                {
                    byte[] buffer = new byte[SizeMax.Width * SizeMax.Height * 3];
                    IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    UInt32 nDataLen = 0;
                    //stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO();
                     stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                    int nRet;
                    //超时获取一帧，超时时间为1秒

                   // nRet = GetOneFrame(pData, ref nDataLen, (uint)SizeMax.Width * (uint)SizeMax.Height * 3, ref stFrameInfo);

                   nRet = GetImageBGR(pData, (uint)SizeMax.Width * (uint)SizeMax.Height * 3, ref stFrameInfo);

                    if (MyCamera.MV_OK == nRet)
                    {
                        if (m_bitmap != null)
                        {
                            /* Update the bitmap with the image data. */
                            CameraOperator_HK.UpdateBitmap(m_bitmap, buffer, stFrameInfo.nWidth, stFrameInfo.nHeight, true);
                            /* To show the new image, request the display control to update itself. */
                        }
                        else /* A new bitmap is required. */
                        {
                            CameraOperator_HK.CreateBitmap(ref m_bitmap, stFrameInfo.nWidth, stFrameInfo.nHeight, true);
                            CameraOperator_HK.UpdateBitmap(m_bitmap, buffer, stFrameInfo.nWidth, stFrameInfo.nHeight, true);
                            //* Provide the display control with the new bitmap. This action automatically updates the display. */
                        }
                        bitmapInfo.m_Bitmap = (Bitmap)m_bitmap.Clone();
                        if (ImageReady != null)
                            ImageReady(bitmapInfo, IsContinuousShot);
                    }
                }
                catch (Exception e)
                {

                }
            }
        }
        #region 修改为连续工作模式 修改连续模式
        public bool AcquisitionMode(bool isTrigger)
        {        
            int nRet = SetEnumValue("AcquisitionMode", 2);// 工作在连续模式
            if (nRet == MyCamera.MV_OK)
            {
                if (isTrigger)
                {
                   nRet= SetEnumValue("TriggerMode", 1);
                    if(nRet!=MyCamera.MV_OK)
                    {
                        return false;
                    }
                    //触发源选择:0 - Line0;
                    //           1 - Line1;
                    //           2 - Line2;
                    //           3 - Line3;
                    //           4 - Counter;
                    //           7 - Software;
                    //nRet= SetEnumValue("TriggerSource", 0);
                    if (nRet != MyCamera.MV_OK)
                    {
                        return false;
                    }
                }
                else
                {
                    nRet= SetEnumValue("TriggerMode", 0);    // 连续模式
                    if (nRet != MyCamera.MV_OK)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        #endregion


        public bool UserSetSave()
        {
            int nRet=CommandExecute("UserSetSave");

            if (nRet == MyCamera.MV_OK)
                return true;

            return false;
        }

        public bool UserSet(int index)
        {
            int nRet = SetEnumValue("UserSetSelector", (uint)index);
            if (nRet==MyCamera.MV_OK)
                return true;

            return false;
        }

        /// <summary>
        /// 设置曝光
        /// </summary>
        /// <param name="exposureTime"></param>
        /// <returns></returns>
        public int SetExposureTime(float exposureTime)
        {
            int nRet = 0;
            SetEnumValue("ExposureAuto", 0);

            nRet = SetFloatValue("ExposureTime", exposureTime);

            return nRet;
        }

        //获取曝光
        public float GetExposureTime()
        {
            float exposureTime = 0;
            GetFloatValue("ExposureTime", ref exposureTime);
            return exposureTime;
        }


        public int SetGain(float exposureTime)
        {
            int nRet = 0;
            SetEnumValue("GainAuto", 0);

            nRet = SetFloatValue("Gain", exposureTime);

            return nRet;
        }

        public float GetGain()
        {
            float exposureTime = 0;
            GetFloatValue("Gain", ref exposureTime);
            return exposureTime;
        }

        //获取当前roi参数
        public Rectangle GetCameraROI()
        {
            int nRet;
            uint offsetX = 0, offsetY = 0, width = 0, height = 0;
            Rectangle rect = new Rectangle(0, 0, 0, 0);//识别区域
            nRet = GetIntValue("OffsetX", ref offsetX);//x轴偏移量
            nRet = GetIntValue("OffsetY", ref offsetY);//y轴偏移量
            nRet = GetIntValue("Width", ref width);//roi宽度
            nRet = GetIntValue("Height", ref height);//roi高度
            rect = new Rectangle((int)offsetX, (int)offsetY, (int)width, (int)height);           
            return rect;
        }

        //获取最大参数
        public Size GetCameraROIMax()
        {
            int nRet;
            Size p = new Size(0,0);
            uint widthMax = 0, heightMax= 0;
            nRet = GetIntValue("WidthMax", ref widthMax);
            nRet = GetIntValue("HeightMax", ref heightMax);
            p = new Size((int)widthMax, (int)heightMax);
            return p;
        }

        //设置参数
        public bool SetROI(Rectangle rect, int widthMax, int heightMax, int increase)
        {
            int nRet;
            int width = rect.Width - rect.Width % increase;
            int height = rect.Height - rect.Height % increase;
            int offsetX = rect.X - rect.X % increase;
            int offsetY = rect.Y - rect.Y % increase;

            if (width + offsetX > widthMax || height + offsetY > heightMax)
            {
                return false;
            }
            nRet = SetIntValue("Width", (uint)width);
            if (nRet != MyCamera.MV_OK)
                return false;
            nRet = SetIntValue("Height", (uint)height);
            if (nRet != MyCamera.MV_OK)
                return false;
            nRet = SetIntValue("OffsetX", (uint)offsetX);
            if (nRet != MyCamera.MV_OK)
                return false;
            nRet = SetIntValue("OffsetY", (uint)offsetY);
            if (nRet != MyCamera.MV_OK)
                return false;

            return true;
        }

        /****************************************************************************
         * @fn           EnumDevices
         * @brief        枚举可连接设备
         * @param        nLayerType       IN         传输层协议：1-GigE; 4-USB;可叠加
         * @param        stDeviceList     OUT        设备列表
         * @return       成功：0；错误：错误码
         ****************************************************************************/
        public static int EnumDevices(uint nLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDeviceList)
        {
            return MyCamera.MV_CC_EnumDevices_NET(nLayerType, ref stDeviceList);
        }


        /****************************************************************************
         * @fn           Open
         * @brief        连接设备
         * @param        stDeviceInfo       IN       设备信息结构体
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int Open(ref MyCamera.MV_CC_DEVICE_INFO stDeviceInfo)
        {
            if (null == m_pCSI)
            {
                m_pCSI = new MyCamera();
                if (null == m_pCSI)
                {
                    return CO_FAIL;
                }
            }
            int nRet;
            nRet = m_pCSI.MV_CC_CreateDevice_NET(ref stDeviceInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            nRet = m_pCSI.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           Close
         * @brief        关闭设备
         * @param        none
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int Close()
        {
            int nRet;

            nRet = m_pCSI.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            nRet = m_pCSI.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            IsCameraOpen = false;
            return CO_OK;
        }


        /****************************************************************************
         * @fn           StartGrabbing
         * @brief        开始采集
         * @param        none
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int StartGrabbing()
        {
            int nRet;
            //开始采集
            nRet = m_pCSI.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /****************************************************************************
         * @fn           StopGrabbing
         * @brief        停止采集
         * @param        none
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int StopGrabbing()
        {
            int nRet;
            nRet = m_pCSI.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            IsGrabbing = false;
            return CO_OK;
        }


        /****************************************************************************
         * @fn           RegisterImageCallBack
         * @brief        注册取流回调函数
         * @param        CallBackFunc          IN        回调函数
         * @param        pUser                 IN        用户参数
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int RegisterImageCallBack(ImageCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterImageCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           RegisterExceptionCallBack
         * @brief        注册异常回调函数
         * @param        CallBackFunc          IN        回调函数
         * @param        pUser                 IN        用户参数
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int RegisterExceptionCallBack(ExceptionCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterExceptionCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetOneFrame
         * @brief        获取一帧图像数据
         * @param        pData                 IN-OUT            数据数组指针
         * @param        pnDataLen             IN                数据大小
         * @param        nDataSize             IN                数组缓存大小
         * @param        pFrameInfo            OUT               数据信息
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetOneFrame(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo)
        {
            pnDataLen = 0;
            int nRet = m_pCSI.MV_CC_GetOneFrame_NET(pData, nDataSize, ref pFrameInfo);
            pnDataLen = pFrameInfo.nFrameLen;
            if (MyCamera.MV_OK != nRet)
            {
                return nRet;
            }
            return nRet;
        }

        public int GetImageBGR(IntPtr pData,UInt32 nDataSize,ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {

            int nRet = m_pCSI.MV_CC_GetImageForBGR_NET(pData, nDataSize, ref pFrameInfo,10);
            
            return nRet;
        }

        public int GetOneFrameTimeout(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, Int32 nMsec)
        {
            pnDataLen = 0;
            int nRet = m_pCSI.MV_CC_GetOneFrameTimeout_NET(pData, nDataSize, ref pFrameInfo, nMsec);
            pnDataLen = pFrameInfo.nFrameLen;
            if (MyCamera.MV_OK != nRet)
            {
                return nRet;
            }

            return nRet;
        }


        /****************************************************************************
         * @fn           Display
         * @brief        显示图像
         * @param        hWnd                  IN        窗口句柄
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int Display(IntPtr hWnd)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_Display_NET(hWnd);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetIntValue
         * @brief        获取Int型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pnValue               OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetIntValue(string strKey, ref UInt32 pnValue)
        {

            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_pCSI.MV_CC_GetIntValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnValue = stParam.nCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetIntValue
         * @brief        设置Int型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetIntValue(string strKey, UInt32 nValue)
        {

            int nRet = m_pCSI.MV_CC_SetIntValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }



        /****************************************************************************
         * @fn           GetFloatValue
         * @brief        获取Float型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pValue                OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetFloatValue(string strKey, ref float pfValue)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pCSI.MV_CC_GetFloatValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pfValue = stParam.fCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetFloatValue
         * @brief        设置Float型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        fValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetFloatValue(string strKey, float fValue)
        {
            int nRet = m_pCSI.MV_CC_SetFloatValue_NET(strKey, fValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetEnumValue
         * @brief        获取Enum型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pnValue               OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetEnumValue(string strKey, ref UInt32 pnValue)
        {
            MyCamera.MVCC_ENUMVALUE stParam = new MyCamera.MVCC_ENUMVALUE();
            int nRet = m_pCSI.MV_CC_GetEnumValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnValue = stParam.nCurValue;

            return CO_OK;
        }



        /****************************************************************************
         * @fn           SetEnumValue
         * @brief        设置Float型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetEnumValue(string strKey, UInt32 nValue)
        {
            int nRet = m_pCSI.MV_CC_SetEnumValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }



        /****************************************************************************
         * @fn           GetBoolValue
         * @brief        获取Bool型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pbValue               OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetBoolValue(string strKey, ref bool pbValue)
        {
            int nRet = m_pCSI.MV_CC_GetBoolValue_NET(strKey, ref pbValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetBoolValue
         * @brief        设置Bool型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        bValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetBoolValue(string strKey,bool bValue)
        {
            int nRet = m_pCSI.MV_CC_SetBoolValue_NET(strKey, bValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetStringValue
         * @brief        获取String型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        strValue              OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetStringValue(string strKey, ref string strValue)
        {
            MyCamera.MVCC_STRINGVALUE stParam = new MyCamera.MVCC_STRINGVALUE();
            int nRet = m_pCSI.MV_CC_GetStringValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            strValue = stParam.chCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetStringValue
         * @brief        设置String型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        strValue              IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetStringValue(string strKey, string strValue)
        {
            int nRet = m_pCSI.MV_CC_SetStringValue_NET(strKey, strValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           CommandExecute
         * @brief        Command命令
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int CommandExecute(string strKey)
        {
            int nRet = m_pCSI.MV_CC_SetCommandValue_NET(strKey);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           SaveImage
         * @brief        保存图片
         * @param        pSaveParam            IN        保存图片配置参数结构体
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SaveImage(ref MyCamera.MV_SAVE_IMAGE_PARAM_EX pSaveParam)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_SaveImageEx_NET(ref pSaveParam);
            return nRet;
        }

        public void ConvertImageBuffer()
        {
           
        }

        public delegate void ImageReadyEventHandler();
        public event ImageReadyEventHandler ImageReadyEvent;
        protected void OnImageReadyEvent()
        {
            if (ImageReadyEvent != null)
            {
                ImageReadyEvent();
            }
        }
        protected Thread m_grabThread;
        private bool m_grabThreadRun = false;
        private bool m_grabOnce = false;

       
        protected void Grab()
        {
            try
            {
                while (m_grabThreadRun)
                {
                    OnImageReadyEvent();
                    if (m_grabOnce)
                    {
                        m_grabThreadRun = false;
                        break;
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception exp)
            {

            }
        }
        //开始采集流
        public int StartGrabImage()
        {
            int nRet=0;
            if (!IsGrabbing)
            {
                //开始采集
                nRet = StartGrabbing();
                if (MyCamera.MV_OK != nRet)
                {                 
                    return -1;
                }
                //标志位置位true
                IsGrabbing = true;
            }
            return nRet;
        }

        public void OneShot()
        {
            if (!m_grabThread.IsAlive) /* Only start when open and not grabbing already. */
            {
                /* Set up the grabbing and start. */
                m_grabOnce = true;
                m_grabThreadRun = true;
                m_grabThread = new Thread(Grab);
                m_grabThread.Start();
            }
        }
        public void ContinueShot()
        {
            if (!m_grabThread.IsAlive)  /* Only start when open and not grabbing already. */
            {
                /* Set up the grabbing and start. */
                m_grabOnce = false;
                m_grabThreadRun = true;
                m_grabThread = new Thread(Grab);
                m_grabThread.Start();
            }
        }
        public void Stop()
        {
            if (m_grabThread.IsAlive) /* Only start when open and grabbing. */
            {
                m_grabThreadRun = false; /* Causes the grab thread to stop. */
                m_grabThread.Join(); /* Wait for it to stop. */
            }
        }
        private static PixelFormat GetFormat(bool color)
        {
            return color ? PixelFormat.Format24bppRgb : PixelFormat.Format8bppIndexed;
        }
        /* Creates a new bitmap object with the supplied properties. */
        public static void CreateBitmap(ref Bitmap bitmap, int width, int height, bool color)
        {
            bitmap = new Bitmap(width, height, GetFormat(color));

            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                ColorPalette colorPalette = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    colorPalette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = colorPalette;
            }
        }
        private static int GetStride(int width, bool color)
        {
            return color ? width * 3 : width;
        }

        /* Copies the raw image data to the bitmap buffer. */
        public static void UpdateBitmap(Bitmap bitmap, byte[] buffer, int width, int height, bool color)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            /* Get the pointer to the bitmap's buffer. */
            IntPtr ptrBmp = bmpData.Scan0;
            /* Compute the width of a line of the image data. */
            int imageStride = GetStride(width, color);
            /* If the widths in bytes are equal, copy in one go. */
            if (imageStride == bmpData.Stride)
            {
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, ptrBmp, bmpData.Stride * bitmap.Height);
            }
            else /* The widths in bytes are not equal, copy line by line. This can happen if the image width is not divisible by four. */
            {
                for (int i = 0; i < bitmap.Height; ++i)
                {
                    Marshal.Copy(buffer, i * imageStride, new IntPtr(ptrBmp.ToInt64() + i * bmpData.Stride), width);
                }
            }
            /* Unlock the bits. */
            bitmap.UnlockBits(bmpData);
        }

      


    }
}
