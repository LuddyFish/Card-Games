using UnityEngine;

[CreateAssetMenu(fileName = "Joker Template", menuName = "Cards/Joker Template")]
public class JokerDefinition : ScriptableObject
{
    public int suit = -1;
    public int rank = -1;

    public Sprite[] faces;
}
