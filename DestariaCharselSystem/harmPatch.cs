using System;
using System.Collections.Generic;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using Vintagestory.API.Datastructures;

namespace DestariaCharselSystem
{
    [HarmonyPatch]
    public class harmPatch
    {
        // Patch InventoryTrader.HandleMoneyTransaction(EntityAgent)
        [HarmonyPrefix]
        [HarmonyPatch(typeof(InventoryTrader), "HandleMoneyTransaction", typeof(EntityAgent))]
        public static bool Prefix_HandleMoneyTransaction(InventoryTrader __instance, EntityAgent eagent, ref bool __result)
        {
            int totalCost = __instance.GetTotalCost();
            int totalGain = __instance.GetTotalGain();
            int playerAssets = InventoryTrader.GetPlayerAssets(eagent); // static method, call via class
            int traderAssets = __instance.GetTraderAssets();            // instance method

            if (playerAssets - totalCost + totalGain < 0 || traderAssets + totalCost - totalGain < 0)
            {
                __result = false;
                return false; // skip original
            }

            int num = totalCost - totalGain;
            if (num > 0)
            {
                InventoryTrader.DeductFromEntity(__instance.Api, eagent, num);
                __instance.GiveToTrader(num);
            }
            else
            {
                var traderField = typeof(InventoryTrader).GetField("entityTrader",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                EntityTradingHumanoid trader = (EntityTradingHumanoid)traderField.GetValue(__instance);

                __instance.GiveOrDrop(
                    eagent,
                    new ItemStack(__instance.Api.World.GetItem(new AssetLocation(Config.Current.RETURN_ITEM)), 1),
                    -num,
                    trader
                );
            }

            __result = true;
            return false; // skip original
        }

        public static int CurrencyValuePerItem(ItemSlot slot)
        {
            if (slot?.Itemstack?.Collectible?.Attributes == null) return 0;
            var currency = slot.Itemstack.Collectible.Attributes["currency"];
            if (currency != null && currency.Type == EnumAttributeType.Object)
            {
                var val = currency["value"];
                return val?.AsInt(0) ?? 0;
            }
            return 0;
        }
    }
}
