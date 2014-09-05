using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VevisoftDLLTest.IdentifyingCode
{
    public partial class FrmCode : Form
    {
        public FrmCode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var opdiag=new OpenFileDialog();
            opdiag.Filter = "图像文件(*.bmp;*.jpg;*gif;*png;*.tif;*.wmf)|"
                        + "*.bmp;*jpg;*gif;*png;*.tif;*.wmf";
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = opdiag.FileName;
                pictureBox1.Image = Image.FromFile(opdiag.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCodeWeb(textBox1.Text, 1014);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var rect = new Vevisoft.WindowsAPI.SystemWindowsAPI.RECT();
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowRect(this.Handle, ref rect);
            int x = rect.Left + pictureBox1.Location.X;
            int y = rect.Top + pictureBox1.Location.Y;
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;
            //
            textBox3.Text= Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCode(x, y, width, height);
        }
    }
}
