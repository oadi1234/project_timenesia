using System.Collections.Generic;
using _2_Scripts.Global;
using _2_Scripts.Global.Events;
using _2_Scripts.Model;
using _2_Scripts.Player;
using _2_Scripts.UI.Elements.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Elements.InGame
{
    public class UIAbilityField : MonoBehaviour
    {
        public AbilityName abilityName = AbilityName.DoubleJump;

        public Sprite NoSkillSprite;
        public Sprite SkillSprite;
        public Image CurrentImage;
        public TooltipType TooltipLocked = TooltipType.NotYetCollected;
        public TooltipType TooltipUnlocked = TooltipType.QuestionMarks;
        private ButtonFrame buttonFrame;
        private CollectedEventType _reactOnCollectedEventType;


        private void Start()
        {
            CurrentImage = GetComponent<Image>();
            CurrentImage.sprite = NoSkillSprite;
            buttonFrame = GetComponent<ButtonFrame>();
            buttonFrame.tooltipType = TooltipLocked;
            OnPlayerEnteredEvent.OnPlayerEntered += UnlockAbilityInMenu;
            _reactOnCollectedEventType = GetReactOnEventType();
            OnAbilityLoad();
        }

        public void OnAbilityLoad()
        {
            if(GameDataManager.Instance.currentGameData.Abilities.GetValueOrDefault(abilityName, false))
            {
                CurrentImage.sprite = SkillSprite;
                buttonFrame.tooltipType = TooltipUnlocked;
            }
        }

        private void UnlockAbilityInMenu(IOnPlayerEnteredEvent obj)
        {
            if(obj.collectedEventType == _reactOnCollectedEventType)
            {
                CurrentImage.sprite = SkillSprite;
                buttonFrame.tooltipType = TooltipUnlocked;
            }
        }

        private CollectedEventType GetReactOnEventType()
        {
            switch(abilityName)
            {
                case AbilityName.DoubleJump:
                    return CollectedEventType.DoubleJumpCollected;
                case AbilityName.Dash:
                    return CollectedEventType.DashCollected;
                case AbilityName.WallJump:
                    return CollectedEventType.WallJumpCollected;
                default:
                    return CollectedEventType.None;
            }
        }
    }
}
