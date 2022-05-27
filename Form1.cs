using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;


namespace SendPicture
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap newBitmap;
        Image file;
        Boolean opened = false; //to check weather image is open in picture box or not
        int blurAmount = 1; //for a blur filter
        int[, ,] buffer;

        void openImage()
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                file = Image.FromFile(openFileDialog1.FileName);
                newBitmap = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = file;
                opened = true;
                buffer = new int[3,  newBitmap.Width, newBitmap.Height];
            }
        }

    void saveImage()
        {
            if (opened)
            {
                SaveFileDialog sfd = new SaveFileDialog(); // create a new save file dialog object
                sfd.Filter = "Images|*.png;*.bmp;*.jpg";
                ImageFormat format = ImageFormat.Png;// you want to store it in by default format
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = Path.GetExtension(sfd.FileName);
                    switch (ext)
                    {
                        case ".jpg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                    }
                    pictureBox1.Image.Save(sfd.FileName, format);
                }



            }
            else { MessageBox.Show("No image loaded, first upload image "); }

        }

        void reload()
        {
            if (!opened)
            {
                MessageBox.Show("Open an Image then apply changes");
            }
            else
            {
                if (opened)
                {
                    file = Image.FromFile(openFileDialog1.FileName);
                    pictureBox1.Image = file;
                    opened = true;
                }
            }
        }

        void blur()
        {
            if (!opened)
            {
                MessageBox.Show("Open an Image then apply changes");
            }
            else
            {
                for (int x = blurAmount; x <= newBitmap.Width - blurAmount; x++)
                {
                    for (int y = blurAmount; y <= newBitmap.Height - blurAmount; y++)
                    {
                        try
                        {
                           // int avgR = 0, avgG = 0, avgB = 0;
                            Color prevX = newBitmap.GetPixel(x - blurAmount, y);
                            Color nextX = newBitmap.GetPixel(x + blurAmount, y);
                            Color prevY = newBitmap.GetPixel(x, y - blurAmount);
                            Color nextY = newBitmap.GetPixel(x, y + blurAmount);

                            int avgR = (int)((prevX.R + nextX.R + prevY.R + nextY.R) / 4);
                            int avgG = (int)((prevX.G + nextX.G + prevY.G + nextY.G) / 4);
                            int avgB = (int)((prevX.B + nextX.B + prevY.B + nextY.B) / 4);

                            newBitmap.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));

                        }
                        catch (Exception) { }
                    }
                }
                pictureBox1.Image = newBitmap;
            }
        }

        void grayscale()
        {
            if (!opened)
            {
                MessageBox.Show("Open an Image then apply changes");
            }
            else
            {
                Image img = pictureBox1.Image; // storing image into img variable of image type from picturebox1
                Bitmap bmpInverted = new Bitmap(img.Width, img.Height);   // creating a Bitmap object to work with images defined by pixel data.

                ImageAttributes ia = new ImageAttributes(); //creating an object of imageattribute ia to change the attribute of images
                ColorMatrix cmPicture = new ColorMatrix(new float[][] //creating the color matrix object to change the colors or apply  image filter on image
                {
                    new float[]{0.299f, 0.299f, 0.299f, 0, 0},
                    new float[]{0.587f, 0.587f, 0.587f, 0, 0},
                    new float[]{0.114f, 0.114f, 0.114f, 0, 0},
                    new float[]{0, 0, 0, 1, 0},
                    new float[]{0, 0, 0, 0, 0}
                });
                ia.SetColorMatrix(cmPicture); //pass the color matrix to imageattribute object ia
                Graphics g = Graphics.FromImage(bmpInverted);   //create a new object of graphics named g
                                                          

                g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);


                g.Dispose(); //Releases all resources used by this Graphics.
                pictureBox1.Image = bmpInverted;
            }
        }

        void ReadPixels()
        {
            for (int x = 0; x < newBitmap.Width; x++)
            {
               
                for (int y = 0; y < newBitmap.Height; y++)
                {   
                   
                    Color c = newBitmap.GetPixel(x, y);

                    buffer[0, x, y] = c.R;
                    buffer[1, x, y] = c.G;
                    buffer[2, x, y] = c.B;
                }
            }
        }

        void sharpen()
        {
            if (!opened)
            {
                MessageBox.Show("Open an Image then apply changes");
            }
            else
            {
                ReadPixels();
                for (int x = 1; x < newBitmap.Width - 2; x++)
                {
                    for (int y = 1; y < newBitmap.Height - 2; y++)
                    {
                        int avgR = 0, avgG = 0, avgB = 0; 
                        avgR = (int)(buffer[0, x, y] + 0.5 * (buffer[0, x, y] - buffer[0, x - 1, y - 1]));
                        avgG = (int)(buffer[1, x, y] + 0.5 * (buffer[1, x, y] - buffer[1, x - 1, y - 1]));
                        avgB = (int)(buffer[2, x, y] + 0.5 * (buffer[2, x, y] - buffer[2, x - 1, y - 1]));
                        if(avgR > 255)
                        {
                            avgR = 255;
                        }
                        if(avgR < 0)
                        {
                            avgR = 0;
                        }
                        if (avgG > 255)
                        {
                            avgG = 255;
                        }
                        if (avgG < 0)
                        { 
                            avgG = 0;
                        }
                        if (avgB > 255)
                        {
                            avgB = 255;
                        }
                        if (avgB < 0)
                        {
                            avgB = 0;
                        }
                        newBitmap.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                    }
                }
                pictureBox1.Image = newBitmap;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void pictureBox(object sender, EventArgs e)
        {

        }
        private void button_Open(object sender, EventArgs e) //Create an Open Button
        {
            openImage();
        }

        private void button_Save(object sender, EventArgs e) //Create an Save Button
        {
            saveImage();
        }

        private void button2_Click(object sender, EventArgs e) //Create a Blur filter Button
        {
            blur();
        }  
        
        private void button3_Click(object sender, EventArgs e) //Create a Sharpen filter Button
        {
            sharpen();
        }

        private void button4_Click(object sender, EventArgs e) //Create a Gray scale filter Button
        {
            grayscale();
        }

        private void button6_Click(object sender, EventArgs e) //Create a None Button
        {
            reload();
        }

        private void updateBlur(object sender, EventArgs e) //Change amount of blur 1-5
        {
            blurAmount = int.Parse(trackBar1.Value.ToString());
        }

        private void label_Click(object sender, EventArgs e)
        {
            
        }

    }
}