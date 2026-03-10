using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class AutoScaleGround : MonoBehaviour
{
    public float height = 1f; // optional fixed height

    void Start() => FitToCamera();
#if UNITY_EDITOR
    void Update() => FitToCamera();
#endif

    private void FitToCamera()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (!sr || sr.drawMode != SpriteDrawMode.Tiled)
        {
            Debug.LogWarning("Set SpriteRenderer Draw Mode to Tiled for proper ground tiling.");
            return;
        }

        // Get camera width in world units
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        // Apply height
        Vector2 newSize = sr.size;
        newSize.x = camWidth;               // fill horizontally
        newSize.y = height > 0 ? height : newSize.y; // fixed height
        sr.size = newSize;

        // Center on bottom of screen if desired
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - camHeight / 2f + newSize.y / 2f, transform.position.z);
    }
}
