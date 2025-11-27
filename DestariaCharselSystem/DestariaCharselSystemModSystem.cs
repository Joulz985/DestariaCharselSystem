namespace DestariaCharselSystem.ModSystem
{ 
    using Vintagestory.API.Client;
    using Vintagestory.API.Common;
    using Vintagestory.API.Server;
    using DestariaCharselSystem.ModConfiguration;

    public class DestariaCharselModSystem : ModSystem
    {

        private const string ConfigFileName = "classchangebook.json";

        private ICoreServerAPI? sapi;

        private ICoreClientAPI capi;

        private ICoreAPI api;

        private IServerNetworkChannel serverChannel;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.Logger.Notification($"[DestariaCharselSystem] ItemClassChangeBook registered on {api.Side}");

            // Register for BOTH client and server sides
            api.RegisterItemClass("DestariaCharselSystem.ItemClassChangeBook", typeof(ItemClassChangeBook));

        }
        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            try
            {
                ModConfig fromDisk;
                if ((fromDisk = api.LoadModConfig<ModConfig>(ConfigFileName)) == null)
                { api.StoreModConfig(ModConfig.Loaded, ConfigFileName); }
                else
                { ModConfig.Loaded = fromDisk; }
            }
            catch
            { api.StoreModConfig(ModConfig.Loaded, ConfigFileName); }

            // Set a property that JSON patches can check
            api.World.Config.SetBool("EnableClassBookRecipe", ModConfig.Loaded.EnableClassBookRecipe);
            if (api.Side == EnumAppSide.Server)
            {
                this.Mod.Logger.Event($"EnableClassBookRecipe set to {ModConfig.Loaded.EnableClassBookRecipe} on server");
            }

        }
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            this.capi = api;

            capi.Network.RegisterChannel("classchangebook")
                .RegisterMessageType<SyncClientPacket>()
                .SetMessageHandler<SyncClientPacket>(packet =>
                {
                    ModConfig.Loaded.EnableClassBookRecipe = packet.EnableClassBookRecipe;
                    this.Mod.Logger.Event($"Received EnableClassBookRecipe of {packet.EnableClassBookRecipe} from server");
                });
        }
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            // send connecting players the config settings
            sapi.Event.PlayerJoin += this.OnPlayerJoin; // add method so we can remove it in dispose to prevent memory leaks
            // register network channel to send data to clients
            this.serverChannel = sapi.Network.RegisterChannel("classchangebook")
                .RegisterMessageType<SyncClientPacket>()
                .SetMessageHandler<SyncClientPacket>((player, packet) => { /* do nothing. idk why this handler is even needed, but it is */ });
        }

        private void OnPlayerJoin(IServerPlayer player)
        {
            // send the connecting player the settings it needs to be synced
            this.serverChannel.SendPacket(new SyncClientPacket
            {
                EnableClassBookRecipe = ModConfig.Loaded.EnableClassBookRecipe,
            }, player);
        }
        public override void Dispose()
        {
            // remove our player join listener so we dont create memory leaks
            if (this.api is ICoreServerAPI sapi)
            {
                sapi.Event.PlayerJoin -= this.OnPlayerJoin;
            }

        }

    }
}

