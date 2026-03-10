using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public float fallSpeed = 2f;   // speed is set by spawner depending on level
    private float bottomY;         // world Y of the screen bottom
     //private bool canFall = false; 

    void Start()
    {
        // Camera.main.ViewportToWorldPoint converts screen (0–1) → world position
        // Vector3 bottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        // bottomY = bottom.y - 1f; // add a small margin so blocks are fully off-screen
        var cam = Camera.main;
        float bottom = cam.transform.position.y - cam.orthographicSize;
        bottomY = bottom - 1f; // little margin off-screen


    }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < bottomY)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            gm.AddScore();
            Destroy(gameObject);
        
        }
    }
}
