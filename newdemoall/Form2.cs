using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace newdemoall
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            readxml();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int j = 0; j < this.dataGridView1.Rows.Count; j++)
                {
                    if (dataGridView1.Rows[j].Selected == true)
                    {
                        this.dataGridView1.Rows.RemoveAt(j);
                    }
                }
                update();

                readxml();
            }
            catch (Exception ex) {
                Loghelper.WriteLog("修改界面删除规则失败",ex);

            }

        }
        public void readxml()
        {
            dataGridView1.Rows.Clear();
            XmlDocument doc = new XmlDocument();
            doc.Load(@"xyrule.xml");

            XmlNodeList no = doc.SelectSingleNode("//Rules").ChildNodes;

            foreach (XmlElement node in no)
            {

                int num = dataGridView1.Rows.Add();
                dataGridView1["Column1", num].Value = node.GetElementsByTagName("name")[0].InnerText;
                dataGridView1["Column2", num].Value = node.GetElementsByTagName("leftup")[0].InnerText;
                dataGridView1["Column3", num].Value = node.GetElementsByTagName("rightdown")[0].InnerText;
                dataGridView1["Column4", num].Value = node.GetElementsByTagName("xmove")[0].InnerText;
                dataGridView1["Column5", num].Value = node.GetElementsByTagName("ymove")[0].InnerText;
            }


        }
        public void update()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(@"xyrule.xml");
            //doc.RemoveAll();       
            int row = dataGridView1.Rows.Count;//得到总行数 
            XmlNode xmlElementpark = doc.SelectSingleNode("//Rules");
            xmlElementpark.RemoveAll();
            //doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", ""));//编写文件头
            //XmlElement xmlElementall;
            //int cell = dataGridView1.Rows[1].Cells.Count;//得到总列数    
            for (int i = 0; i < row ; i++)//得到总行数并在之内循环    
            {
                //同上，创建一个个<student>节点，并且附到<class>之下
                XmlElement xmlElementall = doc.CreateElement("rule");
                XmlElement xmlElement_name = doc.CreateElement("name");
                xmlElement_name.InnerText = dataGridView1["Column1", i].Value.ToString();
                xmlElementall.AppendChild(xmlElement_name);
                XmlElement xmlElement_cardnum = doc.CreateElement("leftup");
                xmlElement_cardnum.InnerText = dataGridView1["Column2", i].Value.ToString();
                xmlElementall.AppendChild(xmlElement_cardnum);
                XmlElement xmlElement_kname = doc.CreateElement("rightdown");
                xmlElement_kname.InnerText = dataGridView1["Column3", i].Value.ToString();
                xmlElementall.AppendChild(xmlElement_kname);
                XmlElement xmlElement_khangbie = doc.CreateElement("xmove");
                xmlElement_khangbie.InnerText = dataGridView1["Column4", i].Value.ToString();
                xmlElementall.AppendChild(xmlElement_khangbie);
                XmlElement xmlElement_kainame = doc.CreateElement("ymove");
                xmlElement_kainame.InnerText = dataGridView1["Column5", i].Value.ToString();
                xmlElementall.AppendChild(xmlElement_kainame);
                xmlElementpark.AppendChild(xmlElementall);
            }

            //doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", ""));//编写文件头
            //doc.AppendChild(xmlElementpark);
            doc.Save(@"xyrule.xml");//保存这个xml文件
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
