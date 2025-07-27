using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Text;
using TMPro;
using UnityEngine;


// Class containing all operations performed on a single character
public class CharacterMeshVertexOperation
{
    public TextController textController;
    public int characterIndex;
    public Color32 originalColor;

    public int materialReferenceIndex;
    public int vertexIndex;
    public TextTransitionType transitionType = TextTransitionType.Fade;
    public List<CharacterCommand> characterCommands = new List<CharacterCommand>();
    public float fontSize;
    public bool stopSignal;

    public List<Color32> gradientColors;
    private int currentGradientIndex = 0;

    private Color32 currentTargetColor;
    private float transitionTime = 0f;
    private float size = 0f;
    private bool operationTimeAddsUp = true;

    private Vector3 operationVector; //base vector used for operating on character mesh
    private float x = 0f; //position adjustment vector values
    private float y = 0f;
    private Color32 operationColor;
    private int chaoticResult = Random.Range(0, 6);

    private static readonly float WAVE_MOD = 0.01f;
    private static readonly float SHAKE_MOD = 0.03f;
    private static readonly float PULSE_MOD = 0.33f;

    public void StopAnimating()
    {
        stopSignal = true;
    }

    public IEnumerator PerformCharacterCalculations()
    {
        while (true)
        {
            CalculateValuesByTransitionType();
            PerformOperationBasedOnCommands();
            OperateOnVector(textController.GetOriginalMeshInfo());
            yield return null;
            ResetOperationValues();

            if (stopSignal && transitionTime > 0f)
            {
                transitionTime -= Time.fixedDeltaTime;
            }
            else if (!stopSignal && transitionTime < textController.animationTime)
            {
                transitionTime += Time.fixedDeltaTime;
            }
            if (stopSignal && transitionTime < 0f)
            {
                EndCoroutineAndRemoveFromTextController();
                yield break;
            }
        }

    }

    private void EndCoroutineAndRemoveFromTextController()
    {
        textController.RemoveFromPerLetterAnimation(characterIndex);
    }

    private void OperateOnVector(TMP_MeshInfo[] originalMeshInfo)
    {
        for (int i = 0; i < 4; i++)
        {
            var positionAdjustmentVector = new Vector3(x, y, 0f);
            operationVector = ((originalMeshInfo[materialReferenceIndex].vertices[vertexIndex + i] - GetMiddlePoint()) * size) + GetMiddlePoint() + positionAdjustmentVector;

            //Color32 transitionColor = Color32.Lerp(textController.transitionFromColor, originalColor, transitionTime / textController.animationTime);
            //operationColor = Color32.Lerp(textController.transitionFromColor, originalColor, transitionTime / textController.animationTime);

            textController.SetMaterialColorAtVertex(materialReferenceIndex, vertexIndex + i, operationColor);
            textController.SetMaterialVerticeAtVertex(materialReferenceIndex, vertexIndex + i, operationVector);
        }
    }

    private void CalculateValuesByTransitionType()
    {
        switch (transitionType)
        {
            case TextTransitionType.FadeEnlarge:
                operationColor = Color32.Lerp(textController.transitionFromColor, originalColor, transitionTime / textController.animationTime);
                goto case TextTransitionType.EnlargeReduce;
            case TextTransitionType.EnlargeReduce:
                size = Mathf.Clamp01(transitionTime / textController.animationTime);
                break;
            case TextTransitionType.FadeReduce:
                size = Mathf.Clamp(3f - (transitionTime / textController.animationTime)*2, 1f, 3f);
                operationColor = Color32.Lerp(textController.transitionFromColor, originalColor, transitionTime / textController.animationTime);
                break;
            case TextTransitionType.Fade:
                operationColor = Color32.Lerp(textController.transitionFromColor, originalColor, transitionTime / textController.animationTime);
                size = 1;
                break;
            case TextTransitionType.Chaotic:
                if (chaoticResult == 0) 
                    goto case TextTransitionType.Simple;
                if (chaoticResult == 1) 
                    goto case TextTransitionType.EnlargeReduce;
                if (chaoticResult == 2) 
                    goto case TextTransitionType.Fade;
                if (chaoticResult == 3)
                    goto case TextTransitionType.ScrollDown;
                if (chaoticResult == 4)
                    goto case TextTransitionType.ScrollUp;
                if (chaoticResult == 5)
                    goto case TextTransitionType.FadeReduce;
                if (chaoticResult == 6)
                    goto case TextTransitionType.FadeEnlarge;
                break;
            case TextTransitionType.ScrollDown:
                size = 1;
                y += Mathf.Pow(((transitionTime/textController.animationTime) - 1) * 2, 2) * fontSize * 0.5f;
                operationColor = Color32.Lerp(textController.transitionFromColor, originalColor, transitionTime / textController.animationTime);
                break;
            case TextTransitionType.ScrollUp:
                size = 1;
                y -= Mathf.Pow((transitionTime / textController.animationTime - 1) * 2, 2) * fontSize * 0.5f;
                operationColor = Color32.Lerp(textController.transitionFromColor, originalColor, transitionTime / textController.animationTime);
                break;
            case TextTransitionType.Simple:
            default:
                if (!stopSignal)
                {
                    operationColor = originalColor;
                    size = 1;
                }
                else
                {
                    operationColor = textController.transitionFromColor;
                    size = 0;
                }
                break;
        }
    }

    private void PerformOperationBasedOnCommands()
    {
        foreach (CharacterCommand command in characterCommands)
        {
            switch (command.type)
            {
                case TextCommandType.Wave:
                    y += Mathf.Sin((characterIndex * 1.5f) + (Time.unscaledTime * 6)) * fontSize * command.magnitude * WAVE_MOD;
                    break;
                case TextCommandType.Shake:
                    x += (Mathf.PerlinNoise((characterIndex + Time.unscaledTime) * 15f, 0) - 0.5f) * fontSize * command.magnitude * SHAKE_MOD;
                    y += (Mathf.PerlinNoise((characterIndex + Time.unscaledTime) * 15f, 1) - 0.5f) * fontSize * command.magnitude * SHAKE_MOD;
                    break;
                case TextCommandType.FadeWave:
                    operationColor = 
                        Color32.Lerp(textController.transitionFromColor, operationColor, 
                        (Mathf.Sin(-characterIndex * (1 / textController.animationTime) + Time.unscaledTime * 4f * command.magnitude) + 1)/2);
                    break;
                case TextCommandType.Pulse:
                    CalculateOperationTimePulse(command);
                    size *= -(Mathf.Pow(Mathf.Clamp01(command.operationTime / command.magnitude), 2)) * PULSE_MOD + 1;
                    break;
                case TextCommandType.GradientWave:
                    if (gradientColors.Count > 0)
                    {
                        CalculateOperationTimeGradientWave(command);
                        var currentGradient  = Color32.Lerp(gradientColors[currentGradientIndex], gradientColors[(currentGradientIndex + 1) % gradientColors.Count], command.operationTime / command.magnitude);
                        operationColor = Color32.Lerp(currentGradient, operationColor, 1f - transitionTime / textController.animationTime);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void CalculateOperationTimePulse(CharacterCommand command)
    {
        if (command.commandSwitch)
        {
            command.operationTime += Time.fixedDeltaTime;
            command.commandSwitch = command.operationTime < command.magnitude;
        }
        else
        {
            command.operationTime -= Time.fixedDeltaTime;
            command.commandSwitch = command.operationTime < 0f;
        }
    }

    private void CalculateOperationTimeGradientWave(CharacterCommand command)
    {
        if (command.operationTime > command.magnitude)
        {
            command.operationTime = 0f;
            currentGradientIndex = (currentGradientIndex + 1) % gradientColors.Count;
        }
        command.operationTime += Time.fixedDeltaTime;
    }

    private void ResetOperationValues()
    {
        x = 0f; //prevents issues like +=noise just spiraling letters everywhere.
        y = 0f;
        operationColor = originalColor;
    }

    private Vector3 GetMiddlePoint()
    {
        return (textController.GetOriginalMeshInfo()[materialReferenceIndex].vertices[vertexIndex + 0]
            + textController.GetOriginalMeshInfo()[materialReferenceIndex].vertices[vertexIndex + 2]) / 2;
    }
}
