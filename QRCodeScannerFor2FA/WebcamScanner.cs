using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace QRCodeScannerFor2FA
{
    public partial class WebcamScanner : Form
    {
        Scanner scanner;

        FilterInfoCollection fiv;
        VideoCaptureDevice captureDevice;

        Timer timer;

        public string Result = "";

        public WebcamScanner()
        {
            InitializeComponent();
            scanner = new Scanner();
            fiv = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            comboBox1.Items.Clear();
            foreach (FilterInfo info in fiv)
            {
                comboBox1.Items.Add(info.Name);
            }
            comboBox1.SelectedIndex = 0;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                Result = scanner.Scan((Bitmap)pictureBox1.Image);
                if (Result != "")
                {
                    timer.Stop();
                    captureDevice.SignalToStop();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (captureDevice != null && captureDevice.IsRunning)
            {
                captureDevice.SignalToStop();
                //captureDevice.Stop();
            }
            captureDevice = new VideoCaptureDevice(fiv[comboBox1.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
        }

        private void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            this.Invoke(new Action(() =>
            {
                pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer.Stop();
            captureDevice.SignalToStop();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
