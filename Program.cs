using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace consolepic
{
    class Program
    {
        private static string fPath = "";

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        [STAThread]
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            var handle = GetStdHandle(-11);
            int mode;
            GetConsoleMode(handle, out mode);
            SetConsoleMode(handle, mode | 0x4);


            string t = "space2a ; console picture drawer";
            for (int i = 0; i < t.Length; i++)
                Console.Write("\x1b[38;5;" + (i + new Random().Next(20, 200)) + "m" + t[i]);

            Console.ResetColor();

            Console.WriteLine("\n\nmenu : \n 'f' : image file\n 'd' : directory with multiple images");
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.F)
                {

                    if (Environment.OSVersion.ToString().IndexOf("Windows") != -1)
                    {
                        Console.Title = "space2a ; console picture drawer";
                        Console.CursorVisible = false;
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;;*.gif;";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                            fPath = openFileDialog.FileName;
                        else return;
                    }
                    else
                    {
                        Console.Write("Enter the image file path :");
                        fPath = Console.ReadLine();
                    }

                    if (fPath != "" && File.Exists(fPath))
                        DrawImage();
                }
                else if (key.Key == ConsoleKey.D)
                {
                    if (Environment.OSVersion.ToString().IndexOf("Windows") != -1)
                    {
                        Console.Title = "space2a ; console picture drawer";
                        Console.CursorVisible = false;
                        FolderBrowserDialog folder = new FolderBrowserDialog();
                        if (folder.ShowDialog() == DialogResult.OK)
                            fPath = folder.SelectedPath;
                        else return;
                    }
                    else
                    {
                        Console.Write("Enter the directory path :");
                        fPath = Console.ReadLine();
                    }

                    if (fPath != "" && Directory.Exists(fPath))
                    {
                        DirectoryInfo d = new DirectoryInfo(fPath);

                        while (true)
                        {
                            foreach (var file in d.GetFiles("*"))
                            {
                                if (file.Extension == ".png" || file.Extension == ".jpeg" || file.Extension == ".jpg" || file.Extension == ".gif")
                                {
                                    fPath = file.FullName;
                                    DrawImage(true, true);
                                    Thread.Sleep(25);
                                }
                            }
                        }
                    }
                }
                else continue;
            }


        }

        static void PrintColor(string text, ConsoleColor foregroundColor, bool fl = true)
        {
            Console.ForegroundColor = foregroundColor;
            if(fl)
                Console.WriteLine(text);
            else
                Console.Write(text);
            
            //Console.ResetColor();
        }

        static void DrawImage(bool skipWait = false, bool returnim = false)
        {
            if(!skipWait)
                PrintColor("creating image bitmap... ", ConsoleColor.Gray, false);
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(fPath);
            }
            catch (Exception) { return; }

            if (!skipWait)
                PrintColor("OK", ConsoleColor.Green, true);

            if (!skipWait)
                PrintColor("ready to draw, press 'enter' to continue...", ConsoleColor.Gray, false);
            if(!skipWait)
                Console.ReadLine();
            DateTime dateTime = DateTime.Now;

            int yL = Console.WindowHeight;
            if (yL >= Console.LargestWindowHeight) yL = Console.LargestWindowHeight - 1;

            Bitmap resized = resizeImage(bitmap, new Size(Console.WindowWidth , yL));

            Console.Clear();

            for (int y = 0; y < resized.Height; y++)
            {
                string xLine = "";
                for (int x = 0; x < resized.Width; x++)
                {
                    var p = resized.GetPixel(x, y);
                    xLine += "\x1b[48;2;" + p.R + ";" + p.G + ";" + p.B + "m ";
                }

                xLine += "\x1b[48;2;" + 0 + ";" + 0+ ";" + 0 + "m";
                try { Console.SetCursorPosition((Console.WindowWidth - resized.Width) / 2, Console.CursorTop); } catch (Exception) { }
                Console.WriteLine(xLine);
            }

            if (returnim)
                return;

            int oldX = Console.WindowWidth;
            int oldY = Console.WindowHeight;
            while (true) 
            { 
                Thread.Sleep(50);
                if (oldX != Console.WindowWidth || oldY != Console.WindowHeight)
                    DrawImage(true);
            }
        }

        static System.Drawing.Bitmap resizeImage(System.Drawing.Image imgToResize, Size size)
        {
            try
            {
                int sourceWidth = imgToResize.Width;
                int sourceHeight = imgToResize.Height;
                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;
                nPercentW = ((float)size.Width / (float)sourceWidth);
                nPercentH = ((float)size.Height / (float)sourceHeight);
                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;
                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);
                Bitmap b = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage((System.Drawing.Image)b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();
                return b;
            }
            catch (Exception)
            {
                Environment.Exit(-2);
            }
            return null;
        }

    }


}