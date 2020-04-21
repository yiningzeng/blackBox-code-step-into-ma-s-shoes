using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HaiKang;

namespace newdemoall
{
    class XYrulescs
    {
        private string name;
        private Point leftup;
        private Point rightdown;
        private int xmove;
        private int ymove;
        private CameraOperator_HK cameraOperator_HK;

        public string Name { get => name; set => name = value; }
        public Point Leftup { get => leftup; set => leftup = value; }
        public Point Rightdown { get => rightdown; set => rightdown = value; }
        public int Xmove { get => xmove; set => xmove = value; }
        public int Ymove { get => ymove; set => ymove = value; }
        public CameraOperator_HK CameraOperator_HK { get => cameraOperator_HK; set => cameraOperator_HK = value; }
    }
}
