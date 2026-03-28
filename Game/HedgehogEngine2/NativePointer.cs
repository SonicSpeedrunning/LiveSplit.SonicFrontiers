using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LiveSplit.SonicFrontiers.GameEngine;

/// <summary>
/// A readonly wrapper around an unmanaged primitive value that exposes it
/// as a platform-native pointer (<see cref="IntPtr"/>).
/// </summary>
/// <typeparam name="T">
/// An unmanaged type whose size is exactly 4 or 8 bytes (e.g. <c>int</c>,
/// <c>uint</c>, <c>long</c>, <c>ulong</c>). Behaviour is undefined for any
/// other size.
/// </typeparam>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Address<T> where T : unmanaged
{
    private readonly T value;

    /// <summary>
    /// Reinterprets the raw bytes of <see cref="value"/> as a
    /// platform-native pointer.
    /// </summary>
    /// <value>
    /// An <see cref="IntPtr"/> whose bit pattern matches the stored value.
    /// On 64-bit platforms <typeparamref name="T"/> should be an 8-byte
    /// type; on 32-bit platforms a 4-byte type.
    /// </value>
    /// <remarks>
    /// <para>
    /// <b>Warning:</b> if <typeparamref name="T"/> is neither 4 nor 8
    /// bytes the property silently reads out-of-bounds memory.  The caller
    /// is responsible for ensuring a matching type is used.
    /// </para>
    /// </remarks>
    unsafe public IntPtr Value => sizeof(T) == 8
        ? (IntPtr)Unsafe.As<T, long>(ref Unsafe.AsRef(value))
        : (IntPtr)Unsafe.As<T, int>(ref Unsafe.AsRef(value));
}