using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;

namespace FuralityGridNode
{
    public class ArtNet
    {
        public byte[] combinedData = new byte[512 * 8];

        private static int listenPort = 6454;
        private static IPAddress listenAddress = IPAddress.Loopback;
        private static UdpClient listener;
        public static Thread listenerThread;
        private static bool started = false;

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
                () => StartListener(out status, ref combinedData)
            );
            listenerThread.Start();
        }

        public void RestartClient()
        {
            ErrorMessage = "";
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
                () => StartListener(out status, ref combinedData)
            );
            listenerThread.Start();
        }

        public static void StartListener(out ArtNetClientStatus status, ref byte[] data)
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

            //IPEndPoint remoteEndPoint = new IPEndPoint(listenAddress, listenPort);
            IPEndPoint remoteEndPoint = new IPEndPoint(listenAddress, listenPort);

            try
            {
                listener = new UdpClient(port: listenPort) { MulticastLoopback = true };
                status = ArtNetClientStatus.Connected;
                Trace.WriteLine("ArtNetClient: Listener started");
            }
            catch (Exception e)
            {
                status = ArtNetClientStatus.Error;
                Trace.WriteLine($"ArtNetException: ${e.Message}");
                return;
            }

            //try
            //{
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
                    if (universe < 8)
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
