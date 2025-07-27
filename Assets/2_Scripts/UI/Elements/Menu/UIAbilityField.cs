using System.Collections.Generic;
using _2_Scripts.Global;
using _2_Scripts.Global.Events;
using _2_Scripts.Global.Events.Model;
using _2_Scripts.Player.model;
using _2_Scripts.UI.Elements.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Elements.Menu
{
    public class UIAbilityField : MonoBehaviour
    {
        public UnlockableName unlockableName = UnlockableName.DoubleJump;

        [SerializeField] private Sprite noSkillSprite;
        [SerializeField] private Sprite skillSprite;
        private Image currentImage;
        [SerializeField] private TooltipType tooltipLocked = TooltipType.NotYetCollected;
        private TooltipType tooltipUnlocked = TooltipType.QuestionMarks;
        private ButtonFrame buttonFrame;
        private CollectedEventType _reactOnCollectedEventType;

        private static readonly Dictionary<UnlockableName, TooltipType> AbilityToTooltip = new()
        {
            { UnlockableName.None, TooltipType.None },
            { UnlockableName.DoubleJump, TooltipType.AbilityDoubleJump },
            { UnlockableName.Dash, TooltipType.AbilityDash },
            { UnlockableName.WallJump, TooltipType.AbilityWallJump },
            { UnlockableName.Aether, TooltipType.SourceAether },
            { UnlockableName.Kinesis, TooltipType.SourceKinesis },
            { UnlockableName.Mind, TooltipType.SourceMind },
            { UnlockableName.Rune, TooltipType.SourceRune },
            { UnlockableName.Entropy, TooltipType.SourceEntropy },
            { UnlockableName.NoSource, TooltipType.SourceNone }
        };

        private static readonly Dictionary<UnlockableName, CollectedEventType> UnlockablePerEventName = new()
        {
            { UnlockableName.DoubleJump, CollectedEventType.DoubleJumpCollected },
            { UnlockableName.Dash, CollectedEventType.DashCollected },
            { UnlockableName.WallJump, CollectedEventType.WallJumpCollected },
            { UnlockableName.Aether, CollectedEventType.AetherCollected },
            { UnlockableName.Kinesis, CollectedEventType.KinesisCollected },
            { UnlockableName.Mind, CollectedEventType.MindCollected },
            { UnlockableName.Rune, CollectedEventType.RuneCollected },
            { UnlockableName.Entropy, CollectedEventType.EntropyCollected }
        };


        private void Start()
        {
            currentImage = GetComponent<Image>();
            currentImage.sprite = noSkillSprite;
            buttonFrame = GetComponent<ButtonFrame>();
            buttonFrame.tooltipType = tooltipLocked;
            OnPlayerEnteredEvent.OnPlayerEntered += UnlockAbilityInMenu;
            _reactOnCollectedEventType =
                UnlockablePerEventName.GetValueOrDefault(unlockableName, CollectedEventType.None);
            tooltipUnlocked = AbilityToTooltip.GetValueOrDefault(unlockableName, TooltipType.QuestionMarks);
            OnAbilityLoad();
        }

        private void OnAbilityLoad()
        {
            if (GameDataManager.Instance.currentGameData.Abilities.GetValueOrDefault(unlockableName, false))
            {
                currentImage.sprite = skillSprite;
                buttonFrame.tooltipType = tooltipUnlocked;
            }
        }

        private void UnlockAbilityInMenu(IOnPlayerEnteredEvent obj)
        {
            if (obj.collectedEventType == _reactOnCollectedEventType)
            {
                currentImage.sprite = skillSprite;
                buttonFrame.tooltipType = tooltipUnlocked;
            }
        }
    }
}