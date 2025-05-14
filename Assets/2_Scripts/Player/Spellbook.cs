using System.Collections.Generic;
using _2_Scripts.Global.Spells;
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

        public Vector3 direction = Vector2.zero;
        public Vector3 originPoint = Vector2.zero;

        private GameObject spellInstance;

        [Header("Single pip spells")]
        [SerializeField] private GameObject sparkboltBeam;

        [SerializeField] private GameObject shieldBuff;

        [Header("Double pip spells")] [SerializeField]
        private GameObject spaceDash;

        private Dictionary<List<EffortType>, BaseSpell> allSpells;

        private static readonly Dictionary<char, EffortType> CharToEffortType = new()
        {
            { 'k', EffortType.Kinesis },
            { 'm', EffortType.Mind },
            { 'e', EffortType.Entropy },
            { 'r', EffortType.Rune },
            { 'a', EffortType.Aether }
        };

        public void PopulateAllSpells()
        {
            // TODO the below could probably be loaded from a file if need arises.
            allSpells = new Dictionary<List<EffortType>, BaseSpell>(new EffortArrayComparer())
            {
                #region wild_magic

                { StringToEfType("remaker"), new BaseSpell("chaosMagic", ChaosMagic, SpellType.Aoe) },
                { StringToEfType("maker"), new BaseSpell("wildMagic", WildMagic, SpellType.Aoe) },
                { StringToEfType("karma"), new BaseSpell("instability", Instability, SpellType.Aoe) },
                { StringToEfType(""), new BaseSpell("dud", Dud, SpellType.Bolt) },

                #endregion

                #region single_pip_spells

                { StringToEfType("a"), new BaseSpell("shield", Shield, SpellType.Buff) },
                { StringToEfType("k"), new BaseSpell("spark", Spark, SpellType.Bolt) },
                { StringToEfType("m"), new BaseSpell("mindProbe", () => { }, SpellType.Meleespell) },

                #endregion

                #region double_pip_spells

                { StringToEfType("ak"), new BaseSpell("strongShield", () => { }, SpellType.Buff) },
                { StringToEfType("ke"), new BaseSpell("spaceDash", () => { }, SpellType.Heavy) },
                { StringToEfType("kk"), new BaseSpell("chainbolt", () => { }, SpellType.Sustained) },

                #endregion

                #region triple_pip_spells

                { StringToEfType("aka"), new BaseSpell("bulwark", () => { }, SpellType.Buff) },
                { StringToEfType("kaa"), new BaseSpell("blast", () => { }, SpellType.Aoe) },
                { StringToEfType("kar"), new BaseSpell("revengeBlast", () => { }, SpellType.Buff) },
                { StringToEfType("ema"), new BaseSpell("lifesteal", () => { }, SpellType.Bolt) },
                { StringToEfType("kkk"), new BaseSpell("telekinesis", () => { }, SpellType.Buff) },
                { StringToEfType("kae"), new BaseSpell("shockwaveCharge", () => { }, SpellType.Heavy) },

                #endregion

                #region quadruple_pip_spells

                { StringToEfType("akkk"), new BaseSpell("electromagneticCannon", () => { }, SpellType.Bolt) },
                { StringToEfType("akea"), new BaseSpell("blackhole", () => { }, SpellType.Aoe) },
                { StringToEfType("aaaa"), new BaseSpell("heal", () => { }, SpellType.Buff) },

                #endregion
            };
        }

        public BaseSpell GetSpellData(List<EffortType> effortCombination)
        {
            // string debugString = "";
            // for (int i = 0; i < effortCombination.Count; i++)
            // {
            //     debugString += effortCombination[i] + " ";
            // }
            // Debug.Log("Spellbook - received effort: " + debugString);

            if (allSpells.TryGetValue(effortCombination, out var data))
            {
                // Debug.Log("Found spell: "+data.Name);
                return data;
            }

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

        #region single_pip_spells

        private void Spark()
        {
            spellInstance = Instantiate(sparkboltBeam, PlayerPosition.GetPlayerTransform());
            spellInstance.GetComponent<BeamSpellHandler>().SetDirection(direction);
            spellInstance.transform.position = new Vector3(
                PlayerPosition.GetPlayerTransform().position.x + originPoint.x,
                PlayerPosition.GetPlayerTransform().position.y + originPoint.y, 0);
            spellInstance.transform.rotation = Quaternion.Euler(0, 0,
                Vector3.Angle(direction, Vector3.right) * (Mathf.Approximately(direction.y, 1) ? -1 : 1));
        }

        private void Shield()
        {
            spellInstance = Instantiate(shieldBuff, PlayerPosition.GetPlayerTransform());
            spellInstance.transform.localScale = direction;
            // spellInstance.GetComponent<ShieldSpellHandler>().SetShieldIntensity(1);
        }

        #endregion

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