using UnityEngine;

[CreateAssetMenu(fileName = "New Span Value", menuName = "Span Value")]
public class SpanValue : ScriptableObject
{
    [Header("Span Value Combination")]
    public int choiceID1;
    public int choiceID2;
/*
    [Header("Vector Points")]
    public Vector3 point1;
    public Vector3 point2;
    */
    [Header("Rotation")]
    public float x;
    public float y;
    public float z;
}
