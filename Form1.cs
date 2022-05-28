using IronOcr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BombParty_Test
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static Image CaptureDesktop()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }
        public static int startx = 775;
        
        public static int starty = 525;
        public static int width = 65;
        public static int height = 85;
        public static Bitmap CaptureWindow(IntPtr handle)
        {
            try
            {
                var rect = new Rect();
                GetWindowRect(handle, ref rect);
                var bounds = new Rectangle(startx, starty, width, height);
                var result = new Bitmap(bounds.Width, bounds.Height);

                using (var graphics = Graphics.FromImage(result))
                {
                    graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }

                return result;
            }
            catch (Exception e) {
                MessageBox.Show("Invalid size, please retry the top left and bottom right." + startx +"" +starty + "" + width + " " +height, "help", MessageBoxButtons.OK);
                startx = 775;
                starty = 775;
                width = 65;
                height = 85;


                return null;
            }
        }




        public Form1()
        {
            InitializeComponent();
            

        }
        void CheckEveryWord()
        {
            string[] reader = File.ReadAllLines(@"D:\words.txt");
            bool trueorfalse = false;
            textBox1.Text = "";
            int words = 0;
            
            for (int i = 0; i < reader.Length; i++)
            {
                if (lettercombo != "")
                {
                    if (reader[i].Contains(lettercombo.ToLower()))
                    {
                        textBox1.Text = textBox1.Text + reader[i] + Environment.NewLine;

                        reader[i] = reader[i].Replace(lettercombo.ToLower(), " " + lettercombo.ToUpper() + " ");

                        textBox1.Text = textBox1.Text + reader[i] + Environment.NewLine + lettercombo + Environment.NewLine;
                        words++;
                        trueorfalse = true;
                        if (words == 3)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (trueorfalse == false)
            {
                textBox1.Text = "no word or detection error";
            }
            
        }
        string lettercombo = "";
        private void button1_Click(object sender, EventArgs e)
        {

            timer1.Start();
        }
        public Bitmap pastimage;
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            string currentword = lettercombo;
            var image = CaptureDesktop();
            pictureBox1.Image = image;
            if (pastimage != image)
            {
                var OCR = new IronTesseract();
                using (var Input = new OcrInput(image))
                {
                    //Input.Deskew();  // use if image not straight
                    //Input.DeNoise(); // use if image contains digital noise
                   
                    
                    Input.Contrast();
                    Input.Invert();
                    
                    //Input.ToGrayScale();
                    var Result = OCR.Read(Input);
                   
                    lettercombo = Result.Text;
                    if (lettercombo.Contains("l"))
                    {
                        lettercombo = lettercombo.Replace('l', 'I');
                    }
                    
                }
                if (lettercombo != currentword)
                {
                    CheckEveryWord();
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            MessageBox.Show("Put your cursor to the top left of where you want.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            Thread.Sleep(3000);
            
            startx = Cursor.Position.X;
            starty = Cursor.Position.Y;
            MessageBox.Show("Done", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            MessageBox.Show("Put your cursor to the bottom right of where you want and wait.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            Thread.Sleep(3000);
            
            width = Cursor.Position.X - startx;
            height = Cursor.Position.Y - starty;
            MessageBox.Show("Done", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            timer1.Start();
        }
    }
}
