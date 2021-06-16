using System;
using UnityEngine;
using KSP.UI.Screens;
using ToolbarControl_NS;

namespace Toggle_Ladder_Exit
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ToggleLadderExit : MonoBehaviour
    {
        static bool? EVA_LADDER_CHECK_END;

        const string KEYBINDCFG = "GameData/ToggleLadderExit/PluginData/keybind.cfg";
        const KeyCode DefaultKeyBind = KeyCode.L;
        KeyCode keyBind = DefaultKeyBind;

        static ToolbarControl toolbarControl;
        internal const string MODID = "ToggleLadderExit";
        internal const string MODNAME = "ToggleLadderExit";

        const string NODENAME = "TOGGLELADDEREXIT";
        const string VALUENAME = "keycode";
        bool toggled = false;


        void Start()
        {
            if (EVA_LADDER_CHECK_END == null)
                EVA_LADDER_CHECK_END = GameSettings.EVA_LADDER_CHECK_END;

            AddToolbarButton();
            GameEvents.onGameSceneSwitchRequested.Add(onGameSceneSwitchRequested);
            LoadKeybind();
        }

        public void LoadKeybind()
        {
            string path = KSPUtil.ApplicationRootPath + KEYBINDCFG;
            if (System.IO.File.Exists(path))
            {
                var keybindcfg = ConfigNode.Load(path);
                ConfigNode node = keybindcfg.GetNode(NODENAME);

                if (node.HasValue(VALUENAME))
                {
                    var keycode = SafeLoad(node.GetValue(VALUENAME), DefaultKeyBind.ToString());
                    keyBind = setActiveKeycode(keycode);
                }
            }
            else
            {
                ConfigNode file = new ConfigNode();
                ConfigNode node = new ConfigNode(NODENAME);
                node.AddValue(VALUENAME, DefaultKeyBind.ToString());
                file.AddNode(node);
                file.Save(path);
            }
        }

        static string SafeLoad(string value, string oldvalue)
        {
            if (value == null)
                return oldvalue;
            return value;
        }

        public KeyCode setActiveKeycode(string keycode)
        {
            var activeKeycode = (KeyCode)Enum.Parse(typeof(KeyCode), keycode);
            if (activeKeycode == KeyCode.None)
            {
                activeKeycode = DefaultKeyBind;
            }

            return activeKeycode;
        }

        void onGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> fta)
        {
            GameSettings.EVA_LADDER_CHECK_END = (bool)EVA_LADDER_CHECK_END;
        }

        void OnDestroy()
        {
            GameSettings.EVA_LADDER_CHECK_END = (bool)EVA_LADDER_CHECK_END;
            GameEvents.onGameSceneSwitchRequested.Remove(onGameSceneSwitchRequested);
        }


        void AddToolbarButton()
        {
            if (toolbarControl == null)
            {
                GameObject gameObject = new GameObject();
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(windowToggle, windowToggle,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.TRACKSTATION,
                    MODID,
                    "ToggleLadderExitBtn",
                    "ToggleLadderExit/PluginData/ToggleLadderExit-38",
                    "ToggleLadderExit/PluginData/ToggleLadderExit-24",
                    MODNAME
                );
            }
            SetToolbarIcon();
        }

        void SetToolbarIcon()
        {
            switch (GameSettings.EVA_LADDER_CHECK_END)
            {
                case true:
                    toolbarControl.SetTexture("ToggleLadderExit/PluginData/ToggleLadderExit-38", "ToggleLadderExit/PluginData/ToggleLadderExit-24");
                    break;
                case false:
                    ScreenMessages.PostScreenMessage("EVA Ladder Check Disabled");
                    toolbarControl.SetTexture("ToggleLadderExit/PluginData/ladder-disabled-38", "ToggleLadderExit/PluginData/ladder-disabled-24");
                    break;
            }
        }

        private void windowToggle()
        {
            GameSettings.EVA_LADDER_CHECK_END = !GameSettings.EVA_LADDER_CHECK_END;
            switch (GameSettings.EVA_LADDER_CHECK_END)
            {
                case true:
                    ScreenMessages.PostScreenMessage("EVA Ladder Check Enabled");
                    break;
                case false:
                    ScreenMessages.PostScreenMessage("EVA Ladder Check Disabled"); 
                    break;
            }
            SetToolbarIcon();
        }


        public void Update()
        {
            if (FlightGlobals.activeTarget.isKerbalEVA())
            {
                bool b = Input.GetKey(keyBind);
                if (!toggled &&  b)
                {
                    toggled = true;
                    windowToggle();
                }
                else
                {
                    if (toggled && !b)
                    {
                        windowToggle();
                        toggled = false;
                    }
                }
            }
        }
    }
}