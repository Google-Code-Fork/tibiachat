namespace Tibia.Addresses
{
    /// <summary>
    /// Client addresses not specific to a player.
    /// </summary>
    public static class Client
    {
        /// <summary>
        /// The system time in ms when the client was started.
        /// Used for Creatures.Distance_BlackSquare calculations.
        /// </summary>
        public static uint StartTime = 0x785310; //8.31

        /// <summary>
        /// Address to the XTea encryption key.
        /// </summary>
        public static uint XTeaKey = 0x77FE4C; //8.31

        /// <summary>
        /// FPS (Frames Per Second) Pointer
        /// </summary>
        public static uint FrameRatePointer = 0x783FF4; //8.31

        /// <summary>
        /// Current fps offset
        /// </summary>
        public static int FrameRateCurrentOffset = 0x0;

        /// <summary>
        /// FPS limit offset
        /// </summary>
        public static int FrameRateLimitOffset = 0x58;

        /// <summary>
        /// Address to activate multiclient.
        /// </summary>
        public static uint MultiClient = 0x1054F4; //8.31


        /// <summary>
        /// Value to be written to the multiclient address.
        /// </summary>
        public static uint MultiClientValue = 0xEB;


        /// <summary>
        /// Address to activate multiclient.
        /// </summary>
        public static uint DMultiClient = 0x505894; //8.31


        /// <summary>
        /// Value to be written to the multiclient address(JMP).
        /// </summary>
        public static byte DMultiClientJMP = 0xEB;

        ///<summary>
        /// Original value of the multiclient address(JNZ).
        /// </summary>
        public static byte DMultiClientJNZ = 0x75; 

        /// <summary>
        /// 8 = Connected | 0 = Disconnected
        /// </summary>
        public static uint Status = 0x7834B0; //8.31

        /// <summary>
        /// Safe mode (don't attack other players)
        /// </summary>
        public static uint SafeMode = 0x780274; //8.31
        /// <summary>
        /// Follow mode while attacking (Follow, keep distance, stand still)
        /// </summary>
        public static uint FollowMode = SafeMode + 4;

        /// <summary>
        /// Attack type (Full attack, half and half, full defense)
        /// </summary>
        public static uint AttackMode = FollowMode + 4;

        /// <summary>
        /// Action state (formerly MouseCursor icon)
        /// </summary>
        public static uint ActionState = 0x783510; //8.31

        /// <summary>
        /// The text of the last message sent to the default channel.
        /// </summary>
        public static uint LastMSGText = 0x785578; //8.31

        /// <summary>
        /// The last player to send a message to the default channel.
        /// </summary>
        public static uint LastMSGAuthor = LastMSGText - 0x28; //8.22

        /// <summary>
        /// The statusbar text to be displayed.
        /// </summary>
        public static uint Statusbar_Text = 0x785328; //8.31
        /// <summary>
        /// The time that the text will be displayed for in the statusbar.
        /// </summary>
        public static uint Statusbar_Time = Statusbar_Text - 4;

        /// <summary>
        /// The id of the last clicked item.
        /// </summary>
        public static uint Click_Id = 0x78354C; // 8.31
        /// <summary>
        /// The amount of the last clicked item (eg. 52 fish)
        /// </summary>
        public static uint Click_Count = Click_Id + 4;
        /// <summary>
        /// The floor that was clicked.
        /// </summary>
        public static uint Click_Z = Click_Id - 0x68;

        /// <summary>
        /// The id of the last item seen (looked at).
        /// </summary>
        public static uint See_Id = Click_Id + 12;
        /// <summary>
        /// The amount of the last item seen (eg. 42 fish).
        /// </summary>
        public static uint See_Count = See_Id + 4;
        /// <summary>
        /// The floor that the last seen item is on.
        /// </summary>
        public static uint See_Z = See_Id - 0x68;
        /// <summary>
        /// The text that came with the last seen item (eg. You see a fish).
        /// </summary>
        public static uint See_Text = 0x77BC80;
        
        // Login Server addresses
        public static uint LoginServerStart = 0x77AD88; //8.31
        public static uint Step_LoginServer = 112;
        public static uint Distance_Port = 100;
        public static uint Max_LoginServers = 10;

        /// <summary>
        /// RSA Key Adress
        /// </summary>
        public static uint RSA = 0x5AA610; //8.31

		  
        /// <summary>
        /// Login character list. This points to the character list.
        /// </summary>
        public static uint LoginCharList = 0x783464; // 8.31

        /* Character List Format
        
        30 bytes - Character name, null terminated string
        30 bytes - Server name, null terminated string
        4 bytes - Binary IP, the IP address in hex
        16 bytes - IP string, null terminated string of the IP address
        2 bytes - Port number
        2 bytes - Padding
        
        */

        /// <summary>
        /// Login character list selected character. This address doesn't move.
        /// </summary>
        public static uint LoginSelectedChar = 0x783460; // 8.31

        //This format is for the character list that is stored at 0x76450D.
        //This format is also how it comes in the packet.
        //This list gets overwritten with different data when connected to the game world.
        
        /* Character List Format
        
        2 bytes - Length of name
        n bytes - Name
        
        2 bytes - Length of sever name
        n bytes - Server name
        
        4 bytes - IP address
        2 bytes - Port number
        
        */


        /// <summary>
        /// Pointer to an address. When that address has 0x4E added to
        /// it, it points to the game window rect 
        /// struct.
        /// </summary>
        public static uint GameWindowRectPointer = 0x6327E8; //8.31
        public static uint GameWindowBar = 0x635B58; //8.31
        /*
            Several notes are needed on this one.
            1) This address is in the stack so it is very volitile. However it appears
               that it does not change as long as the player stays logged in.
            2) This address is always in the same place as long as the player
               is logged in.
            3) This address points to another address. Once you obtain that address
               you must add 0x4E and that will point to the begining of the struct.
         
            Struct Layout (each 4 bytes):
            X, Y, Width, Height
        */

        public static uint DatPointer = 0x77FE6C; //8.31
        
        public static uint DialogBegin = 0x635B54; //8.31
        public static uint DialogLeft = 0x14;
        public static uint DialogTop = 0x18;
        public static uint DialogWidth = 0x1C;
        public static uint DialogHeight = 0x20;
        public static uint DialogCaption = 0x50;


        /// <summary>
        /// Auto login stuff
        /// </summary>
        public static uint LoginPassword = 0x78348C; // 8.31
        public static uint LoginAccountStr = LoginPassword + 32;
        public static uint LoginAccountNum = LoginAccountStr + 12;

        public static uint LoginPatch = 0x47935E; // 8.11
        public static uint LoginPatch2 = 0x47A2B3; // 8.11

        public static byte Nop = 0x90;
        public static byte[] LoginPatchOrig = new byte[] { 0xE8, 0x0D, 0x1D, 0x09, 0x00 };
        public static byte[] LoginPatchOrig2 = new byte[] { 0xE8, 0xC8, 0x15, 0x09, 0x00 };
    }
}
