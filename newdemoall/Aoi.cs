using Emgu.CV;
using Emgu.CV.Structure;
using newdemoall.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static newdemoall.Form1;
using static newdemoall.OneStitchSidePcb;

namespace newdemoall
{
    public class Aoi
    {
        #region newA
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct FP1
        {
            public int feature;        // 特征类型编号
            public uint n_boxes;     // 输出的bbox个数
            public int ks;               // 卷积核尺寸，与图像尺寸成正比
            public float f_lb;           // 特征下限
            public float f_ub;           // 特征上限
            public int a_lb;             // 面积下限
            public int a_ub;             // 面积上限
            public float rb_lb;
            public float whr_eps;
            public float ar_eps;
        };

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct feature_bbox_t_container
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public Rectangle[] bboxlist;
        };

        [DllImport(@"aoi-v3-release.dll", EntryPoint = "feature_filter_csharp_v3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int feature_filter(IntPtr iplImage, ref feature_bbox_t_container f, FP1 fP1, IntPtr mask);
        #endregion

        public enum side { none = 0, left = 1, up = 2, right = 4, down = 8 };
        // 图像匹配算子
        // img: 待搜索的图像
        // templ: 目标模板
        // pos: 匹配结果（输出），目标模板templ在img内的区域的左上角顶点位置
        // binarize: 是否在预处理中加二值化操作，默认不进行(false)
        // method: 匹配度指标
        // return: 匹配结果得分
        //     method使用默认时，值不大于1；得分越高说明匹配程度越高；反之，接近0可认为基本不匹配
        //     method=1时，值不小于0；得分越低说明匹配程度越高
        [DllImport("aoi-v2-release.dll", EntryPoint = "marker_match_crop", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern double marker_match_crop(IntPtr iplImage, IntPtr patch, ref Point point, ref Rectangle rectangle, bool binarize = false, int method = 1, bool debug = false);

        [DllImport("aoi-v2-release.dll", EntryPoint = "marker_match", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern double marker_match(IntPtr iplImage, IntPtr patch, ref Point point);
        [DllImport("aoi-v2-release.dll", EntryPoint = "stitch_v2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void stitchv2(IntPtr img, Rectangle rectangle, IntPtr patch, ref Rectangle roi_patch, int side1, int overlap_lb1, int overlap_ub1, int drift_ub1, int side2 = 0, int overlap_lb2 = 0, int overlap_ub2 = 0, int drift_ub2 = 0);
        [DllImport("aoi-v2-release.dll", EntryPoint = "copy_to", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int copy_to(IntPtr iplImage, IntPtr patch, Rectangle rectangle);

        /// <summary>
        /// 拼图主函数
        /// </summary>
        /// <param name="oneSidePcb"></param>
        public static void StitchMain(OneStitchSidePcb oneSidePcb, StitchCallBack stitchCallBack)
        {
            bool needSave = false;
            double or_hl = oneSidePcb.or_hl;
            double or_hu = oneSidePcb.or_hu;
            double or_vl = oneSidePcb.or_vl;
            double or_vu = oneSidePcb.or_vu;
            double dr_hu = oneSidePcb.dr_hu;
            double dr_vu = oneSidePcb.dr_vu;
            BitmapInfo bitmapInfo;
            lock (oneSidePcb.bitmaps)
            {
                bitmapInfo = oneSidePcb.bitmaps.Dequeue();
                Bitmap half = BitmapScaleHelper.ScaleToSize(bitmapInfo.bitmap, (float)0.5);
                Emgu.CV.Image<Bgr, Byte> currentFrame = new Emgu.CV.Image<Bgr, Byte>(half);
                Mat imgOld = new Mat();
                CvInvoke.BitwiseAnd(currentFrame, currentFrame, imgOld);
                int edge = 3;
                Mat img = new Mat(imgOld, new Rectangle(new Point(edge, edge), new Size(imgOld.Width - edge * 2, imgOld.Height - edge * 2)));

                //Mat smallmat = new Mat();
                //CvInvoke.Resize(img, img, new Size(Convert.ToInt32(img.Cols * 0.5), Convert.ToInt32(img.Rows * 0.5)));
                //AoiAi.hello(smallmat.Ptr);

                //第一行
                if (oneSidePcb.currentRow == 0)
                {
                    if (oneSidePcb.currentCol == 0)
                    {
                        #region 判断s型还是z字形
                        if (oneSidePcb.zTrajectory) //Z形
                        {
                            oneSidePcb.trajectorySide = (int)side.left;
                            int x = Convert.ToInt32(img.Cols * (oneSidePcb.allCols - 1) * dr_hu);
                            int y = Convert.ToInt32(img.Rows * (oneSidePcb.allRows - 1) * dr_vu);
                            int dstRows = Convert.ToInt32(img.Rows * (oneSidePcb.allRows + (oneSidePcb.allRows - 1) * (dr_vu * 2 - or_vl)));
                            int dstCols = Convert.ToInt32(img.Cols * (oneSidePcb.allCols + (oneSidePcb.allCols - 1) * (dr_hu * 2 - or_hl)));
                            oneSidePcb.roi = new Rectangle(x, y, img.Cols, img.Rows);
                            oneSidePcb.dst = new Mat(dstRows, dstCols, img.Depth, img.NumberOfChannels); // 第一张图不要0,0 最好留一些像素
                        }
                        else // S型
                        {
                            oneSidePcb.trajectorySide = (int)side.right;

                            int dstRows = Convert.ToInt32(img.Rows * (oneSidePcb.allRows + (oneSidePcb.allRows - 1) * (dr_vu * 2 - or_vl)));
                            int dstCols = Convert.ToInt32(img.Cols * (oneSidePcb.allCols + (oneSidePcb.allCols - 1) * (dr_hu * 2 - or_hl)));
                            int x = Convert.ToInt32((dstCols - img.Cols) * (1 - dr_hu));
                            int y = Convert.ToInt32(img.Rows * (oneSidePcb.allRows - 1) * dr_vu);
                            oneSidePcb.roi = new Rectangle(x, y, img.Cols, img.Rows);
                            oneSidePcb.dst = new Mat(dstRows, dstCols, img.Depth, img.NumberOfChannels); // 第一张图不要0,0 最好留一些像素
                        }
                        #endregion
                    }
                    else
                    {
                        stitchv2(oneSidePcb.dst.Ptr, oneSidePcb.roi, img.Ptr, ref oneSidePcb.roi, oneSidePcb.trajectorySide, Convert.ToInt32(img.Cols * or_hl), Convert.ToInt32(img.Cols * or_hu), Convert.ToInt32(img.Rows * dr_vu));
                    }
                    //oneSidePcb.dst.Save(@"C:\Users\Administrator\Desktop\suomi-test-img\" + oneSidePcb.currentRow + "-" + oneSidePcb.currentCol + ".jpg");
                    copy_to(oneSidePcb.dst.Ptr, img.Ptr, oneSidePcb.roi);

                    //oneSidePcb.dst.Save(@"C:\Users\Administrator\Desktop\suomi-test-img\" + oneSidePcb.currentRow + "-" + oneSidePcb.currentCol + ".jpg");

                    oneSidePcb.currentCol++;
                    if (oneSidePcb.currentCol >= oneSidePcb.allCols)
                    {
                        oneSidePcb.currentCol = 0;
                        oneSidePcb.currentRow++;
                        if (oneSidePcb.trajectorySide == (int)side.left)
                        {
                            oneSidePcb.trajectorySide = (int)side.right;
                        }
                        else if (oneSidePcb.trajectorySide == (int)side.right)
                        {
                            oneSidePcb.trajectorySide = (int)side.left;
                        }
                    }
                    //oneSidePcb.dst.Save(@"C:\Users\Administrator\Desktop\suomi-test-img\row1.jpg");
                }
                else // 其他行
                {
                    if (Convert.ToBoolean(oneSidePcb.currentRow % 2)) //偶行
                    {
                        if (oneSidePcb.currentCol == 0)
                        {
                            stitchv2(oneSidePcb.dst.Ptr, oneSidePcb.roi, img.Ptr, ref oneSidePcb.roi, (int)side.up, Convert.ToInt32(img.Cols * or_vl), Convert.ToInt32(img.Cols * or_vu), Convert.ToInt32(img.Rows * dr_hu));
                            //oneSidePcb.roi = oneSidePcb.roi0;
                        }
                        else
                        {
                            stitchv2(oneSidePcb.dst.Ptr, oneSidePcb.roi, img.Ptr, ref oneSidePcb.roi, oneSidePcb.trajectorySide, Convert.ToInt32(img.Cols * or_hl), Convert.ToInt32(img.Cols * or_hu), Convert.ToInt32(img.Rows * dr_vu), (int)side.up, Convert.ToInt32(img.Rows * or_vl), Convert.ToInt32(img.Rows * or_vu), Convert.ToInt32(img.Cols * dr_hu));
                        }
                    }
                    else
                    {
                        if (oneSidePcb.currentCol == 0)
                        {
                            stitchv2(oneSidePcb.dst.Ptr, oneSidePcb.roi, img.Ptr, ref oneSidePcb.roi, (int)side.up, Convert.ToInt32(img.Cols * or_vl), Convert.ToInt32(img.Cols * or_vu), Convert.ToInt32(img.Rows * dr_hu));
                            //oneSidePcb.roi = oneSidePcb.roi0;
                        }
                        else
                        {
                            stitchv2(oneSidePcb.dst.Ptr, oneSidePcb.roi, img.Ptr, ref oneSidePcb.roi, oneSidePcb.trajectorySide, Convert.ToInt32(img.Cols * or_hl), Convert.ToInt32(img.Cols * or_hu), Convert.ToInt32(img.Rows * dr_vu), (int)side.up, Convert.ToInt32(img.Rows * or_vl), Convert.ToInt32(img.Rows * or_vu), Convert.ToInt32(img.Cols * dr_hu));
                        }
                    }
                    //oneSidePcb.dst.Save(@"C:\Users\Administrator\Desktop\suomi-test-img\" + oneSidePcb.currentRow + "-" + oneSidePcb.currentCol + ".jpg");
                    copy_to(oneSidePcb.dst.Ptr, img.Ptr, oneSidePcb.roi);
                    //oneSidePcb.dst.Save(@"C:\Users\Administrator\Desktop\suomi-test-img\" + oneSidePcb.currentRow + "-" + oneSidePcb.currentCol + ".jpg");
                    oneSidePcb.currentCol++;
                    if (oneSidePcb.currentRow >= oneSidePcb.allRows - 1 && oneSidePcb.currentCol >= oneSidePcb.allCols)
                    {
                        needSave = true;
                    }
                    else if (oneSidePcb.currentCol >= oneSidePcb.allCols)
                    {
                        oneSidePcb.currentCol = 0;
                        oneSidePcb.currentRow++;
                        if (oneSidePcb.trajectorySide == (int)side.left) oneSidePcb.trajectorySide = (int)side.right;
                        else if (oneSidePcb.trajectorySide == (int)side.right) oneSidePcb.trajectorySide = (int)side.left;
                    }
                }
                #region 实时更新采集框
                stitchCallBack(false, oneSidePcb, bitmapInfo, new RectangleF((float)(oneSidePcb.roi.Location.X * 0.5),
                        (float)(oneSidePcb.roi.Location.Y * 0.5),
                        (float)(oneSidePcb.roi.Size.Width * 0.5),
                        (float)(oneSidePcb.roi.Size.Height * 0.5)));
                #endregion

                #region 释放资源
                img.Dispose();
                currentFrame.Dispose();
                //bitmap.Dispose();
                #endregion

                if (needSave) // 加这里主要是为了优化workingForm.imgBoxWorking最后一个框的显示问题
                {
                    stitchCallBack(true, oneSidePcb, bitmapInfo, new RectangleF());
                    Mat smallmat = new Mat();
                    CvInvoke.Resize(oneSidePcb.dst, smallmat, new Size(Convert.ToInt32(oneSidePcb.dst.Cols * oneSidePcb.scale), Convert.ToInt32(oneSidePcb.dst.Rows * oneSidePcb.scale)));
                    if (oneSidePcb.zTrajectory)
                    {
                        string saveFile = Path.Combine(oneSidePcb.savePath, "front.jpg");
                        smallmat.Save(saveFile);
                        saveFile = Path.Combine(oneSidePcb.savePath, "back.jpg");
                        smallmat.Save(saveFile);
                    }
                    else
                    {
                        string saveFile = Path.Combine(oneSidePcb.savePath, "back.jpg");
                        smallmat.Save(saveFile);
                    }
                    smallmat.Dispose();
                    oneSidePcb.dst.Dispose();

                }
            }
        }
    }
}

