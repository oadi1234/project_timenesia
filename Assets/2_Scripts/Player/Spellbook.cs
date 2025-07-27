using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Global.Spells;
using _2_Scripts.Global.Utility;
using _2_Scripts.Spells;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;
using UnityEngine.Serialization;

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

        [Header("Double pip spells")] 
        [SerializeField] private GameObject spaceDash;

        [Header("Triple pip spells")]
        [SerializeField] private GameObject blast;

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

                { StringToEffortCombination("remaker"), new BaseSpell("chaosMagic", ChaosMagic, SpellType.Aoe) },
                { StringToEffortCombination("maker"), new BaseSpell("wildMagic", WildMagic, SpellType.Aoe) },
                { StringToEffortCombination("karma"), new BaseSpell("instability", Instability, SpellType.Aoe) },
                { StringToEffortCombination(""), new BaseSpell("dud", Dud, SpellType.Bolt) },

                #endregion

                #region single_pip_spells

                { StringToEffortCombination("a"), new BaseSpell("shield", Shield, SpellType.Buff) },
                { StringToEffortCombination("k"), new BaseSpell("spark", Spark, SpellType.Bolt) },
                { StringToEffortCombination("m"), new BaseSpell("mindProbe", () => { }, SpellType.Meleespell) },

                #endregion

                #region double_pip_spells

                { StringToEffortCombination("ak"), new BaseSpell("strongShield", StrongShield, SpellType.Buff) },
                { StringToEffortCombination("ke"), new BaseSpell("spaceDash", SpaceDash, SpellType.Heavy) },
                { StringToEffortCombination("kk"), new BaseSpell("chainbolt", () => { }, SpellType.Sustained) },

                #endregion

                #region triple_pip_spells

                { StringToEffortCombination("aka"), new BaseSpell("bulwark", () => { }, SpellType.Buff) },
                { StringToEffortCombination("kaa"), new BaseSpell("blast", Blast, SpellType.Aoe) },
                { StringToEffortCombination("kar"), new BaseSpell("revengeBlast", () => { }, SpellType.Buff) },
                { StringToEffortCombination("ema"), new BaseSpell("lifesteal", () => { }, SpellType.Bolt) },
                { StringToEffortCombination("kkk"), new BaseSpell("telekinesis", () => { }, SpellType.Buff) },
                { StringToEffortCombination("kae"), new BaseSpell("shockwaveCharge", () => { }, SpellType.Heavy) },

                #endregion

                #region quadruple_pip_spells

                { StringToEffortCombination("akkk"), new BaseSpell("electromagneticCannon", () => { }, SpellType.Bolt) },
                { StringToEffortCombination("akea"), new BaseSpell("blackhole", () => { }, SpellType.Aoe) },
                { StringToEffortCombination("aaaa"), new BaseSpell("heal", () => { }, SpellType.Buff) },
                { StringToEffortCombination("eeee"), new BaseSpell("timeschism", Timeschism, SpellType.Buff) },
                { StringToEffortCombination("aeer"), new BaseSpell("bluePortal", () => { }, SpellType.Bolt) },
                { StringToEffortCombination("reea"), new BaseSpell("orangePortal", () => { }, SpellType.Bolt) }

                #endregion
            };
        }

        public BaseSpell GetSpellCastData(List<EffortType> effortCombination)
        {
            // Debug.Log(effortCombination.Aggregate("", (current, effort) => current + (effort + " ")));
            if (allSpells.TryGetValue(effortCombination, out var data))
            {
                return data;
            }

            return GetWildMagic(effortCombination.Count);
        }
        
        public BaseSpell GetSpellDataForUI(List<EffortType> effortCombination)
        {
            return allSpells.GetValueOrDefault(effortCombination, new BaseSpell("invalid", () => { }, SpellType.Bolt));
        }

        public static List<EffortType> StringToEffortCombination(string effortType)
        {
            var effortArray = new List<EffortType>();
            for (int i = 0; i < effortType.Length; i++)
            {
                if (CharToEffortType.TryGetValue(effortType[i], out var effort))
                    effortArray.Add(effort);
            }

            return effortArray;
        }
        
        public static int EffortCombinationToInt(List<EffortType> effortCombination)
        {
            int result = 0;
            int pow = 1;
            for (int i = effortCombination.Count - 1; i >= 0; i--)
            {
                result += (int)effortCombination[i] * pow;
                pow *= 10;
            }

            return result;
        }

        public static List<EffortType> IntToEffortCombination(int value)
        {
            List<EffortType> result = new();
            while (value > 0)
            {
                EffortType effort = (EffortType) (value % 10);
                value /= 10;
                result.Add(effort);
            }
            return result;
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
        }

        #endregion
        
        #region double_pip_spells

        private void StrongShield()
        {
            //TODO change below to its own instance of shield spell. It uses the base shieldBuff now.
            //TODO make it so that it is a longer spell, requiring longer duration buff.
            // There is a bit of a logic rework needed in PlayerAnimationStateHandler.cs for that.
            spellInstance = Instantiate(shieldBuff, PlayerPosition.GetPlayerTransform());
            spellInstance.transform.localScale = direction;
            spellInstance.GetComponent<ShieldSpellHandler>().SetShieldIntensity(2);
        }

        private void SpaceDash()
        {
            spellInstance = Instantiate(spaceDash, PlayerPosition.GetPlayerTransform());
            // spellInstance.transform.rotation = Quaternion.FromToRotation(direction, Vector3.right);
            spellInstance.transform.localScale = direction;
        }
        
        #endregion
        
        #region triple_pip_spells

        private void Blast()
        {
            spellInstance = Instantiate(blast, PlayerPosition.GetPlayerTransform());
            spellInstance.transform.localScale = direction;
        }
        
        #endregion
        #region quadruple_pip_spells

        private void Timeschism()
        {
            //TODO spell saves game.
        }

        #endregion

        #region wild_magic_methods
        
        public BaseSpell GetWildMagic(int effortCombinationCount)
        {
            return effortCombinationCount switch
            {
                2 => allSpells[StringToEffortCombination("karma")],
                3 => allSpells[StringToEffortCombination("maker")],
                4 => allSpells[StringToEffortCombination("remaker")],
                _ => allSpells[StringToEffortCombination("")]
            };
        }

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
            Debug.Log("Dud.");
        }

        #endregion
    }
}