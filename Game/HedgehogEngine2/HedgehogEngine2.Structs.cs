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
    [FieldOffset(0x130)] public readonly Address<long> GameObjects;
    [FieldOffset(0x138)] public readonly int noOfGameObjects;

    /// <summary>
    /// An array of pointers to `hh::game::GameService` instances.
    /// </summary>
    [FieldOffset(0x150)] public readonly Address<long> GameServices;
    [FieldOffset(0x158)] public readonly int noOfGameServices;

    /// <summary>
    /// A pointer to an instance of `app::MyApplication`.
    /// </summary>
    [FieldOffset(0x350)] public readonly Address<long> gameApplication;
}


/// <summary>
/// A struct representation of `app::MyApplication`.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct GameApplication
{
    /// <summary>
    /// An array of pointers to `app::game::ApplicationExtension` instances.
    /// </summary>
    [FieldOffset(0x80)] public readonly Address<long> ApplicationExtensions;
    [FieldOffset(0x88)] public readonly short noOfApplicationExtensions;
}


/// <summary>
/// A struct representation of `app::game::ApplicationSequenceExtension`.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct ApplicationSequenceExtension
{
    /// <summary>
    /// A pointer to an instance of `app::game::GameMode`.
    /// </summary>
    [FieldOffset(0x78)] public readonly Address<long> GameMode;

    [FieldOffset(0x122)] public readonly byte Flags0;
    [FieldOffset(0x123)] public readonly byte Flags1;
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
    [FieldOffset(0xB0)] public readonly Address<long> extensions;
    [FieldOffset(0xB8)] public readonly short noOfExtensions;
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct LevelInfo
{
    [FieldOffset(0x78)] public readonly Address<long> stageData;
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct StageData
{
    [FieldOffset(0x18)] public readonly Address<long> Name;
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct SaveManager
{
    [FieldOffset(0xB8)] public readonly Address<long> saveInterface;
}