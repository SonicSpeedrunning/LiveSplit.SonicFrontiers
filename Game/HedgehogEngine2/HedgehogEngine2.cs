using JHelper.Common.MemoryUtils;
using JHelper.Common.ProcessInterop;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using LiveSplit.Options;

namespace LiveSplit.SonicFrontiers.GameEngine;

/// <summary>
/// The HedgehogEngine2 class provides memory interaction for games based on the Hedgehog Engine 2.
/// This class has been tailored for Sonic Frontiers, enabling real-time memory analysis
/// and caching of essential game objects like GameManager, Boss instances, and game modes.
/// </summary>
internal class HedgehogEngine2
{
    /// <summary>
    /// Pointer to the main base address of the game manager instance.
    /// </summary>
    private readonly IntPtr pGameManager; // Points to an instance of hh:game::GameManager

    /// <summary>
    /// Runtime Type Information instance used to look up and identify in-memory types.
    /// </summary>
    internal RTTI RTTI { get; }

    /// <summary>
    /// Dictionary that stores cached addresses for various game services.
    /// </summary>
    private readonly GameServiceResolver _services = []; 

    /// <summary>
    /// Dictionary that stores cached addresses for different in-game objects.
    /// </summary>
    private readonly GameObjectResolver _objects = [];

    /// <summary>
    /// Dictionary that stores cached addresses for application extensions.
    /// </summary>
    private readonly Dictionary<string, IntPtr> _extensions = [];

    /// <summary>
    /// The name of the current instance of app::game::GameMode, recovered through RTTI
    /// </summary>
    internal string GameMode { get; private set; } = string.Empty;

    /// <summary>
    /// Address of the game's routine responsible for pausing the game when the main window goes out of focus
    /// </summary>
    internal IntPtr HWndAddress { get; }

    /// <summary>
    /// Offset for the internal IGT applied during Cyber Stages.
    /// </summary>
    /// <remarks>
    /// Generally known to be a fixed, constant value (around 0.07 seconds).
    /// In this autosplitter the value is directly read from memory and cached
    /// here for usage during the main update loop.
    /// </remarks>
    public float IGTSubtraction { get; }

    public byte ApplicationSequenceExtensionFlags0 { get; private set; }
    public byte ApplicationSequenceExtensionFlags1 { get; private set; }

    /// <summary>
    /// Initializes a new instance of the HedgehogEngine2 class and locates
    /// the pointer to the main game manager in memory.
    /// </summary>
    /// <param name="process">The target process</param>
    public HedgehogEngine2(ProcessMemory process)
    {
        // Perform memory scan to find GameManager base address.
        pGameManager = process
            .MainModule
            .ScanAll(new ScanPattern(7, "48 89 41 18 48 89 2D") { OnFound = addr => addr + 0x4 + process.Read<int>(addr) })
            .First();

        HWndAddress = process
            .MainModule
            .ScanAll(new ScanPattern(0, "?? 36 48 8B 52 28"))
            .First();

        // Initialize RTTI for type information.
        RTTI = new RTTI(process);

        // Retrieve the IGT offset for Cyber Stages
        // FIXME: THIS CODE NEEDS TO BE REVIEWED AND COMPLETED AS IT BREAKS WITH OLDER VERSIONS OF THE GAME
        IntPtr igtSub = process.MainModule.Scan(new ScanPattern(0xE, "F3 0F 11 49 ?? F3 41 0F 58 ?? F3 0F 5C 0D") { OnFound = addr => addr + 4 + process.Read<int>(addr)});
        Log.Info(igtSub.ToString("X"));
        if (!process.Read<float>(igtSub, out float igt))
            throw new InvalidProgramException("Could not find scan address for IGT subtraction");
        IGTSubtraction = igt;
    }

    /// <summary>
    /// Resets all cached memory addresses, forcing the next update to re-cache them.
    /// </summary>
    private void ResetCache()
    {
        GameMode = string.Empty;
        _services.Clear();
        _objects.Clear();
        _extensions.Clear();
    }

    /// <summary>
    /// Updates the cached memory addresses for important game objects by reading from
    /// the process memory. This method should be called periodically to refresh the cached values.
    /// </summary>
    /// <param name="process">The target process memory handler.</param>
    [SkipLocalsInit]
    public void Update(ProcessMemory process)
    {
        // Step 1: Reset the cached addresses at the beginning of each update cycle
        ResetCache();

        // Step 2: Attempt to read the main GameManager struct
        if (!process.ReadPointer(pGameManager, out IntPtr _gameManager)
            || !process.Read(_gameManager, out GameManager gameManager))
            return;


        // Scan the game services
        if (gameManager.noOfGameServices > 0 && gameManager.noOfGameServices < 2048)
        {
            using (ArrayRental<Address<long>> rent = new(gameManager.noOfGameServices))
            {
                if (process.ReadArray(gameManager.GameServices.Value, rent.Span))
                {
                    foreach (var entry in rent.Span)
                    {
                        if (_services.Lookup(process, entry.Value, out string value))
                            _services[value] = entry.Value;
                    }
                }
            }
        }

        // Scan game application extensions.
        if (process.Read(gameManager.gameApplication.Value, out GameApplication gameApplication)
            && gameApplication.noOfApplicationExtensions > 0
            && gameApplication.noOfApplicationExtensions < 64)
        {
            IntPtr ase = IntPtr.Zero;

            using (ArrayRental<Address<long>> rent = new(gameApplication.noOfApplicationExtensions))
            {
                // Read array of pointers for ApplicationSequenceExtension
                if (process.ReadArray(gameApplication.ApplicationExtensions.Value, rent.Span))
                {
                    foreach (var entry in rent.Span)
                    {
                        // Identify ApplicationSequenceExtension using RTTI lookup.
                        if (RTTI.Lookup(entry.Value, out string value))
                        {
                            if (value == "ApplicationSequenceExtension")
                            {
                                ase = entry.Value;
                                break;
                            }
                        }
                    }
                }
            }

            // If ApplicationSequenceExtension is found, locate GameMode.
            if (ase != IntPtr.Zero)
            {
                // Try to read the pointer for GameMode instance.
                if (process.Read(ase, out ApplicationSequenceExtension extension))
                {
                    ApplicationSequenceExtensionFlags0 = extension.Flags0;
                    ApplicationSequenceExtensionFlags1 = extension.Flags1;

                    if (RTTI.Lookup(extension.GameMode.Value, out string gameModeText))
                        GameMode = gameModeText;

                    // Read current instance of GameModeExtension.
                    if (process.Read(extension.GameMode.Value, out GameMode gameMode) && gameMode.noOfExtensions > 0 && gameMode.noOfExtensions < 128)
                    {
                        using (ArrayRental<Address<long>> rent = gameMode.noOfExtensions < 25 ? new(stackalloc Address<long>[gameMode.noOfExtensions]) : new(gameMode.noOfExtensions))
                        {
                            // Retrieve array of extensions
                            if (process.ReadArray(gameMode.extensions.Value, rent.Span))
                            {
                                foreach (var entry in rent.Span)
                                {
                                    if (RTTI.Lookup(entry.Value, out string value))
                                        _extensions[value] = entry.Value;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Scan the game objects.
        if (gameManager.noOfGameObjects > 0 && gameManager.noOfGameObjects < 4096)
        {
            using (ArrayRental<Address<long>> rent = new(gameManager.noOfGameObjects))
            {
                if (process.ReadArray(gameManager.GameObjects.Value, rent.Span))
                {
                    foreach (var entry in rent.Span)
                    {
                        if (_objects.Lookup(process, entry.Value, out string value))
                            _objects[value] = entry.Value;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Retrieves a cached instance pointer of a game service by name.
    /// </summary>
    /// <param name="name">The name of the service to retrieve.</param>
    /// <param name="instance">The instance pointer of the service, if found.</param>
    /// <returns>True if the service is found; otherwise, false.</returns>
    public bool GetService(string name, out IntPtr instance)
    {
        return _services.TryGetValue(name, out instance);
    }

    /// <summary>
    /// Retrieves a cached instance pointer of a game object by name.
    /// </summary>
    /// <param name="name">The name of the object to retrieve.</param>
    /// <param name="instance">The instance pointer of the object, if found.</param>
    /// <returns>True if the object is found; otherwise, false.</returns>
    public bool GetObject(string name, out IntPtr instance)
    {
        return _objects.TryGetValue(name, out instance);
    }

    /// <summary>
    /// Retrieves a cached instance pointer of an application extension by name.
    /// </summary>
    /// <param name="name">The name of the extension to retrieve.</param>
    /// <param name="instance">The instance pointer of the extension, if found.</param>
    /// <returns>True if the extension is found; otherwise, false.</returns>
    public bool GetExtension(string name, out IntPtr instance)
    {
        return _extensions.TryGetValue(name, out instance);
    }
}