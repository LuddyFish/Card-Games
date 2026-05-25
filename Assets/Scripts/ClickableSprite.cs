using System;
using UnityEngine;

public class ClickableSprite : MonoBehaviour
{
    private bool SpriteClicked = false;

    public Action OnClick;
    public Action OnTrueClick;

    private void OnMouseDown()
    {
        SpriteClicked = true;
        OnClick?.Invoke();
    }

    private void OnMouseUp()
    {
        if (SpriteClicked)
        {
            OnTrueClick?.Invoke();
            SpriteClicked = false;
        }
    }
}
