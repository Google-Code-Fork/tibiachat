using System;
using System.Collections.Generic;

namespace Tibia.Objects
{
    /// <summary>
    /// Battle list object.
    /// </summary>
    public class BattleList
    {
        private Client client;

        /// <summary>
        /// Create a battlelist object.
        /// </summary>
        /// <param name="c"></param>
        public BattleList(Client c)
        {
            client = c;
        }

        /// <summary>
        /// Get a list of all the creatures on the battlelist that match with match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<Creature> GetCreatures(Predicate<Creature> match)
        {
            List<Creature> creatures = new List<Creature>();
            for (uint i = Addresses.BattleList.Start; i < Addresses.BattleList.End; i += Addresses.BattleList.Step_Creatures)
            {
                Creature creature = new Creature(client, i);
                if (match(creature))
                    creatures.Add(creature);
            }
            return creatures;

        }

        /// <summary>
        /// Get a list of all the creatures on the battlelist.
        /// </summary>
        /// <returns></returns>
        public List<Creature> GetCreatures()
        {
            List<Creature> creatures = new List<Creature>();
            for (uint i = Addresses.BattleList.Start; i < Addresses.BattleList.End; i += Addresses.BattleList.Step_Creatures)
            {
                if (client.ReadByte(i + Addresses.Creature.Distance_IsVisible) == 1)
                    creatures.Add(new Creature(client, i));
            }
            return creatures;
        }

        /// <summary>
        /// Get a list of all the cratures with the specified string in the name.
        /// </summary>
        /// <param name="creatureName"></param>
        /// <returns></returns>
        public List<Creature> GetCreatures(string creatureName)
        {
            return GetCreatures(creatureName, false);
        }

        public List<Creature> GetCreaturesOnLoc(Location loc)
        {
            return GetCreatures().FindAll(delegate(Creature c)
            {
                return c.Location.Equals(loc);
            });
        }

        /// <summary>
        /// Get a list of all the creatures with the specified name.
        /// </summary>
        /// <param name="creatureName"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        public List<Creature> GetCreatures(string creatureName, bool wholeWord)
        {
            return GetCreatures().FindAll(delegate(Creature c)
            {
                if (wholeWord)
                    return c.Name.Equals(creatureName, StringComparison.CurrentCultureIgnoreCase);
                else
                    return c.Name.IndexOf(creatureName, StringComparison.CurrentCultureIgnoreCase) != -1;
            });
        }
        /// <summary>
        /// Get all the members of a party
        /// </summary>
        /// <returns></returns>
        public List<Creature> GetPartyMembers()
        {
            return GetCreatures().FindAll(delegate(Creature c)
            {
                return c.InParty();
            });
        }
        
        /// <summary>
        /// Get list of Partymembers with a HPBar below xx percent
        /// </summary>
        /// <param name="hpPercentage"></param>
        /// <returns></returns>
        public List<Creature> GetPartyMembers(uint hpPercentage)
        {
            return GetCreatures().FindAll(delegate(Creature c)
            {
                return c.InParty() && c.HPBar <= hpPercentage;
            });
        }

        /// <summary>
        /// Get list of creatures attacking the player.
        /// </summary>
        /// <returns></returns>
        public List<Creature> GetCreaturesAttacking()
        {
            return GetCreatures().FindAll(delegate(Creature c)
            {
                return c.IsAttacking();
            });
        }

        /// <summary>
        /// Get list of creatures around location.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="radius"></param>
        /// <param name="floor"></param>
        /// <param name="players"></param>
        /// <returns></returns>
        public List<Creature> GetCreaturesAroundLocation(Location location, int maxDistance)
        {
            return GetCreaturesAroundLocation(location, maxDistance, true, true);
        }

        /// <summary>
        /// Get list of creatures around location.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="radius"></param>
        /// <param name="floor"></param>
        /// <param name="players"></param>
        /// <returns></returns>
        public List<Creature> GetCreaturesAroundLocation(Location location, int maxDistance, bool sameFloor)
        {
            return GetCreaturesAroundLocation(location, maxDistance, sameFloor, true);
        }

        /// <summary>
        /// Get list of creatures around location.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="radius"></param>
        /// <param name="floor"></param>
        /// <param name="players"></param>
        /// <returns></returns>
        public List<Creature> GetCreaturesAroundLocation(Location location, int maxDistance, bool sameFloor, bool onlyMonsters)
        {
            return GetCreatures().FindAll(delegate(Creature c)
            {
                return c.DistanceTo(location) <= maxDistance && (c.Z == location.Z || !sameFloor) && (c.Type == Tibia.Constants.CreatureType.NPC || !onlyMonsters) && (!c.IsSelf());
            });
        }

        /// <summary>
        /// Get a list of creatures with the specified name, ignoring spaces.
        /// </summary>
        /// <param name="creatureName"></param>
        /// <param name="wholeWord">If true, matches the entire name; false, searches inside.</param>
        /// <returns></returns>
        public List<Creature> GetCreaturesIgnoreSpace(string creatureName, bool wholeWord)
        {
            creatureName = creatureName.Replace(" ", "");
            return GetCreatures().FindAll(delegate(Creature c)
            {
                if (wholeWord)
                {
                    string name = c.Name;
                    name = name.Replace(" ", "");
                    return name.Equals(creatureName, StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    string name = c.Name;
                    name = name.Replace(" ", "");
                    return name.IndexOf(creatureName, StringComparison.CurrentCultureIgnoreCase) != -1;
                }   
            });
        }
        
        /// <summary>
        /// Get a list of creatures with the specified string in the name, ignoring spaces.
        /// </summary>
        /// <param name="creatureName"></param>
        /// <returns></returns>
        public List<Creature> GetCreaturesIgnoreSpace(string creatureName)
        {
            return GetCreaturesIgnoreSpace(creatureName, false);
        }

        /// <summary>
        /// Get the creature with the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Creature GetCreature(int id)
        {
            return GetCreatures().Find(delegate(Creature c)
            {
                return c.Id == id;
            });
        }

        /// <summary>
        /// Get the first creature with the specified string in the name.
        /// </summary>
        /// <param name="creatureName"></param>
        /// <returns></returns>
        public Creature GetCreature(string creatureName)
        {
            return GetCreature(creatureName, false);
        }

        /// <summary>
        /// Get the first creature with the specified name.
        /// </summary>
        /// <param name="creatureName"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        public Creature GetCreature(string creatureName, bool wholeWord)
        {
            return GetCreatures().Find(delegate(Creature c)
            {
                if (wholeWord)
                    return c.Name.Equals(creatureName, StringComparison.CurrentCultureIgnoreCase);
                else
                    return c.Name.IndexOf(creatureName, StringComparison.CurrentCultureIgnoreCase) != -1;
            });
        }

        public void ShowInvisible(bool on)
        {
            if (on)
            {
                client.WriteByte(Addresses.Map.RevealInvisible1,
                    Addresses.Map.RevealInvisible1Edited);
                client.WriteByte(Addresses.Map.RevealInvisible2,
                    Addresses.Map.RevealInvisible2Edited);
            }
            else
            {
                client.WriteByte(Addresses.Map.RevealInvisible1,
                    Addresses.Map.RevealInvisible1Default);
                client.WriteByte(Addresses.Map.RevealInvisible2,
                    Addresses.Map.RevealInvisible2Default);
            }
        }

        public void ShowInvisible()
        {
            ShowInvisible(true);
        }

        /// <summary>
        /// Show invisible creatures by replacing their invisible outfit with a specified type.
        /// </summary>
        /// <param name="outfitType"></param>
        /// <returns></returns>
        public int ShowInvisible(Constants.OutfitType outfitType)
        {
            int replacedCount = 0;

            List<Creature> creatures = GetCreatures(delegate(Creature c)
            {
                return c.Outfit == Constants.OutfitType.Invisible;
            });

            foreach(Creature c in creatures)
            {
                c.Outfit = outfitType;
                replacedCount++;
            }

            return replacedCount;
        }
    }
    
}
