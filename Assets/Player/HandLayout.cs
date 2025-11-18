using System.Collections;
using UnityEngine;

public class HandLayout : MonoBehaviour
{
    public float width = 5;

    [Range(0.2f, 1f)]
    [SerializeField] private float spacing;

    public void SetCards()
    {
        if (transform.childCount == 0) return;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.localPosition = GetPosition(i);
        }
    }

    private Vector2 GetPosition(int index)
    {
        if (transform.childCount == 0) return Vector2.zero;
        spacing = Mathf.Clamp(width / transform.childCount, 0, 1f);
        float remainingSpace = width - spacing * transform.childCount;
        float padding = remainingSpace * 0.5f;

        return new Vector2(padding + -width * 0.5f + spacing * index, 0);
    }

    public void ReceiveCard(Transform card, int position, float duration)
    {
        StartCoroutine(AnimationUtilities.Lerp(card, card.position, GetPosition(position) + (Vector2)transform.position, duration));
    }
}
