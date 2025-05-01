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
        delegate bool SendImageFn(IntPtr self, IntPtr pixels, uint w, uint h, uint glFmt, bool invert);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void ReleaseSenderFn(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool SetSenderName(IntPtr self, string name);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool UpdateSenderFn(IntPtr self, string name, uint w, uint h);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool SetSenderNameFn(IntPtr self, string name);

        static IntPtr _obj;
        static CreateSenderFn _CreateSender;
        static SendImageFn _SendImage;
        static ReleaseSenderFn _ReleaseSender;
        static UpdateSenderFn _UpdateSender;
        static SetSenderNameFn _SetName;

        static SpoutWrapper()
        {
            _obj = GetSpout();

            IntPtr vTable = Marshal.ReadIntPtr(_obj);

            int sz = IntPtr.Size;

            _SetName = Marshal.GetDelegateForFunctionPointer<SetSenderNameFn>(Marshal.ReadIntPtr(vTable, 0));
            //_ReleaseSender = Marshal.GetDelegateForFunctionPointer<ReleaseSenderFn>(Marshal.ReadIntPtr(vTable, 2 * sz));
            _SendImage = Marshal.GetDelegateForFunctionPointer<SendImageFn>(Marshal.ReadIntPtr(vTable, 5 * sz));
            _CreateSender = Marshal.GetDelegateForFunctionPointer<CreateSenderFn>(Marshal.ReadIntPtr(vTable, 49 * sz));
            //_UpdateSender = Marshal.GetDelegateForFunctionPointer<UpdateSenderFn>(Marshal.ReadIntPtr(vTable, 50 * sz));
        }

        public static void CreateSender(string name)
        {
            _CreateSender(_obj, name, 100, 100, 0);
            _SetName(_obj, name);
        }
        public static void SendImage(byte[] data, int sizeX, int sizeY)
        {
            GCHandle pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
            _SendImage(_obj, pinnedArray.AddrOfPinnedObject(), (uint)sizeX, (uint)sizeY, 0x1908, false);
            pinnedArray.Free();
        }
        //public static void ReleaseSender()
        //{
        //    _ReleaseSender(_obj);
        //}
        //static void UpdateSender(string name, int w, int h)
        //{
        //    return _UpdateSender(_obj, name, (uint)w, (uint)h);
        //}
        public static void SetName(string name)
        {
            _SetName(_obj, name);
        }
    }
}
