using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    public class BitTag
    {
        internal static int TotalTags = 0;
        internal static BitTag[] byID = new BitTag[32];
        private static Dictionary<string, BitTag> byName = new Dictionary<string, BitTag>(StringComparer.OrdinalIgnoreCase);

        public static BitTag Get(string name)
        {
#if DEBUG
            if (!byName.ContainsKey(name))
                throw new Exception("No tag with the name '" + name + "' has been defined!");
#endif
            return byName[name];
        }

        public int ID;
        public int Value;
        public string Name;

        public BitTag(string name)
        {
#if DEBUG
            if (TotalTags >= 32)
                throw new Exception("Maximum tag limit of 32 exceeded!");
            if (byName.ContainsKey(name))
                throw new Exception("Two tags defined with the same name: '" + name + "'!");
#endif

            ID = TotalTags;
            Value = 1 << TotalTags;
            Name = name;

            byID[ID] = this;
            byName[name] = this;

            TotalTags++;
        }

        public static implicit operator int(BitTag tag)
        {
            return tag.Value;
        }
    }
}
