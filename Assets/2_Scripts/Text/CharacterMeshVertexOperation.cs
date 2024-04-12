using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Class containing all operations performed on a single character
public class CharacterMeshVertexOperation : MonoBehaviour
{
    public TextController textController;
    public int characterIndex;
    public Color32 originalColor;
    public Color32 fadeOutColor;
    public float magnitude;
    public float transitionTime = 0f;
    public int sign = 0;
    public int materialReferenceIndex;
    public int vertexIndex;
    public float size = 0f;

    public List<Color32> gradientColors;
    private int currentGradientIndex = 0;

    private Vector3 operationVector; //base vector used for operating on character mesh
    private Vector3 positionAdjustmentVector; //for adding in things like shake, wave or other positioning changes.

    private IEnumerator coroutine;
    //private void Awake()
    //{
    //    StartCoroutine(OperateOnVertex());
    //}


    private void Update()
    {
        
    }

    private void OperateOnVector(TMP_MeshInfo[] originalMeshInfo, float operationPercent)
    {
        for (int i = 0; i < 4; i++)
        {
            operationVector = ((originalMeshInfo[materialReferenceIndex].vertices[vertexIndex + i] - GetMiddlePoint()) * size) + GetMiddlePoint() + positionAdjustmentVector * magnitude;
            //textController.GetTextInfo().meshInfo[materialReferenceIndex].vertices[vertexIndex + i] = operationVector;

            Color32 transitionColor = Color32.Lerp(originalColor, fadeOutColor, 1f - operationPercent);

            textController.GetTextInfo().meshInfo[materialReferenceIndex].colors32[vertexIndex + i] = transitionColor;
        }
    }

    private Vector3 GetMiddlePoint()
    {
        return (textController.GetOriginalMeshInfo()[materialReferenceIndex].vertices[vertexIndex + 0]
            + textController.GetOriginalMeshInfo()[materialReferenceIndex].vertices[vertexIndex + 2]) / 2;
    }
}
