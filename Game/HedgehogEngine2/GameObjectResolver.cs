using JHelper.Common.ProcessInterop;
using JHelper.Common.ProcessInterop.API;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LiveSplit.SonicFrontiers.GameEngine;

/// <summary>
/// A helper class used to identify GameObject types by reading their in-memory name field.
/// This implementation reads a string pointer and caches the result cached based on the
/// object's vtable address to avoid repeated memory reads.
/// </summary>
internal class GameObjectResolver : Dictionary<string, IntPtr>
{
    /// <summary>
    /// Cache of resolved object names, keyed by vtable pointer.
    /// This avoids redundant memory reads for objects of the same type.
    /// </summary>
    private readonly Dictionary<IntPtr, string> cache = new Dictionary<IntPtr, string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameObjectResolver"/> class.
    /// </summary>
    internal GameObjectResolver() { }
    
    /// <summary>
    /// Attempts to resolve the name of a GameObject instance from memory.
    /// 
    /// The method reads the object's vtable pointer to use as a cache key,
    /// then reads a string pointer at a fixed offset (<c>0xB8</c>) from the instance.
    /// </summary>
    /// <param name="instanceAddress">The memory address of the object instance.</param>
    /// <param name="value">Outputs the resolved object name if successful; otherwise an empty string.</param>
    /// <returns>True if the name was successfully read or retrieved from cache; otherwise false.</returns>
    public bool Lookup(ProcessMemory process, IntPtr instanceAddress, out string value)
    {
        value = string.Empty;

        // Validate the instance address and read the vtable pointer.
        if (!process.Read(instanceAddress, out GameObject gameObject))
            return false;

        // Check if the name for this vtable is already cached.
        if (cache.TryGetValue(gameObject.vtable.Value, out value))
            return true;

        // Read the ASCII string from memory.
        if (!process.ReadString(gameObject.nameAddr.Value, 127, StringType.ASCII, out value))
            return false;

        // Cache the result for future lookups.
        cache[gameObject.vtable.Value] = value;
        return true;
    }

    /// <summary>
    /// A struct representation of `hh::game::GameObject`.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    private struct GameObject
    {
        [FieldOffset(0x00)] public Address<long> vtable;
        [FieldOffset(0x30)] public byte flags;
        [FieldOffset(0x48)] public Address<long> pGameManager;
        [FieldOffset(0xB8)] public Address<long> nameAddr;
    }
}