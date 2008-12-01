using System.Collections.Generic;

namespace Tibia.Addresses
{
    /// <summary>
    /// Battle list adddresses and distances.
    /// </summary>
    public static class BattleList
    {
        /// <summary>
        /// Distance between creatures.
        /// </summary>
        public static uint Step_Creatures = 0xA0;

        /// <summary>
        /// Maximum number of creatures.
        /// </summary>
        public static uint Max_Creatures = 150;

        /// <summary>
        /// Start of the battle list.
        /// </summary>
        public static uint Start = 0x062AD50; //8.31 Player.Exp + 108;

        /// <summary>
        /// End of the battle list.
        /// </summary>
        public static uint End = Start + (Step_Creatures * Max_Creatures); //8.31
    }
}
