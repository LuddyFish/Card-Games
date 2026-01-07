using UnityEngine;

/// <summary>
/// Element that is an attachment for <see cref="Canvas"/> <c>GameObject</c>s where their <br/>
/// render mode is set to <see cref="RenderMode.ScreenSpaceCamera"/>
/// </summary>
[RequireComponent(typeof(Canvas))]
public class SetToCamera : MonoBehaviour
{
    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }
}
