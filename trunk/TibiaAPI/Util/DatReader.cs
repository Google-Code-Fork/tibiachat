using System;
using System.Collections.Generic;
using Tibia.Objects;

namespace Tibia.Util
{
    [Obsolete("Use the Item class instead. This class will be removed in future versions.")]
    public class DatReader
    {
        Client client;
        uint baseAddr;
        uint itemInfoAddr;

        public DatReader(Client c)
        {
            client = c;
            baseAddr = (uint)client.ReadInt(Addresses.Client.DatPointer);
            itemInfoAddr = (uint)client.ReadInt(baseAddr + 8);
        }

        public int ItemCount()
        {
            return client.ReadInt(baseAddr + 4);
        }

        public DatItem GetItem(Item item)
        {
            return GetItem(item.Id);
        }

        public DatItem GetItem(uint itemId)
        {
            return new DatItem(client, itemInfoAddr + 0x4C * (itemId - 100), itemId);
        }
    }
}
