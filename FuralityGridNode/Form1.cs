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
        static int bladeSizeX = 1920;
        static int bladeSizeY = 208;
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


        // CRC-8 (x⁸ + x² + x + 1)
        public static byte Crc8For6(
            byte b0, byte b1, byte b2,
            byte b3, byte b4, byte b5)
        {
            uint crc = 0;
            uint poly = 0x07;

            uint[] data = { b0, b1, b2, b3, b4, b5 };
            foreach (uint v in data)
            {
                crc ^= v;
                for (int i = 0; i < 8; ++i)
                    crc = (crc & 0x80) != 0
                          ? ((crc << 1) ^ poly) & 0xFF
                          : (crc << 1) & 0xFF;
            }
            return (byte)crc;
        }

        // CRC-4 (x⁴ + x + 1)
        public static byte Crc4For6(
            byte b0, byte b1, byte b2,
            byte b3, byte b4, byte b5)
        {
            uint crc = 0;
            uint poly = 0x03;

            uint[] data = { b0, b1, b2, b3, b4, b5 };
            foreach (uint v in data)
            {
                for (int bit = 7; bit >= 0; --bit)
                {
                    uint inBit = (v >> bit) & 1;
                    uint top = (crc >> 3) & 1;
                    crc = ((crc << 1) | inBit) & 0xF;
                    if (top == 1) crc ^= poly;
                }
            }
            return (byte)(crc << 4); // put crc on the left and pad 0s
        }

        void DrawSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte R, byte G, byte B, byte A)
        {
            if (sizeX <= 0 || sizeY <= 0)
                return;
            if (X >= dataSizeX || Y >= dataSizeY)
                return;
            if (X + sizeX <= 0 || Y + sizeY <= 0)
                return;

            int startX = Math.Max(X, 0);
            int startY = Math.Max(Y, 0);
            int endX = Math.Min(X + sizeX, dataSizeX);
            int endY = Math.Min(Y + sizeY, dataSizeY);

            for (int y = startY; y < endY; y++)
            {
                int rowIndex = (y * dataSizeX + startX) * 4;

                for (int x = startX; x < endX; x++)
                {
                    int idx = rowIndex + (x - startX) * 4;
                    data[idx + 0] = B;  // B
                    data[idx + 1] = G;  // G
                    data[idx + 2] = R;  // R
                    data[idx + 3] = A;  // A
                }
            }
        }
        void DrawSquareBinary(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, bool value)
        {
            DrawSquare(data, dataSizeX, dataSizeY, X, Y, sizeX, sizeY,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0, 255);
        }
        bool GetBit(byte currentByte, int index)
        {
            return (currentByte & (1 << index)) != 0;
        }
        public void DrawData(byte[] combinedData)
        {
            bladeSizeX = 1920;
            bladeSizeY = 208;
            byte[] output = new byte[bladeSizeX * bladeSizeY * 4];

            string selectedItem = colorTypeDropdown.SelectedItem as string;

            largeCRC.Enabled = (selectedItem == "Binary");
            selectRig.Enabled = (selectedItem == "FRig");
            LoadLayout.Enabled = !(selectedItem == "Binary");
            UnloadLayout.Enabled = !(selectedItem == "Binary");

            if (selectedItem == "VRSL")
            {
                int size = 16;
                int countX = bladeSizeX / 16;
                int countY = bladeSizeY / 16;
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

                        DrawSquare(output, bladeSizeX, bladeSizeY, x * size, y * size, size, size, data, data, data, 255);
                    }
                }
            }
            else if (selectedItem == "Packed")
            {
                int size = 16;
                int countX = bladeSizeX / 16;
                int countY = bladeSizeY / 16;
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
                    DrawSquare(output, bladeSizeX, bladeSizeY, x * size, y * size, size, size, dataR, dataG, dataB, 255);
                }
            }
            else if (selectedItem == "FRig")
            {
                int size = 16;
                int countX = bladeSizeX / 16;
                int countY = bladeSizeY / 16;
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

                        DrawColorSquare(output, bladeSizeX, bladeSizeY, pixelX, pixelY, size, size, data, fixture.GridColor, false);

                        index++;
                    }
                }
                else
                {
                    var frigFile = new FRigFile();
                    var frigBaseFile = FuralityGridNode.Properties.Resources.Blockout;
                    var frigBase = Encoding.UTF8.GetString(frigBaseFile);
                    currentRig = frigFile.LoadFromJsonString(frigBase).ConvertToFRig();
                }
            }
            else if (selectedItem == "Binary")
            {
                if (largeCRC.Checked)
                {
                    bladeSizeX = 1920;
                    bladeSizeY = 224;
                }
                else
                {
                    bladeSizeX = 1920;
                    bladeSizeY = 208;
                }
                output = new byte[bladeSizeX * bladeSizeY * 4];
                int size = 4;
                for (int i = 0; i < combinedData.Length; i++)
                {
                    int x = i / 6;
                    int y = i % 6;
                    byte currentByte = combinedData[i];
                    for (int j = 0; j < 8; j++)
                    {
                        int y2 = y * 8 + j;
                        bool value = GetBit(currentByte, 7 - j);
                        DrawSquareBinary(output, bladeSizeX, bladeSizeY, x * size, y2 * size, size, size, value);
                    }
                    // at the end of each row, calculate crc
                    if (y == 5) 
                    {
                        byte mask = 0;

                        if (largeCRC.Checked)
                        {
                            mask = Crc8For6(
                                combinedData[i - 5],
                                combinedData[i - 4],
                                combinedData[i - 3],
                                combinedData[i - 2],
                                combinedData[i - 1],
                                combinedData[i - 0]);
                            for (int j = 0; j < 8; j++)
                            {
                                bool value = GetBit(mask, j);
                                DrawSquareBinary(output, bladeSizeX, bladeSizeY, x * size, (6 * 8 + j) * size, size, size, value);
                            }
                        }
                        else
                        {
                            mask = Crc4For6(
                                combinedData[i - 5],
                                combinedData[i - 4],
                                combinedData[i - 3],
                                combinedData[i - 2],
                                combinedData[i - 1],
                                combinedData[i - 0]);
                            for (int j = 0; j < 4; j++)
                            {
                                bool value = GetBit(mask, j+4);
                                DrawSquareBinary(output, bladeSizeX, bladeSizeY, x * size, (6 * 8 + j) * size, size, size, value);
                            }
                        }
                    }
                }
            }
            if (output == null)
                return;

            SpoutWrapper.SendImage(output, bladeSizeX, bladeSizeY);


            // Draw preview!
            int previewScale = 8;
            if(selectedItem == "Binary")
                previewScale = 4;
            int previewSizeX = (bladeSizeX / previewScale);
            int previewSizeY = (bladeSizeY / previewScale);
            byte[] preview = new byte[previewSizeX * previewSizeY * 4];
            for (int i = 0; i < (previewSizeX * previewSizeY); i++)
            {
                int x = (i % previewSizeX);
                int y = (i / previewSizeX);

                int i2 = x + y * previewSizeX;

                int j = (x * previewScale + y * previewScale * previewSizeX * previewScale);
                preview[i*4+0] = output[j*4+0]; // R
                preview[i*4+1] = output[j*4+1]; // G
                preview[i*4+2] = output[j*4+2]; // B
                preview[i*4+3] = output[j*4+3]; // A
            }

            // win32 fast draw byte[] to a control. Idk why the default C# functions are so disgustingly slow.
            GCHandle pinnedArray = GCHandle.Alloc(preview, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            Graphics gr = gridPreview.CreateGraphics();
            IntPtr hdc = gr.GetHdc();
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader = new BITMAPINFOHEADER
            {
                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = previewSizeX,
                biHeight = -previewSizeY, // Negative height to indicate a top-down DIB
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0, // BI_RGB
                biSizeImage = (uint)(previewSizeX * previewSizeY) // pixel count
            };
            StretchDIBits(hdc, 8, 16, previewSizeX, previewSizeY, 0, 0, previewSizeX, previewSizeY, pointer, ref bmi, 0, 0x00CC0020);
            gr.ReleaseHdc(hdc);
            pinnedArray.Free();
        }

        private void DrawColorSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte dataIn, int selector, bool bgra)
        {
            for (int x = X; x < X + sizeX; x++)
            {
                for (int y = Y; y < Y + sizeY; y++)
                {
                    int index = (x + y * dataSizeX) * 4;
                    switch (selector) {
                        case 1:
                            if (bgra)
                                data[index + 2] = dataIn; // R
                            else
                                data[index + 0] = dataIn;
                                break;
                        case 2:
                            data[index + 1] = dataIn; // G
                            break;
                        case 3:
                            if (bgra)
                                data[index + 0] = dataIn; // B
                            else
                                data[index + 2] = dataIn;
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
                    data = new byte[512 * 16];

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
            Application.Exit();
            Environment.Exit(0);
        }

        private void colorTypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawData(artnetClient.combinedData);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
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

            bladeSizeX = layout.dmxSizeX * 16;
            bladeSizeY = layout.dmxSizeY * 16;

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
            bladeSizeX = 1920;
            bladeSizeY = 208;
            layoutStatus.Text = $"VRSL\nsize: 1920x208\nchannels: 1560";
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
