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

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.Logger.Notification($"[DestariaCharselSystem] ItemClassChangeBook registered on {api.Side}");

            // Register for BOTH client and server sides
            api.RegisterItemClass("DestariaCharselSystem.ItemClassChangeBook", typeof(ItemClassChangeBook));

        }

    }
}
