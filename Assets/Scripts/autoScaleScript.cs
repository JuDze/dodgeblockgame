using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class autoScaleScript : MonoBehaviour
{
    void Start()
    {
        FitToCamera();
    }

#if UNITY_EDITOR
    void Update() // so it updates in Editor too
    {
        FitToCamera();
    }
#endif

    private void FitToCamera()
    {
        Camera cam = Camera.main;
        if (!cam || !TryGetComponent(out SpriteRenderer sr)) return;

        // Get camera size in world units
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        // Get sprite size in world units
        float spriteHeight = sr.sprite.bounds.size.y;
        float spriteWidth = sr.sprite.bounds.size.x;

        // Compute new scale so sprite covers entire screen
        Vector3 scale = transform.localScale;
        scale.x = camWidth / spriteWidth;
        scale.y = camHeight / spriteHeight;
        transform.localScale = scale;
    }
}
