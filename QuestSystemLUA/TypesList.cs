using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestSystemLUA
{
    public class TypesList
    {
        public static Dictionary<string, byte> tileTypeNames = new Dictionary<string, byte>();
        public static Dictionary<string, byte> wallTypeNames = new Dictionary<string, byte>();

        public static void SetupTyps()
        {
            tileTypeNames.Add("dirt", 0);
            tileTypeNames.Add("stone", 1);
            tileTypeNames.Add("grass", 2);
            tileTypeNames.Add("iron ore", 6);
            tileTypeNames.Add("copper ore", 7);
            tileTypeNames.Add("gold ore", 8);
            tileTypeNames.Add("silver ore", 9);
            tileTypeNames.Add("wooden platform", 19);
            tileTypeNames.Add("demonite ore", 22);
            tileTypeNames.Add("corrupted grass", 23);
            tileTypeNames.Add("ebonstone", 25);
            tileTypeNames.Add("wood", 30);
            tileTypeNames.Add("corruption thorn", 32);
            tileTypeNames.Add("meteorite", 37);
            tileTypeNames.Add("gray brick", 38);
            tileTypeNames.Add("clay brick", 39);
            tileTypeNames.Add("clay", 40);
            tileTypeNames.Add("blue brick", 41);
            tileTypeNames.Add("green brick", 43);
            tileTypeNames.Add("pink brick", 44);
            tileTypeNames.Add("gold brick", 45);
            tileTypeNames.Add("silver brick", 46);
            tileTypeNames.Add("copper brick", 47);
            tileTypeNames.Add("spike", 48);
            tileTypeNames.Add("cobweb", 51);
            tileTypeNames.Add("sand", 53);
            tileTypeNames.Add("glass", 54);
            tileTypeNames.Add("obsidian", 56);
            tileTypeNames.Add("ash", 57);
            tileTypeNames.Add("hellstone", 58);
            tileTypeNames.Add("mud", 59);
            tileTypeNames.Add("jungle grass", 60);
            tileTypeNames.Add("sapphire", 63);
            tileTypeNames.Add("ruby", 64);
            tileTypeNames.Add("emerald", 65);
            tileTypeNames.Add("topaz", 66);
            tileTypeNames.Add("amethyst", 67);
            tileTypeNames.Add("diamond", 68);
            tileTypeNames.Add("mushroom grass", 70);
            tileTypeNames.Add("obsidian brick", 75);
            tileTypeNames.Add("hellstone brick", 76);
            tileTypeNames.Add("water", 253);
            tileTypeNames.Add("lava", 254);
            wallTypeNames.Add("stone wall", 1);
            wallTypeNames.Add("dirt wall untakeable", 2);
            wallTypeNames.Add("ebonstone wall", 3);
            wallTypeNames.Add("wood wall", 4);
            wallTypeNames.Add("gray brick wall", 5);
            wallTypeNames.Add("red brick wall", 6);
            wallTypeNames.Add("blue brick wall", 7);
            wallTypeNames.Add("green brick wall", 8);
            wallTypeNames.Add("pink brick wall", 9);
            wallTypeNames.Add("gold brick wall", 10);
            wallTypeNames.Add("silver brick wall", 11);
            wallTypeNames.Add("copper brick wall", 12);
            wallTypeNames.Add("hellstone brick wall", 13);
            wallTypeNames.Add("obsidian brick wall", 14);
            wallTypeNames.Add("mud wall", 15);
            wallTypeNames.Add("dirt wall", 16);
        }
    }
}
