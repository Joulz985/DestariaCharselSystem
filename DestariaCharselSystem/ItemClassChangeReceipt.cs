using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using DestariaCharselSystem;

namespace DestariaCharselSystem
{
    public class ItemClassChangeReceipt : Item
    {

        public override void OnHeldInteractStart(ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(itemslot, byEntity, blockSel, entitySel, firstEvent, ref handling);
            handling = EnumHandHandling.PreventDefault;
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            return secondsUsed < 2;
        }

        public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            return true;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot itemslot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (secondsUsed < 0.1) return;
            // Only run server-side
            //if (byEntity.World.Side != EnumAppSide.Server) return;

            if (byEntity.World.Side != EnumAppSide.Server) return;


            if (byEntity is EntityPlayer player)
            {
                
                IServerPlayer sPlayer = player.World.PlayerByUid(player.PlayerUID) as IServerPlayer;
                if (sPlayer == null) return;

                var sapi = byEntity.World.Api as ICoreServerAPI;
                sapi.InjectConsole($"/player {sPlayer.PlayerName} allowcharselonce");
                
                itemslot.TakeOut(1);
                itemslot.MarkDirty();
                sapi.SendMessage(sPlayer, GlobalConstants.GeneralChatGroup, "You may now change your class! Type .charsel in chat to get started", EnumChatType.Notification);
            }
        }

    }
}

