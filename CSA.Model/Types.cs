using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSA.Model
{
    public enum ServerType
    {
        Dedicated = 100,    // 'd'
        Listen = 108,       // 'l'
        SourceTV = 115      // 'p' (proxy)
    }

    public enum Environment
    {
        Windows = 119,  // 'w'
        Mac1 = 109,     // 'm' 
        Mac2 = 111,     // 'o'
        Linux = 108     // 'l'
    }

    public enum Header
    {
        Unknown = 0,
        GoldSource = 0x6D,  // 'm'
        Source = 0x49       // 'I'
    }

    public enum Visibility
    {
        Public = 0,
        Private = 1
    }

    /// <summary>
    /// https://developer.valvesoftware.com/wiki/Steam_Application_ID
    /// </summary>
    public enum SteamApplicationId 
    {
        Unknown = -1,
        CounterStrike = 10,
        TheShip = 2400
    }

    public enum Vac
    {
        Unknown,
        Unsecured = 0,
        Secured = 1
    }

    public enum GoldSourceMod
    {
        Unknown,
        HalfLife = 0,
        HalfLifeMod = 1
    }

    public enum GoldSourceModType
    {
        Unknown,
        SingleAndMultiplayer = 0,
        MultiplayerOnly = 1
    }

    public enum GoldSourceModDll
    {
        Unknown,
        HalfLife = 0,
        Own = 1
    }
}
