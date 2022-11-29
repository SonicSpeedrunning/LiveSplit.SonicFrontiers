using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace LiveSplit.SonicFrontiers
{   
    class RTTI : Dictionary<string, IntPtr>
    {
        private readonly Dictionary<string, int> RTTI_Entries = new Dictionary<string, int>();

        /// <summary>
        /// This class returns a Dictionary of RTTI classes with the vtable addresses for each one.
        /// In order to use this, use AddEntry() to import the RTTI classes you need to employ in
        /// your autosplitter.
        /// </summary>
        public RTTI(SignatureScanner scanner)
        {
            var mainMbase = (long)scanner.Process.MainModuleWow64Safe().BaseAddress;

            var init = scanner.ScanAll(new SigScanTarget("2E 3F 41 56"), 8);

            foreach (var entry in init)
                RTTI_Entries.Add(new DeepPointer(entry).DerefString(scanner.Process, 255).Replace(".?AV", "").Replace("@@", "").Replace("@", "::"), (int)((long)entry - mainMbase - 0x10));
        }

        /// <summary>
        /// Adds an entry to the RTTI dictionary. The function will calculate the vtable address.
        /// If you entered an invalid class name, this will return an exception.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="entry"></param>
        public void AddEntry(SignatureScanner scanner, string entry)
        {
            var temp = (long)scanner.Scan(new SigScanTarget(BitConverter.GetBytes(RTTI_Entries[entry])), 4) - 0xC;
            var temp2 = scanner.Scan(new SigScanTarget(BitConverter.GetBytes(temp)), 8) + 0x8;
            this[entry] = temp2;
        }

        /// <summary>
        /// Clears the internal RTTI_Entries dictionary. Used as a memory-saving feature.
        /// </summary>
        public void ClearDict()
        {
            RTTI_Entries.Clear();
        }
    }
}
