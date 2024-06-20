using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace s1_zzz
{


    /// <summary>
    /// 窗体类
    /// </summary>
    public partial class Form1 : Form
    {

        /// <summary>
        /// 主窗体
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            mainFrm = this;

            FindSerialList();
        }


        /// <summary>
        /// 定义窗体字段
        /// </summary>
        public static Form1 mainFrm;


        /// <summary>
        /// 定义ProcessDelegate枚举
        /// </summary>
        /// <param name="str"></param>
        public delegate void ProcessDelegate(string str);


        /// <summary>
        /// 定义寻找串口方法
        /// </summary>
        public void FindSerialList()//定义寻找串口方法
        {
            string[] PortName = System.IO.Ports.SerialPort.GetPortNames();
            System.IO.Ports.SerialPort curr_Port;
            comboBox1.Items.Clear();//清理搜素串口框里内容
            foreach (string temp in PortName)//foreach(type 迭代变量声明 in Arrayname)
            {
                //curr_Port = new System.IO.Ports.SerialPort(temp);
                //curr_Port.Open();
                //if (curr_Port.IsOpen)
                //{
                // Coding。。。。
                //}
                //curr_Port.Close();
                comboBox1.Items.Add(temp);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        static byte[] recvall(Socket sock, int count)//在recvall方法中定义Socket接口的sock变量和count变量
        {
            byte[] buf = new byte[1024 * 1024 * 8];//无符号整数buf数组初始化
            byte[] newbuf = new byte[1024 * 1024 * 8];//传输的时候都是用Byte字节来传输的   这是8MB
            int index = 0;
            while (count > 0)
            {
                //0, body.Length, SocketFlags.None
                int length = sock.Receive(newbuf, 0, count, SocketFlags.None);
                //int Socket.Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
                //使用指定的Socket，从绑定的SocketFlags接收指定的字节数，存入接收缓冲区的指定偏移量位置，返回收到的字节数

                newbuf.CopyTo(buf, index);
                //void Array.CopyTo(Array array, int index)
                //从指定的目标数组索引处开始，将当前一维数组的所有元素复制到指定的一维数组中，索引指定为32位整数

                index = index + length;
                count -= length;//count = count - length
            }
            return buf;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int bytesToInt(byte[] src, int offset)//int Robotcontroller.bytesToInt(byte[] src, int offset)  方法
        {
            int value;
            value = (int)((src[offset] & 0xFF)//逻辑运算符 + 移位运算符
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }
        static byte[] arrServerRecMsg = new byte[1024 * 1024 * 8];//传输的时候都是用Byte字节来传输的   这是8MB
        static bool isRecved = false;//字段


        /// <summary>
        /// 
        /// </summary>
        /// <param name="socketclientpara"></param>
        static private void recv(object socketclientpara)
        //void Form1.recv(object socketclientpara)  方法  
        {
            byte[] arrlen = new byte[32];//定义arrlen数组初始化且长度为32
            ProcessDelegate up = delegate (string text)
            // ProcessDelegate接收客户端发来的信息，客户端套接字对象
            //将委托中text字符串数据赋值到客服端发来的信息变量up中
            {
                mainFrm.listBox1.Items.Add(text.ToString());//将text添加到listbox.Items中
            };
            while (true)
            {
                try
                {
                    int len = 0;
                    if (arrlen[4] == 32)
                    {
                        len = (arrlen[0] - 48) * 1000 + (arrlen[1] - 48) * 100 + (arrlen[2] - 48) * 10 + (arrlen[3] - 48) * 1;
                    }
                    else
                    {
                        len = (arrlen[0] - 48) * 10000 + (arrlen[1] - 48) * 1000 + (arrlen[2] - 48) * 100 + (arrlen[3] - 48) * 10 + (arrlen[4] - 48) * 1;
                    }
                    isRecved = true;


                    Thread.Sleep(5);//将当前进程挂起5毫秒          
                    isRecved = false;
                }
                catch (Exception ex)
                {

                    break;
                }
            }
        }


        /// <summary>
        /// 串口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)//定义serialPort1_DataReceived方法
        {
            try
            {
                int len = serialPort1.BytesToRead;//BytesToRead获取接收缓冲区中的字节数
                Byte[] buf = new byte[len]; //定义buf数组初始化且长度为len
                int length = serialPort1.Read(buf, 0, len);
                //int System.IO.Ports.SerialPort.Read(type[] buffer, int offset, int count)
                //从 System.IO.Ports.SerialPort输入缓冲区读取一些字节并将那些字节写入字节数组中指定的偏移量处
                string result = System.Text.Encoding.ASCII.GetString(buf);
                //string Encoding.GetString(byte[] byte)
                //在派生类重写时，将指定字节数组中的所有字节解码为一个字
            }
            catch (Exception ex)
            { }
        }


        /// <summary>
        /// 使用缓冲区的数据将指定数量的字节写入串行端口
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static int serialWrite(Byte[] buf, int len)
        {
            if (mainFrm.serialPort1.IsOpen)
            {
                mainFrm.serialPort1.Write(buf, 0, len);
                ////void System.IO.Ports.SeriaPort.Write(Byte[] buffer, int offest, int count)
                //使用缓冲区的数据将指定数量的字节写入串行端口
                Thread.Sleep(5);
                mainFrm.serialPort1.Write(buf, 0, len);
                Thread.Sleep(5);
                mainFrm.serialPort1.Write(buf, 0, len);
                Thread.Sleep(5);
            }
            else
                mainFrm.toolStripStatusLabel2.Text = mainFrm.comboBox1.Text + "Error!!!";
            return 1;
        }


        /// <summary>
        /// 定义robot_init
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public int robot_init(Byte[] cmd)
        {
            return 1;
        }


        /// <summary>
        /// 点击连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) return;
            robot_init(new Byte[] { 0, 3 });
            //int robotcmd.robot_init(byte[] cmd)
            if (comboBox1.Text.Trim() == "")
            //cong“”字符串的开头和结尾删除所有空白字符剩余的字符串
            {
                MessageBox.Show("请正确输入串口号！");
                return;
            }
            serialPort1.PortName = comboBox1.Text;//串口号导入comboBox1
            try
            {
                serialPort1.Open();
                if (serialPort1.IsOpen)
                {
                    toolStripStatusLabel2.Text = comboBox1.Text + "Opened !";//打开串口后输入状态到状态0位
                }
                else
                {
                    toolStripStatusLabel2.Text = comboBox1.Text + "Error !";
                    button1.Text = "打开"; }

            }
            finally
            {

            }
        }


        /// <summary>
        /// 点击断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            toolStripStatusLabel2.Text = "serialPort Close !";
            button1.Text = "连接";
        }





        
       





        /// <summary>
        /// 定义sel_control_id
        /// </summary>
        int sel_control_id;



        /// <summary>
        /// 东麒S2(RS485)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            /*radioButton1.Checked = true;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
            radioButton10.Checked = false;*/
            label2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            label3.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            label4.Visible = false;
            button7.Visible = false;
            button8.Visible = false;
            label5.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            label6.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            

            
            

            button20.Visible = true;
            button21.Visible = true;
            button22.Visible = false;
            button23.Visible = false;
            button24.Visible = false;
            

            label2.Text = "夹爪";
            button3.Text = "张开/正转";
            button4.Text = "闭合/反转";
            label3.Text = "关节二";
            button5.Text = "正转/上仰";
            button6.Text = "反转/下俯";
            sel_control_id = 2;
        }


        /// <summary>
        /// 东麒S3(RS485)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            /*radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = true;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
            radioButton10.Checked = false;*/
            label2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            label3.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            label4.Visible = true;
            button7.Visible = true;
            button8.Visible = true;
            label5.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            label6.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            

            
            

            button20.Visible = true;
            button21.Visible = true;
            button22.Visible = true;
            button23.Visible = false;
            button24.Visible = false;
            

            label2.Text = "夹爪";
            button3.Text = "张开/正转";
            button4.Text = "闭合/反转";
            label3.Text = "关节二";
            button5.Text = "正转/上仰";
            button6.Text = "反转/下俯";
            label4.Text = "关节三";
            button7.Text = "正转/上仰";
            button8.Text = "反转/下俯";
            sel_control_id = 3;
        }


        /// <summary>
        /// 东麒S5(RS485)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            /*radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = true;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
            radioButton10.Checked = false;*/
            label2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            label3.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            label4.Visible = true;
            button7.Visible = true;
            button8.Visible = true;
            label5.Visible = true;
            button9.Visible = true;
            button10.Visible = true;
            label6.Visible = true;
            button11.Visible = true;
            button12.Visible = true;
            

            
            

            button20.Visible = true;
            button21.Visible = true;
            button22.Visible = true;
            button23.Visible = true;
            button24.Visible = true;
            

            label2.Text = "夹爪";
            button3.Text = "张开/正转";
            button4.Text = "闭合/反转";
            label3.Text = "关节二";
            button5.Text = "正转/上仰";
            button6.Text = "反转/下俯";
            label4.Text = "关节三";
            button7.Text = "正转/上仰";
            button8.Text = "反转/下俯";
            label5.Text = "关节四";
            button9.Text = "正转/上仰";
            button10.Text = "反转/下俯";
            label6.Text = "关节五";
            button11.Text = "正转/上仰";
            button12.Text = "反转/下俯";
            sel_control_id = 5;
        }


        /// <summary>
        /// 东麒L10(RS485)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            /*radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = true;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
            radioButton10.Checked = false;*/
            label2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            label3.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            label4.Visible = true;
            button7.Visible = true;
            button8.Visible = true;
            label5.Visible = true;
            button9.Visible = true;
            button10.Visible = true;
            label6.Visible = true;
            button11.Visible = true;
            button12.Visible = true;
            

            
            

            button20.Visible = true;
            button21.Visible = true;
            button22.Visible = true;
            button23.Visible = true;
            button24.Visible = true;
            

            label2.Text = "夹爪";
            button3.Text = "张开/正转";
            button4.Text = "闭合/反转";
            label3.Text = "关节二";
            button5.Text = "正转/上仰";
            button6.Text = "反转/下俯";
            label4.Text = "关节三";
            button7.Text = "正转/收缸";
            button8.Text = "反转/伸缸";
            label5.Text = "关节四";
            button9.Text = "正转/收缸";
            button10.Text = "反转/伸缸";
            label6.Text = "关节五";
            button11.Text = "正转/收缸";
            button12.Text = "反转/伸缸";
            sel_control_id = 5;
        }













        /// <summary>
        /// 五轴控制协议
        /// </summary>
        /*
        3E A2 01 04 E5 C0 D4 01 00 95  夹爪夹
        3E A2 01 04 E5 40 2B FE FF 68  夹爪松
        3E 80 01 00 BF 夹爪停
        3E A2 02 04 E6 C0 D4 01 00 95  旋转+
        3E A2 02 04 E6 40 2B FE FF 68  旋转-
        3E 80 02 00 C0 旋转停
        3E A2 03 04 E7 C0 D4 01 00 95  3上
        3E A2 03 04 E7 40 2B FE FF 68  3下
        3E 80 03 00 C1  3停
        3E A2 04 04 E8 C0 D4 01 00 95  4上
        3E A2 04 04 E8 40 2B FE FF 68  4下
        3E 80 04 00 C2  4停
        3E A2 05 04 E9 60 EA 00 00 4A  5右
        3E A2 05 04 E9 A0 15 FF FF B3  5左
        3E 80 05 00 C3  5停
        */


        /// <summary>
        /// 定义关节
        /// </summary>
        #region
        bool b_joint_1 = false;//在方法外定义均为false
        bool b_joint_2 = false;//每一个到底指什么？
        bool b_joint_3 = false;
        bool b_joint_4 = false;
        bool b_joint_5 = false;
        bool b_joint_6 = false;
        bool b_joint_7 = false;
        bool b_joint2_rcv = false;
        #endregion

        /// <summary>
        /// 电机状态添加到ListBox控件
        /// </summary>
        /// <param name="lbox"></param>
        public void ListBoxAutoCroll(ListBox lbox)
        {
            lbox.TopIndex = lbox.Items.Count - (int)(lbox.Height / lbox.ItemHeight);
            lbox.SelectedIndex = lbox.Items.Count - 1;
            Thread.Sleep(100);
        }

        

        /// <summary>
        /// 定义新数组变量buf1
        /// </summary>
        Byte[] buf1 = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//定义新数组变量buf1


        /// <summary>
        /// 抱闸使能定义
        /// </summary>
        bool is_brake = false;//false:no
        int brake_id = 1;


        /// <summary>
        /// 抱闸使能
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isopen"></param>
        void fun_brake(int id, int isopen)
        {

            //Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };// 
            buf1[0] = 0x3e;
            buf1[1] = 0xa0;
            buf1[2] = (Byte)id;
            buf1[3] = (Byte)isopen;
            buf1[4] = (Byte)(buf1[0] + buf1[1] + buf1[2] + buf1[3]);
            Form1.serialWrite(buf1, 5);
            Thread.Sleep(10);
        }


        /// <summary>
        /// 抱闸按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       /*rivate void button19_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("请正确输入串口号！");
                return;
            }
            brake_id = (int)comboBox2.SelectedIndex + 1;
            if (!is_brake)
            {
                fun_brake(brake_id, 01);
                is_brake = true;
            }
            else
            {
                fun_brake(brake_id, 00);
                is_brake = false;
            }
            if (is_brake) button8.Text = "已打开";
            else
                button8.Text = "已关闭";
        }*/


        /// <summary>
        /// 电机使能
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        void motor_Enable(int id, int enable)
        {
            buf1[0] = 0x3e;//DATA[0] 头字节 0x3e
            if (enable == 1)//电机在运行
                buf1[1] = 0x88;
            else
                buf1[1] = 0x81;//电机停止命令的DATA[1] 命令字节为0x81
            buf1[2] = (Byte)id;//DATA[2] ID字节 0x01~0x20
            buf1[3] = (Byte)0;//DATA[3] 数据长度字节 0x00
            buf1[4] = (Byte)(buf1[0] + buf1[1] + buf1[2] + buf1[3]); //DATA[4] 帧头校验字节 DATA[0]~DATA[3]字节校验和
            Form1.serialWrite(buf1, 5);
            //serialPort1.Read(readbuf, 0, 100);
            Thread.Sleep(10);//将当前进程挂起10毫秒
        }


        /// <summary>
        /// 电机力矩
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_value"></param>
        void update_tor(int id, int _value)
        {
            _value = (_value) * 2;
            buf1[0] = 0x3e;
            buf1[1] = 0xa1;
            buf1[2] = (Byte)id;
            buf1[3] = 0x02;
            buf1[4] = (Byte)(buf1[0] + buf1[1] + buf1[2] + buf1[3]);
            buf1[5] = (Byte)(_value & 0x000000ff);
            buf1[6] = (Byte)((_value & 0x0000ff00) >> 8);
            buf1[7] = (Byte)(buf1[5] + buf1[6]);
            Form1.serialWrite(buf1, 8);
            //serialPort1.Read(readbuf, 0, 100);
            Thread.Sleep(50);
        }


        /// <summary>
        /// 电机速度
        /// </summary>
        /// <param name="id"></param>
        /// <param name="speed"></param>
        void update_speed(int id, int speed)
        {
            //速度闭环控制命令（10byte）
            //主机发送命令以控制电机速度，控制值SpeedControl为int32_t类型，对应实际转速为0.01dps/LSB
            speed = speed * 100;
            buf1[0] = 0x3e;//头字节 固定
            buf1[1] = 0xa2;//命令字节 固定
            buf1[2] = (Byte)id;//ID字节 0x01~0x20（ID）
            buf1[3] = 0x04; //数据长度字节 固定
            buf1[4] = (Byte)(buf1[0] + buf1[1] + buf1[2] + buf1[3]);//帧头校验字节 DATA[0]~DATA[3]字节校验和
            buf1[5] = (Byte)(speed & 0x000000ff);//电机速度低字节 *(uint8_t)(&SpeedControl)
            buf1[6] = (Byte)((speed & 0x0000ff00) >> 8);//电机速度 *(uint8_t)(&SpeedControl+1)
            buf1[7] = (Byte)((speed & 0x00ff0000) >> 16);//电机速度 *(uint8_t)(&SpeedControl+2)
            buf1[8] = (Byte)((speed & 0xff000000) >> 24); //电机速度高字节 *(uint8_t)(&SpeedControl+3)
            buf1[9] = (Byte)(buf1[5] + buf1[6] + buf1[7] + buf1[8]);//数据校验字节 DATA[5]~DATA[8]字节校验和
            Form1.serialWrite(buf1, 10);
            //serialPort1.Read(readbuf, 0, 100);
            Thread.Sleep(50);
        }

        /// <summary>
        /// 电机位置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        void update_location(int id, int location)
        {
            location = location * 100;
            buf1[0] = 0x3e;
            buf1[1] = 0xa7;
            buf1[2] = (Byte)id;
            buf1[3] = 0x04;
            buf1[4] = (Byte)(buf1[0] + buf1[1] + buf1[2] + buf1[3]);//帧头校验字节 DATA[0]~DATA[3]字节校验和
            buf1[5] = (Byte)(location & 0x000000ff);
            buf1[6] = (Byte)((location & 0x0000ff00) >> 8);
            buf1[7] = (Byte)((location & 0x00ff0000) >> 16);
            buf1[8] = (Byte)((location & 0xff000000) >> 24);
            buf1[9] = (Byte)(buf1[5] + buf1[6] + buf1[7] + buf1[8]);
            /* buf1[7] = (Byte)((location & 0x0000000000ff0000) >> 16);
             buf1[8] = (Byte)((location & 0x00000000ff000000) >> 24); 
             buf1[9] = (Byte)((location & 0x000000ff00000000) >> 32);
             buf1[10] = (Byte)((location & 0x0000ff0000000000) >> 40);
             buf1[11] = (Byte)((location & 0x00ff000000000000) >> 48);
             buf1[12] = (Byte)((location & 0xff0000000000000) >> 56);*/
           // buf1[9] = (Byte)(buf1[5] + buf1[6] + buf1[7] );//数据校验字节 DATA[5]~DATA[8]字节校验和
            Form1.serialWrite(buf1, 10);
            //serialPort1.Read(readbuf, 0, 100);
            Thread.Sleep(50);
        }


        /// <summary>
        /// 夹爪：张开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if (radioButton1.Checked == true)
            {
                if (sel_control_id == 2)
                {
                    /*if (b_joint_2 == false)
                    {*/
                        motor_Enable(0x02, 1);
                        //update_speed(0x02, -trackBar1.Value);//同夹相比反向相同速度
                        b_joint_2 = true;
                        listBox1.Items.Add("关节1 " + button3.Text + "中!");
                   /* }
                    else
                    {
                        motor_Enable(0x02, 0);
                        b_joint_2 = false;
                    }*/
                }
                else
                {
                    if (b_joint_1 == false)
                    {
                        //夹爪轴ID：1
                        //轴1控制：（速度环）
                        //反转：3E A2 01 04 E5 40 2B FE FF 68
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//3E A2 01 04 E5 40 2B FE FF 68
                        buf[0] = 0x3e;
                        buf[1] = 0xa2;
                        buf[2] = 0x01;
                        buf[3] = 0x04;
                        buf[4] = 0xe5;
                        buf[5] = 0x40;
                        buf[6] = 0x2b;
                        buf[7] = 0xfe;
                        buf[8] = 0xff;
                        buf[9] = 0x68;
                        Form1.serialWrite(buf, 10);
                        b_joint_1 = true;
                        label2.BackColor = Color.LawnGreen; 

                    }
                    else
                    {
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//3E 80 01 00 BF 夹爪停
                        buf[0] = 0x3e;
                        buf[1] = 0x80;
                        buf[2] = 0x01;
                        buf[3] = 0x00;
                        buf[4] = 0xbf;
                        Form1.serialWrite(buf, 5);
                        b_joint_1 = false;
                    }
                }
            }
            else if ((radioButton3.Checked == true)|| (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
               
                motor_Enable(0x01, 1);
                //update_speed(0x01, (-trackBar1.Value));//同夹相比反向相同速度
                b_joint_1 = true;
                label2.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节1 " + button3.Text + "中!");
                   
            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                
              /*  if (sel_control_id == 2)
                {*/
                    if (b_joint_2 == false)
                    {
                        motor_Enable(0x01, 1);
                       // update_speed(0x01, -(trackBar2.Value));

                    update_tor(0x01, -635);
                    b_joint_2 = true;
                    listBox1.Items.Add("关节1" + button3.Text + "中!");
                }
                    else
                    {
                        motor_Enable(0x01, 0);
                        b_joint_2 = false;
                    listBox1.Items.Add("关节1 " + "停止" + "!");
                }
               /* }
                else
                {
                    if (b_joint_2 == false)
                    {
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//旋转 -
                        buf[0] = 0x3e;
                        buf[1] = 0xa2;
                        buf[2] = 0x02;
                        buf[3] = 0x04;
                        buf[4] = 0xe6;
                        buf[5] = 0x40;
                        buf[6] = 0x2b;
                        buf[7] = 0xfe;
                        buf[8] = 0xff;
                        buf[9] = 0x68;
                        //Robotcontroller.serialWrite(buf, 10);
                        b_joint_2 = true;
                    }
                    else
                    {
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//3E 80 01 00 BF 夹爪停
                        buf[0] = 0x3e;
                        buf[1] = 0x80;
                        buf[2] = 0x02;
                        buf[3] = 0x00;
                        buf[4] = 0xc0;
                        //Robotcontroller.serialWrite(buf, 5);
                        b_joint_2 = false;
                    }
                }*/


            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true)
            { }
            ListBoxAutoCroll(listBox1);

        }


        /// <summary>
        /// 夹爪：闭合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if (radioButton1.Checked == true)
            {
                if (sel_control_id == 2)
                {
                    /*if (b_joint_2 == false)
                    {*/
                    motor_Enable(0x02, 1);//804行电机是能参数修改-使能打开
                   // update_speed(0x02, trackBar1.Value);//836行调整电机的速度
                    b_joint_2 = true;
                    listBox1.Items.Add("关节1 " + button4.Text + "中!");
                    /*}
                    else
                    {
                        motor_Enable(0x02, 0);//804行电机是能参数修改-使能打开
                        b_joint_2 = false;
                    }*/
                }
                else//sel_control_id !== 2
                {
                    if (b_joint_1 == false)//不在进行夹的动作
                    {
                        //夹爪轴ID：1
                        //轴1控制：（速度环）
                        //正转：3E A2 01 04 E5 C0 D4 01 00 95
                        //停止：3E 80 01 00 BF 
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//夹爪夹
                        buf[0] = 0x3e;
                        buf[1] = 0xa2;
                        buf[2] = 0x01;
                        buf[3] = 0x04;
                        buf[4] = 0xe5;
                        buf[5] = 0xc0;
                        buf[6] = 0xd4;
                        buf[7] = 0x01;
                        buf[8] = 0x00;
                        buf[9] = 0x95;
                        Form1.serialWrite(buf, 10);//serialWrite 487行方法
                        b_joint_1 = true;
                        label2.BackColor = Color.LawnGreen;
                    }
                    else//b_joint_1 !== false 说明在运动（夹）
                    {
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//3E 80 01 00 BF 夹爪停
                        buf[0] = 0x3e;
                        buf[1] = 0x80;
                        buf[2] = 0x01;
                        buf[3] = 0x00;
                        buf[4] = 0xbf;
                        Form1.serialWrite(buf, 5);
                        b_joint_1 = false;
                    }
                }
            }
            else if ((radioButton3.Checked == true) || (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                motor_Enable(0x01, 1);//804行电机是能参数修改-使能打开
               // update_speed(0x01, trackBar1.Value);//836行调整电机的速度
                b_joint_1 = true;
                label2.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节1 " + button4.Text + "中!");

            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                /*if (b_joint_1 == false)
                {
                    motor_Enable(0x01, 1);
                    update_speed(0x01, trackBar1.Value);
                    b_joint_1 = true;
                    label2.BackColor = Color.LawnGreen;
                    listBox1.Items.Add("关节1 " + button4.Text + "中!");

                }
                else
                {
                    motor_Enable(0x01, 0);
                    b_joint_1 = false;
                    listBox1.Items.Add("关节1 " + "停止" + "!");


                }*/

                /*if (b_joint_1 == false)//不在进行夹的动作
                {
                    //夹爪轴ID：1
                    //轴1控制：（速度环）
                    //正转：3E A2 01 04 E5 C0 D4 01 00 95
                    //停止：3E 80 01 00 BF 
                    Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//夹爪夹
                    buf[0] = 0x3e;
                    buf[1] = 0xa2;
                    buf[2] = 0x01;
                    buf[3] = 0x04;
                    buf[4] = 0xe5;
                    buf[5] = 0x60;
                    buf[6] = 0x73;
                    buf[7] = 0xff;
                    buf[8] = 0xff;
                    buf[9] = 0xd1;
                    //Form1.serialWrite(buf, 10);//serialWrite 487行方法
                    b_joint_1 = true;
                    label2.BackColor = Color.LawnGreen;
                    listBox1.Items.Add("关节1 " + button4.Text + "中!");
                }
                else//b_joint_1 !== false 说明在运动（夹）
                {
                    Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//3E 80 01 00 BF 夹爪停
                    buf[0] = 0x3e;
                    buf[1] = 0x80;
                    buf[2] = 0x01;
                    buf[3] = 0x00;
                    buf[4] = 0xbf;
                    //Form1.serialWrite(buf, 5);
                    b_joint_1 = false;
                    listBox1.Items.Add("关节1 " + "停止" + "!");
                }*/
               /* if (sel_control_id == 2)//sel_control_id是什么？？？
                {*/
                    if (b_joint_2 == false)
                    {
                        motor_Enable(0x01, 1);
                    //update_speed(0x01, trackBar2.Value);
                    update_tor(0x01, 635);
                    b_joint_2 = true;
                    listBox1.Items.Add("关节1 " + button4.Text + "中!");
                }
                    else
                    {
                        motor_Enable(0x01, 0);
                        b_joint_2 = false;
                    listBox1.Items.Add("关节1 " + "停止" + "!");
                }

              /*  }
                else
                {
                    if (b_joint_2 == false)
                    {
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//旋转 +
                        buf[0] = 0x3e;
                        buf[1] = 0xa2;
                        buf[2] = 0x02;
                        buf[3] = 0x04;
                        buf[4] = 0xe6;
                        buf[5] = 0xc0;
                        buf[6] = 0xd4;
                        buf[7] = 0x01;
                        buf[8] = 0x00;
                        buf[9] = 0x95;
                        //Robotcontroller.serialWrite(buf, 10);
                        b_joint_2 = true;
                    }
                    else
                    {
                        Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//3E 80 01 00 BF 夹爪停
                        buf[0] = 0x3e;
                        buf[1] = 0x80;
                        buf[2] = 0x02;
                        buf[3] = 0x00;
                        buf[4] = 0xc0;
                        //Robotcontroller.serialWrite(buf, 5);
                        b_joint_2 = false;

                    }
                }*/
            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true)
            { }
            ListBoxAutoCroll(listBox1);

        }


        /// <summary>
        /// 关节二：正转/上仰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if (radioButton1.Checked == true) 
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                motor_Enable(0x01, 1);
               // update_speed(0x01, trackBar1.Value);//同夹相比反向相同速度
                b_joint_1 = true;
                label3.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节2 " + button3.Text + "中!");
            }
            else if ((radioButton3.Checked == true) || (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                motor_Enable(0x02, 1);
               // update_speed(0x02, trackBar2.Value);
                b_joint_2 = true;
                label3.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节2 " + button5.Text + "中!");
            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_2 == false)
                {
                    motor_Enable(0x02, 1);
                    // update_speed(0x02, trackBar2.Value);
                    //update_tor(0x02, 2000);
                      update_location(0x02, 1800);
                    b_joint_2 = true;
                    label3.BackColor = Color.LawnGreen;
                    listBox1.Items.Add("关节2 " + button5.Text + "中!");
                }
               else
                {
                   /* motor_Enable(0x02, 0);
                    b_joint_2 = false;
                    listBox1.Items.Add("关节2 " + "停止" + "!");
                    //b_joint2_rcv = false;*/
                }
            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true) { }
            
            ListBoxAutoCroll(listBox1);

        }


        /// <summary>
        /// 关节二：反转/下俯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if (radioButton1.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                motor_Enable(0x01, 1);
               // update_speed(0x01, (-trackBar1.Value));
                b_joint_1 = true;
                label3.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节2 " + button4.Text + "中!");
            }
            else if ((radioButton3.Checked == true) ||(radioButton4.Checked == true))
{
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                motor_Enable(0x02, 1);
              //  update_speed(0x02, (-trackBar2.Value));
                b_joint_2 = true;
                label3.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节2 " + button6.Text + "中!");

            }
            else if (radioButton2.Checked == true)
{}
            else if (radioButton5.Checked == true)
{}
            else if (radioButton6.Checked == true)
{
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_2 == false)
                {

                    motor_Enable(0x02, 1);
                    // update_speed(0x02, (-trackBar2.Value));
                    //update_tor(0x02, -2000);
                      update_location(0x02, -1800);
                    b_joint_2 = true;
                    label3.BackColor = Color.LawnGreen;
                    listBox1.Items.Add("关节2 " + button6.Text + "中!");
                }
                else
                {
                  /*  motor_Enable(0x02, 0);
                    b_joint_2 = false;
                    b_joint2_rcv = false;
                    listBox1.Items.Add("关节2 " + "停止" + "!");*/
                }
            }
            else if (radioButton7.Checked == true)
{}
            else if (radioButton8.Checked == true)
{}
            else if (radioButton9.Checked == true)
{}
            else if (radioButton10.Checked == true)
{}          
            ListBoxAutoCroll(listBox1);
        }


        /// <summary>
        /// 关节三：正转/上仰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if ((radioButton1.Checked == true)||(radioButton3.Checked == true) ||(radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }

                fun_brake(0x03, 1);
                motor_Enable(0x03, 1);

              //      update_speed(0x03, trackBar3.Value);

                    b_joint_3 = true;
                label4.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节3 " + button7.Text + "中!");
              
            }
            else if (radioButton2.Checked == true)
            {

            }
            else if (radioButton5.Checked == true)
            {

            }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_3 == false)
                {

                    fun_brake(0x03, 1);
                    motor_Enable(0x03, 1);

                    //update_speed(0x03, trackBar3.Value);
                    update_tor(0x03, 635);

                    

                    b_joint_3 = true;
                label4.BackColor = Color.LawnGreen;

                listBox1.Items.Add("关节3 " + button7.Text + "中!");
                }
                else
                {
                    fun_brake(0x03, 0);
                    motor_Enable(0x03, 0);
                    b_joint_3 = false;
                    listBox1.Items.Add("关节3 " + "停止" + "!");

                }
            }
            else if (radioButton7.Checked == true)
            {

            }
            else if (radioButton8.Checked == true)
            {

            }
            else if (radioButton9.Checked == true)
            {

            }
            else if (radioButton10.Checked == true)
            {

            }
            
            ListBoxAutoCroll(listBox1);
        }


        /// <summary>
        /// 关节三：反转/下俯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if ((radioButton1.Checked == true) || (radioButton3.Checked == true) || (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                fun_brake(0x03, 1);
                motor_Enable(0x03, 1);
             //       update_speed(0x03, (-trackBar3.Value));
                    b_joint_3 = true;
                label4.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节3 " + button8.Text + "中!");
              
            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_3 == false)
                {
                fun_brake(0x03, 1);
                motor_Enable(0x03, 1);
                   // update_speed(0x03, (-trackBar3.Value));
                    update_tor(0x03, -635);
                    b_joint_3 = true;
                label4.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节3 " + button8.Text + "中!");
                }
                else
                {
                    fun_brake(0x03, 0);
                    motor_Enable(0x03, 0);
                    b_joint_3 = false;
                    listBox1.Items.Add("关节3 " + "停止" + "!");
                }
            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true)
            { }
            
            ListBoxAutoCroll(listBox1);
        }


        /// <summary>
        /// 关节四：正转/上仰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if ((radioButton1.Checked == true) || (radioButton3.Checked == true) || (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                fun_brake(0x04, 1);
                motor_Enable(0x04, 1);
             //       update_speed(0x04, trackBar4.Value);
                    b_joint_4 = true;
                label5.BackColor = Color.LawnGreen;

            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_4 == false)
                {
                    fun_brake(0x04, 1);
                motor_Enable(0x04, 1);
                    //update_speed(0x04, trackBar4.Value);
                    update_tor(0x04, 620);
                    b_joint_4 = true;
                label5.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节4 " + button9.Text + "中!");
                }
                else
                {
                    fun_brake(0x04, 0);
                    motor_Enable(0x04, 0);
                    b_joint_4 = false;
                    listBox1.Items.Add("关节4 " + "停止" + "!");
                }
                ListBoxAutoCroll(listBox1);
            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true)
            { }
            ListBoxAutoCroll(listBox1);

        }


        /// <summary>
        /// 关节四：反转/下俯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if ((radioButton1.Checked == true) || (radioButton3.Checked == true) || (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
               
                    motor_Enable(0x04, 1);
             //       update_speed(0x04, (-trackBar4.Value));
                    b_joint_4 = true;
                label5.BackColor = Color.LawnGreen;

            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_4 == false)
                {
                    fun_brake(0x04, 1);
                    motor_Enable(0x04, 1);
                    // update_speed(0x04, -trackBar4.Value);
                    update_tor(0x04, -620);
                    b_joint_4 = true;
                label5.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节4 " + button10.Text + "中!");
                }
                else
                {
                    fun_brake(0x04, 0);
                    motor_Enable(0x04, 0);
                    b_joint_4 = false;
                    listBox1.Items.Add("关节4 " + "停止" + "!");

                }
            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true)
            { }
            
            ListBoxAutoCroll(listBox1);
            
        }


        /// <summary>
        /// 关节五：正转/上仰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if ((radioButton1.Checked == true) || (radioButton3.Checked == true) || (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
               
                    motor_Enable(0x05, 1);
             //       update_speed(0x05, trackBar5.Value);
                    b_joint_5 = true;
                label6.BackColor = Color.LawnGreen;
                listBox1.Items.Add("关节5 " + button11.Text + "中!");
           
            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_5 == false)
                {
                    fun_brake(0x05, 1);
                    motor_Enable(0x05, 1);
                    update_speed(0x05, 2000);
                    b_joint_5 = true;
                    label6.BackColor = Color.LawnGreen;
                    listBox1.Items.Add("关节5 " + button11.Text + "中!");
                }
                else
                {
                    fun_brake(0x05, 0);
                    motor_Enable(0x05, 0);
                    b_joint_5 = false;
                    label6.BackColor = Color.Transparent;
                    listBox1.Items.Add("关节5 " + "停止" + "!");
                }
            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true)
            { }
            ListBoxAutoCroll(listBox1);
        }


        /// <summary>
        /// 关节五：反转/下俯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            if ((radioButton1.Checked == true) || (radioButton3.Checked == true) || (radioButton4.Checked == true))
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
               
                    motor_Enable(0x05, 1);
             //       update_speed(0x05, -trackBar5.Value);
                    b_joint_5 = true;
                label6.BackColor = Color.LawnGreen;

            }
            else if (radioButton2.Checked == true)
            { }
            else if (radioButton5.Checked == true)
            { }
            else if (radioButton6.Checked == true)
            {
                if (!serialPort1.IsOpen)
                {
                    listBox1.Items.Add("请正确输入串口号！");
                    return;
                }
                if (b_joint_5 == false)
                {
                    fun_brake(0x05, 1);
                    motor_Enable(0x05, 1);
                    update_speed(0x05, -2000);
                    b_joint_5 = true;
                    label6.BackColor = Color.LawnGreen;
                    listBox1.Items.Add("关节5 " + button12.Text + "中!");
                }
                else
                {
                    fun_brake(0x05, 0);
                    motor_Enable(0x05, 0);
                    b_joint_5 = false;
                    label6.BackColor = Color.Transparent;
                    listBox1.Items.Add("关节5 " + "停止" + "!");
                }
            }
            else if (radioButton7.Checked == true)
            { }
            else if (radioButton8.Checked == true)
            { }
            else if (radioButton9.Checked == true)
            { }
            else if (radioButton10.Checked == true)
            { }
            ListBoxAutoCroll(listBox1);
        }


        /// <summary>
        /// 停止运动定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Robotcontroller_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//不是左吗？  这个枚举项修改为Left时会出现，点击一瞬间停止且点击结束继续运行，他不会停止
            //enum System.Window.Forms.MouseButtons
            //指定定义哪个鼠标按钮曾按下的常数
            //MouseButtons.Right 鼠标右按钮曾按下
            {
                button17_Click(sender, null);
            }
            else
            {}
        }


        /// <summary>
        /// 停止运动指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            Byte[] buf = new Byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//3E 80 01 00 BF 夹爪停
            buf[0] = 0x3e;
            buf[1] = 0x80;
            buf[2] = 0x01;
            buf[3] = 0x00;
            buf[4] = 0xbf;
            Form1.serialWrite(buf, 5);
            b_joint_1 = false;
            label2.BackColor = Color.Transparent;

            buf[0] = 0x3e;
            buf[1] = 0x80;
            buf[2] = 0x02;
            buf[3] = 0x00;
            buf[4] = 0xc0;
            Form1.serialWrite(buf, 5);
            b_joint_2 = false;
            label3.BackColor = Color.Transparent;


            buf[0] = 0x3e;
            buf[1] = 0x80;
            buf[2] = 0x03;
            buf[3] = 0x00;
            buf[4] = 0xc1;
            Form1.serialWrite(buf, 5);
            b_joint_3 = false;
            label4.BackColor = Color.Transparent;


            buf[0] = 0x3e;
            buf[1] = 0x80;
            buf[2] = 0x04;
            buf[3] = 0x00;
            buf[4] = 0xc2;
            Form1.serialWrite(buf, 5);
            b_joint_4 = false;
            label5.BackColor = Color.Transparent;


            buf[0] = 0x3e;
            buf[1] = 0x80;
            buf[2] = 0x05;
            buf[3] = 0x00;
            buf[4] = 0xc3;
            Form1.serialWrite(buf, 5);
            b_joint_5 = false;
            label6.BackColor = Color.Transparent;
        }


        /// <summary>
        /// 一轴滑条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label9.Text = trackBar1.Value.ToString() + "转/分";
            if (trackBar1.Value < 0)
                ;//  bt_1.Text = "松开";
            else
                ;//   bt_1.Text = "夹";
            b_joint_1 = false;
            b_joint_2 = false;
            b_joint_3 = false;
            b_joint_4 = false;
            b_joint_5 = false;
            b_joint_6 = false;
            b_joint_7 = false;
        }*/


        /// <summary>
        /// 二轴滑条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      /*  private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label10.Text = trackBar2.Value.ToString() + "转/分";
            if (trackBar2.Value < 0)
                ;//  bt_2.Text = "左旋转";
            else
                ;//  bt_2.Text = "右旋转";
            b_joint_1 = false;
            b_joint_2 = false;
            b_joint_3 = false;
            b_joint_4 = false;
            b_joint_5 = false;
            b_joint_6 = false;
            b_joint_7 = false;
        }*/


        /// <summary>
        /// 三轴滑条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    /*    private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label11.Text = trackBar3.Value.ToString() + "转/分";
            if (trackBar3.Value < 0)
                ;//  bt_3.Text = "气缸升长";
            else
                ;// bt_3.Text = "气缸缩短";
            b_joint_1 = false;
            b_joint_2 = false;
            b_joint_3 = false;
            b_joint_4 = false;
            b_joint_5 = false;
            b_joint_6 = false;
            b_joint_7 = false;
        }*/


        /// <summary>
        /// 四轴滑条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    /*    private void trackBar4_Scroll(object sender, EventArgs e)
        {
            label12.Text = trackBar4.Value.ToString() + "转/分";
            if (trackBar4.Value < 0)
                ;//  bt_4.Text = "气缸升长";
            else
                ;// bt_4.Text = "气缸缩短";
            b_joint_1 = false;
            b_joint_2 = false;
            b_joint_3 = false;
            b_joint_4 = false;
            b_joint_5 = false;
            b_joint_6 = false;
            b_joint_7 = false;
        }*/


        /// <summary>
        /// 五轴滑条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    /*    private void trackBar5_Scroll(object sender, EventArgs e)
        {
            label13.Text = trackBar5.Value.ToString() + "转/分";
            if (trackBar5.Value < 0)
                ;//  bt_3.Text = "气缸升长";
            else
                ;// bt_3.Text = "气缸缩短";
            b_joint_1 = false;
            b_joint_2 = false;
            b_joint_3 = false;
            b_joint_4 = false;
            b_joint_5 = false;
            b_joint_6 = false;
            b_joint_7 = false;
        }*/


        /// <summary>
        /// 六轴滑条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       /*rivate void trackBar6_Scroll(object sender, EventArgs e)
        {
            label14.Text = trackBar6.Value.ToString() + "转/分";
            if (trackBar6.Value < 0)
                ;//  bt_3.Text = "气缸升长";
            else
                ;// bt_3.Text = "气缸缩短";
            b_joint_1 = false;
            b_joint_2 = false;
            b_joint_3 = false;
            b_joint_4 = false;
            b_joint_5 = false;
            b_joint_6 = false;
            b_joint_7 = false;
        }*/


        /// <summary>
        /// 七轴滑条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       /*rivate void trackBar7_Scroll(object sender, EventArgs e)
        {
            label15.Text = trackBar7.Value.ToString() + "转/分";
            if (trackBar7.Value < 0)
                ;//  bt_3.Text = "气缸升长";
            else
                ;// bt_3.Text = "气缸缩短";
            b_joint_1 = false;
            b_joint_2 = false;
            b_joint_3 = false;
            b_joint_4 = false;
            b_joint_5 = false;
            b_joint_6 = false;
            b_joint_7 = false;
        }*/


        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }


        /// <summary>
        /// 一轴停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button20_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            motor_Enable(0x01, 0);
            b_joint_1 = false;
            label2.BackColor = Color.Transparent;
            listBox1.Items.Add("关节1 " + "停止" + "!");
        }


        /// <summary>
        /// 二轴停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button21_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            motor_Enable(0x02, 0);
            b_joint_2 = false;
            label3.BackColor = Color.Transparent;
            listBox1.Items.Add("关节2 " + "停止" + "!");
            b_joint2_rcv = false;
        }


        /// <summary>
        /// 三轴停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button22_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            fun_brake(0x03, 0);
            motor_Enable(0x03, 0);
            b_joint_3 = false;
            label4.BackColor = Color.Transparent;
            listBox1.Items.Add("关节3 " + "停止" + "!");

        }


        /// <summary>
        /// 四轴停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button23_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            fun_brake(0x04, 0);
            motor_Enable(0x04, 0);
            b_joint_4 = false;
            label5.BackColor = Color.Transparent;
            listBox1.Items.Add("关节4 " + "停止" + "!");
        }


        /// <summary>
        /// 五轴停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button24_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            motor_Enable(0x05, 0);
            b_joint_5 = false;
            label6.BackColor = Color.Transparent;
            listBox1.Items.Add("关节5 " + "停止" + "!");
        }


        /// <summary>
        /// 六轴停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    /*  private void button25_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            motor_Enable(0x06, 0);
            b_joint_6 = false;
            label7.BackColor = Color.Transparent;
            listBox1.Items.Add("关节6 " + "停止" + "!");
        }*/


        /// <summary>
        /// 七轴停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    /*  private void button26_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                listBox1.Items.Add("请正确输入串口号！");
                return;
            }
            motor_Enable(0x07, 0);
            b_joint_7 = false;
            label8.BackColor = Color.Transparent;
            listBox1.Items.Add("关节7 " + "停止" + "!");
        }*/

        /// <summary>
        /// 输出中的else if语句格式
        /// </summary>
        /*
        if((radioButton1.Checked == true)||(radioButton3.Checked == true) ||(radioButton4.Checked == true))
        {}
        else if (radioButton2.Checked == true)
        {}
        else if (radioButton5.Checked == true)
        {}
        else if (radioButton6.Checked == true)
        {}
        else if (radioButton7.Checked == true)
        {}
        else if (radioButton8.Checked == true)
        {}
        else if (radioButton9.Checked == true)
        {}
        else if (radioButton10.Checked == true)
        {}    */















    }
}
