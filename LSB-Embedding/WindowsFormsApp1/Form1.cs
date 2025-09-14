using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1_TextChanged(null, null);
        }

        // 讀檔
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG(*.jpg)|*.jpg|" + "BMP(*.BMP)|*.bmp|" + "GIF(*.GIT)|*.gif|" + "所有檔案|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Width = Image.FromFile(openFileDialog.FileName).Width;
                pictureBox1.Height = Image.FromFile(openFileDialog.FileName).Height;
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
            }
        }

        // 灰階處理
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //並宣告bm1為Bitmap
                Bitmap bm1 = (Bitmap)pictureBox1.Image;

                //掃影像每一個pixel並做處理
                for (int y = 0; y <= bm1.Height - 1; y++)
                {
                    for (int x = 0; x <= bm1.Width - 1; x++)
                    {
                        Color c1 = bm1.GetPixel(x, y);
                        ///
                        /// 請寫下灰階處理程式碼
                        ///
                        int a = c1.A;
                        int r = c1.R;
                        int g = c1.G;
                        int b = c1.B;
                        int avg = (r + g + b) / 3;
                        bm1.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                    }
                }
                //處理完後放回pictureBox1
                pictureBox1.Image = bm1;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        byte[] pByMask = new byte[] { 0x80, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };
        private System.Windows.Forms.PictureBox pictureBox2;
        Bitmap[] pbmBitPlane = new Bitmap[8];  //  8個位元平面
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                button2.PerformClick();
                //並宣告bm1為Bitmap
                Bitmap bm1 = (Bitmap)pictureBox1.Image;

                for (int i = 0; i < pbmBitPlane.Length; i++)
                {
                    pbmBitPlane[i] = (Bitmap)pictureBox1.Image.Clone();
                    //掃影像每一個pixel並做處理
                    for (int y = 0; y <= bm1.Height - 1; y++)
                    {
                        for (int x = 0; x <= bm1.Width - 1; x++)
                        {
                            Color c1 = pbmBitPlane[i].GetPixel(x, y);
                            ///
                            /// 請寫下取出位元平面的程式碼
                            ///
                            int color = c1.R & pByMask[i];
                            if (color != 0) { color = 128; }  // 更能看清楚圖片前後差別
                            pbmBitPlane[i].SetPixel(x, y, Color.FromArgb(color, color, color));
                        }
                    }

                    this.pictureBox2 = new System.Windows.Forms.PictureBox();
                    this.pictureBox2.Location = new System.Drawing.Point(pictureBox1.Width * (i % 3), 40 + pictureBox1.Height * (i / 3 + 1));
                    this.pictureBox2.Name = "pictureBox2";
                    this.pictureBox2.Size = new System.Drawing.Size(100, 50);
                    this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
                    this.pictureBox2.TabIndex = 5;
                    this.pictureBox2.TabStop = false;
                    this.Controls.Add(this.pictureBox2);
                    //處理完後放回pictureBox
                    pictureBox2.Image = pbmBitPlane[i];
                }
                this.ClientSize = new Size(pictureBox1.Width * 3, pictureBox1.Height * 4 + 40);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 存檔
        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the  
                // File type selected in the dialog box.  
                // NOTE that the FilterIndex property is one-based. 
                string filename1 = Path.GetDirectoryName(saveFileDialog1.FileName.ToString());
                string filename2 = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName.ToString());
                string filename = filename1 + "/" + filename2;
                Image img = pictureBox1.Image;

                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        img.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        for (int i = 0; i < pbmBitPlane.Length; i++)
                        {
                            img = (Image)pbmBitPlane[i];
                            img.Save(filename + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        break;

                    case 2:
                        img.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        for (int i = 0; i < pbmBitPlane.Length; i++)
                        {
                            img = (Image)pbmBitPlane[i];
                            img.Save(filename + i + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                        break;

                    case 3:
                        img.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                        for (int i = 0; i < pbmBitPlane.Length; i++)
                        {
                            img = (Image)pbmBitPlane[i];
                            img.Save(filename + i + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                        }
                        break;
                }
            }
        }

        byte[] byStr;
        byte[] byReStr;
        byte[] byHide;
        // 藏入
        private void button5_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text + '\0';  // 多藏入一個表示結束的byte
            byStr = Encoding.Default.GetBytes(str);
            byHide = new byte[8 * byStr.Length];
            int nCount = 0;
            int nTemp = 0;

            for (int i = 0; i < byStr.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    nTemp = (byStr[i] & pByMask[j]);
                    if (nTemp > 0)
                        byHide[i * 8 + j] = 1;
                    else
                        byHide[i * 8 + j] = 0;
                }
            }

            try
            {
                //並宣告bm1為Bitmap
                Bitmap bm1 = (Bitmap)pictureBox1.Image;

                //掃影像對每一個pixel做處理
                nCount = 0;
                for (int y = 0; y <= bm1.Height - 1; y++)
                {
                    for (int x = 0; x <= bm1.Width - 1; x++)
                    {
                        Color c1 = bm1.GetPixel(x, y);
                        int r1 = c1.R;
                        int g1 = c1.G;
                        int b1 = c1.B;
                        ///
                        /// 請寫下藏入LSB的程式碼
                        ///

                        #region LSB
                        r1 &= 0xFFFE;
                        r1 += byHide[nCount++];
                        bm1.SetPixel(x, y, Color.FromArgb(r1, g1, b1));
                        #endregion

                        if (nCount >= byHide.Length)
                            break;
                    }
                    if (nCount >= byHide.Length)
                        break;
                }
                //處理完後放回pictureBox1
                pictureBox1.Image = bm1;
                MessageBox.Show("藏入成功:" + str);
            }
            catch (NullReferenceException nullex)
            {
                MessageBox.Show(nullex.Message + "請先載入圖檔");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //取出
        private void button6_Click(object sender, EventArgs e)
        {
            int nCount1 = 0, nCount2 = 0;
            //並宣告bm1為Bitmap
            Bitmap bm1 = (Bitmap)pictureBox1.Image;
            //宣告寬及高
            int w1 = pictureBox1.Image.Width;
            int h1 = pictureBox1.Image.Height;
            if (w1 * h1 % 8 == 0)
                byReStr = new byte[w1 * h1 / 8];
            else
                byReStr = new byte[w1 * h1 / 8 + 1];

            try
            {
                //掃影像對每一個pixel做處理
                nCount1 = 0;
                nCount2 = 0;
                for (int y = 0; y <= h1 - 1; y++)
                {
                    for (int x = 0; x <= w1 - 1; x++)
                    {
                        Color c1 = bm1.GetPixel(x, y);
                        ///
                        /// 請寫下取出LSB的程式碼
                        ///

                        #region Take out LSB message.
                        int r = c1.R;
                        if (r %2 == 1)
                        {
                            byReStr[nCount1] = (byte)(byReStr[nCount1] | pByMask[nCount2 % 8]);
                        }
                        #endregion

                        if (nCount2 % 8 == 7)
                        {
                            if (byReStr[nCount1] == '\0')
                            {
                                break;
                            }
                            nCount1++;
                        }
                        nCount2++;
                    }
                    if (byReStr[nCount1] == '\0')
                    {
                        break;
                    }

                }
                MessageBox.Show(Encoding.Default.GetString(byReStr));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                button5.Enabled = false;
            }
            else
            {
                button5.Enabled = true;
            }

        }
    }
}
