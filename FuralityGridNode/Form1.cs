using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FuralityGridNode
{
    public partial class Form1 : Form
    {
        static int size = 16;
        static int countY = 13;
        static int countX = 120;
        static bool customLayout;
        static bool unicastCheck = true;

        private ArtNet artnetClient;

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

        void DrawSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte R, byte G, byte B, byte A)
        {
            for (int x = X; x < X + sizeX; x++)
            {
                for (int y = Y; y < Y + sizeY; y++)
                {
                    int index = (x + y * dataSizeX) * 4;
                    data[index + 0] = B; // R
                    data[index + 1] = G; // G
                    data[index + 2] = R; // B
                    data[index + 3] = A; // A
                }
            }
        }

        public void DrawData(byte[] combinedData)
        {
            int sizeX = countX * size;
            int sizeY = countY * size;
            int previewScale = 2;
            byte[] output = new byte[sizeX * sizeY * 4];
            byte[] preview = new byte[countX * countY * 4 * (previewScale * previewScale)];

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
                        int channel = i + universe * 512;
                        byte data = combinedData[channel];
                        int x = channel / countY;
                        int y = channel % countY;

                        if (customLayout)
                        {
                            if (channel >= layoutMapping.Count)
                                continue;
                            x = layoutMapping[channel].x;
                            y = layoutMapping[channel].y;
                        }

                        DrawSquare(preview, countX * previewScale, countY* previewScale, x * previewScale, y * previewScale, previewScale, previewScale, data, data, data, 255);
                        DrawSquare(output, sizeX, sizeY, x * size, y * size, size, size, data, data, data, 255); // ((data > 0) ? (byte)255 : (byte)0)
                    }
                }
            }
            else if (selectedItem == "Packed")
            {
                for (int channel = 0; channel < ((512 * 8) / 3); channel++)
                {
                    byte dataR = combinedData[channel * 3 + 0];
                    byte dataG = combinedData[channel * 3 + 1];
                    byte dataB = combinedData[channel * 3 + 2];

                    int x = channel / countY;
                    int y = channel % countY;
                    if (customLayout)
                    {
                        if (channel >= layoutMapping.Count)
                            continue;
                        x = layoutMapping[channel].x;
                        y = layoutMapping[channel].y;
                    }
                    DrawSquare(preview, countX * previewScale, countY * previewScale, x * previewScale, y * previewScale, 1 * previewScale, 1 * previewScale, dataR, dataG, dataB, 255);
                    DrawSquare(output, sizeX, sizeY, x * size, y * size, size, size, dataR, dataG, dataB, 255);
                }
            }
            else if (selectedItem == "FRig")
            {
                if (currentRig != null && currentRig.Fixtures != null)
                {
                    int index = 0;
                    foreach (Fixture fixture in currentRig.Fixtures)
                    {
                        int gridX = fixture.GridChannel / countY;
                        int gridY = fixture.GridChannel % countY;
                        if (customLayout)
                        {
                            if (fixture.GridChannel >= layoutMapping.Count)
                                continue;
                            gridX = layoutMapping[fixture.GridChannel].x;
                            gridY = layoutMapping[fixture.GridChannel].y;
                        }
                        int pixelX = gridX * size;
                        int pixelY = gridY * size;

                        byte data = combinedData[fixture.UnityChannel - 1];

                        int color = fixture.GridColor;

                        DrawColorSquare(preview, countX * previewScale, countY * previewScale, gridX * previewScale, gridY * previewScale, 1 * previewScale, 1 * previewScale, data, fixture.GridColor);
                        DrawColorSquare(output, sizeX, sizeY, pixelX, pixelY, size, size, data, fixture.GridColor);

                        index++;
                    }
                }
                else
                {
                    //var frigFile = new FRigFile();
                    //var frigBaseFile = FuralityGridNode.Properties.Resources.FrigBaseDebug;
                    //var frigBase = Encoding.UTF8.GetString(frigBaseFile);
                    //currentRig = frigFile.LoadFromJsonString(frigBase).ConvertToFRig();
                }
            }
            if (output == null)
                return;

            SpoutWrapper.SendImage(output, sizeX, sizeY);
            
            // win32 fast draw byte[] to a control. Idk why the default C# functions are so disgustingly slow.
            GCHandle pinnedArray = GCHandle.Alloc(preview, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            Graphics gr = gridPreview.CreateGraphics();
            IntPtr hdc = gr.GetHdc();
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader = new BITMAPINFOHEADER
            {
                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = countX* previewScale,
                biHeight = -countY * previewScale, // Negative height to indicate a top-down DIB
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0, // BI_RGB
                biSizeImage = (uint)(countY * previewScale * countX * previewScale)
            };
            StretchDIBits(hdc, 8, 16, countX * previewScale, countY * previewScale, 0, 0, countX * previewScale, countY * previewScale, pointer, ref bmi, 0, 0x00CC0020);
            gr.ReleaseHdc(hdc);
            pinnedArray.Free();
        }

        private void DrawColorSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte dataIn, int selector)
        {
            for (int x = X; x < X + sizeX; x++)
            {
                for (int y = Y; y < Y + sizeY; y++)
                {
                    int index = (x + y * dataSizeX) * 4;
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
                            data[index + 4] = dataIn; // A
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
            SpoutWrapper.CreateSender("Furality Grid Node");
            layoutStatus.Text = $"VRSL\nsize: 1920x208\nchannels: 1560";
        }

        void StartArtNetClient()
        {
            artnetClient = new ArtNet(ipInput.Text, portInput.Text);
            artnetClient.Unicast = unicastCheck;
            artnetClient.StartClient();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (testAnimationTime > 0)
            {
                byte[] data;
                if (customLayout)
                    data = new byte[layoutMapping.Count*4];
                else
                    data = new byte[512 * 4];

                for (int i = 0; i < data.Length; i++)
                {
                    float sin = (float)Math.Sin(i / 80.0f + testAnimationTime * 4.0f);
                    if (sin < 0)
                        sin += 1;
                    float t = (float)Math.Min(Math.Max(sin, 0), 1);
                    float fade = Math.Min(Math.Max(8 - Math.Abs(testAnimationTime * 16 - 8), 0), 1);
                    data[i] = (byte)(t*fade*255);
                }
                DrawData(data);
                testAnimationTime -= 0.0025f;
                return;
            }
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
                DrawData(artnetClient.combinedData);
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
            DrawData(artnetClient.combinedData);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void inputChanged_TextChanged(object sender, EventArgs e)
        {
            DrawData(artnetClient.combinedData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("Button1 Clicked");
            artnetClient.Unicast = unicastCheck;
            artnetClient.RestartClient(ipInput.Text, portInput.Text);
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

        [Serializable]
        struct DMXCoord
        {
            public float uvX;
            public float uvY;
            public int dmxX;
            public int dmxY;
            public int channel;
        }

        [Serializable]
        class DMXLayout
        {
            public int resolutionX;
            public int resolutionY;
            public int dmxSizeX;
            public int dmxSizeY;
            public int channelCount;
            public List<DMXCoord> coords = new List<DMXCoord>();
        }
        Dictionary<int, (int x, int y)> layoutMapping;

        
        private void LoadLayout_Click(object sender, EventArgs e)
        {
            layoutStatus.Text = "Loading...";

            OpenFileDialog f = new OpenFileDialog();
            f.Title = "Select Layout File.";
            f.Filter = "Json files (*.json) | *.json";

            if (f.ShowDialog() != DialogResult.OK)
                return;

            if (!File.Exists(f.FileName))
                return;

            string json = File.ReadAllText(f.FileName);
            DMXLayout layout = JsonSerializer.Deserialize<DMXLayout>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive=false, IncludeFields = true, WriteIndented = true });

            //this.Size = new Size(layout.resolutionX, layout.resolutionY);

            countX = layout.dmxSizeX;
            countY = layout.dmxSizeY;

            layoutMapping = new Dictionary<int, (int x, int y)>();
            for (int i = 0; i < layout.coords.Count; i++)
            {
                if (layout.coords[i].uvX < 0 || layout.coords[i].uvX > 1 || layout.coords[i].uvY < 0 || layout.coords[i].uvY > 1)
                {
                    MessageBox.Show("Layout file had data outside of the screen and will not load.", "Layout file error.");
                    layoutStatus.Text = $"VRSL\nsize: 1920x208\nchannels: 1560";
                    return;
                }
                layoutMapping.Add(i, ((layout.coords[i].dmxX), (layout.coords[i].dmxY)));
            }

            layoutStatus.Text = $"{Path.GetFileNameWithoutExtension(f.FileName)}\nsize: {layout.resolutionX}x{layout.resolutionY}\nchannels: {layout.channelCount}";
            customLayout = true;
        }

        private void UnloadLayout_Click(object sender, EventArgs e)
        {
            customLayout = false;
            countY = 13;
            countX = 120;
            layoutStatus.Text = $"VRSL\nsize: 1920x208\nchannels: 1560";
            //this.Size = new Size(1920, 208);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            unicastCheck = checkBox.Checked;
        }

        float testAnimationTime = 0;
        private void testAnimation_Click(object sender, EventArgs e)
        {
            testAnimationTime = 1.0f;
        }
    }
}
