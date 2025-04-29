using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FuralityGridNode
{
    public partial class Form1 : Form
    {
        static int size = 16;
        static int countY = 13;
        static int countX = (512 / countY + 1);

        private ArtNet artnetClient;

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
        static byte[] combinedData = new byte[512 * 8];

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
                    data[index + 0] = B; // R
                    data[index + 1] = G; // G
                    data[index + 2] = R; // B
                }
            }
        }

        public void DrawData()
        {
            combinedData = artnetClient.combinedData;
            int sizeX = countX * size * 3;
            int sizeY = countY * size;
            byte[] output = new byte[sizeX * sizeY * 3];

            string selectedItem = colorTypeDropdown.SelectedItem as string;
#if DEBUG
            //selectedItem = "FRig";
#endif

            if (selectedItem == "VRSL")
            {
                for (int universe = 0; universe < 3; universe++)
                {
                    for (int i = 0; i < 512; i++)
                    {
                        byte data = combinedData[i + universe * 512];
                        int x = i / 13;
                        int y = i % 13;
                        DrawSquare(output, sizeX, sizeY, (x + universe * countX) * size, y * size, size, size, data, data, data);
                    }
                }
            }
            else if (selectedItem == "Packed")
            {
                for (int i = 0; i < ((512 * 8) / 3); i++)
                {
                    byte dataR = combinedData[i * 3 + 0];
                    byte dataG = combinedData[i * 3 + 1];
                    byte dataB = combinedData[i * 3 + 2];

                    int x = i / 13;
                    int y = i % 13;
                    DrawSquare(output, sizeX, sizeY, x * size, y * size, size, size, dataR, dataG, dataB);
                }
            }
            else
            {
                if (currentRig != null)
                {
                    if (currentRig != null && currentRig.Fixtures != null)
                    {
                        int index = 0;
                        foreach (Fixture fixture in currentRig.Fixtures)
                        {
                            int gridX = fixture.GridChannel / countY;
                            int gridY = fixture.GridChannel % countY;

                            int pixelX = gridX * size;
                            int pixelY = gridY * size;

                            byte data = combinedData[fixture.UnityChannel - 1];

                            int color = fixture.GridColor;

                            DrawColorSquare(output, sizeX, sizeY, pixelX, pixelY, size, size, data, fixture.GridColor);

                            index++;
                        }
                    }
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

        private void DrawColorSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte dataIn, int selector)
        {
            for (int x = X; x < X + sizeX; x++)
            {
                for (int y = Y; y < Y + sizeY; y++)
                {
                    int index = (x + y * dataSizeX) * 3;
                    switch (selector) {
                        case 1:
                            data[index + 2] = dataIn; // R
                            break;
                        case 2:
                            data[index + 1] = dataIn; // G
                            break;
                        case 3:
                            data[index + 0] = dataIn; // B
                            break;
                        default:
                            data[index + 2] = dataIn; // R
                            data[index + 1] = dataIn; // G
                            data[index + 0] = dataIn; // B
                            break;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartArtNetClient();
        }

        void StartArtNetClient()
        {
            artnetClient = new ArtNet(ipInput.Text, portInput.Text);
            artnetClient.StartClient();
        }

        private void Config_Click(object sender, EventArgs e)
        {
            configPanel.Visible = !configPanel.Visible;
            Application.DoEvents();
            DrawData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
#if DEBUG
           //Trace.WriteLine($"ArtNet Status: {artnetClient.status}");
#endif
            if (artnetClient.status == ArtNet.ArtNetClientStatus.Waiting)
                update = true;
            if (artnetClient.status == ArtNet.ArtNetClientStatus.ReceivingData)
                update = true;
            if (artnetClient.status != ArtNet.ArtNetClientStatus.Disconnected)
                update = true;
            if (artnetClient.status != ArtNet.ArtNetClientStatus.Error)
                update = true;

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
            statusLabel.Text = artnetClient.status.ToString();
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
            DrawData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("Button1 Clicked");
            artnetClient.RestartClient(ipInput.Text, portInput.Text);
            //RestartClient();
            //StartArtNetClient();
        }

        private void selectRig_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "FRig Files (*.frig)|*.frig|All Files (*.*)|*.*";
                openFileDialog.Title = "Select FRig File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
#if DEBUG
                    Trace.WriteLine($"Yay it found the file, path: {filePath}");
#endif
                    LoadFRigFile(filePath);
                }
            }
        }

        private FRig currentRig;

        private void LoadFRigFile(string filePath)
        {
            try
            {
                // Use the static FromJson method here too for consistency and error handling
                FRigFile frigFile = new FRigFile();
                frigFile = frigFile.LoadFromFile(filePath);
                currentRig = frigFile.ConvertToFRig();
                if (currentRig != null)
                {
                    Trace.WriteLine($"FRig file loaded");
                    if (currentRig.Fixtures != null)
                        Trace.WriteLine($"FRig fixtures loaded successfully. Version: Micca Broke it, Fixtures: {currentRig.Fixtures.Length}");
                } else {
                     Trace.WriteLine($"Failed to load FRig file: {filePath}");
                     // Optionally show a MessageBox error here as well
                }
            }
            catch (Exception ex)
            {
                // Catch potential File IO errors or other unexpected exceptions
                MessageBox.Show($"Error loading FRig file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentRig = null; // Ensure rig is null on error
            }
        }
    }
}
