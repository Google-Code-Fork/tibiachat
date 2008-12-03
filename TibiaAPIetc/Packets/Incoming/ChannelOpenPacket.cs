﻿using System;
using Tibia.Objects;

namespace Tibia.Packets
{
    public class ChannelOpenPacket : Packet
    {
        private int channelId;
        private string channelName;

        public int ChannelId
        {
            get { return channelId; }
        }
        public string ChannelName
        {
            get { return channelName; }
        }

        public ChannelOpenPacket() : base()
        {
            type = PacketType.ChannelOpen;
            destination = PacketDestination.Client;
        }

        public ChannelOpenPacket( byte[] data) : this()
        {
            ParseData(data);
        }

        public new bool ParseData(byte[] packet)
        {
            if (base.ParseData(packet))
            {
                if (type != PacketType.ChannelOpen) return false;
                PacketBuilder p = new PacketBuilder( packet, 3);
                channelId = p.GetInt();
                channelName = p.GetString();
                index = p.Index;
                return true;
            }
            else 
            {
                return false;
            }
        }

        public static ChannelOpenPacket Create( Tibia.Objects.Channel channel)
        {
            return Create(channel.Id, channel.Name);
        }

        public static ChannelOpenPacket Create( ChatChannel channelId, string channelName)
        {
            PacketBuilder p = new PacketBuilder(PacketType.ChannelOpen);
            p.AddInt((int)channelId);
            p.AddString(channelName);
            return new ChannelOpenPacket(p.GetPacket());
        }
    }
}
