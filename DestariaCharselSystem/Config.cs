using System.Collections.Generic;
using Vintagestory.API.Common;

namespace DestariaCharselSystem
{
    public class Config
    {
        public static Config Current;

        // Map from item code to its currency value
        public Dictionary<string, int> COINS_VALUES = new Dictionary<string, int>
        {
            { "destaria:gold-coin", 256 },
            { "destaria:silver-coin", 64 },
            { "destaria:copper-coin", 1 },
            { "game:gear-rusty", 0 }
        };

        // Item given when traders need to return currency (fallback)
        public string RETURN_ITEM = "destaria:gold-coin";
    }
}
