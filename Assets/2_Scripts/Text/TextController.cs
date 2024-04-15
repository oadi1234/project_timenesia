using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public Color32 transitionFromColor;
    public List<TextCommand> commands = new List<TextCommand>();
    public TextTransitionType transitionType = TextTransitionType.Fade;
    public List<Color32> gradientColorList = new List<Color32>(); // TODO replace with Gradient class, which does the exact same thing and is straight up better than a list.

    public float animationTime = 2f;
    public float charDelay = 0.1f;
    public float commandCooldown = 20f;
    public bool canBeFastForwarded = false;
    public bool transitionAllAtOnce = false;
    public bool hideFromRightToLeft = false;
    public bool startHidden = true;

    private TMP_Text text;

    private TMP_TextInfo textInfo;
    private TMP_MeshInfo[] originalMeshInfo;
    private Color32 originalTextColor;
    public TextState currentTextState = TextState.Hidden;
    public TextState nextTextState = TextState.None;

    private Dictionary<int, List<CharacterCommand>> commandsDictionary = new Dictionary<int, List<CharacterCommand>>(); // character index - list of commands pair.
    private float currentCommandCooldown = 0f;
    private float currentCharDelayShow = 0f;
    private float currentCharDelayHide = 0f;
    private bool[] coroutineRunningForGivenIndex;
    private int lastVisibleCharacterIndex;
    private int currentlyAnimatedShow = 0;
    private int currentlyAnimatedHide = 0;

    public CharacterMeshVertexOperation[] perLetterAnimation;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        text.color = transitionFromColor;
        PrepareText();
        UpdateMesh();
        if (!startHidden)
            ShowText();
    }

    private void OnDisable()
    {
        currentTextState = TextState.Hidden;
        nextTextState = TextState.None;
        SetAllVerticesToZero();
        text.color = transitionFromColor;
        UpdateMesh();
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            coroutineRunningForGivenIndex[i] = false;
            perLetterAnimation[i] = null;
        }
    }

    private void Update()
    {
        // TODO this probably needs to be moved to some sort of dialogue controller.
        if (canBeFastForwarded && Input.GetKeyDown(KeyCode.Return)) 
        {
            nextTextState = TextState.Skip;
        }

        currentCommandCooldown += Time.fixedDeltaTime;
        if (currentCommandCooldown>=commandCooldown)
        {
            currentCommandCooldown = 0;
            HandleNextTextCommand();
        }

        if (currentTextState != TextState.Hidden)
            UpdateMesh();
    }

    public void SetMaterialColorAtVertex(int materialReferenceIndex, int vertexIndex, Color32 result)
    {
        textInfo.meshInfo[materialReferenceIndex].colors32[vertexIndex] = result;
    }

    public void SetMaterialVerticeAtVertex(int materialReferenceIndex, int vertexIndex, Vector3 result)
    {
        textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] = result;
    }

    public TMP_MeshInfo[] GetOriginalMeshInfo()
    {
        return originalMeshInfo;
    }

    public void SetText(string text)
    {
        this.text.text = text;
        Initialize();
    }

    public bool IsHidden()
    {
        return currentTextState == TextState.Hidden;
    }
    private void Initialize()
    {
        TranslateCommandListToPerIndexDictionary();
        text = GetComponent<TMP_Text>();
        text.overrideColorTags = true;
        originalTextColor = text.color;
        textInfo = text.textInfo;
        text.ForceMeshUpdate();
        coroutineRunningForGivenIndex = new bool[textInfo.characterCount];
        perLetterAnimation = new CharacterMeshVertexOperation[textInfo.characterCount];

        for (lastVisibleCharacterIndex = textInfo.characterCount - 1; lastVisibleCharacterIndex >= 0 && !textInfo.characterInfo[lastVisibleCharacterIndex].isVisible; --lastVisibleCharacterIndex) ;

        originalMeshInfo = textInfo.CopyMeshInfoVertexData(); //store original mesh info data for animating.
    }

    public void SetCommands(List<TextCommand> commands)
    {
        this.commands = commands;
        TranslateCommandListToPerIndexDictionary();
    }

    public void ShowText()
    {
        nextTextState = TextState.AnimationShow;
        currentCommandCooldown = commandCooldown;
    }

    public void HideText()
    {
        currentCommandCooldown = 0;
        nextTextState = TextState.AnimationHide;
    }

    public void RemoveFromPerLetterAnimation(int charIndex)
    {
        perLetterAnimation[charIndex] = null;
    }

    private void HandleNextTextCommand()
    {
        switch (currentTextState)
        {
            case TextState.Visible:
                if (nextTextState==TextState.AnimationHide)
                {
                    if (hideFromRightToLeft)
                        StartCoroutine(HideTextCoroutineRightToLeft());
                    else
                        StartCoroutine(HideTextCoroutine());
                    nextTextState = TextState.None;
                }
                break;
            case TextState.Hidden:
                if (nextTextState == TextState.AnimationShow)
                {
                    StartCoroutine(ShowTextCoroutine());
                    nextTextState = TextState.None;
                }
                break;
            case TextState.AnimationShow:
                if (nextTextState == TextState.Skip)
                {
                    //TODO Add possibility to skip text animation here.
                }
                break;
            default:
                break;
        }
    }

    public void PrepareText()
    {
        text.color = transitionFromColor;
        SetAllVerticesToZero();
    }

    private void SetAllVerticesToZero()
    {
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (textInfo.meshInfo[i].vertices != null)
            {
                for (int j = 0; j < textInfo.meshInfo[i].vertices.Length; j++)
                    textInfo.meshInfo[i].vertices[j] = Vector3.zero;
            }
        }
    }

    private IEnumerator HideTextCoroutine()
    {
        currentTextState = TextState.AnimationHide;
        currentlyAnimatedHide = 0;
        currentCharDelayHide = 0f;
        while (perLetterAnimation[lastVisibleCharacterIndex]!=null)
        {

            if (currentlyAnimatedHide < textInfo.characterCount) 
            { 
                TMP_CharacterInfo charInfo = textInfo.characterInfo[currentlyAnimatedHide];
                if (charInfo.isVisible && coroutineRunningForGivenIndex[currentlyAnimatedHide])
                {
                    perLetterAnimation[currentlyAnimatedHide].StopAnimating();
                    coroutineRunningForGivenIndex[currentlyAnimatedHide] = false;
                }
            }

            currentCharDelayHide += Time.fixedDeltaTime;
            if (currentCharDelayHide >= charDelay || transitionAllAtOnce)
            {
                currentCharDelayHide = 0f;
                currentlyAnimatedHide++;
            }

            yield return null;
        }
        currentTextState = TextState.Hidden;
    }

    private IEnumerator HideTextCoroutineRightToLeft()
    {
        currentTextState = TextState.AnimationHide;
        currentlyAnimatedHide = lastVisibleCharacterIndex;
        currentCharDelayHide = 0f;
        while (perLetterAnimation[0] != null)
        {

            if (currentlyAnimatedHide >= 0)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[currentlyAnimatedHide];
                if (charInfo.isVisible && coroutineRunningForGivenIndex[currentlyAnimatedHide])
                {
                    perLetterAnimation[currentlyAnimatedHide].StopAnimating();
                    coroutineRunningForGivenIndex[currentlyAnimatedHide] = false;
                }
            }

            currentCharDelayHide += Time.fixedDeltaTime;
            if (currentCharDelayHide >= charDelay || transitionAllAtOnce)
            {
                currentCharDelayHide = 0f;
                currentlyAnimatedHide--;
            }

            yield return null;
        }
        currentTextState = TextState.Hidden;
    }

    private IEnumerator ShowTextCoroutine()
    {
        currentTextState = TextState.AnimationShow;
        currentlyAnimatedShow = 0;
        currentCharDelayShow = 0f;
        while (perLetterAnimation[lastVisibleCharacterIndex] == null)
        {
            if (currentlyAnimatedShow < textInfo.characterCount)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[currentlyAnimatedShow];
                if (charInfo.isVisible && !coroutineRunningForGivenIndex[currentlyAnimatedShow])
                {
                    perLetterAnimation[currentlyAnimatedShow] = CreateNewMeshVertexOperation(charInfo, currentlyAnimatedShow);
                    StartCoroutine(perLetterAnimation[currentlyAnimatedShow].PerformCharacterCalculations());
                    coroutineRunningForGivenIndex[currentlyAnimatedShow] = true;
                }
            }

            currentCharDelayShow += Time.fixedDeltaTime;
            if (currentCharDelayShow >= charDelay || transitionAllAtOnce)
            {
                currentCharDelayShow = 0f;
                currentlyAnimatedShow++;
            }
            
            yield return null;
        }
        currentTextState = TextState.Visible;
    }

    public void UpdateMesh()
    {
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private void TranslateCommandListToPerIndexDictionary()
    {
        for (int i = 0; i < commands.Count; i++)
        {
            if (commands[i].indexStart <= commands[i].indexEnd)
            {
                List<CharacterCommand> characterCommands;

                List<int> indexList = Enumerable.Range(commands[i].indexStart, commands[i].indexEnd - commands[i].indexStart + 1).ToList();

                for (int charIndex = 0; charIndex < indexList.Count; charIndex++)
                {
                    if (commandsDictionary.ContainsKey(indexList[charIndex]))
                        characterCommands = commandsDictionary[indexList[charIndex]];
                    else
                        characterCommands = new List<CharacterCommand>();

                    characterCommands.Add(new CharacterCommand(commands[i], indexList[charIndex]));

                    commandsDictionary[indexList[charIndex]] = characterCommands;
                }
            }

        }
    }

    private CharacterMeshVertexOperation CreateNewMeshVertexOperation(TMP_CharacterInfo charInfo, int characterIndex)
    {
        CharacterMeshVertexOperation operation = new CharacterMeshVertexOperation();
        operation.textController = this;
        operation.materialReferenceIndex = charInfo.materialReferenceIndex;
        operation.vertexIndex = charInfo.vertexIndex;
        operation.transitionType = transitionType;
        if (commandsDictionary.ContainsKey(characterIndex))
        {
            operation.characterCommands = commandsDictionary[characterIndex];

        }
        operation.gradientColors = gradientColorList;
        operation.characterIndex = characterIndex;
        operation.fontSize = text.fontSize;
        operation.stopSignal = false;
        operation.originalColor = originalTextColor;
        return operation;
    }
}
