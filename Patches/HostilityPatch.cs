using System.Linq;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using Comfort.Common;

namespace SniperBros.Patches
{
    internal class HostilityPatches : ModulePatch 
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotOwner), nameof(BotOwner.method_10));
        }

        [PatchPostfix]
        private static void PatchPostfix(BotOwner __instance)
        {
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

            if (role != WildSpawnType.marksman && role != WildSpawnType.bossBoarSniper)
            {
                SetMarksmanAsFriends(__instance);
            }
            else
            {
                SetOtherAiAsFriends(__instance);
            }
        }
        
        // Set all AI as friendly to marksman and vice versa
        private static void SetMarksmanAsFriends(BotOwner newBot)
        {
            var aiMarksman = Singleton<GameWorld>.Instance.AllAlivePlayersList
                .Where(p => p.IsAI && p.Profile.Info?.Settings?.Role is WildSpawnType.marksman or WildSpawnType.bossBoarSniper);
            foreach (var marksman in aiMarksman)
            {
                newBot.BotsGroup.AddAlly(marksman);
                marksman.AIData?.BotOwner.BotsGroup.AddAlly(newBot.GetPlayer);
            }
        }
        
        private static void SetOtherAiAsFriends(BotOwner newBot)
        {
            var aiBots = Singleton<GameWorld>.Instance.AllAlivePlayersList
                .Where(p => p.IsAI);
            foreach (var ai in aiBots)
            {
                newBot.BotsGroup.AddAlly(ai);
                ai.AIData?.BotOwner.BotsGroup.AddAlly(newBot.GetPlayer);
            }
        }
    }
}
