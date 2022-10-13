using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;

namespace STM9_plugin
{
    [BepInDependency("pl.szikaka.receiver_2_modding_kit")]
    [BepInPlugin("Ciarencew.STM9", "STM9 Plugin", "1.0.0")]
    internal class MainPlugin : BaseUnityPlugin
    {
        public static MainPlugin instance
        {
            get;
            private set;
        }

        public static readonly string folder_name = "STM9_Files";
        private void Awake()
        {
            Logger.LogInfo("STM9 Main Plugin loaded!");

            instance = this;
        }
    }

}
