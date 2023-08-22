using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace QRCodeScannerFor2FA
{
    public partial class ScreenScanner : Form
    {
        const int cGrip = 15;

        Scanner scanner;

        Timer timer;

        public string Result = "";

        public ScreenScanner()
        {
            InitializeComponent();
            scanner = new Scanner();
            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += Timer_Tick;
            timer.Start();

            Rectangle rect = new Rectangle(System.Drawing.Point.Empty, this.Size);
            Region region = new Region(rect);

            rect.X += 20;
            rect.Y += 50;
            rect.Width -= 40;
            rect.Height -= 70;
            //rect.Inflate(-1 * (this.Width / 3), -1 * (this.Height / 3));
            region.Exclude(rect);

            this.Region = region;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                //Point pos = new Point(m.LParam.ToInt32());
                //pos = this.PointToClient(pos);
                //if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                //{
                //    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                //    return;
                //}
                m.Result = (IntPtr)2;  // HTCAPTION
                return;
            }
            base.WndProc(ref m);
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
        //    ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
        //}

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                Result = scanner.Scan(GetImageInHole());
                if (Result != "")
                {
                    timer.Stop();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }));
        }

        private Bitmap GetImageInHole()
        {
            //Bitmap bmp = new Bitmap(this.Size.Width, this.Size.Height);
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bmp);
            //g.CopyFromScreen(new System.Drawing.Point(this.Location.X, this.Location.Y), new System.Drawing.Point(0, 0), new System.Drawing.Size(this.Size.Width, this.Size.Height));
            g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), Screen.PrimaryScreen.Bounds.Size);
            g.Flush();
            g.Dispose();
            return bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer.Stop();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
