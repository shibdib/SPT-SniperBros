using BepInEx;

namespace SniperBros
{
    [BepInPlugin("com.shibdib.sniperbros", "SniperBros", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new Patches.HostilityPatches().Enable();
        }
    }
}
