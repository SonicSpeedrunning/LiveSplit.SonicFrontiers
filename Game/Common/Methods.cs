using System;
using LiveSplit.ComponentUtil;

namespace LiveSplit.SonicFrontiers
{
    public static class ExtensionMethods
    {
        public static IntPtr ScanOrThrow(this SignatureScanner ss, SigScanTarget sst)
        {
            IntPtr tempAddr = ss.Scan(sst);
            CheckPtr(tempAddr);
            return tempAddr;
        }

        public static void CheckPtr(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new SigscanFailedException();
        }
    }

    public class SigscanFailedException : Exception
    {
        public SigscanFailedException() { }
        public SigscanFailedException(string message) : base(message) { }
    }
}