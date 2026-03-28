using JHelper.Common.ProcessInterop;
using JHelper.Common.ProcessInterop.API;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LiveSplit.SonicFrontiers.GameEngine;

/// <summary>
/// A helper class used to identify GameService types by reading their in-memory name field.
/// This implementation reads a string pointer and caches the result cached based on the
/// object's vtable address to avoid repeated memory reads.
/// </summary>
internal class GameServiceResolver : Dictionary<string, IntPtr>
{
    /// <summary>
    /// Cache of resolved object names, keyed by vtable pointer.
    /// This avoids redundant memory reads for objects of the same type.
    /// </summary>
    private readonly Dictionary<IntPtr, string> cache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServiceResolver"/> class.
    /// </summary>
    internal GameServiceResolver() { }

    /// <summary>
    /// Attempts to resolve the name of a GameService instance from memory.
    /// </summary>
    /// <param name="instanceAddress">The memory address of the service class instance.</param>
    /// <param name="value">Outputs the resolved object name if successful; otherwise an empty string.</param>
    /// <returns>True if the name was successfully read or retrieved from cache; otherwise false.</returns>
    public bool Lookup(ProcessMemory process, IntPtr instanceAddress, out string value)
    {
        value = string.Empty;

        // Validate the instance address and read the vtable pointer.
        if (!process.Read(instanceAddress, out GameService gameService))
            return false;

        // Check if the name for this vtable is already cached.
        if (cache.TryGetValue(gameService.vtable.Value, out value))
            return true;

        // Read the pointer to the name string from the object.
        if (!process.ReadPointer(gameService.pStaticClass.Value, out IntPtr nameAddr))
            return false;

        // Read the ASCII string from memory.
        if (!process.ReadString(nameAddr, 127, StringType.ASCII, out value))
            return false;

        // Cache the result for future lookups.
        cache[gameService.vtable.Value] = value;
        return true;
    }

    /// <summary>
    /// A struct representation of `hh::game::GameService`.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    private struct GameService
    {
        [FieldOffset(0x0)] public Address<long> vtable;
        [FieldOffset(0x30)] public Address<long> pGameManager;
        [FieldOffset(0x38)] public Address<long> pStaticClass;
    }
}