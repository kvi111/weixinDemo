using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace weixinDemo
{
    public partial class FormLogin : Form
    {
        public static FormLogin instance;
        public static FormMain formMain;
        public StartUI startUI;
        public Thread t;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            instance = this;
            formMain = new FormMain();
            startUI = new StartUI();

            t = new Thread(new ThreadStart(StartWeixin));
            //t.IsBackground = true;
            t.Start();
        }
        
        public void StartWeixin()
        {
            startUI.start();
        }

        public void ShowMain()
        {
            MethodInvoker MethInvo = new MethodInvoker(ShowMain1);
            BeginInvoke(MethInvo);
        }
        public void ShowMain1()
        {
            formMain.Show();
        }

        public void SetQRImage(Byte[] data)
        {
            Image image = Utils.BytesToImage(data);
            if (pictureBox1.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                //Action<Image> actionDelegate = (x) => { pictureBox1.Image = x; };
                // 或者
                Action<Image> actionDelegate = delegate (Image img) { pictureBox1.Image = img; };
                pictureBox1.Invoke(actionDelegate, image);
            }
            else
            {
                pictureBox1.Image = image;
            }
            //pictureBox1.Image = Utils.BytesToImage(data);
        }

        public void SetVisable(bool isVisable)
        {
            if (this.InvokeRequired)
            {
                Action<bool> actionDelegate = delegate (bool isVis) { this.Visible = isVis; };
                this.Invoke(actionDelegate, isVisable);
            }
            else
            {
                this.Visible = isVisable;
            }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormClose();
        }

        public void FormClose()
        {
            try
            {
                t.Abort();
            }
            catch
            {

            }
        }
    }
}
