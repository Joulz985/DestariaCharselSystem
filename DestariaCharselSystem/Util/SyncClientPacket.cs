using ProtoBuf;

namespace DestariaCharselSystem.ModSystem
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SyncClientPacket
    {

        public bool EnableClassBookRecipe;

    }
}