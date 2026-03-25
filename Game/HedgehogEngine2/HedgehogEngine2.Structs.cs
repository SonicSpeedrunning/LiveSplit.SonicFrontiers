using System;
using System.Runtime.InteropServices;

namespace LiveSplit.SonicFrontiers.GameEngine;

/// <summary>
/// A struct representation of `hh::game::GameManager`.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct GameManager
{
    /// <summary>
    /// An array of pointers to `hh::game::GameObject` instances.
    /// </summary>
    [FieldOffset(0x130)] private readonly long _pGameObjects;
    [FieldOffset(0x138)] public readonly int noOfGameObjects;

    /// <summary>
    /// An array of pointers to `hh::game::GameService` instances.
    /// </summary>
    [FieldOffset(0x150)] private readonly long _pGameServices;
    [FieldOffset(0x158)] public readonly int noOfGameServices;

    /// <summary>
    /// A pointer to an instance of `app::MyApplication`.
    /// </summary>
    [FieldOffset(0x350)] private readonly long _gameApplication;

    /// <summary>
    /// An array of pointers to `hh::game::GameObject` instances.
    /// </summary>
    public IntPtr GameObjects => (IntPtr) _pGameObjects;

    /// <summary>
    /// An array of pointers to `hh::game::GameService` instances.
    /// </summary>
    public IntPtr GameServices => (IntPtr) _pGameServices;

    /// <summary>
    /// A pointer to an instance of `app::MyApplication`.
    /// </summary>
    public IntPtr GameApplication => (IntPtr) _gameApplication;
}


/// <summary>
/// A struct representation of `app::MyApplication`.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct GameApplication
{
    [FieldOffset(0x80)] private readonly long _applicationExtensions;
    [FieldOffset(0x88)] public readonly short noOfApplicationExtensions;

    /// <summary>
    /// An array of pointers to `app::game::ApplicationExtension` instances.
    /// </summary>
    public IntPtr ApplicationExtensions => (IntPtr)_applicationExtensions;
}


/// <summary>
/// A struct representation of `app::game::ApplicationSequenceExtension`.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct ApplicationSequenceExtension
{
    [FieldOffset(0x78)] public readonly long _gameMode;

    /// <summary>
    /// A pointer to an instance of `app::game::GameMode`.
    /// </summary>
    public IntPtr GameMode => (IntPtr)_gameMode;
}

/// <summary>
/// A struct representation of `app::game::GameMode`.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct GameMode
{
    /// <summary>
    /// An array of pointers to instances of `app::game::GameModeExtension`.
    /// </summary>
    [FieldOffset(0xB0)] public readonly long _extensions;
    [FieldOffset(0xB8)] public readonly short noOfExtensions;

    /// <summary>
    /// An array of pointers to instances of `app::game::GameModeExtension`.
    /// </summary>
    public IntPtr Extensions => (IntPtr)_extensions;
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct LevelInfo
{
    [FieldOffset(0x78)] private readonly long _stageData;

    public IntPtr StageData => (IntPtr)_stageData;
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct StageData
{
    [FieldOffset(0x18)] private readonly long _name;

    public IntPtr Name => (IntPtr)_name;
}

[StructLayout(LayoutKind.Explicit, Size = 0xC0)]
public readonly struct SaveManager
{
    [FieldOffset(0xB8)] private readonly long _saveInterface;

    public IntPtr SaveInterface => (IntPtr)_saveInterface;
}