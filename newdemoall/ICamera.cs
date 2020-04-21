using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using HaiKang;
namespace ImageControl
{
    public delegate void ImageReadyEventScreen(BitmapInfo map);
    public interface ICamera
    {       
        event ImageReadyEventScreen ScreenEvent;
        int m_hCamera { get; }
        string CameraName { get; set; }
        bool IsOpen { get; }
        int GetCameraCount { get; }
        bool ConnectCamera(uint index);
        void OneShot();
        void ContinueShot();
        bool CameraStop();
        bool CameraClose();
        Size GetCameraROIMax { get; }
        Rectangle GetCameraROI { get;}
        bool SetROI(Rectangle rect, int widthMax, int heightMax, int increase);
        float ExposureTime { get; set; }
        float Gain { get; set; } 
        float InitExpourseTime { get; set; }//曝光参数
        float InitGain { get; set; }
        bool UserSetSave();
        bool SetTriggerMode(bool b);
        Bitmap CurrentImage();
        Bitmap GetMaxImage();

    }
}
