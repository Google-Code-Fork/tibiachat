using System;
using System.Text;

namespace Tibia.Objects
{
    /// <summary>
    /// Represents a tile in the map structure
    /// </summary>
    public struct Tile
    {
        public Location Location;
        public uint Number;
        public uint Address;
        public uint Id;

        public Tile(uint n) : this(n, 0) { }
        public Tile(uint n, uint a) : this(n, a, new Location(), 0) { }
        public Tile(uint n, uint a, Location l, uint i)
        {
            Location = l;
            Number = n;
            Address = a;
            Id = i;
        }

        /// <summary>
        /// Returns the string representation of this struct.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(16);
            builder.Append("{ Number=" + Number.ToString() + ", ");
            builder.Append("Address=" + Address.ToString() + ", ");
            builder.Append("Id=" + Id.ToString() + " }");

            return builder.ToString();
        }
    }

    /// <summary>
    /// Represents a Login Server
    /// </summary>
    public class LoginServer
    {
        public string Server = string.Empty;
        public short Port = 7171;

        public LoginServer() { }

        public LoginServer(string server)
        {
            Server = server;
        }

        public LoginServer(string server, short port)
        {
            Server = server;
            Port = port;
        }

        public override string ToString()
        {
            return Server + ":" + Port;
        }
    }

    public struct Channel
    {
        public Packets.ChatChannel Id;
        public string Name;

        public Channel(Packets.ChatChannel id, string name)
        {
            Id = id;
            Name = name;
        }
    }
    public struct Rect
    {
        private int top;
        private int bottom;
        private int left;
        private int right;
        private int width;
        private int height;
        public int Top
        {
            get { return top; }
        }
        public int Bottom
        {
            get { return bottom; }
        }
        public int Left
        {
            get { return left; }
        }
        public int Rigth
        {
            get { return right; }
        }
        public int Height
        {
            get { return height; }
        }
        public int Width
        {
            get { return width; }
        }
        public Rect(Util.WinApi.RECT r)
        {
            top = r.top;
            bottom = r.bottom;
            left = r.left;
            right = r.right;
            width = r.right - r.left;
            height = r.bottom - r.top;
        }
    }

    public struct ClientPathInfo
    {
        public string Path;
        public string Version;

        public ClientPathInfo(string path, string version)
        {
            Path = path;
            Version = version;
        }


        public override string ToString()
        {
            return Path + " [" + Version + "]";
        }
    }
}
