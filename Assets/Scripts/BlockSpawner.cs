using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BlockSpawner : MonoBehaviour
{
[Header("Block Settings")]
    public GameObject blockPrefab;
    public float spawnInterval = 2f;     // how often to spawn
    public float blockFallSpeed = 2f;    // start speed
    public float edgePadding = 0.2f;     // keep blocks a bit inside the screen
    public float spawnAbove = 0.5f;      // how far above the top to spawn
   // private bool canSpawn = false;
    [Header("Level Settings")]
    public int level = 1;
    public float speedIncrease = 0.5f;   // extra speed per level
    public float spawnFaster = 0.1f;     // shorter interval per level

    private float timer;


    void Awake(){
       enabled = false; // start disabled, enabled by GameManager
       Debug.Log("[Spawner] Start() called, enabled=" + enabled);
    }
        void Update()
    {
        
        timer += Time.deltaTime;
       // Debug.Log($"Spawner running, timer = {timer:F2}, timeScale = {Time.timeScale}");

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBlock();
        }
    }

        public void NextLevel()
    {
        // Increase falling speed
        blockFallSpeed += speedIncrease;

        // Decrease spawn interval, but keep a reasonable lower limit
        spawnInterval = Mathf.Max(0.5f, spawnInterval - spawnFaster);

        //Debug.Log($"Spawner leveled up → fallSpeed: {blockFallSpeed:F2}, spawnInterval: {spawnInterval:F2}");
    }

    
    void SpawnBlock()
    {
        StartCoroutine(SpawnWaveCoroutine());
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        Camera cam = Camera.main;
        if (!cam.orthographic)
        {
            Debug.LogWarning("Camera is not orthographic; switching math accordingly or set orthographic = true.");
        }

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth  = camHalfHeight * cam.aspect;
        float spawnY = cam.transform.position.y + camHalfHeight + 0.5f;

        // number of blocks per wave (based on aspect ratio)
        int blockCount = Mathf.Clamp(Mathf.FloorToInt(cam.aspect * 3f), 1, 6);

        float blockHalfWidth = GetHalfWidthWorld(blockPrefab);
        float leftX  = cam.transform.position.x - camHalfWidth  + blockHalfWidth;
        float rightX = cam.transform.position.x + camHalfWidth  - blockHalfWidth;
        float totalWidth = rightX - leftX;
        float spacing = totalWidth / (blockCount + 1);

        // Precompute all spawn positions
        List<Vector3> spawnPositions = new List<Vector3>();
        for (int i = 0; i < blockCount; i++)
        {
            float x = leftX + spacing * (i + 1);
            x += Random.Range(-spacing * 0.3f, spacing * 0.3f);
            float yOffset = Random.Range(-0.3f, 0.3f);
            spawnPositions.Add(new Vector3(x, spawnY + yOffset, 0f));
        }

        // Randomize the order of spawn positions
        for (int i = 0; i < spawnPositions.Count; i++)
        {
            Vector3 temp = spawnPositions[i];
            int randIndex = Random.Range(i, spawnPositions.Count);
            spawnPositions[i] = spawnPositions[randIndex];
            spawnPositions[randIndex] = temp;
        }

        // Spawn each block in random order with delay
        foreach (Vector3 pos in spawnPositions)
        {
            GameObject block = Instantiate(blockPrefab, pos, Quaternion.identity);

            var fb = block.GetComponent<FallingBlock>();
            fb.fallSpeed = blockFallSpeed + (level - 1) * speedIncrease;

            yield return new WaitForSeconds(0.2f); // delay between blocks
        }
    }



/// <summary>
/// Returns half the visual width in world units for an object,
/// considering all child SpriteRenderers/Renderers.
/// </summary>
float GetHalfWidthWorld(GameObject go)
{
    // Prefer SpriteRenderers (2D)
    var srs = go.GetComponentsInChildren<SpriteRenderer>();
    if (srs.Length > 0)
    {
        float maxX = float.NegativeInfinity;
        float minX = float.PositiveInfinity;
        foreach (var sr in srs)
        {
            // Renderer bounds are already in world space and include scale & rotation
            var b = sr.bounds;
            maxX = Mathf.Max(maxX, b.max.x);
            minX = Mathf.Min(minX, b.min.x);
        }
        return (maxX - minX) * 0.5f;
    }

    // Fallback: any Renderer (in case you use meshes/UI renderers in world space)
    var rens = go.GetComponentsInChildren<Renderer>();
    if (rens.Length > 0)
    {
        float maxX = float.NegativeInfinity;
        float minX = float.PositiveInfinity;
        foreach (var r in rens)
        {
            var b = r.bounds;
            maxX = Mathf.Max(maxX, b.max.x);
            minX = Mathf.Min(minX, b.min.x);
        }
        return (maxX - minX) * 0.5f;
    }

    // If no renderers found, assume zero width
    return 0f;
}


}
