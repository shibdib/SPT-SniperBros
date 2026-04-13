using System.Linq;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using Comfort.Common;
using BepInEx.Logging;

namespace SniperBros.Patches
{
    internal class HostilityPatches : ModulePatch 
    {
        public static ManualLogSource LogSource;
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotOwner), nameof(BotOwner.method_10));
        }

        [PatchPostfix]
        private static void PatchPostfix(BotOwner __instance)
        {
            LogSource = Logger;
            // Role sanity check
            var role = __instance.Profile.Info?.Settings?.Role;
            if (role is null) {
                return;
            }
            
            // AI sanity check
            var ai = __instance.IsAI;
            if (!ai)
            {
                return;
            }

            SetAiAsFriends(__instance);
        }
        
        // Set all AI as friendly to marksman and vice versa
        private static void SetAiAsFriends(BotOwner newBot)
        {
            var aiMarksman = Singleton<GameWorld>.Instance.AllAlivePlayersList
                .Where(p => p.IsAI && p.Profile.Info?.Settings?.Role == WildSpawnType.marksman);
            foreach (var marksman in aiMarksman)
            {
                newBot.BotsGroup.AddAlly(marksman);
                marksman.AIData?.BotOwner.BotsGroup.AddAlly(newBot.GetPlayer);
            }
        }
    }
}
