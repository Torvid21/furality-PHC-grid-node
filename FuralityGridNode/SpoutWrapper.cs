using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace FuralityGridNode
{
    class SpoutWrapper
    {
        [DllImport("SpoutLibrary.dll", EntryPoint = "GetSpout", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr GetSpout();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool CreateSenderFn(IntPtr self, string name, uint w, uint h, uint fmt);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool CreateReceiverFn(IntPtr self, string name, ref uint w, ref uint h);


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool SendImageFn(IntPtr self, IntPtr pixels, uint w, uint h, uint glFmt, bool invert);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool ReceiveImageFn(IntPtr self, IntPtr pixels, uint glFmt, bool invert, uint HostFbo);


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool SetSenderNameFn(IntPtr self, string name);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool SetReceiverNameFn(IntPtr self, string name);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool CheckReceiverFn(IntPtr self, string name, ref uint w, ref uint h, ref bool connected);

        static IntPtr _obj;
        static CreateSenderFn _CreateSender;
        static CreateReceiverFn _CreateReceiver;

        static SendImageFn _SendImage;
        static ReceiveImageFn _ReceiveImage;

        static SetSenderNameFn _SetSenderName;
        static SetReceiverNameFn _SetReceiverName;
        static CheckReceiverFn _CheckReceiver;

        static SpoutWrapper()
        {
            _obj = GetSpout();

            IntPtr vTable = Marshal.ReadIntPtr(_obj);

            int sz = IntPtr.Size;

            _SetSenderName = Marshal.GetDelegateForFunctionPointer<SetSenderNameFn>(Marshal.ReadIntPtr(vTable, 0));
            _SendImage = Marshal.GetDelegateForFunctionPointer<SendImageFn>(Marshal.ReadIntPtr(vTable, 5 * sz));
            _CreateSender = Marshal.GetDelegateForFunctionPointer<CreateSenderFn>(Marshal.ReadIntPtr(vTable, 107 * sz));

            _SetReceiverName = Marshal.GetDelegateForFunctionPointer<SetReceiverNameFn>(Marshal.ReadIntPtr(vTable, 14 * sz));
            _CreateReceiver = Marshal.GetDelegateForFunctionPointer<CreateReceiverFn>(Marshal.ReadIntPtr(vTable, 109 * sz));
            _ReceiveImage = Marshal.GetDelegateForFunctionPointer<ReceiveImageFn>(Marshal.ReadIntPtr(vTable, 17 * sz));

            _CheckReceiver = Marshal.GetDelegateForFunctionPointer<CheckReceiverFn>(Marshal.ReadIntPtr(vTable, 110 * sz));
        }

        public static void CreateSender(string name)
        {
            _CreateSender(_obj, name, 100, 100, 0);
            _SetSenderName(_obj, name);
        }
        public static void SendImage(byte[] data, int sizeX, int sizeY)
        {
            GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
            _SendImage(_obj, pinnedArray.AddrOfPinnedObject(), (uint)sizeX, (uint)sizeY, 0x1908, false);
            pinnedArray.Free();
        }

        public static (int x, int y) CreateReciever(string name)
        {
            uint w = 0;
            uint h = 0;
            _CreateReceiver(_obj, name, ref w, ref h);
            _SetReceiverName(_obj, name);
            return ((int)w, (int)h);
        }

        public static void RecieveImage(ref byte[] data, string name)
        {
            uint w = 0;
            uint h = 0;
            bool connected = false;
            _CheckReceiver(_obj, name, ref w, ref h, ref connected);

            if(data == null || data.Length != w * h * 4)
                data = new byte[w * h * 4];

            GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
            _ReceiveImage(_obj, pinnedArray.AddrOfPinnedObject(), 0x1908, false, 0);
            pinnedArray.Free();
        }


        public static void SetName(string name)
        {
            _SetSenderName(_obj, name);
        }
    }
}
