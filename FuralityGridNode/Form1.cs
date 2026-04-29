using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace FuralityGridNode
{
    public partial class Form1 : Form
    {
        private ArtNet artnetClient;

        private byte[] midiData = new byte[512 * ArtNet.maxUniverses];
        List<byte[]> overlayData = new List<byte[]>();
        private int midiScanPosition = 0;
        private int midiCatchup = 0;
        private long midiUpdate = 0;
        private string midiSavedDevice = "";
        private int maxMidiChannels = 4096;
        private int bankStatus = 0;

        bool framerate = false;
        SpoutWrapper spoutForLT;
        SpoutWrapper spout;

        private FileStream logStream;

        private OutputDevice midiOutput;

        [Serializable]
        class FuralityGridNodeSettings
        {
            public string ArtNetAddress;
            public string ArtNetPort;
            public bool Unicast;
            public string RigType;
            public string MidiDevice;
            public bool Is1440pModeOn;
            public bool TimecodeOn;
        }

        public void SaveSettings()
        {
            FuralityGridNodeSettings settings = new FuralityGridNodeSettings();
            settings.ArtNetAddress = ipInput.Text;
            settings.ArtNetPort = portInput.Text;
            settings.Unicast = unicast.Checked;
            //settings.RigType = rigTypeDropdown.SelectedItem != null ? rigTypeDropdown.SelectedItem.ToString() : "Binary";
            settings.MidiDevice = midiDevice.SelectedItem != null ? midiDevice.SelectedItem.ToString() : "(none)";
            settings.Is1440pModeOn = res1440p.Checked;
            settings.TimecodeOn = worldLT.Checked;
            string json = JsonSerializer.Serialize<FuralityGridNodeSettings>(settings, new JsonSerializerOptions { PropertyNameCaseInsensitive = false, IncludeFields = true, WriteIndented = true });
            File.WriteAllText("FuralityGridNodeSettings.json", json);
        }
        public void LoadSettings()
        {
            string json = File.ReadAllText("FuralityGridNodeSettings.json");
            FuralityGridNodeSettings settings = JsonSerializer.Deserialize<FuralityGridNodeSettings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = false, IncludeFields = true, WriteIndented = true });
            ipInput.Text = settings.ArtNetAddress;
            portInput.Text = settings.ArtNetPort;
            unicast.Checked = settings.Unicast;
            midiSavedDevice = settings.MidiDevice;
            res1440p.Checked = settings.Is1440pModeOn;
            worldLT.Checked = settings.TimecodeOn;
        }

        public Form1()
        {
            InitializeComponent();
            Graphics g = this.CreateGraphics();
            this.MouseDown += new MouseEventHandler(MainForm_MouseDown);
            g.Clear(Color.Black);

            if (!File.Exists("FuralityGridNodeSettings.json"))
            {
                SaveSettings();
            }
            else
            {
                LoadSettings();
            }

            populateMidi();
            connectMidi();

            allWindows = FindWindowsByTitle("VRChat", true, true);
            vrchatWindowSelect.DataSource = allWindows;
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
        public static extern bool ReleaseCapture(); [DllImport("user32.dll")]
        static extern bool GetWindowLongPtr(IntPtr hWnd, int nIndex, out long dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern long GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int Left, Top, Right, Bottom; }

        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOZORDER = 0x0004;
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;

        void SetWindowSize(IntPtr hwnd, int width, int height)
        {
            uint style = (uint)GetWindowLongPtr(hwnd, GWL_STYLE);
            uint exStyle = (uint)GetWindowLongPtr(hwnd, GWL_EXSTYLE);

            RECT rect = new RECT { Left = 0, Top = 0, Right = width, Bottom = height };
            AdjustWindowRectEx(ref rect, style, false, exStyle);

            int totalW = rect.Right - rect.Left;
            int totalH = rect.Bottom - rect.Top;

            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, totalW, totalH, SWP_NOMOVE | SWP_NOZORDER);
        }
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        public static IntPtr[] FindWindowsByTitle(string title, bool exactMatch = false, bool visibleOnly = true)
        {
            var results = new List<IntPtr>();

            EnumWindows((hWnd, lParam) =>
            {
                if (visibleOnly && !IsWindowVisible(hWnd))
                    return true; // keep enumerating

                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                    return true;

                var sb = new StringBuilder(length + 1);
                GetWindowText(hWnd, sb, sb.Capacity);
                string windowTitle = sb.ToString();

                bool match = exactMatch
                    ? string.Equals(windowTitle, title, StringComparison.Ordinal)
                    : windowTitle.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0;

                if (match)
                    results.Add(hWnd);

                return true; // continue enumeration
            }, IntPtr.Zero);

            return results.ToArray();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern int GetDIBits(IntPtr hdc, IntPtr hBitmap, uint start, uint lines,
            IntPtr buffer, ref BITMAPINFO2 bmi, uint usage);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO2
        {
            public int biSize, biWidth, biHeight;
            public short biPlanes, biBitCount;
            public int biCompression, biSizeImage, biXPelsPerMeter, biYPelsPerMeter, biClrUsed, biClrImportant;
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private void setVRChatFocus_Click(object sender, EventArgs e)
        {
            if (vrchatWindowSelect.SelectedItem == null)
                return;

            IntPtr hwnd = (IntPtr)vrchatWindowSelect.SelectedItem;

            SetForegroundWindow(hwnd);
        }

        uint PW_CLIENTONLY = 0x1;
        uint PW_RENDERFULLCONTENT = 0x2;

        IntPtr hWnd;
        IntPtr hdcWindow;
        IntPtr hdcMem;
        IntPtr hBitmap;
        public int SizeX;
        public int SizeY;
        int pixelCount;
        byte[] srcBuffer;
        byte[] result;
        GCHandle pin;
        BITMAPINFO2 bmi;
        public void CaptureSetup(IntPtr hwnd, int width = 265, int height = 52)
        {
            this.hWnd = hwnd;
            SizeX = width;
            SizeY = height;
            pixelCount = width * height;

            hdcWindow = GetDC(hWnd);
            hdcMem = CreateCompatibleDC(hdcWindow);
            hBitmap = CreateCompatibleBitmap(hdcWindow, width, height);
            SelectObject(hdcMem, hBitmap);

            srcBuffer = new byte[width * 4 * height];
            pin = GCHandle.Alloc(srcBuffer, GCHandleType.Pinned);
            result = new byte[pixelCount * 4];

            bmi = new BITMAPINFO2
            {
                biSize = 40,
                biWidth = width,
                biHeight = -height,
                biPlanes = 1,
                biBitCount = 32,
            };
        }

        public byte[] Capture()
        {
            if (hWnd == IntPtr.Zero)
                return null;

            PrintWindow(hWnd, hdcMem, PW_CLIENTONLY | PW_RENDERFULLCONTENT);

            GetDIBits(hdcMem, hBitmap, 0, (uint)SizeY, pin.AddrOfPinnedObject(), ref bmi, 0);

            for (int i = 0; i < pixelCount; i++)
            {
                int off = i * 4;
                result[off + 0] = srcBuffer[off + 2]; // R
                result[off + 1] = srcBuffer[off + 1]; // G
                result[off + 2] = srcBuffer[off + 0]; // B
                result[off + 3] = 255;                // A
            }

            return result;
        }


        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        static bool update = false;
        static string statusText = "";
        static string statusTextLast = "";

        public static bool IsValidIndex<T>(IList<T> source, int index)
        {
            return source != null && index >= 0 && index < source.Count;
        }

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

            crc = (crc << 4);

            return (byte)crc; // put crc on the left and pad 0s
        }

        void SetPixel(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, 
            byte R, byte G, byte B, byte A, 
            bool setR = true, bool setG = true, bool setB = true, bool setA = true)
        {
            if (X >= dataSizeX || Y >= dataSizeY)
                return;

            if (X + 1 <= 0 || Y + 1 <= 0)
                return;

            int rowIndex = (Y * dataSizeX) * 4;
            int idx = rowIndex + X * 4;
            if(setR) data[idx + 0] = B;  // B
            if(setG) data[idx + 1] = G;  // G
            if(setB) data[idx + 2] = R;  // R
            if(setA) data[idx + 3] = A;  // A
        }
        void SetPixel(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, bool value)
        {
            SetPixel(data, dataSizeX, dataSizeY, X, Y,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0, 255);
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
        void DrawSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, bool value)
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

        byte[] ResizeImage(byte[] captured, int inputSizeX, int inputSizeY, float scale)
        {
            if (captured == null)
                return null;

            int outW = (int)(inputSizeX * scale);
            int outH = (int)(inputSizeY * scale);
            byte[] result = new byte[outW * outH * 4];

            for (int y = 0; y < outH; y++)
            {
                int srcY = (int)(y / scale);
                for (int x = 0; x < outW; x++)
                {
                    int srcX = (int)(x / scale);
                    int srcIdx = (srcY * inputSizeX + srcX) * 4;
                    int dstIdx = (y * outW + x) * 4;
                    result[dstIdx + 0] = captured[srcIdx + 0];
                    result[dstIdx + 1] = captured[srcIdx + 1];
                    result[dstIdx + 2] = captured[srcIdx + 2];
                    result[dstIdx + 3] = captured[srcIdx + 3];
                }
            }
            return result;
        }

        byte[] DrawGrid(byte[] combinedData, int bladeSizeX, int bladeSizeY)
        {
            int maxLength = Math.Min(combinedData.Length, 512 * 3);

            byte[] result = new byte[bladeSizeX * bladeSizeY * 4];

            for (int i = 0; i < maxLength; i++)
            {
                int turboExpandOffset = i / 6 / bladeSizeX;
                int x = (i / 6) % bladeSizeX;
                int y = i % 6;
                byte currentByte = combinedData[i];
                for (int j = 0; j < 8; j++)
                {
                    int y2 = y * 8 + j;
                    bool value = GetBit(currentByte, 7 - j);
                    SetPixel(result, bladeSizeX, bladeSizeY, x, y2 + turboExpandOffset * (bladeSizeY), value);
                }
                // at the end of each row, calculate crc
                if (y == 5)
                {
                    byte mask = Crc4For6(combinedData[i - 5], combinedData[i - 4], combinedData[i - 3], combinedData[i - 2], combinedData[i - 1], combinedData[i - 0]);
                    for (int j = 0; j < 4; j++)
                    {
                        bool value = GetBit(mask, 7 - j);
                        SetPixel(result, bladeSizeX, bladeSizeY, x, (6 * 8 + j) + turboExpandOffset * (bladeSizeY), value);
                    }
                }
            }

            return result;
        }

        bool ReadBit(byte[] colorData, int sizeX, int x, int y)
        {
            int idx = x + y * sizeX;
            return colorData[idx * 4] > 128; // times 4 because it's 4 channels (RGBA)
        }

        const int bufferSizeInSeconds = 20;
        const float delayInSeconds = 15.0f;
        const int bufferSize = 30 * bufferSizeInSeconds;
        int currentBufferIndex = 0;
        double[] timecodeBuffer = new double[bufferSize]; // 10 seconds
        byte[][] dataBuffer = new byte[bufferSize][];

        Stopwatch sw = new Stopwatch();
        IntPtr hwndLast;
        public void DrawData(byte[] combinedData)
        {
            long checkTimestamp;
            checkTimestamp = Stopwatch.GetTimestamp();
            long startTimestamp = checkTimestamp;

            string debug = "";

            if (vrchatWindowSelect.SelectedItem != null)
            {
                IntPtr hwnd = (IntPtr)vrchatWindowSelect.SelectedItem;
                if (hwnd != hwndLast)
                {
                    hwndLast = hwnd;
                    CaptureSetup(hwnd, 256 * 4, 52 * 4);
                }
            }

            if (!sw.IsRunning)
                sw.Start();

            double timecode = (sw.ElapsedMilliseconds / 1000.0) + 100.0; // start 100 seconds in
            byte[] bytes = BitConverter.GetBytes((int)(timecode*100));
            if (combinedData.Length >= 3)
            {
                combinedData[0] = bytes[0];
                combinedData[1] = bytes[1];
                combinedData[2] = bytes[2];
            }

            double timecodePlayout = timecode - delayInSeconds;

            byte[] captured = Capture();
            byte[] resized = ResizeImage(captured, 256*4, 52 * 4, 0.25f);// new byte[256 * 52 * 4];
            // decode timecode from game
            double timecodeFromBot = 0;
            bool[] bits = new bool[3 * 8];
            if (captured != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        bits[(i*8)+j] = ReadBit(resized, 256, 0, (i * 8) + (7-j));
                    }
                }
                int[] data = new int[1];
                new BitArray(bits).CopyTo(data, 0);
                // we can think of this timecode as a "cursor", the times at which the LT's performance is being played out.
                // because of the buffer, it may end up a little in the past.. so we need to buffer a bunch of it and wait
                // for the current time to catch up.
                timecodeFromBot = data[0] / 100.0;
                
                timecodeBuffer[currentBufferIndex] = timecodeFromBot;
                dataBuffer[currentBufferIndex] = captured;
                currentBufferIndex++;
                currentBufferIndex %= bufferSize;

                if (captured.Length == (256*4 * 52*4)*4)
                {
                    spout.SendImage(captured, 256*4, 52*4);
                }
            }

            double smallestDelta = 99999;
            int smallestDeltaIdx = -1;
            for (int i = 0; i < timecodeBuffer.Length; i++)
            {
                double delta = Math.Abs(timecodeBuffer[i] - timecodePlayout);
                if (delta < smallestDelta)
                {
                    smallestDeltaIdx = i;
                    smallestDelta = delta;
                }
            }

            double closestTimecode = -1;

            if (smallestDeltaIdx >= 0)
            {
                closestTimecode = timecodeBuffer[smallestDeltaIdx];
            }

            debug += "closest timecode: " + closestTimecode.ToString("0.00") + "\n";
            debug += "timecode: " + timecode.ToString("0.00") + "\n";
            debug += "timecodeFromBot: " + timecodeFromBot.ToString("0.00") + "\n";
            debug += "timecodePlayout: " + timecodePlayout.ToString("0.00") + "\n";
            debug += "\n";

            if (framerate)
                debug += "FRAMERATE\n";
            else
                debug += "\n";

            debug += "setup: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            checkTimestamp = Stopwatch.GetTimestamp();

            int bladeSizeX = res1440p.Checked ? 640 : 480;
            int bladeSizeY = 8 * 6 + 4;
            int outputScale = 4;

            byte[] grid = DrawGrid(combinedData, bladeSizeX, bladeSizeY);
            byte[] output = ResizeImage(grid, bladeSizeX, bladeSizeY, outputScale);
            spoutForLT.SendImage(output, bladeSizeX * outputScale, bladeSizeY * outputScale);

            if (midiOutput != null)
            {
                if (isMidiReady())
                {
                    //Midi updates
                    int midiUpdates = 0;
                    int midiCap = bigDataCheck.Checked ? 3200 : 100;
                    int scanCap = bigDataCheck.Checked ? 100 : 10;
                    int maxMidiChannels = bigDataCheck.Checked ? 16384 : 4096;
                    for (int i = midiCatchup; i < combinedData.Length; i++)
                    {
                        if ((combinedData[i] != midiData[i] || (i >= midiScanPosition && i < midiScanPosition + scanCap)) && i < maxMidiChannels)
                        {
                            midiUpdates++;
                            if (midiUpdates >= midiCap)
                            {
                                midiCatchup = i;
                                break;
                            }
                            midiData[i] = combinedData[i];
            
                            int bank = i / 2048;
            
                            if (bank != bankStatus)
                            {
                                ChangeBanks(bank);
                                midiUpdates++;
                            }
            
                            int t = i - (bank * 2048);
            
                            if (t < 1024)
                            {
                                NoteOnEvent noteOn = new NoteOnEvent();
                                noteOn.Channel = (FourBitNumber)((t >> 6) & 0xF);
                                noteOn.NoteNumber = (SevenBitNumber)(((t << 1) & 0x7F) + ((combinedData[i] >> 7) & 0x1));
                                noteOn.Velocity = (SevenBitNumber)(combinedData[i] & 0x7F);
                                midiOutput.SendEvent(noteOn);
                            }
                            else {
                                t = t - 1024;
                                NoteOffEvent noteOff = new NoteOffEvent();
                                noteOff.Channel = (FourBitNumber)((t >> 6) & 0xF);
                                noteOff.NoteNumber = (SevenBitNumber)(((t << 1) & 0x7F) + ((combinedData[i] >> 7) & 0x1));
                                noteOff.Velocity = (SevenBitNumber)(combinedData[i] & 0x7F);
                                midiOutput.SendEvent(noteOff);
                            }
                        }
                    }
            
                    if (midiUpdates < midiCap)
                    {
                        midiCatchup = 0;
                    }
            
                    midiStatus.Text = "Connected - Sending Data";
                    midiStatus.ForeColor = Color.Black;
            
                    midiScanPosition += scanCap;
                    if (midiScanPosition > maxMidiChannels)
                    {
                        midiScanPosition = 0;
                    }
            
                    midiWatchdog();
                    midiUpdate = Stopwatch.GetTimestamp();
                } else
                {
                    float midiTimeout = (float) (Stopwatch.GetTimestamp() - midiUpdate) / (float) Stopwatch.Frequency;
                    if (midiTimeout > 1)
                    {
                        midiCatchup = 0;
                        midiStatus.Text = "Connected - Waiting";
                        midiStatus.ForeColor = Color.Black;
            
                        midiReset();
            
                        midiUpdate = Stopwatch.GetTimestamp();
                    }
                }
            }

            DrawImage(gridPreview.CreateGraphics(), grid, bladeSizeX, bladeSizeY);
            debug += "render: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            checkTimestamp = Stopwatch.GetTimestamp();

            long totalTime = (Stopwatch.GetTimestamp() - startTimestamp) * 1000 * 1000 / Stopwatch.Frequency;
            framerate = totalTime > 33000;
            if(framerate)
                slow.ForeColor = Color.Red;
            else
                slow.ForeColor = Color.Black;
            slow.Text = debug;
        }

        void DrawImage(Graphics gr, byte[] data, int sizeX, int sizeY)
        {
            GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            IntPtr hdc = gr.GetHdc();
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader = new BITMAPINFOHEADER
            {
                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = sizeX,
                biHeight = -sizeY, // Negative height to indicate a top-down DIB
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0, // BI_RGB
                biSizeImage = (uint)(sizeX * sizeY) // pixel count
            };
            StretchDIBits(hdc, 8, 16, sizeX, sizeY, 0, 0, sizeX, sizeY, pointer, ref bmi, 0, 0x00CC0020);
            gr.ReleaseHdc(hdc);
            pinnedArray.Free();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            spoutForLT = new SpoutWrapper("PHC GridNode - For LT", true);
            spout = new SpoutWrapper("PHC GridNode", true);
            StartArtNetClient();
        }

        void StartArtNetClient()
        {
            artnetClient = new ArtNet(ipInput.Text, portInput.Text);
            artnetClient.StartClient();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (testAnimationTime > 0)
            {
                float sin2 = (float)Math.Sin(testAnimationTime * 8.0f) * 0.5f + 0.5f;
                byte[] data;
                data = new byte[(int)(512 * 16 * sin2)];

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
            if (artnetClient != null)
                DrawData(artnetClient.combinedData);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void inputChanged_TextChanged(object sender, EventArgs e)
        {
            if(artnetClient != null)
                DrawData(artnetClient.combinedData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("Button1 Clicked");
            if (artnetClient != null)
            {
                artnetClient.Unicast = unicast.Checked;
                artnetClient.RestartClient(ipInput.Text, portInput.Text);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            Application.Exit();
            Environment.Exit(0);
        }

        private void populateMidi()
        {
            string selected = midiSavedDevice;
            if (midiDevice.SelectedItem != null)
            {
                selected = midiDevice.SelectedItem.ToString();
            }

            midiDevice.Items.Clear();
            ICollection<OutputDevice> devices = OutputDevice.GetAll();

            midiDevice.Items.Add("(none)");
            if (selected == null)
            {
                midiDevice.SelectedIndex = 0;
            }

            foreach (OutputDevice device in devices)
            {
                int t = midiDevice.Items.Add(device.Name);
                if (device.Name == selected)
                {
                    midiDevice.SelectedIndex = t;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (midiDevice.SelectedIndex == 0)
            {
                midiStatus.Text = "Disconnected";
                if (midiOutput != null)
                    midiOutput.Dispose();
                midiOutput = null;
            }
            else
            {
                connectMidi();
            }

            SaveSettings();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            populateMidi();
            connectMidi();
            midiReset();
        }

        private void connectMidi()
        {
            if (midiOutput != null)
            {
                midiOutput.Dispose();
                midiOutput = null;
            }

            try
            {
                if (midiDevice.SelectedItem != null)
                {
                    midiOutput = OutputDevice.GetByName(midiDevice.SelectedItem.ToString());
                    midiStatus.Text = "Connected";
                }
                else
                {
                    midiStatus.Text = "Failed to connect";
                }
            }
            catch
            {
                midiStatus.Text = "Failed to connect";
            }
        }

        private void findVRCLog()
        {
            if (logStream != null)
            {
                logStream.Close();
                logStream = null;
            }

            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string log = "";
            if (editorCheck.Checked)
            {
                log = path + "\\..\\Local\\Unity\\Editor\\Editor.log";
            }
            else
            {
                string[] logs = Directory.GetFiles(path + "\\..\\LocalLow\\VRChat\\VRChat", "output_log_*.txt", SearchOption.TopDirectoryOnly);
                if (logs.Length == 0) return;

                Array.Sort(logs);
                log = logs[logs.Length - 1];
            }

            logStream = new FileStream(log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            //forward to the end to wait on it
            if (logStream.Length != 0)
            {
                logStream.Position = logStream.Length - 1;
            }
        }

        private bool isMidiReady()
        {
            if (logStream == null)
            {
                findVRCLog();
                return false;
            }

            int length = (int) (logStream.Length - logStream.Position);

            byte[] searchWord = { (byte) 'M', (byte)'I', (byte)'D', (byte)'I', (byte)'R', (byte)'E', (byte)'A', (byte)'D', (byte)'Y', };

            if (length > 1)
            {
                int c;
                int i = 0;
                while ((c = logStream.ReadByte()) != -1) { 
                    if (c == searchWord[i])
                    {
                        i++;

                        if (i >= searchWord.Length)
                        {
                            logStream.Position = logStream.Length - 1;
                            return true;
                        }
                    }
                }
            } else
            {
                return false;
            }

            return false;
        }

        //Todo: Wait for callback to ensure the bank swap was triggered
        private void ChangeBanks(int bank)
        {
            bankStatus = bank;

            SendMidiControl(15, 127, bank);
        }

        private void midiWatchdog()
        {
            if (midiOutput != null)
            {
                SendMidiControl(15, 127, 127);
            }
        }

        private void SendMidiControl(int channel, int control, int value)
        {
            if (midiOutput != null)
            {
                ControlChangeEvent midiWD = new ControlChangeEvent();
                midiWD.Channel = (FourBitNumber)channel;
                midiWD.ControlNumber = (SevenBitNumber)control;
                midiWD.ControlValue = (SevenBitNumber)value;

                midiOutput.SendEvent(midiWD);
            }
        }

        //Unlocks world receiver
        private void midiKnock()
        {
            SendMidiControl(15, 127, 101);
            SendMidiControl(15, 127, 120);
            SendMidiControl(15, 127, 107);
        }

        private void midiReset()
        {
            midiData = new byte[16384];
            midiScanPosition = 0;
            midiCatchup = 0;

            findVRCLog();
            ChangeBanks(0);
            midiKnock();
            midiWatchdog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            midiReset();
        }

        private void screenshot(object sender, EventArgs e)
        {
            return;
            int previewSizeX = 0;//bladeSizeX * previewScale);
            int previewSizeY = 0;//(bladeSizeY * previewScale);
            
            int outputSizeX = 0;//ladeSizeX * outputScale;
            int outputSizeY = 0;//bladeSizeY * outputScale;
            
            Bitmap image = new Bitmap(outputSizeX, outputSizeY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            GCHandle pinnedArray = GCHandle.Alloc(preview, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            Graphics gr = Graphics.FromImage(image);
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
            StretchDIBits(hdc, 0, 0, outputSizeX, outputSizeY, 0, 0, previewSizeX, previewSizeY, pointer, ref bmi, 0, 0x00CC0020);
            gr.ReleaseHdc(hdc);
            pinnedArray.Free();
            
            string filename = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fff");
            
            image.Save($"{filename}.png", ImageFormat.Png);
        }

        float testAnimationTime = 0;
        IntPtr[] allWindows;

        private void testAnimation_Click(object sender, EventArgs e)
        {
            // clear all overwrites when test animation plays
            testAnimationTime = 1.0f;
        }

        private void setVRChatSize_Click(object sender, EventArgs e)
        {
            if (vrchatWindowSelect.SelectedItem == null)
                return;

            IntPtr hwnd = (IntPtr)vrchatWindowSelect.SelectedItem;

            if (res1440p.Checked)
                SetWindowSize(hwnd, 2560, 1440);
            else
                SetWindowSize(hwnd, 1920, 1080);
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            allWindows = FindWindowsByTitle("VRChat", true, true);
            vrchatWindowSelect.DataSource = allWindows;
        }
    }
}
