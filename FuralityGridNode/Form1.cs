using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace FuralityGridNode
{
    public partial class Form1 : Form
    {
        static int size = 16;
        static int countY = 13;
        static int countX = (512 / countY + 1);
        //static Bitmap bmp;

        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            form = this;
            this.MouseDown += new MouseEventHandler(MainForm_MouseDown);
            g.Clear(Color.Black);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        [DllImport("gdi32.dll")]
        public static extern int StretchDIBits(
            IntPtr hdc,
            int xDest,
            int yDest,
            int DestWidth,
            int DestHeight,
            int xSrc,
            int ySrc,
            int SrcWidth,
            int SrcHeight,
            IntPtr lpBits,
            ref BITMAPINFO lpBitsInfo,
            uint iUsage,
            uint dwRop);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // Define the necessary constants
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        public static Graphics g;
        public static Form1 form;
        static byte[] combinedData = new byte[512*8];

        static int listenPort = 6454;
        static IPAddress listenAddress = IPAddress.Loopback;
        static UdpClient listener;
        static Thread listenerThread;
        private static void StartListener()
        {
            statusText = "listener started";
            if (listener != null)
                listener.Close();
            try
            {
                listener = new UdpClient(listenPort);
            }
            catch(Exception e)
            {
                statusText = e.Message;
                return;
            }
            
            IPEndPoint groupEP = new IPEndPoint(listenAddress, listenPort);

            while (true)
            {
                if (listenAddress == null)
                    return;

                statusText = "listening on " + listenAddress.ToString() + ":" + listenPort;
                byte[] bytes = listener.Receive(ref groupEP);

                int opcode = (bytes[8 + 1] << 8) | bytes[8 + 0];
                if (opcode == 0x5000)
                {
                    int universe = (bytes[12 + 3] << 8) | bytes[12 + 2];
                    if (universe < 8)
                    {
                        for (int i = 0; i < 512; i++)
                        {
                            combinedData[i + universe * 512] = bytes[i + 18];
                        }
                        update = true;
                    }
                }
            }

            listener.Close();
        }

        static bool update = false;
        static string statusText = "";
        static string statusTextLast = "";

        void DrawSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte R, byte G, byte B)
        {
            for (int x = X; x < X + sizeX; x++)
            {
                for (int y = Y; y < Y + sizeY; y++)
                {
                    int index = (x + y * dataSizeX) * 3;
                    data[index + 0] = B; // B
                    data[index + 1] = G;
                    data[index + 2] = R;
                }
            }
        }

        public void DrawData()
        {
            int sizeX = countX * 16 * 3;
            int sizeY = countY * 16;
            byte[] output = new byte[sizeX * sizeY * 3];

            string selectedItem = colorTypeDropdown.SelectedItem as string;

            if (selectedItem == "VRSL")
            {
                for (int universe = 0; universe < 3; universe++)
                {
                    for (int i = 0; i < 512; i++)
                    {
                        byte data = combinedData[i + universe * 512];
                        int x = i / 13;
                        int y = i % 13;
                        DrawSquare(output, sizeX, sizeY, (x + universe * countX) * 16, y * 16, 16, 16, data, data, data);
                    }
                }
            }
            else if (selectedItem == "Packed")
            {
                for (int i = 0; i < ((512*8) / 3); i++)
                {
                    byte dataR = combinedData[i * 3 + 0];
                    byte dataG = combinedData[i * 3 + 1];
                    byte dataB = combinedData[i * 3 + 2];

                    int x = i / 13;
                    int y = i % 13;
                    DrawSquare(output, sizeX, sizeY, x * 16, y * 16, 16, 16, dataR, dataG, dataB);
                }
            }
            if (output == null)
                return;

            // win32 fast draw byte[] to a control. Idk why the default C# functions are so disgustingly slow.
            GCHandle pinnedArray = GCHandle.Alloc(output, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            IntPtr hdc = g.GetHdc();
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader = new BITMAPINFOHEADER
            {
                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = sizeX,
                biHeight = -sizeY, // Negative height to indicate a top-down DIB
                biPlanes = 1,
                biBitCount = 24,
                biCompression = 0, // BI_RGB
                biSizeImage = (uint)(sizeY * sizeX)
            };
            StretchDIBits(hdc, 0, 0, sizeX, sizeY, 0, 0, sizeX, sizeY, pointer, ref bmi, 0, 0x00CC0020);
            g.ReleaseHdc(hdc);
            pinnedArray.Free();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RestartClient();
        }

        void RestartClient()
        {
            statusText = "listener starting";

            int.TryParse(portInput.Text, out listenPort);
            IPAddress.TryParse(ipInput.Text, out listenAddress);

            if (listenAddress == null)
            {
                statusText = "invalid IP";
                return;
            }

            if (listenPort == 0)
            {
                statusText = "invalid Port";
                return;
            }

            if (listenerThread != null)
                listenerThread.Abort();

            listenerThread = new Thread(new ThreadStart(StartListener));
            listenerThread.Start();
        }

        private void Config_Click(object sender, EventArgs e)
        {
            configPanel.Visible = !configPanel.Visible;
            Application.DoEvents();
            DrawData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (update)
            {
                update = false;
                DrawData();
            }

            if (statusTextLast != statusText)
            {
                statusLabel.Text = statusText;
                statusTextLast = statusText;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void colorTypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawData();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void inputChanged_TextChanged(object sender, EventArgs e)
        {
            RestartClient();
            DrawData();
        }

        private void PackDMX_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "Image files (*.png) | *.png";

            if (f.ShowDialog() != DialogResult.OK) // Make sure the user clicked ok
                return;

            if (!File.Exists(f.FileName)) // Make sure the file exists
                return;

            var (data, sizeX, sizeY) = Utils.ReadPng(f.FileName);

            if(data == null) // Make sure it loaded correctly
                return;

            int pixelsX = sizeX / 16;
            int pixelsY = sizeY / 16;

            List<(float x, float y)> Coords = new List<(float x, float y)> ();

            int FilledPixelCount = 0;
            for (int x = 0; x < pixelsX; x++)
            {
                for (int y = 0; y < pixelsY; y++)
                {
                    // Check if all 16 pixels are fully transparent
                    bool found = true;
                    for (int X = 0; X < 16; X++)
                    {
                        for (int Y = 0; Y < 16; Y++)
                        {
                            int lX = x * 16 + X;
                            int lY = y * 16 + Y;

                            int i = lX + lY * sizeX;
                            byte R = data[i * 4 + 0];
                            byte G = data[i * 4 + 1];
                            byte B = data[i * 4 + 2];
                            byte A = data[i * 4 + 3];
                            if (A > 0)
                            {
                                found = false;
                                break;
                            }
                        }
                        if (!found)
                            break;
                    }

                    // If they are, fill the pixel with checkerboard
                    if (found)
                    {
                        // Save coordinate for json
                        float tX = (x * 16 + 8) / (float)sizeX;
                        float tY = (y * 16 + 8) / (float)sizeY;
                        Coords.Add((tX, tY));

                        FilledPixelCount++;
                        for (int X = 0; X < 16; X++)
                        {
                            for (int Y = 0; Y < 16; Y++)
                            {
                                int lX = x * 16 + X;
                                int lY = y * 16 + Y;

                                int i = lX + lY * sizeX;
                                data[i * 4 + 0] = 255;
                                data[i * 4 + 1] = 0;
                                data[i * 4 + 2] = 255;
                                data[i * 4 + 3] = 255;
                                if ((x % 2) == (y % 2)) // Pink and black checkerboard pattern
                                {
                                    data[i * 4 + 0] = 0;
                                    data[i * 4 + 1] = 0;
                                    data[i * 4 + 2] = 0;
                                    data[i * 4 + 3] = 255;
                                }
                                
                            }
                        }
                    }
                }
            }

            string json = JsonSerializer.Serialize(Coords, new JsonSerializerOptions { IncludeFields = true });
            json = json.Replace("Item1", "x");
            json = json.Replace("Item2", "y");
            File.WriteAllText("DMXPack.json", json);
            Utils.WritePng("DMXPack.png", data, sizeX, sizeY);
        }
    }
}
