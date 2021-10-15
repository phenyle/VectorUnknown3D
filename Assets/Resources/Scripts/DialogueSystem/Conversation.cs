using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation")]
public class Conversation: ScriptableObject
{
    public Dialogue[] dialogues;
}

[System.Serializable]
public struct Dialogue
{
    public Character character;

    [TextArea(3, 10)]
    public string[] sentences;
}
