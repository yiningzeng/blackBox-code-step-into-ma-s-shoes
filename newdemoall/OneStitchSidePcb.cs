using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace newdemoall
{
    /// <summary>
    /// 默认都是Front A面
    /// </summary>
    public class OneStitchSidePcb
    {
        public class BitmapInfo
        {
            public string name { get; set; }
            public Bitmap bitmap { get; set; }
        }
        #region AI参数
        public int equalDivision { get; set; } = 1; // #表示检测的图像按照边长等分的数量，=2的话就是4等分
        public int overlap { get; set; } = 50; // #表示等分的时候重叠的区域
        public bool saveCropImg { get; set; } = false; //#是否保存等分的图片
        public bool detectMultiScale { get; set; } = false; // #开启多尺度检测的时候要把equalDivision设置为2
        public float confidence { get; set; } = (float)0.01;
        #endregion

        #region 其他参数
        public string pcbId;
        public string savePath;
        #endregion

        #region 拼图参数
        //double or_hl = 0.3; // lower bound for horizontal overlap ratio
        //double or_hu = 0.4; // upper
        double or_hl_end = 0.1; // overlap ratio is different at the end of each row
        double or_hu_end = 0.6;
        //double or_vl = 0.1; // vertical
        //double or_vu = 0.5;
        //double dr_hu = 0.02; // upper bound for horizontal drift ratio
        //double dr_vu = 0.02; // vertical

        public double or_hl { get; set; } = 0.3; // lower bound for horizontal overlap ratio
        public double or_hu { get; set; } = 0.4; // upper
        public double or_vl { get; set; } = 0.1; // vertical
        public double or_vu { get; set; } = 0.5;
        public double dr_hu { get; set; } = 0.02; // upper bound for horizontal drift ratio
        public double dr_vu { get; set; } = 0.02; //
        public int allNum; // 图片总数
        public int allRows; // 总行数
        public int allCols; // 总列数
        public int currentRow = 0; // 拼图当前行
        public int currentCol = 0; // 拼图当前列
        public bool zTrajectory = true; // 默认是Z轨迹，背面的话是S字形轨迹
        public int trajectorySide; //主要用于拼图的时候切换对其边
        public Mat dst = null; // 最终输出大图
        public Rectangle roi = new Rectangle(); // 对齐的参考的区域
        public double scale { get; set; } = 0.25;
        public bool stitchEnd = false; //拼图结束的标示
        //图片队列
        public Queue<BitmapInfo> bitmaps = new Queue<BitmapInfo>();
        #endregion
    }
}
