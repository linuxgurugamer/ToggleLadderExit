

using ToolbarControl_NS;
using UnityEngine;

namespace Toggle_Ladder_Exit
{

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar2 : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(ToggleLadderExit.MODID, ToggleLadderExit.MODNAME);
        }
    }

}
