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
using System.Collections;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //刪除不需要的PictureBox
        //除pictureBox1固定不刪除外，參數中的pictureBox也不刪除。其餘pictureBox刪除
        private void releaseOtherPictureBox(params PictureBox[] pPicBox)
        {
            try
            {
                for (int i = this.Controls.Count - 1; i >= 0; i--)
                {
                    if (this.Controls[i] is PictureBox)
                    {
                        if (!this.Controls[i].Equals(pictureBox1)) // pictureBox1固定不刪除
                        {
                            if (!pPicBox.Contains(this.Controls[i])) // 包含在傳入的參數內之pictureBox也不刪除
                            {
                                this.Controls[i].Dispose();
                            }
                        }
                    }
                }
            }
            catch { };
        }

        //載入圖片
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG(*.jpg)|*.jpg|" + "BMP(*.bmp)|*.bmp|" + "GIF(*.gif)|*.gif|" + "PNG(*.png)|*.png|" + "所有檔案|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                releaseOtherPictureBox();
                for (int i = 0; i < pbBWTLevel.Length; i++)
                {
                    pbBWTLevel[i] = false;
                }
                pictureBox1.Width = Image.FromFile(openFileDialog.FileName).Width;
                pictureBox1.Height = Image.FromFile(openFileDialog.FileName).Height;
                pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
            }
        }

        PictureBox[] pbBox = new PictureBox[3];
        bool[] pbBWTLevel = new bool[] { false, false, false };
        bool[] pbVisible = new bool[] { false, false, false };
        Bitmap[] pbmBWT = new Bitmap[4];
        int[] pnColor = new int[3];

        // 1~3階小波轉換
        private void buttonDWT_Click(object sender, EventArgs e)
        {
            DialogResult result;
            while (pictureBox1.Image == null)
            {
                result = MessageBox.Show("請載入圖片", "載入圖片提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    button1.PerformClick();
                }
                else
                {
                    return;
                }
            }

            if (((Button)sender).Equals(button3))
            {
                //pbBWTLevel[1] = false;
                //pbBWTLevel[2] = false;
                pbVisible[0] = true;
                pbVisible[1] = false;
                pbVisible[2] = false;

                //releaseOtherPictureBox(pbBox[0]);
                if (!pbBWTLevel[0])
                    button3_Click(button3, null);
                for (int i = 0; i < pbBox.Length; i++)
                {
                    if (pbBox[i] != null)
                        pbBox[i].Visible = pbVisible[i];
                }

            }
            else if (((Button)sender).Equals(button4))
            {
                pbVisible[0] = true;
                pbVisible[1] = true;
                pbVisible[2] = false;

                if (!pbBWTLevel[0])
                    button3_Click(button3, null);
                if (!pbBWTLevel[1])
                    button3_Click(button4, null);
                for (int i = 0; i < pbBox.Length; i++)
                {
                    if (pbBox[i] != null)
                        pbBox[i].Visible = pbVisible[i];
                }

            }
            else if (((Button)sender).Equals(button5))
            {
                pbVisible[0] = true;
                pbVisible[1] = true;
                pbVisible[2] = true;
                if (!pbBWTLevel[0])
                    button3_Click(button3, null);
                if (!pbBWTLevel[1])
                    button3_Click(button4, null);
                if (!pbBWTLevel[2])
                    button3_Click(button5, null);
                for (int i = 0; i < pbBox.Length; i++)
                {
                    if (pbBox[i] != null)
                        pbBox[i].Visible = pbVisible[i];
                }

            }
        }

        // 1~3階小波轉換
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //宣告寬及高
                int w1 = pictureBox1.Image.Width;
                int h1 = pictureBox1.Image.Height;
                int nLo = 0;
                int nPad = 32;
                int x, y;

                if (pbmBWT[0] == null)
                    pbmBWT[0] = (Bitmap)pictureBox1.Image.Clone();
                if (pbmBWT[1] == null)
                    pbmBWT[1] = (Bitmap)pictureBox1.Image.Clone();
                if (pbmBWT[2] == null)
                    pbmBWT[2] = (Bitmap)pictureBox1.Image.Clone();
                if (pbmBWT[3] == null)
                    pbmBWT[3] = (Bitmap)pictureBox1.Image.Clone();

                if (((Button)sender).Equals(button3))   // 1階小波轉換要處理的影像區域
                {
                    w1 = pictureBox1.Image.Width;
                    h1 = pictureBox1.Image.Height;
                    nLo = 0;
                    pbmBWT[3] = (Bitmap)pictureBox1.Image.Clone();
                    nPad = 32;
                }
                else if (((Button)sender).Equals(button4)) // 2階小波轉換要處理的影像區域
                {
                    w1 = pictureBox1.Image.Width / 2;
                    h1 = pictureBox1.Image.Height / 2;
                    nLo = 1;
                    nPad = 48;
                    pbmBWT[3] = (Bitmap)pbmBWT[0].Clone();
                }
                else if (((Button)sender).Equals(button5))  // 3階小波轉換要處理的影像區域
                {
                    w1 = pictureBox1.Image.Width / 4;
                    h1 = pictureBox1.Image.Height / 4;
                    nLo = 2;
                    nPad = 64;
                    pbmBWT[3] = (Bitmap)pbmBWT[1].Clone();
                }

                //掃影像每一個pixel並做處理
                // 水平分割
                for (y = 0; y < h1; y++)
                {
                    for (x = 0; x < w1; x++)
                    {
                        Color c1 = pbmBWT[3].GetPixel(x, y);
                        if (x < w1 / 2)   // DWT左半部程式碼
                        {
                            pnColor[0] = (pbmBWT[3].GetPixel(x * 2, y).R + pbmBWT[3].GetPixel(x * 2 + 1, y).R) / 2;
                            pnColor[1] = (pbmBWT[3].GetPixel(x * 2, y).G + pbmBWT[3].GetPixel(x * 2 + 1, y).G) / 2;
                            pnColor[2] = (pbmBWT[3].GetPixel(x * 2, y).B + pbmBWT[3].GetPixel(x * 2 + 1, y).B) / 2;
                        }
                        else if (x >= w1 / 2) // DWT右半部程式碼
                        {
                            if (w1 % 2 == 1 && x == w1 - 1) // odd columns
                            {
                                pnColor[0] = pbmBWT[3].GetPixel((x - (w1 / 2)) * 2, y).R / 2;
                                pnColor[1] = pbmBWT[3].GetPixel((x - (w1 / 2)) * 2, y).G / 2;
                                pnColor[2] = pbmBWT[3].GetPixel((x - (w1 / 2)) * 2, y).B / 2;
                            }
                            else
                            {
                                pnColor[0] = Math.Abs((pbmBWT[3].GetPixel((x - (w1 / 2)) * 2, y).R - pbmBWT[3].GetPixel((x - (w1 / 2)) * 2 + 1, y).R)) / 2 + nPad; // 
                                pnColor[1] = Math.Abs((pbmBWT[3].GetPixel((x - (w1 / 2)) * 2, y).G - pbmBWT[3].GetPixel((x - (w1 / 2)) * 2 + 1, y).G)) / 2 + nPad; // 
                                pnColor[2] = Math.Abs((pbmBWT[3].GetPixel((x - (w1 / 2)) * 2, y).B - pbmBWT[3].GetPixel((x - (w1 / 2)) * 2 + 1, y).B)) / 2 + nPad; // 
                            }
                        }
                        pbmBWT[2].SetPixel(x, y, Color.FromArgb(pnColor[0], pnColor[1], pnColor[2]));
                    }
                }

                // 垂直分割                
                for (y = 0; y < h1; y++)
                {
                    for (x = 0; x < w1; x++)
                    {
                        Color c1 = pbmBWT[1].GetPixel(x, y);
                        if (y < h1 / 2) // DWT上半部程式碼
                        {
                            ///
                            ///  寫入DWT上半部的程式碼
                            ///
                            pnColor[0] = (pbmBWT[2].GetPixel(x, y * 2).R + pbmBWT[2].GetPixel(x, y * 2 + 1).R) / 2;
                            pnColor[1] = (pbmBWT[2].GetPixel(x, y * 2).G + pbmBWT[2].GetPixel(x, y * 2 + 1).G) / 2;
                            pnColor[2] = (pbmBWT[2].GetPixel(x, y * 2).B + pbmBWT[2].GetPixel(x, y * 2 + 1).B) / 2;
                        }
                        else if (y >= h1 / 2) // DWT下半部程式碼
                        {
                            if (h1 % 2 == 1 && y == h1 - 1) // odd rows
                            {
                                ///
                                ///  寫入DWT下半部的程式碼1
                                ///
                                pnColor[0] = pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2).R / 2;
                                pnColor[1] = pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2).G / 2;
                                pnColor[2] = pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2).B / 2;
                            }
                            else
                            {
                                ///
                                ///  寫入DWT下半部的程式碼2
                                ///
                                pnColor[0] = Math.Abs((pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2).R - pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2 + 1).R)) / 2 + nPad; // 
                                pnColor[1] = Math.Abs((pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2).G - pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2 + 1).R)) / 2 + nPad; // 
                                pnColor[2] = Math.Abs((pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2).B - pbmBWT[2].GetPixel(x, (y - (h1 / 2)) * 2 + 1).R)) / 2 + nPad; // 
                            }
                        }
                        pbmBWT[3].SetPixel(x, y, Color.FromArgb(pnColor[0], pnColor[1], pnColor[2]));
                    }
                }

                int n = 0;
                if (((Button)sender).Equals(button3))
                {
                    n = 0;
                }
                else if (((Button)sender).Equals(button4))
                {
                    n = 1;
                }
                else if (((Button)sender).Equals(button5))
                {
                    n = 2;
                }

                pbBox[n] = new System.Windows.Forms.PictureBox();
                pbBox[n].Location = new System.Drawing.Point(10 + nLo * (pictureBox1.Width + 1), 40 + pictureBox1.Height);
                //pbBox[n].Name = "pictureBox2";
                pbBox[n].Size = new System.Drawing.Size(100, 50);
                pbBox[n].SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
                pbBox[n].TabIndex = 5;
                pbBox[n].TabStop = false;
                this.Controls.Add(pbBox[n]);
                //處理完後放回pictureBox
                pbBox[n].Image = pbmBWT[3];
                pbBox[n].Refresh();
                if (((Button)sender).Equals(button3))
                {
                    pbmBWT[0] = pbmBWT[3];
                    pbBWTLevel[0] = true;
                    pbBWTLevel[1] = false;
                    pbBWTLevel[2] = false;
                }
                else if (((Button)sender).Equals(button4))
                {
                    pbmBWT[1] = pbmBWT[3];
                    pbBWTLevel[1] = true;
                    pbBWTLevel[2] = false;
                }
                else if (((Button)sender).Equals(button5))
                {
                    pbmBWT[2] = pbmBWT[3];
                    pbBWTLevel[2] = true;
                }
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
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the  
                // File type selected in the dialog box.  
                // NOTE that the FilterIndex property is one-based. 
                string filename1 = Path.GetDirectoryName(saveFileDialog1.FileName.ToString());
                string filename2 = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName.ToString());
                string filename = filename1 + "/" + filename2;
                Image img = pictureBox1.Image;
                int nSaveCount = pbmBWT.Length - 1;


                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        img.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        for (int i = 0; i < nSaveCount; i++)
                        {
                            img = (Image)pbmBWT[i];
                            img.Save(filename + i + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        break;

                    case 2:
                        img.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        for (int i = 0; i < nSaveCount; i++)
                        {
                            img = (Image)pbmBWT[i];
                            img.Save(filename + i + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                        break;

                    case 3:
                        img.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                        for (int i = 0; i < nSaveCount; i++)
                        {
                            img = (Image)pbmBWT[i];
                            img.Save(filename + i + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                        }
                        break;
                }
            }
        }
    }
}
