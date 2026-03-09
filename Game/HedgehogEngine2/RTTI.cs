using JHelper.Common.ProcessInterop;
using JHelper.Common.ProcessInterop.API;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LiveSplit.SonicFrontiers.GameEngine;

/// <summary>
/// A class used to perform RTTI (Run-Time Type Information) lookups
/// for game objects in a specific process's memory space. This class 
/// identifies object types by analyzing RTTI descriptors in memory 
/// based on the object's vtable address.
/// </summary>
internal class RTTI
{
    // Regex to validate the RTTI signature format. Specifically matches RTTI names that follow the pattern 
    // ".?AV" followed by the class name (which does not contain '@') and ending with one or more '@' characters.
    private static readonly Regex regex = new(@"^\.\?AV([^@]+)@.*@@$", RegexOptions.Compiled);

    /// <summary>
    /// The process memory class used to read memory from the target process
    /// </summary>
    private readonly ProcessMemory process;

    /// <summary>
    /// A cache for storing previously identified RTTI type names, 
    /// mapped by memory offset, to reduce redundant lookups.
    /// </summary>
    private readonly Dictionary<IntPtr, string> cache = new Dictionary<IntPtr, string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="RTTI"/> class with the specified process memory interface.
    /// </summary>
    /// <param name="process">The process memory interface for reading data.</param>
    internal RTTI(ProcessMemory process)
    {
        this.process = process;
    }

    /// <summary>
    /// Looks up the RTTI (Run-Time Type Information) name for an object
    /// located at the given instance address.
    /// </summary>
    /// <param name="instanceAddress">The memory address of the object instance.</param>
    /// <param name="value">Outputs the RTTI type name if found, otherwise an empty string.</param>
    /// <returns>True if the lookup succeeds; otherwise, false.</returns>
    public bool Lookup(IntPtr instanceAddress, out string value)
    {
        value = string.Empty;

        // Validate the instance address and read the vtable pointer.
        if (instanceAddress == IntPtr.Zero || !process.ReadPointer(instanceAddress, out IntPtr vtable))
            return false;

        nint baseAddress = process.MainModule.BaseAddress;
        nint endAddress = process.MainModule.BaseAddress + process.MainModule.ModuleMemorySize;

        // Ensure the vtable address is within bounds and non-zero
        if (vtable == IntPtr.Zero || vtable < baseAddress || vtable > endAddress)
            return false;

        // Check if the type name for this offset is already cached
        if (cache.TryGetValue(vtable, out value))
            return true;
        else
            value = string.Empty;

        // Adjust pointer calculations and offsets based on the process architecture.
        IntPtr rttiDescriptorAddress = vtable - process.PointerSize;

        // Read the RTTI descriptor pointer.
        if (!process.ReadPointer(rttiDescriptorAddress, out IntPtr addr))
            return false;

        // Offset to the RTTI type name location does not depend on process architecture.
        if (!process.Read(addr + 0xC, out int val))
            return false;

        // Calculate the final address for reading the RTTI type name string.
        IntPtr typeNameAddress = process.Is64Bit ? baseAddress + val + 0x10 : (IntPtr)val + 0x8;
        if (!process.ReadString(typeNameAddress, 127, StringType.ASCII, out string finalValue))
            return false;

        // Use regex to match and extract the actual type name from the RTTI string.
        Match match = regex.Match(finalValue);

        // Return false if the regex does not match
        if (!match.Success)
            return false;

        value = match.Groups[1].Value;
        cache[vtable] = value;
        return true;
    }
}
