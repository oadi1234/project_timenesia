using System.Collections.Generic;
using _2_Scripts.Player.model;
using _2_Scripts.Player.utility;
using _2_Scripts.Spells;
using UnityEngine;

namespace _2_Scripts.Player
{
    [CreateAssetMenu(fileName = "spellbook", menuName = "ScriptableObjects/Spellbook", order = 6)]
    public class Spellbook : ScriptableObject
    {
        static Spellbook _instance;
        
        public static Spellbook Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<Spellbook>("spellbook");
                }

                return _instance;
            }
        }

        [SerializeField] private GameObject spellPrefab;
        
        private Dictionary<List<EffortType>, BaseSpell> allSpells = new(new EffortArrayComparer()) //by default all spells should only have max 4 capacity, except wild magic
        {
            #region wild_magic
            { StringToEfType("remaker"), new BaseSpell("chaosMagic", "chaosMagic.description", ChaosMagic, SpellType.Aoe)}, //wild magic at 4 pips
            { StringToEfType("maker"), new BaseSpell("wildMagic", "wildMagic.description", WildMagic, SpellType.Aoe)},    //wild magic at 3 pips
            { StringToEfType("karma"), new BaseSpell("instability", "instability.description", Instability, SpellType.Aoe)},  //wild magic at 2 pips
            { StringToEfType(""), new BaseSpell("dud", "dud.description", Dud, SpellType.Bolt)},               //dud at 1 pip
            #endregion
            #region single_capacity_spells
            { StringToEfType("a"), new BaseSpell("shield", "shield.description", () => {}, SpellType.Buff)},
            { StringToEfType("k"), new BaseSpell("sparkbolt", "sparkbolt.description", Sparkbolt, SpellType.Bolt)},
            { StringToEfType("m"), new BaseSpell("mindProbe", "mindProbe.description", () => {}, SpellType.Meleespell)},
            #endregion
            #region double_capacity_spells
            { StringToEfType("ak"), new BaseSpell("strongShield", "strongShield.description", () => {}, SpellType.Buff)},
            { StringToEfType("ke"), new BaseSpell("spaceDash", "spaceDash.description", () => {}, SpellType.Heavy)},
            #endregion
            #region triple_capacity_spells
            { StringToEfType("aka"), new BaseSpell("bulwark", "bulwark.description", () => {}, SpellType.Buff)},
            { StringToEfType("kaa"), new BaseSpell("blast", "blast.description", () => {}, SpellType.Aoe)},
            { StringToEfType("kar"), new BaseSpell("revengeBlast", "revengeBlast.description", () => {}, SpellType.Buff)},
            #endregion
        };

        private static readonly Dictionary<char, EffortType> CharToEffortType = new()
        {
            { 'k', EffortType.Kinesis },
            { 'm', EffortType.Mind },
            { 'e', EffortType.Entropy },
            { 'r', EffortType.Rune },
            { 'a', EffortType.Aether }
        };

        public BaseSpell GetSpellData(List<EffortType> effortCombination)
        {
            string debugString = "";
            for (int i = 0; i<effortCombination.Count; i++)
            {
                debugString += effortCombination[i] + " ";
            }
            Debug.Log("Received combination: " + debugString);
            if (allSpells.TryGetValue(effortCombination, out var data))
                return data;
            return effortCombination.Count switch
            {
                2 => allSpells[StringToEfType("karma")],
                3 => allSpells[StringToEfType("maker")],
                4 => allSpells[StringToEfType("remaker")],
                _ => allSpells[StringToEfType("")]
            };
        }

        public static List<EffortType> StringToEfType(string effortType)
        {
            var effortArray = new List<EffortType>();
            for (int i = 0; i < effortType.Length; i++)
            {
                if (CharToEffortType.TryGetValue(effortType[i], out var effort))
                    effortArray.Add(effort);
            }
            return effortArray;
        }

        private static void Sparkbolt()
        {
            Debug.Log("Sparkbolt!");
        }
        
        #region wild_magic_methods
        
        private static void WildMagic() // at 3 pips
        {
            Debug.Log("Wild Magic!");
        }

        private static void ChaosMagic() // at 4 pips
        {
            Debug.Log("Chaos!");
        }

        private static void Instability() // at 2 pips
        {
            Debug.Log("Instability!");
        }

        private static void Dud() // at 1 pips
        {
            Debug.Log("lol."); 
        }
        #endregion
    }
}
