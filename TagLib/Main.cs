using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityModManagerNet.UnityModManager;
using System.Reflection;
using HarmonyLib;
using SA.GoogleDoc;
using UnityEngine;

namespace TagLib
{
    public static class Main
    {
        public static ModEntry ModEntry { get; private set; }
        public static ModEntry.ModLogger Logger { get; private set; }
        public static Harmony Harmony { get; private set; }
        public static void Load(ModEntry modEntry)
        {
            ModEntry = modEntry;
            Logger = modEntry.Logger;
            Harmony = new Harmony(modEntry.Info.Id);
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
