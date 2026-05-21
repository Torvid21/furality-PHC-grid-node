using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FuralityGridNode
{
    public class ArtNet
    {
        public const int maxUniverses = 32;
        public byte[] combinedData = new byte[512 * maxUniverses];

        private static int listenPort = 6454;
        private static IPAddress listenAddress = IPAddress.Parse("2.0.0.2");
        private static UdpClient listener;
        private static UdpClient sender;
        public static Thread listenerThread;
        private static bool started = false;
        public bool Unicast;

        private static bool alive = false;

        public ArtNet(string address, string port)
        {
            if (!IPAddress.TryParse(address, out listenAddress))
                status = ArtNetClientStatus.MalformedAddress;

            if (!int.TryParse(port, out listenPort))
                status = ArtNetClientStatus.MalformedPortInput;

            if (listenPort < 1 || listenPort > 65535)
                status = ArtNetClientStatus.MalformedPortInput;
        }

        public enum ArtNetClientStatus
        {
            Connected,
            Disconnected,
            Connecting,
            Error,
            Destroyed,
            Aborted,
            Closed,
            ReceivingData,
            MalformedInput,
            MalformedAddress,
            MalformedPortInput,
            Waiting,
            ReceivedData,
        }

        public string ErrorMessage = "";

        public ArtNetClientStatus status = ArtNetClientStatus.Disconnected;

        static byte sequence = 0;
        // Send function only tested for sending to itself, 
        // In future to send to other lighting control programs
        // some work may be needed to get it to talk ArtNet properly x>
        public void Send(byte[] universeData, string address, string port)
        {
            IPAddress sendAddress;
            int sendPort;
            if (!IPAddress.TryParse(address, out sendAddress))
                return;

            if (!int.TryParse(port, out sendPort))
                return;

            if (sendPort < 1 || sendPort > 65535)
                return;
            //if (universeData.Length % 512 != 0) // needs to be some number of universes
            //    return;

            if (sender == null)
            {
                sender = new UdpClient();
            }

            int universeCount = (int)Math.Ceiling(universeData.Length / 512.0);

            byte[] outData = new byte[universeCount * 512];
            for (int i = 0; i < universeData.Length; i++)
            {
                outData[i] = universeData[i];
            }

            for (int universe = 0; universe < universeCount; universe++)
            {
                byte[] data = new byte[18 + 512];
                // cookie
                data[0] = (byte)'A';
                data[1] = (byte)'r';
                data[2] = (byte)'t';
                data[3] = (byte)'-';
                data[4] = (byte)'N';
                data[5] = (byte)'e';
                data[6] = (byte)'t';
                data[7] = 0;

                // opcode
                data[8] = 0x00;
                data[9] = 0x50;

                // protocol verison
                data[10] = 0;
                data[11] = 14;

                // sequence, physical
                data[12] = sequence++;
                data[13] = 0;

                // universe little endian
                data[14] = (byte)universe;
                data[15] = 0;

                // length hi, length lo
                data[16] = 2;
                data[17] = 0;

                for (int i = 0; i < 512; i++)
                {
                    data[i + 18] = outData[i + universe * 512];
                }

                sender.Send(data, 512 + 18, sendAddress.ToString(), sendPort);
            }
        }

        public void StartClient()
        {
            ErrorMessage = "";
#if DEBUG
            Trace.WriteLine($"Starting client: {alive}");
#endif
            started = true;
            if (status == ArtNetClientStatus.MalformedInput)
            {
                ErrorMessage = "Malformed Input";
                if (listener != null) listener.Close();
                return;
            }

            if (listenerThread != null)
            {
                alive = false;
                listenerThread.Abort();
                status = ArtNetClientStatus.Closed;
                ErrorMessage = "Listener Thread was null";
            }

            alive = true;
            listenerThread = new Thread(
                () => StartListener(out status, ref combinedData, this.Unicast)
            );
            listenerThread.Start();
        }

        public void RestartClient(string ip, string port)
        {
            ErrorMessage = "";
            IPAddress.TryParse(ip, out listenAddress);
            int.TryParse(port, out listenPort);
#if DEBUG
            Trace.WriteLine($"Restarting Client: {alive} {started} {status.ToString()}");
#endif
            if (!started)
            {
                status = ArtNetClientStatus.Error;
                ErrorMessage = "ArtNetClient: Don't restart an unstarted client";
                Trace.WriteLine("ArtNetClient: Don't restart an unstarted client");
                return;
            }
            if (status == ArtNetClientStatus.MalformedInput)
            {
                if (listener != null) listener.Close();
                ErrorMessage = "ArtNetClient: Malformed Input";
                Trace.WriteLine("ArtNetClient: Malformed Input");
                return;
            }

            if (listenerThread != null)
            {
                Trace.WriteLine($"Joining thread: {alive}");
                alive = false;
                listenerThread.Abort();
                status = ArtNetClientStatus.Closed;
            }

            alive = true;
            listenerThread = new Thread(
                () => StartListener(out status, ref combinedData, this.Unicast)
            );
            listenerThread.Start();
        }

        public static void StartListener(out ArtNetClientStatus status, ref byte[] data, bool unicast)
        {
#if DEBUG
            Trace.WriteLine($"Start Listening: {alive}");
#endif
            status = ArtNetClientStatus.Disconnected;
            if (!alive)
            {
                status = ArtNetClientStatus.Closed;

                if (listener != null)
                    listener.Close();

                Trace.WriteLine($"ArtNetClient: Listener got closed");

                // Clean combinedData so we draw nothing
                Array.Clear(data, 0, data.Length);
                return;
            }

            status = ArtNetClientStatus.Connecting;
            Trace.WriteLine("ArtNetClient: Connecting");
            if (listener != null)
            {
                listener.Close();
                Trace.WriteLine("ArtNetClient: Closing Listener");
            }

            if (listenAddress == null) // invalid address
            {
                if (listener != null)
                    listener.Close();

                Trace.WriteLine("ArtNetClient: Invalid IP Address");
                return;
            }

            IPEndPoint remoteEndPoint = new IPEndPoint(listenAddress, listenPort);

            try
            {
                if (unicast)
                {
                    listener = new UdpClient(AddressFamily.InterNetwork);
                    listener.ExclusiveAddressUse = false;
                    listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    listener.Client.Bind(remoteEndPoint);
                    status = ArtNetClientStatus.Connected;
                    Trace.WriteLine("ArtNetClient: Listener started");
                } else
                {
                    listener = new UdpClient(listenPort);
                    status = ArtNetClientStatus.Connected;
                    Trace.WriteLine("ArtNetClient: Listener started");
                }
            }
            catch (Exception e)
            {
                status = ArtNetClientStatus.Error;
                Trace.WriteLine($"ArtNetException: {e.Message}");
                return;
            }

            while (true)
            {
                if (listenAddress == null)
                {
                    status = ArtNetClientStatus.MalformedInput;
                    Trace.WriteLine("ArtNetClient: Malformed input");
                    return;
                }

                if (listener == null)
                {
                    status = ArtNetClientStatus.Error;
                    Trace.WriteLine("ArtNetClient: Unexpected error");
                    return;
                }

                byte[] bytes = Array.Empty<byte>();
                bytes = listener.Receive(ref remoteEndPoint);
                status = ArtNetClientStatus.ReceivingData;
                int opcode = (bytes[8 + 1] << 8) | bytes[8 + 0];
                if (opcode == 0x5000)
                {
                    int universe = (bytes[12 + 3] << 8) | bytes[12 + 2];
                    if (universe < maxUniverses)
                    {
                        for (int i = 0; i < 512; i++)
                        {
                            data[i + universe * 512] = bytes[i + 18];
                        }
                    }
                    status = ArtNetClientStatus.ReceivedData;
                }
            }
        }

        ~ArtNet()
        {
            if (listener != null) listener.Close();
            Array.Clear(combinedData, 0, combinedData.Length);
            status = ArtNetClientStatus.Destroyed;
        }
    }
}
