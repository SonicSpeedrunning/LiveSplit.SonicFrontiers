using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;

namespace LiveSplit.SonicFrontiers
{   
    class RTTI : Dictionary<string, IntPtr>
    {
        /// <summary>
        /// This class returns a Dictionary of RTTI classes with the vtable addresses for each one.
        /// In order to use this, use AddEntry() to import the RTTI classes you want to employ in
        /// your autosplitter.
        /// </summary>
        /// <param name="scanner">A SignatureScanner object that will be used to search for the RTTI entried in memory</param>
        /// <param name="entries">A string[] with the names of the RTTI classes for which we want to calculate the vTable address. The names must be precise: if a class name doesn't exist, an Exception will be thrown.</param>
        public RTTI(SignatureScanner scanner, params string[] entries)
        {
            Dictionary<string, int> RTTI_Entries = new Dictionary<string, int>();
            long mainMbase = (long)scanner.Process.MainModuleWow64Safe().BaseAddress;
            IEnumerable<IntPtr> init = scanner.ScanAll(new SigScanTarget("2E 3F 41 56"), 8);

            foreach (var entry in init)
                RTTI_Entries.Add(scanner.Process.ReadString(entry + 4, 250).Replace("@@", "").Replace("@", "::"), (int)((long)entry - mainMbase - 0x10));

            foreach (var entry in entries)
            {
                long temp = (long)scanner.ScanOrThrow(new SigScanTarget(BitConverter.GetBytes(RTTI_Entries[entry])), 4) - 0xC;
                IntPtr vTableAddr = scanner.ScanOrThrow(new SigScanTarget(BitConverter.GetBytes(temp)), 8) + 0x8;
                this[entry] = vTableAddr;
            }

            RTTI_Entries.Clear();
        }
    }
}
