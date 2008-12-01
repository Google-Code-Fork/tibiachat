using System;
using System.Collections.Generic;
using Tibia.Packets;

namespace Tibia.Objects
{
    /// <summary>
    /// Represents one stack of items. Can also represent a type of item (with no location in memory).
    /// </summary>
    public class Item
    {
        protected Client client;
        protected uint id;
        protected string name;
        protected byte count;
        protected ItemLocation loc;
        protected bool found;

        #region Constructors
        public Item() : this(0) { }
        public Item(Client client, bool found) : this(0, "", 0, null, client, found) { }
        public Item(uint id) : this(id, "") { }
        public Item(uint id, string name) : this(id, name, 0, null, null, false) { }
        public Item(ItemLocation loc) : this(0, "", 0, loc, null, false) { }
        public Item(uint id, byte count, ItemLocation loc, Client client, bool found) : this(id, "", count, loc, client, found) { }

        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="id">item id</param>
        /// <param name="name">item name (only used when representing an item type)</param>
        /// <param name="count">number of items in the stack (also charges on a rune)</param>
        /// <param name="loc">location in game</param>
        /// <param name="client">client (used for sending packets)</param>
        /// <param name="found">used when searching</param>
        public Item(uint id, string name, byte count, ItemLocation loc, Client client, bool found)
        {
            Id = id;
            Name = name;
            Count = count;
            Loc = loc;
            Found = found;
            this.client = client;
        }
        #endregion

        #region Packet Functions

        /// <summary>
        /// Opens a container in the same window. Only works if the item is a container.
        /// </summary>
        /// <param name="container">Which container window to open in.</param>
        /// <returns></returns>
        public bool OpenContainer(byte container)
        {
            return client.Send(Packets.ItemUsePacket.Create(
                client, loc, id, loc.stackOrder, container));
        }

        /// <summary>
        /// Use the item (eg. eat food).
        /// </summary>
        /// <returns></returns>
        public bool Use()
        {
            return client.Send(Packets.ItemUsePacket.Create(
                client, loc, id, loc.stackOrder, 0x0F));
        }

        /// <summary>
        /// Use the item on a tile (eg. fish, rope, pick, shovel, etc).
        /// </summary>
        /// <param name="onTile"></param>
        /// <returns></returns>
        public bool Use(Objects.Tile onTile)
        {
            return client.Send(Packets.ItemUseOnPacket.Create(
                client, loc, id, 0, onTile.Location, onTile.Id, 0));
        }



        /// <summary>
        /// Use the item on a tile location.
        /// Gets the tile id automatically.
        /// </summary>
        /// <param name="onLocation"></param>
        /// <returns></returns>
        public bool Use(Location onLocation)
        {
            MapSquare square = client.Map.CreateMapSquare(onLocation);
            return Use(square.Tile);
        }

        /// <summary>
        /// Use an item on another item.
        /// Not tested.
        /// </summary>
        /// <param name="onItem"></param>
        /// <returns></returns>
        public bool Use(Objects.Item onItem)
        {
            return client.Send(Packets.ItemUseOnPacket.Create(
                client, loc, id, 0, onItem.Loc, onItem.Id, 0));
        }

        /// <summary>
        /// Use an item on a creature (eg. use a rune on someone, drink a manafluid).
        /// If it is a player shoot on xyz coors, if it is a creature shoot through
        /// the battlelist (more accurate).
        /// </summary>
        /// <param name="onCreature"></param>
        /// <returns></returns>
        public bool Use(Objects.Creature onCreature)
        {
            return client.Send(Packets.ItemUseOnPacket.Create(
                client, loc, id, loc.ToBytes()[4], onCreature.Location, 0x63, 0));
        }

        /// <summary>
        /// Use the item on yourself
        /// </summary>
        /// <returns></returns>
        public bool UseOnSelf()
        {
            int playerID = client.ReadInt(Addresses.Player.Id);
            byte stack = 0;
            if (id == Constants.Items.Bottle.Vial.Id) stack = count;
            return client.Send(Packets.ItemUseBattlelistPacket.Create(
                client, ItemLocation.Hotkey(), id, stack, playerID));
        }

        /// <summary>
        /// Move an item to a location (eg. move a blank rune to the right hand).
        /// </summary>
        /// <param name="toLocation"></param>
        /// <returns></returns>
        public bool Move(Objects.ItemLocation toLocation)
        {
            if (client == null) return false;
            
            return Move(new Objects.Item(toLocation));
        }

        /// <summary>
        /// Move an item into another item (eg. put an item into a backpack).
        /// </summary>
        /// <param name="toItem"></param>
        /// <returns></returns>
        public bool Move(Objects.Item toItem)
        {
            return client.Send(Packets.ItemMovePacket.Create(
                client, loc, id, loc.ToBytes()[4],toItem.Loc, count));
        }

        #region Get/Set Properties
        /// <summary>
        /// Gets the client associated with this item;
        /// </summary>
        public Client Client
        {
            get { return client; }
            set { client = value; }
        }
        /// <summary>
        /// Gets or sets the id of the item.
        /// </summary>
        public uint Id
        {
            get { return id; }
            set { id = value; }
        }
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// Gets or sets the amount stacked of this item.
        /// </summary>
        public byte Count
        {
            get { return count; }
            set { count = value; }
        }

        /// <summary>
        /// Gets the total number of items/objects in Tibia.
        /// </summary>
        public uint TotalItemCount
        {
            get
            {
                uint baseAddr = (uint)client.ReadInt(Addresses.Client.DatPointer);
                return (uint)client.ReadInt(baseAddr + 4);
            }
        }

        /// <summary>
        /// Gets or sets the location of this item.
        /// </summary>
        public ItemLocation Loc
        {
            get { return loc; }
            set { loc = value; }
        }
        /// <summary>
        /// Gets or sets whether this item is found.
        /// </summary>
        public bool Found
        {
            get { return found; }
            set { found = value; }
        }
        #endregion

        /// <summary>
        /// Get the packet bytes for an item location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static byte[] ItemLocationToBytes(Objects.ItemLocation location)
        {            
            byte[] bytes = new byte[5];

            switch (location.type)
            {
                case Constants.ItemLocationType.Container:
                    bytes[00] = 0xFF;
                    bytes[01] = 0xFF;
                    bytes[02] = Convert.ToByte(0x40 + location.container);
                    bytes[03] = 0x00;
                    bytes[04] = location.position;
                    break;
                case Constants.ItemLocationType.Slot:
                    bytes[00] = 0xFF;
                    bytes[01] = 0xFF;
                    bytes[02] = Convert.ToByte(location.slot);
                    bytes[03] = 0x00;
                    bytes[04] = 0x00;
                    break;
                case Constants.ItemLocationType.Ground:
                    bytes[00] = Packet.Lo(location.groundLocation.X);
                    bytes[01] = Packet.Hi(location.groundLocation.X);
                    bytes[02] = Packet.Lo(location.groundLocation.Y);
                    bytes[03] = Packet.Hi(location.groundLocation.Y);
                    bytes[04] = Convert.ToByte(location.groundLocation.Z);
                    break;
            }

            return bytes;
        }

        /// <summary>
        /// Gets the DatAddress of the item in the dat structure.
        /// </summary>
        public uint DatAddress
        {
            get
            {
                uint baseAddr = (uint)client.ReadInt(Addresses.Client.DatPointer);
                return (uint)client.ReadInt(baseAddr + 8) + 0x4C * (id - 100);
            }
        }

        /// <summary>
        /// Gets or sets the visible width of the item.
        /// </summary>
        public int Width
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Width); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.Width, value); }
        }

        /// <summary>
        /// Gets or sets the visible height of the item.
        /// </summary>
        public int Height
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Height); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.Height, value); }
        }

        public int Unknown1
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Unknown1); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.Unknown1, value); }
        }

        public int Layers
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Layers); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.Layers, value); }
        }

        public int PatternX
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.PatternX); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.PatternX, value); }
        }

        public int PatternY
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.PatternY); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.PatternY, value); }
        }

        public int PatternDepth
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.PatternDepth); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.PatternDepth, value); }
        }

        public int Phase
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Phase); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.Phase, value); }
        }

        /// <summary>
        /// Gets the DatAddress of the sprite structure of the item.
        /// </summary>
        public int Sprites
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Sprites); }
            //set { client.WriteInt(DatAddress + Addresses.DatItem.Sprites, value); }
        }

        /// <summary>
        /// Gets or sets the flags of the item.
        /// </summary>
        public int Flags
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Flags); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.Flags, value); }
        }

        /// <summary>
        /// Gets or sets the walking speed of the item. (Walkable tiles)
        /// </summary>
        public int WalkSpeed
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.WalkSpeed); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.WalkSpeed, value); }
        }

        /// <summary>
        /// Gets or sets the text limit of the item. (Writable items)
        /// </summary>
        public int TextLimit
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.TextLimit); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.TextLimit, value); }
        }

        /// <summary>
        /// Gets or sets the light radius of the item. (Light items)
        /// </summary>
        public int LightRadius
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.LightRadius); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.LightRadius, value); }
        }

        /// <summary>
        /// Gets or sets the light color of the item. (Light items)
        /// </summary>
        public int LightColor
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.LightColor); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.LightColor, value); }
        }

        /// <summary>
        /// Gets or sets how many pixels the item is shifted horizontally from the lower
        /// right corner of the tile.
        /// </summary>
        public int ShiftX
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.ShiftX); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.ShiftX, value); }
        }

        /// <summary>
        /// Gets or sets how many pixels the item is shifted vertically from the lower
        /// right corner of the tile.
        /// </summary>
        public int ShiftY
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.ShiftY); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.ShiftY, value); }
        }

        /// <summary>
        /// Gets or sets the height created by the item when a character walks over.
        /// </summary>
        public int WalkHeight
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.WalkHeight); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.WalkHeight, value); }
        }

        /// <summary>
        /// Gets or sets the color shown by the item in the automap.
        /// </summary>
        public int AutomapColor
        {
            get { return client.ReadInt(DatAddress + Addresses.DatItem.Automap); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.Automap, value); }
        }

        /// <summary>
        /// Gets or sets the help id for the item in Help mode.
        /// </summary>
        public Addresses.DatItem.Help LensHelp
        {
            get { return (Addresses.DatItem.Help)client.ReadInt(DatAddress + Addresses.DatItem.LensHelp); }
            set { client.WriteInt(DatAddress + Addresses.DatItem.LensHelp, (int)value); }
        }
        #endregion

        #region Composite Properties

        /// <summary>
        /// Returns whether the item has an extra byte (count, fluid type, charges, etc).
        /// </summary>
        public bool HasExtraByte
        {
            get
            {
                return GetFlag(Tibia.Addresses.DatItem.Flag.IsStackable) ||
                       GetFlag(Tibia.Addresses.DatItem.Flag.IsRune) ||
                       GetFlag(Tibia.Addresses.DatItem.Flag.IsSplash) ||
                       GetFlag(Tibia.Addresses.DatItem.Flag.IsFluidContainer);
            }
        }

        #endregion

        /// <summary>
        /// Check whether or not this item is in a list (checks by ID only)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>True if the item is in the list, false if not</returns>
        public bool IsInList<T>(IEnumerable<T> list) where T : Item
        {
            if (Id != 0)
            {
                foreach (T i in list)
                {
                    if (Id == i.Id) return true;
                }
                return false;
            }
            else
                return false;
        }

        public override string ToString()
        {
            return Name;
        }

        #region Flags
        public bool GetFlag(Addresses.DatItem.Flag flag)
        {
            return (Flags & (int)flag) == (int)flag;
        }

        public void SetFlag(Addresses.DatItem.Flag flag, bool on)
        {
            if (on)
                Flags |= (int)flag;
            else
                Flags &= ~(int)flag;
        }
        #endregion
    }

    #region Special Item Types

    /// <summary>
    /// Represents a food item. Same as regular item but also stores regeneration time.
    /// </summary>
    public class Food : Item
    {
        public uint RegenerationTime;

        public Food(uint id, string name, uint regenerationTime) : base(id, name)
        {
            RegenerationTime = regenerationTime;
        }
    }

    /// <summary>
    /// Represents a rune item. Contains metadata relating specifically to runes.
    /// </summary>
    public class TransformingItem : Item
    {
        public Spell Spell;
        public uint SoulPoints;
        public Item OriginalItem;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="id">item id</param>
        /// <param name="name">item name</param>
        /// <param name="spell">spell used to create the item</param>
        /// <param name="soulPoints">amount of soul points needed to make the item</param>
        /// <param name="originalItem">the item that the spell words are used on to create this item</param>
        public TransformingItem(uint id, string name, Spell spell, uint soulPoints, Item originalItem)
            : base(id, name)
        {
            Spell = spell;
            SoulPoints = soulPoints;
            OriginalItem = originalItem;
        }
    }

    /// <summary>
    /// Represents a rune item. Contains metadata relating specifically to runes.
    /// </summary>
    public class Rune : TransformingItem
    {
        /// <summary>
        /// Default rune constructor.
        /// </summary>
        /// <param name="id">item id</param>
        /// <param name="name">item name</param>
        /// <param name="spell">spell used to create the rune</param>
        /// <param name="soulPoints">amount of soul points needed to make the rune</param>
        public Rune(uint id, string name, Spell spell, uint soulPoints)
            : base(id, name, spell, soulPoints, Constants.Items.Rune.Blank)
        {
        }
    }

    #endregion

    /// <summary>
    /// Represents an item's location in game/memory. Can be either a slot, an inventory location, or on the ground.
    /// </summary>
    public class ItemLocation
    {
        public Constants.ItemLocationType type;
        public byte container;
        public byte position;
        public Location groundLocation;
        public byte stackOrder;
        public Constants.SlotNumber slot;

        /// <summary>
        /// Create a new item location at the specified slot (head, backpack, right, left, etc).
        /// </summary>
        /// <param name="s">slot</param>
        public ItemLocation(Constants.SlotNumber s)
        {
            type = Constants.ItemLocationType.Slot;
            slot = s;
        }

        /// <summary>
        /// Create a new item loction at the specified inventory location.
        /// </summary>
        /// <param name="c">container</param>
        /// <param name="p">position in container</param>
        public ItemLocation(byte c, byte p)
        {
            type = Constants.ItemLocationType.Container;
            container = c;
            position = p;
            stackOrder = p;
        }

        /// <summary>
        /// Create a new item location from a general location and stack order.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="stack"></param>
        public ItemLocation(Location l, byte stack)
        {
            type = Constants.ItemLocationType.Ground;
            groundLocation = l;
            stackOrder = stack;
        }

        /// <summary>
        /// Create a new item location at the specified location.
        /// </summary>
        /// <param name="l"></param>
        public ItemLocation(Location l)
        {
            type = Constants.ItemLocationType.Ground;
            groundLocation = l;
            stackOrder = 1;
        }

        /// <summary>
        /// Return a blank item location for packets (FF FF 00 00 00)
        /// </summary>
        /// <returns></returns>
        public static ItemLocation Hotkey()
        {
            return new ItemLocation(Constants.SlotNumber.None);
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[5];

            switch (type)
            {
                case Constants.ItemLocationType.Container:
                    bytes[00] = 0xFF;
                    bytes[01] = 0xFF;
                    bytes[02] = Convert.ToByte(0x40 + container);
                    bytes[03] = 0x00;
                    bytes[04] = position;
                    break;
                case Constants.ItemLocationType.Slot:
                    bytes[00] = 0xFF;
                    bytes[01] = 0xFF;
                    bytes[02] = Convert.ToByte(slot);
                    bytes[03] = 0x00;
                    bytes[04] = 0x00;
                    break;
                case Constants.ItemLocationType.Ground:
                    bytes[00] = Packet.Lo(groundLocation.X);
                    bytes[01] = Packet.Hi(groundLocation.X);
                    bytes[02] = Packet.Lo(groundLocation.Y);
                    bytes[03] = Packet.Hi(groundLocation.Y);
                    bytes[04] = Convert.ToByte(groundLocation.Z);
                    break;
            }

            return bytes;
        }
    }
}
