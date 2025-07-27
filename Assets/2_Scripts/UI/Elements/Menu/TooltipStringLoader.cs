using System.Collections.Generic;
using _2_Scripts.UI.Elements.Enum;
using UnityEngine;

namespace _2_Scripts.UI.Elements.Menu
{
    public class TooltipStringLoader : MonoBehaviour
    {
        private static readonly Dictionary<TooltipType, string> TooltipsForAbility = new()
        {
            // TODO the strings might change structure, depending on how i18n is implemented. placeholders.
            { TooltipType.AbilityDash, "ability.dash.description" },
            {TooltipType.AbilityDoubleJump, "ability.doublejump.description" },
            {TooltipType.AbilityWallJump, "ability.walljump.description" },
            {TooltipType.None, "none"}, //should not occur.
            {TooltipType.QuestionMarks, "generic.questionmarks"},
            {TooltipType.NotYetCollected, "ability.mystery.description"}, //"This technique remains a mystery..."
            {TooltipType.InventoryTab, "playermenu.inventory.description"}, //"Your abilities, equipment and passive effects."
            {TooltipType.SpellsTab, "playermenu.spellbook.description"}, // "Your spellbook"
            {TooltipType.MapTab, "playermenu.map.description"},
            {TooltipType.JournalTab, "playermenu.journal.description"},
            {TooltipType.SourceNone, "playermenu.spellbook.source.none.description"},
            {TooltipType.SourceAether, "playermenu.spellbook.source.aether.description"},
            {TooltipType.SourceKinesis, "playermenu.spellbook.source.kinesis.description"},
            {TooltipType.SourceMind, "playermenu.spellbook.source.mind.description"},
            {TooltipType.SourceRune, "playermenu.spellbook.source.rune.description"},
            {TooltipType.SourceEntropy, "playermenu.spellbook.source.entropy.description"},
            
        };

        public string GetTooltip(TooltipType name)
        {
            return TooltipsForAbility.GetValueOrDefault(name, "none");
        }
    }
}
