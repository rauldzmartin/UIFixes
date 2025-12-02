using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using SPTarkov.Server.Core.DI;
using System.Collections.Generic;

namespace UIFixes.Server
{
    [HarmonyPatch(typeof(InventoryHelper), nameof(InventoryHelper.RemoveItem))]
    public class FastAccessCleanerPatch
    {
        public static void Prefix(string pmcId, string itemId, string sessionID)
        {
            try
            {
                var saveServerType = Type.GetType("SPTarkov.Server.Core.Server.SaveServer, SPTarkov.Server.Core");
                if (saveServerType == null) return;

                dynamic saveServer = ServiceLocator.ServiceProvider.GetService(saveServerType);
                if (saveServer == null) return;

                dynamic profile = saveServer.GetProfile(sessionID);
                if (profile == null) return;

                // Access properties dynamically
                var characters = profile.Characters;
                if (characters == null) return;

                var pmc = characters.Pmc;
                if (pmc == null) return;

                var inventory = pmc.Inventory;
                if (inventory == null) return;

                var fastAccess = inventory.FastAccess as Dictionary<string, string>;
                if (fastAccess == null) return;
                
                var slotToRemove = fastAccess.FirstOrDefault(x => x.Value == itemId).Key;

                if (!string.IsNullOrEmpty(slotToRemove))
                {
                    fastAccess.Remove(slotToRemove);
                }
            }
            catch (Exception)
            {
                // Ignore errors
            }
        }
    }
}
