﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WindowsFormsApp1.AdjustedForm
{
    public partial class ChangeImageSizeForm : Form
    {
        [DllImport("imgFunc.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        //xtimes ：x軸放大倍數(double,>0.1)
        //ytimes ：x軸放大倍數(double,>0.1)
        //isfullsize ：記憶體大小是否隨著放大縮小而改變 (true：隨著新圖改變大小，false：不改變大小，可直接設成true)
        static extern void changeImageSize(IntPtr src, int width, int height, double xtimes, double ytimes, bool isfullsize, out int dst_width, out int dst_height, out IntPtr dstBuffer);

        Form1 topForm;
        Mat source;

        public ChangeImageSizeForm()
        {
            InitializeComponent();
        }

        public ChangeImageSizeForm(Form1 topForm) : this()
        {
            this.topForm = topForm;
            source = BitmapConverter.ToMat(topForm.pictureBox.Image as Bitmap);
        }

        private void OpenResizeForm(object sender, EventArgs e)
        {
            Bitmap src = BitmapConverter.ToBitmap(source);

            Form resizeForm = new Form();
            resizeForm.BackgroundImage = src;
            resizeForm.BackgroundImageLayout = ImageLayout.Stretch;
            resizeForm.Width = (int)((float)src.Width * float.Parse(widthTextBox.Text));
            resizeForm.Height = (int)((float)src.Height * float.Parse(HeightTextBox.Text));
            resizeForm.SizeChanged += new EventHandler(this.Resize_FormResize);
            resizeForm.FormClosed += new FormClosedEventHandler(this.Resize_FormClosed);
            resizeForm.Text = "調整窗口大小來調整圖片大小";

            widthTextBox.Enabled = false;
            HeightTextBox.Enabled = false;

            resizeForm.Show();
        }

        private void Resize_FormResize(object sender, EventArgs e)
        {
            Form resizeForm = sender as Form;
            HeightTextBox.Text = ((float)resizeForm.Height / (float)source.Height).ToString();
            widthTextBox.Text = ((float)resizeForm.Width / (float)source.Width).ToString();
        }

        public void Resize_FormClosed(object sender, FormClosedEventArgs e)
        {
            widthTextBox.Enabled = true;
            HeightTextBox.Enabled = true;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            double xtime = double.Parse(widthTextBox.Text), ytime = double.Parse(HeightTextBox.Text);
            Mat src = source.Clone();
            changeImageSize(src.Data, src.Width, src.Height, xtime, ytime, true, out int dstWidth, out int dstHeight, out IntPtr dst);
            Mat dstImage = new Mat(dstHeight, dstWidth, MatType.CV_8UC3, dst);

            topForm.pictureBox.Image = BitmapConverter.ToBitmap(dstImage);
        }
    }
}
