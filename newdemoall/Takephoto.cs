using HaiKang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace newdemoall
{

    public class Takephoto
    {
        
        public CameraOperator_HK cameraOperator;
        public void tack() {
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


    }
}
