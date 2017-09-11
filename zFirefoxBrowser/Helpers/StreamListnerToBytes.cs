using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zFirefoxBrowser.Helpers
{
    public unsafe class StreamListnerToBytes : nsIStreamListener
    {
        public event Action<byte[]> OnstremFinished;
        List<byte> buffer = new List<byte>();

        public unsafe void OnDataAvailable(nsIRequest aRequest, nsISupports aContext, nsIInputStream aInputStream, ulong aOffset, uint aCount)
        {
            IntPtr aBuf = Marshal.AllocCoTaskMem((int)aCount);
            aInputStream.Read(aBuf, aCount);
            byte[] tempBuffer = new byte[aCount];
            Marshal.Copy(aBuf, tempBuffer, 0, (int)aCount);
            buffer.AddRange(tempBuffer);
        }

        public void OnStartRequest(nsIRequest aRequest, nsISupports aContext)
        {
            buffer.Clear();
        }

        public void OnStopRequest(nsIRequest aRequest, nsISupports aContext, int aStatusCode)
        {
            OnstremFinished?.Invoke(buffer.ToArray());
        }
    }
}
