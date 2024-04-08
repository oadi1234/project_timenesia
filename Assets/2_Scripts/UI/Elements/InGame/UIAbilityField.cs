using _2___Scripts.Global;
using _2___Scripts.Global.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityField : MonoBehaviour
{
    public AbilityName abilityName = AbilityName.DoubleJump;

    public Sprite NoSkillSprite;
    public Sprite SkillSprite;
    public Image CurrentImage;
    public TooltipType TooltipLocked = TooltipType.NotYetCollected;
    public TooltipType TooltipUnlocked = TooltipType.QuestionMarks;
    private ButtonFrame buttonFrame;
    private IOnPlayerEnteredEvent.EventType reactOnEventType;


    private void Start()
    {
        CurrentImage = GetComponent<Image>();
        CurrentImage.sprite = NoSkillSprite;
        buttonFrame = GetComponent<ButtonFrame>();
        buttonFrame.tooltipType = TooltipLocked;
        OnPlayerEnteredEvent.OnPlayerEntered += UnlockAbilityInMenu;
        reactOnEventType = GetReactOnEventType();
        OnAbilityLoad();
    }

    public void OnAbilityLoad()
    {
        if(GameDataManager.Instance.stats.abilities.GetValueOrDefault(abilityName, false))
        {
            CurrentImage.sprite = SkillSprite;
            buttonFrame.tooltipType = TooltipUnlocked;
        }
    }

    private void UnlockAbilityInMenu(IOnPlayerEnteredEvent obj)
    {
        if(obj.eventType == reactOnEventType)
        {
            CurrentImage.sprite = SkillSprite;
            buttonFrame.tooltipType = TooltipUnlocked;
        }
    }

    private IOnPlayerEnteredEvent.EventType GetReactOnEventType()
    {
        switch(abilityName)
        {
            case AbilityName.DoubleJump:
                return IOnPlayerEnteredEvent.EventType.DoubleJumpCollected;
            case AbilityName.Dash:
                return IOnPlayerEnteredEvent.EventType.DashCollected;
            case AbilityName.WallJump:
                return IOnPlayerEnteredEvent.EventType.WallJumpCollected;
            default:
                return IOnPlayerEnteredEvent.EventType.None;
        }
    }
}
