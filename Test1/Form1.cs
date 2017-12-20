using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Generic;
using System.IO;


namespace FaceDetect {
    public partial class Form1 : Form {
        static string picupdateurl= ConfigurationManager.AppSettings["picupdateurl"];
        static int facelevel = int.Parse(ConfigurationManager.AppSettings["facelevel"]);
        static int continuousfacecount = int.Parse(ConfigurationManager.AppSettings["continuousfacecount"]);

        //人脸特征分类器
        private string haarXmlPath = "haarcascade_frontalface_alt2.xml";
        // web camera  
        private Capture capture;
        private bool capture_flag = true;
        private Image<Bgr, Byte> frame = null;
        Image<Gray, Byte> gray = null;
        //上一次是否处理完成
        private static object isFalg = new object();
        //计时器
        private System.Timers.Timer capture_tick;
        private int count = 0;
        Form2 f2 = new Form2();

        double scale = 1;
        Image<Bgr, Byte> smallframe = null;
        public Form1() {
            InitializeComponent();

            capture_tick = new System.Timers.Timer();
            capture_tick.Interval = 150;
            capture_tick.Enabled = Enabled;
            capture_tick.Stop();
            capture_tick.Elapsed += new ElapsedEventHandler(CaptureProcess);

            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// 人脸识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void CaptureProcess(object sender, EventArgs arg) {
            lock (isFalg) {
                try {
                    frame = new Image<Bgr, byte>(capture.QueryFrame().Bitmap);
                    if (frame != null) {
                        //smallframe = frame.Resize(1 / scale, Emgu.CV.CvEnum.Inter.Linear);//缩放摄像头拍到的大尺寸照片  
                        gray = frame.Convert<Gray, Byte>(); //将其转换为灰度  
                        gray._EqualizeHist();//均衡化

                        CascadeClassifier ccr = new CascadeClassifier(haarXmlPath);
                        Rectangle[] rects = ccr.DetectMultiScale(gray, 1.1, facelevel, new Size(20, 20), Size.Empty);
                        if (rects.Length > 0) {
                            count++;
                            //不中断检测到人脸规定次数，则保存图片
                            if (count == continuousfacecount) {
                                //按钮2点击保存图片
                                PushPic();
                                //button2.PerformClick();
                            }
                            foreach (Rectangle r in rects) {
                                frame.Draw(r, new Bgr(Color.Red), 2);//绘制检测框  
                            }
                        } else {
                            count = 0;
                        }
                        pictureBox1.Image = frame.Bitmap;
                        frame.Dispose();
                        gray.Dispose();
                    }
                } catch (Exception ex) {
                    MessageBox.Show("系统异常-> " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            string picPath = System.IO.Directory.GetCurrentDirectory()+@"\pic";
            if (!Directory.Exists(picPath)) {
                Directory.CreateDirectory(picPath);
            }
            capture = new Capture();
            capture_tick.Start();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            //提示窗体关闭
            f2.Close();
            // 停止定时器
            timer1.Stop();
        }

        private void PushPic() {
            try {
                Image toSave = this.frame.Bitmap;
                Random aa = new Random();
                string str = System.IO.Directory.GetCurrentDirectory();
                string picaddr = str+ @"\pic\" + aa.Next() + ".jpg";
                toSave.Save(picaddr);
                //base64字符串
                string base64Str = DH.Tools.ConvertTools.ConvertVal.ImgConvertBase64(picaddr, "jpg");
                RequistData rd = new RequistData() { base64String = base64Str };
                //string result = "true";
                string result = DH.Tools.NetWorkTools.WebApiTools.DoPost<string>(rd, picupdateurl);
                if (result.Equals("true")) {
                    this.BeginInvoke(new MethodInvoker(() => {
                        timer1.Interval = 500;
                        timer1.Start();
                        f2.ShowDialog();
                    }));
                }
                count = 0;
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        

        private void button1_Click(object sender, EventArgs e) {
            //if (capture == null) {
            //    try {
            //        capture = new Capture();
            //        capture_tick.Start();
            //    } catch (NullReferenceException except) {
            //        MessageBox.Show(except.Message);
            //    }
            //}

            //if (capture != null) {
            //    if (capture_flag) {
            //        capture_tick.Start();
            //        button1.Text = "停止";
            //    } else {
            //        capture_tick.Stop();
            //        button1.Text = "开始";
            //    }
            //    capture_flag = !capture_flag;
            //}
        }
    }

    public class RequistData {
        public string base64String { get; set; }
    }
}
