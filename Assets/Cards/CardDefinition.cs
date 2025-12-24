using UnityEngine;

[CreateAssetMenu(fileName = "Card Template", menuName = "Cards/Card Template")]
public class CardDefinition : ScriptableObject
{
    public int suit;
    public int rank;

    [Header("Front faces")]
    public Sprite lowContrast;
    public Sprite highContrast;
}
