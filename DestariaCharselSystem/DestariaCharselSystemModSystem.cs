using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace DestariaCharselSystem
{
    public class DestariaCharselModSystem : ModSystem
    {
        private ICoreServerAPI? sapi;

        private Dictionary<string, int> coinValues = new Dictionary<string, int>
        {
            { "destaria:gold-coin", 1 },
            { "destaria:silver-coin", 1},
            { "game:gear-rusty", 0 }
        };

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.Logger.Notification($"[DestariaCharselSystem] ItemClassChangeReceipt registered on {api.Side}");

            // Register for BOTH client and server sides
            api.RegisterItemClass("DestariaCharselSystem.ItemClassChangeReceipt", typeof(ItemClassChangeReceipt));

            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                sapi.Logger.Notification("[DestariaCharselSystem] Mod running server-side");

                // Run after all items are loaded
                sapi.Event.ServerRunPhase(EnumServerRunPhase.GameReady, () =>
                {
                    ApplyCurrencyValues();
                });
            }
        }
        private void ApplyCurrencyValues()
        {
            if (sapi == null) return;

            foreach (var item in sapi.World.Items)
            {
                if (item is CollectibleObject co && co.Attributes != null)
                {
                    string codeStr = co.Code.ToString();

                    if (coinValues.ContainsKey(codeStr))
                    {
                        // Directly set the "currency" token
                        co.Attributes.Token["currency"] = JToken.FromObject(new { value = coinValues[codeStr] });

                        co.Attributes = new Vintagestory.API.Datastructures.JsonObject(co.Attributes.Token);

                        sapi.Logger.Notification($"[DestariaCharselSystem] Set currency for {codeStr} to {coinValues[codeStr]}");
                    }
                }
            }
        }
    }
}
