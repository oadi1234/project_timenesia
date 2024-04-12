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
    public List<Color32> gradientColorList = new List<Color32>();

    public float animationTime = 2f;
    public float charDelay = 0.1f;
    public float commandCooldown = 20f;
    public float hideDelay = 0.5f;
    public bool canBeFastForwarded = false;
    public bool transitionAllAtOnce = false;
    public bool hideFromRightToLeft = false;

    private TMP_Text text;

    private TMP_TextInfo textInfo;
    private TMP_MeshInfo[] originalMeshInfo;
    private Color32 originalTextColor;
    private TextState currentTextState = TextState.Hidden;
    private TextState nextTextState = TextState.None;

    public Dictionary<int, List<CharacterCommand>> commandDictionaryPerIndex = new Dictionary<int, List<CharacterCommand>>(); // character index - list of commands pair.
    private float currentCommandCooldown = 0f;
    private float currentCharDelayShow = 0f;
    private float currentCharDelayHide = 0f;
    private float[] elapsedTimePerCharacterTransition;
    private bool[] shouldCharacterBeShown;
    private bool[] coroutineRunningForGivenIndex;
    private int lastVisibleCharacterIndex;
    private int currentlyAnimatedShow = 0;
    private int currentlyAnimatedHide = 0;

    private IEnumerator showTextCoroutine;
    private IEnumerator hideTextCoroutine;
    private List<CharacterMeshVertexOperation> perLetterAnimation = new List<CharacterMeshVertexOperation>();

    private void Awake()
    {
        TranslateCommandListToPerIndexDictionary();
        text = GetComponent<TMP_Text>();
        text.overrideColorTags = true;
        originalTextColor = text.faceColor;
        textInfo = text.textInfo;
        text.ForceMeshUpdate();
        elapsedTimePerCharacterTransition = new float[textInfo.characterCount];
        shouldCharacterBeShown = new bool[textInfo.characterCount];
        coroutineRunningForGivenIndex = new bool[textInfo.characterCount];
        for (int i = 0; i < textInfo.characterCount; i++)
            elapsedTimePerCharacterTransition[i] = 0;

        for (lastVisibleCharacterIndex = textInfo.characterCount - 1; lastVisibleCharacterIndex >= 0 && !textInfo.characterInfo[lastVisibleCharacterIndex].isVisible; --lastVisibleCharacterIndex);

        originalMeshInfo = textInfo.CopyMeshInfoVertexData(); //store original mesh info data for animating.
        PrepareTextBasedOnTransitionType();

        showTextCoroutine = ShowTextCoroutine();
        hideTextCoroutine = HideTextCoroutine();


        UpdateMesh();
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

        if (currentTextState != TextState.Hidden && commands.Count > 0)
            UpdateMesh();
    }

    public TMP_TextInfo GetTextInfo()
    {
        return textInfo;
    }

    public TMP_MeshInfo[] GetOriginalMeshInfo()
    {
        return originalMeshInfo;
    }

    public void SetText(string text)
    {
        this.text.text = text;
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

    private void HandleNextTextCommand()
    {
        switch (currentTextState)
        {
            case TextState.Visible:
                if (nextTextState==TextState.AnimationHide)
                {
                    hideTextCoroutine = HideTextCoroutine();
                    StartCoroutine(hideTextCoroutine);
                    nextTextState = TextState.None;
                }
                break;
            case TextState.Hidden:
                if (nextTextState == TextState.AnimationShow)
                {
                    showTextCoroutine = ShowTextCoroutine();
                    StartCoroutine(showTextCoroutine);
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
                    if (commandDictionaryPerIndex.ContainsKey(charIndex))
                        characterCommands = commandDictionaryPerIndex[charIndex];
                    else
                        characterCommands = new List<CharacterCommand>();

                    characterCommands.Add(new CharacterCommand(commands[i], charIndex));

                    commandDictionaryPerIndex[charIndex] = characterCommands;
                }
            }

        }
    }

    private void PrepareTextBasedOnTransitionType()
    {
        switch (transitionType)
        {
            case TextTransitionType.Simple:
            case TextTransitionType.Enlarge:
                SetAllVerticesToZero();
                break;
            case TextTransitionType.Fade:
                SetInitialColor();
                break;
        }
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

    private void SetInitialColor()
    {
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (textInfo.meshInfo[i].vertices != null)
            {
                for (int j = 0; j < textInfo.meshInfo[i].colors32.Length; j++)
                    textInfo.meshInfo[i].colors32[j] = transitionFromColor;
            }
        }
    }

    private IEnumerator HideTextCoroutine()
    {
        yield return new WaitForSeconds(hideDelay);
        currentTextState = TextState.AnimationHide;
        currentlyAnimatedHide = 0;
        shouldCharacterBeShown[0] = false;
        currentCharDelayHide = 0f;
        while (elapsedTimePerCharacterTransition[lastVisibleCharacterIndex] > 0f)
        {

            if (currentlyAnimatedHide < textInfo.characterCount && !shouldCharacterBeShown[currentlyAnimatedHide]) 
            { 
                TMP_CharacterInfo charInfo = textInfo.characterInfo[currentlyAnimatedHide];
                if (charInfo.isVisible && !coroutineRunningForGivenIndex[currentlyAnimatedHide])
                {
                    coroutineRunningForGivenIndex[currentlyAnimatedHide] = true;
                    StartCoroutine(AnimateCharacterHide(charInfo, currentlyAnimatedHide));
                }
            }

            currentCharDelayHide += Time.fixedDeltaTime;
            if (currentCharDelayHide >= charDelay || transitionAllAtOnce)
            {
                currentCharDelayHide = 0f;
                currentlyAnimatedHide++;
                if (currentlyAnimatedHide < textInfo.characterCount)
                    shouldCharacterBeShown[currentlyAnimatedHide] = false;
            }

            yield return null;
        }

        //end all per character animations here.
        currentTextState = TextState.Hidden;
    }

    private IEnumerator ShowTextCoroutine()
    {
        currentTextState = TextState.AnimationShow;
        currentlyAnimatedShow = 0;
        shouldCharacterBeShown[0] = true;
        currentCharDelayShow = 0f;
        while (elapsedTimePerCharacterTransition[lastVisibleCharacterIndex] < animationTime)
        {
            if (currentlyAnimatedShow < textInfo.characterCount && shouldCharacterBeShown[currentlyAnimatedShow])
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[currentlyAnimatedShow];
                if (charInfo.isVisible && !coroutineRunningForGivenIndex[currentlyAnimatedShow])
                {
                    coroutineRunningForGivenIndex[currentlyAnimatedShow] = true;
                    StartCoroutine(AnimateCharacterShow(charInfo, currentlyAnimatedShow));
                    //start all per character animations here
                    CreateCharacterAnimationCoroutine(charInfo, currentlyAnimatedShow);
                }
            }

            currentCharDelayShow += Time.fixedDeltaTime;
            if (currentCharDelayShow >= charDelay || transitionAllAtOnce)
            {
                currentCharDelayShow = 0f;
                currentlyAnimatedShow++;
                if (currentlyAnimatedShow < textInfo.characterCount)
                    shouldCharacterBeShown[currentlyAnimatedShow] = true;
            }
            
            yield return null;
        }
        currentTextState = TextState.Visible;
    }

    private void UpdateMesh()
    {
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private IEnumerator AnimateCharacterShow(TMP_CharacterInfo charInfo, int characterIndex)
    {
        //Get vertices per character. Each character has 4 vertices in its mesh.
        //following textInfo.meshInfo is sort of a draft data stored by TMP_TextInfo.
        //by the way - all of this is horribly documented on the internet so I am trying
        // to sort of save it here.
        while (elapsedTimePerCharacterTransition[characterIndex] < animationTime)
        {
            PerformAnimationBasedOnTransitionType(charInfo, characterIndex, true);
            yield return null;
        }
        coroutineRunningForGivenIndex[characterIndex] = false;
    }

    private IEnumerator AnimateCharacterHide(TMP_CharacterInfo charInfo, int characterIndex)
    {
        //as method above.
        while (elapsedTimePerCharacterTransition[characterIndex] > 0f)
        {
            PerformAnimationBasedOnTransitionType(charInfo, characterIndex, false);
            yield return null;
        }
        coroutineRunningForGivenIndex[characterIndex] = false;
    }

    private void CreateCharacterAnimationCoroutine(TMP_CharacterInfo charInfo, int characterIndex)
    {
        List<CharacterCommand> charCommands;
        if (commandDictionaryPerIndex.ContainsKey(characterIndex))
        {
             charCommands = commandDictionaryPerIndex[characterIndex];
        }
        else charCommands = new List<CharacterCommand>();

        for (int i = 0; i < charCommands.Count; i++)
        {
            //execute coroutine per each command
            PerformAnimationBasedOnCommand(charInfo, charCommands[i].type, i);
        }
    }

    private void PerformAnimationBasedOnCommand(TMP_CharacterInfo charInfo, TextCommandType textCommandType, int characterIndex)
    {
        IEnumerator characterCoroutine;
        switch (textCommandType)
        {
            case TextCommandType.Shake:
                //characterCoroutine = ShakyText(charInfo, characterIndex);
                //StartCoroutine(characterCoroutine);
                break;
            case TextCommandType.Wave:
                break;
            case TextCommandType.FadeWave:
                break;
            case TextCommandType.None:
            case TextCommandType.Gradient:
            case TextCommandType.Pulse:
            default:
                break;
        }
    }

    //private IEnumerator ShakyText(TMP_CharacterInfo charInfo, int characterIndex)
    //{

    //    while (true)
    //    {
    //        Vector3 adjustmentVector = new Vector3(
    //            (Mathf.PerlinNoise((characterIndex + Time.unscaledTime) * 5f, 0) - 0.5f) * text.fontSize * 0.06f,
    //            (Mathf.PerlinNoise((characterIndex + Time.unscaledTime) * 5f, 1) - 0.5f) * text.fontSize * 0.06f,
    //            0);
    //        for (int i = 0; i < 4; i++)
    //            textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + i] = originalMeshInfo_working[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + i] + adjustmentVector;
    //        yield return null;
    //    }
    //}

    private void PerformAnimationBasedOnTransitionType(TMP_CharacterInfo charInfo, int characterIndex, bool transitionInAnimation)
    {
        if (transitionInAnimation)
            elapsedTimePerCharacterTransition[characterIndex] += Time.fixedDeltaTime;
        else
            elapsedTimePerCharacterTransition[characterIndex] -= Time.fixedDeltaTime;

        switch (transitionType)
        {
            case TextTransitionType.Simple:
                AssignModifiedVerticeToMeshInfo(charInfo, getMiddlePoint(charInfo), transitionInAnimation ? 1 : 0);
                break;
            case TextTransitionType.Enlarge:
                AssignModifiedVerticeToMeshInfo(charInfo, getMiddlePoint(charInfo), Mathf.Clamp01((elapsedTimePerCharacterTransition[characterIndex] / animationTime)));
                break;
            case TextTransitionType.Fade:
                AssignModifiedAlphaValueToMeshInfo(charInfo, elapsedTimePerCharacterTransition[characterIndex] / animationTime);
                break;
        }
    }

    private void AssignModifiedVerticeToMeshInfo(TMP_CharacterInfo charInfo, Vector3 middlePoint, float size)
    {
        //We grab the meshes from text, grab our current character and modify given vertice by given amount
        for (int i = 0; i < 4; i++)
        {
            Vector3 modifiedVector = ((originalMeshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + i] - middlePoint) * size) + middlePoint;
            textInfo.meshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + i] = modifiedVector;
        }
    }

    private void AssignModifiedAlphaValueToMeshInfo(TMP_CharacterInfo charInfo, float alphaPercent)
    {
        for (int i = 0; i < 4; i++)
        {
            textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[charInfo.vertexIndex + i] = Color32.Lerp(originalTextColor, transitionFromColor, 1f - alphaPercent);
        }
    }

    private Vector3 getMiddlePoint(TMP_CharacterInfo charInfo )
    {
        return (originalMeshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + 0] 
            + originalMeshInfo[charInfo.materialReferenceIndex].vertices[charInfo.vertexIndex + 2]) / 2;
    }
}
