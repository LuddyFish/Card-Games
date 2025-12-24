using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Deck", menuName = "Cards/Default Card Set")]
public class CardDeckSet : ScriptableObject
{
    [Header("Back Covers")]
    public Sprite lowContrast;
    public Sprite highContrast;

    [Header("Deck")]
    public List<CardDefinition> cards;
}
