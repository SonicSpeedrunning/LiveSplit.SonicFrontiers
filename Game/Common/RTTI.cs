using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace LiveSplit.SonicFrontiers
{
    class RTTI : Dictionary<string, long>
    {
        // Black magic.
        // Ok not really, but I don't really want to mess up with this code anyway
        public RTTI(SignatureScanner scanner, Process game, params string[] entries)
        {
            var mainMbase = (long)game.MainModuleWow64Safe().BaseAddress;
            var init = scanner.ScanAll(new SigScanTarget("2E 3F 41 56"), 8);

            var RTTI_Entries = new Dictionary<string, int>();

            foreach (var entry in init)
                RTTI_Entries.Add(new DeepPointer(entry).DerefString(game, 255).Replace(".?AV", "").Replace("@@", "").Replace("@", "::"), (int)((long)entry - mainMbase - 0x10));

            foreach (var _entry in entries)
                this[_entry] = (long)scanner.Scan(new SigScanTarget(BitConverter.GetBytes((long)scanner.Scan(new SigScanTarget(BitConverter.GetBytes(RTTI_Entries[_entry])), 4) - 0xC)), 8) + 0x8;

            RTTI_Entries.Clear();
        }
    }
}
