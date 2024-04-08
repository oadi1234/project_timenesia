using System.Collections.Generic;
using _2_Scripts.UI.Elements.Enum;
using UnityEngine;

namespace _2_Scripts.UI.Elements.InGame
{
    public class TooltipStringLoader : MonoBehaviour
    {
        //TODO in the future set it so that it loads from an external file. It will help with localization.
        private Dictionary<TooltipType, string> tooltipsForAbility = new Dictionary<TooltipType, string>();

        private void Awake()
        {
            tooltipsForAbility.Add(TooltipType.AbilityDash, "Do a quick dash with C button");
            tooltipsForAbility.Add(TooltipType.AbilityDoubleJump, "Double jump by pressing spacebar mid air.");
            tooltipsForAbility.Add(TooltipType.AbilityWallJump, "While next to a wall, slide down and jump off of it using spacebar.");
            tooltipsForAbility.Add(TooltipType.None, "");
            tooltipsForAbility.Add(TooltipType.QuestionMarks, "???");
            tooltipsForAbility.Add(TooltipType.NotYetCollected, "This technique remains a mystery...");
            tooltipsForAbility.Add(TooltipType.InventoryTab, "Your abilities, equipment and passive effects.");
            tooltipsForAbility.Add(TooltipType.SpellsTab, "Your spellbook.");
            tooltipsForAbility.Add(TooltipType.MapTab, "Map of local area.");
            tooltipsForAbility.Add(TooltipType.JournalTab, "Miscellaneous information on beasts, enemies or other residents of the world.");
            // TODO remove, deserialize from a file or something.
        }

        public string GetTooltip(TooltipType name)
        {
            return tooltipsForAbility.GetValueOrDefault(name, "If you can see this then something went wrong.");
        }
    }
}
