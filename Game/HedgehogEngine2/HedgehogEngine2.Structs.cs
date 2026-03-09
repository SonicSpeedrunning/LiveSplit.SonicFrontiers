using System;
using System.Runtime.InteropServices;

namespace LiveSplit.SonicFrontiers.GameEngine;


[StructLayout(LayoutKind.Explicit, Size = 0x358)]
public readonly struct GameManager
{
    [FieldOffset(0x130)] private readonly long _pGameObjects;
    [FieldOffset(0x138)] public readonly int noOfGameObjects;

    [FieldOffset(0x150)] private readonly long _pGameServices;
    [FieldOffset(0x158)] public readonly int noOfGameServices;

    [FieldOffset(0x350)] private readonly long _gameApplication;

    public IntPtr GameObjects => (IntPtr) _pGameObjects;
    public IntPtr GameServices => (IntPtr) _pGameServices;
    public IntPtr GameApplication => (IntPtr) _gameApplication;
}

[StructLayout(LayoutKind.Explicit, Size = 0x94)]
public readonly struct GameApplication
{
    [FieldOffset(0x80)] private readonly long _applicationExtensions;
    [FieldOffset(0x88)] public readonly short noOfApplicationExtensions;

    public IntPtr ApplicationExtensions => (IntPtr)_applicationExtensions;
}

[StructLayout(LayoutKind.Explicit, Size = 0x80)]
public readonly struct ApplicationSequenceExtension
{
    [FieldOffset(0x78)] public readonly long _gameMode;

    public IntPtr GameMode => (IntPtr)_gameMode;
}

[StructLayout(LayoutKind.Explicit)]
public readonly struct GameMode
{
    [FieldOffset(0xB0)] public readonly long _extensions;
    [FieldOffset(0xB8)] public readonly short noOfExtensions;

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