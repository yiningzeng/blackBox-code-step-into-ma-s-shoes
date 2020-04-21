using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TestDevice;

namespace newdemoall
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("已有另一个程序在运行");
                Application.ExitThread();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //bbox_t_container boxlist = new bbox_t_container();
            // VS要用管理员权限打开
            // VS要用管理员权限打开
            // VS要用管理员权限打开

            ////初始化检测器
            //AITestSDK.init(@"\yunsheng\data\config.data",
            //    // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
            //    // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
            //    // 特别说明，这里的路径一定要用 '/' 不能用反斜杠
            //    @"\yunsheng\data\ai-voc_last.weights",
            //    0);
            Application.Run(new Form1());
        }
    }
}
