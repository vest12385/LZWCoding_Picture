using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics; //Stopwatch

namespace LZWCoding_Picture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //讀取檔案位址(need OpenFileDialog)
            openFileDialog1.Filter = "影像檔(*.jpg,*.jpge,*.bmp,*.gif,*.ico,*.png,*.tif,*.wmf)|*.jpg;*.jpge;*.bmp;*.gif;*.ico;*.png;*.tif;*.wmf";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                textBox1.Text = openFileDialog1.FileName;
            //textBox.Text.Substring(textBox1.Text.LastIndexOf(@"\") + 1, textBox1.Text.Count() - textBox1.Text.LastIndexOf(@"\") - 1)
            try
            {
                Image picture = new Bitmap(textBox1.Text);
                pictureBox1.Image = picture;
                Bitmap sr1 = new Bitmap(textBox1.Text);
                textBox4.Text = textBox1.Text.Substring(0, textBox1.Text.LastIndexOf(@"\"));
            }
                catch (Exception ex) { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox4.Text.Equals(""))
            {
                MessageBox.Show("尚有位置沒有填寫");
            }
            else
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                FileInfo file1 = new FileInfo(textBox4.Text + @"\EncodeDictionary.txt");
                FileInfo file2 = new FileInfo(textBox4.Text + @"\DecodeDictionary.txt");
                if (file1.Exists || file2.Exists)
                {
                    file1.Delete();
                    file2.Delete();
                }
                List<List<string>> PixelColor = getPixelColor(pictureBox1.Image);
                List<Dictionary<string, int>> dictionary = new List<Dictionary<string,int>>();
                List<string> outputColor = new List<string>();
                List<List<string>> PixelColorDecoder = new List<List<string>>();
                foreach( List<string> oneColor in PixelColor )
                {
                    LZWEncoder.LZW_Encode encoder = new LZWEncoder.LZW_Encode(oneColor, textBox4.Text);
                    dictionary.Add(encoder.getDictionary);
                    outputColor.Add(encoder.getOutput);
                    using (StreamWriter sw1 = new StreamWriter(textBox4.Text + @"\Output.txt"))
                    {
                        sw1.WriteLine( "one Color" );
                        sw1.Write(encoder.getOutput);
                    }
                }
                for (int i = 0; i < outputColor.Count(); i++ )
                {
                    LZWDecoder.LZW_Decode decoder = new LZWDecoder.LZW_Decode(dictionary[i], outputColor[i], textBox4.Text);
                    PixelColorDecoder.Add(decoder.getInput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList());
                }
                Image picture = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height, PixelFormat.Format24bppRgb);
                picture = reconstruction(PixelColorDecoder, picture);
                picture.Save(textBox4.Text + @"\" + textBox1.Text.Substring(textBox1.Text.LastIndexOf(@"\") + 1, textBox1.Text.LastIndexOf(@".") - textBox1.Text.LastIndexOf(@"\") - 1) + "_decode" + ".png", ImageFormat.Png);
                pictureBox2.Image = picture;
                //FileInfo f1 = new FileInfo(textBox4.Text + @"\Output.txt");
                //FileInfo f2 = new FileInfo(textBox4.Text + @"\Origin Input.txt");
                //textBox7.Text = f2.Length.ToString();
                //textBox8.Text = f1.Length.ToString();
                //double CompressionRate = (double)f2.Length / (double)f1.Length;
                //textBox6.Text = CompressionRate.ToString();
                sw.Stop();
                MessageBox.Show((Convert.ToDecimal(sw.ElapsedMilliseconds) / 1000).ToString() + "S");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
                //讀取資料夾位址(need FolderBrowserDialog)
                folderBrowserDialog1.SelectedPath = "";
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    textBox4.Text = folderBrowserDialog1.SelectedPath;
        }

        private List<List<string>> getPixelColor(Image picture)
        {
            List<List<string>> PixelColor = new List<List<string>>();
            List<string> R = new List<string>();
            List<string> G = new List<string>();
            List<string> B = new List<string>();
            Bitmap pictureBitmap = new Bitmap(picture);
            for (int pixelHight = 0; pixelHight < picture.Height; pixelHight++ )
            {
                for (int pixelWidth = 0; pixelWidth < picture.Width; pixelWidth++)
                {
                    R.Add(pictureBitmap.GetPixel(pixelWidth, pixelHight).R.ToString() + ",");
                    G.Add(pictureBitmap.GetPixel(pixelWidth, pixelHight).G.ToString() + ",");
                    B.Add(pictureBitmap.GetPixel(pixelWidth, pixelHight).B.ToString() + ",");
                }
            }
            PixelColor.Add(R);
            PixelColor.Add(G);
            PixelColor.Add(B);
            return PixelColor;
        }
        private Image reconstruction(List<List<string>> PixelColorDecoder, Image picture)
        {
            Bitmap pictureBitmap = new Bitmap(picture);
            for (int pixelHight = 0; pixelHight < picture.Height; pixelHight++)
            {
                for (int pixelWidth = 0; pixelWidth < picture.Width; pixelWidth++)
                {
                    pictureBitmap.SetPixel(pixelWidth, pixelHight, Color.FromArgb(Convert.ToByte(PixelColorDecoder[0][(picture.Height) * pixelHight + pixelWidth]),
                                                                                  Convert.ToByte(PixelColorDecoder[1][(picture.Height) * pixelHight + pixelWidth]),
                                                                                  Convert.ToByte(PixelColorDecoder[2][(picture.Height) * pixelHight + pixelWidth])));
                    //MessageBox.Show(pictureBitmap.GetPixel(pixelWidth, pixelHight).G.ToString());
                }
            }
            Bitmap clone = new Bitmap(pictureBitmap.Width, pictureBitmap.Height, PixelFormat.Format24bppRgb);
            //MessageBox.Show(pictureBox1.Image.PixelFormat.ToString());
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(pictureBitmap, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            //MessageBox.Show(clone.PixelFormat.ToString());
            return clone;
        }
    }
}
