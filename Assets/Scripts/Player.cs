using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [Header("References")]
    [SerializeField] private Transform visual;   // assign the child "Visual"
    [SerializeField] private Animator anim;      // assign Animator from Visual

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private string runBoolParam = "isRunning";

    

    private Rigidbody2D rb;
    private bool facingRight = true;
    private float moveInput;

    // NEW: cache to avoid re-setting same values every frame
    private bool prevIsRunning = false;
    private int  prevDirSign   = 0;   // -1, 0, 1

    void Awake()
    {
        gm = gm ?? FindObjectOfType<GameManager>();
        
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        // Fallbacks if not assigned
        if (visual == null && transform.childCount > 0) visual = transform.GetChild(0);
        if (anim == null && visual != null) anim = visual.GetComponent<Animator>();

        // Initialize facing from current scale (prevents unexpected flip on start)
        float sx = (visual != null ? visual.localScale.x : transform.localScale.x);
        if (!Mathf.Approximately(sx, 0f))
            facingRight = sx > 0f;

        if (anim != null)
            {
                anim.updateMode = AnimatorUpdateMode.Normal;
                anim.SetBool(runBoolParam, false);
            }
    }

    void Start()
    {
        SnapToGround();
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        moveInput = 0f;
        

        // Ignore input if pointer/touch is over UI
        if (IsPointerOverUI())
        {
            // Only update animator if value actually changed
            if (anim && prevIsRunning != false)
            {
                anim.SetBool(runBoolParam, false);
                prevIsRunning = false;
            }
            return;
        }


  

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            Vector3 charScreen = Camera.main.WorldToScreenPoint(transform.position);
            moveInput = Input.mousePosition.x < charScreen.x ? -1f : 1f;
        }
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) > 0.01f) moveInput = Mathf.Sign(h);
#else
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
                {
                    Vector3 charScreen = Camera.main.WorldToScreenPoint(transform.position);
                    moveInput = t.position.x < charScreen.x ? -1f : 1f;
                    break;
                }
            }
        }
#endif
        bool isRunning = Mathf.Abs(moveInput) > 0.01f;

        // set Animator bool only when it changes
        if (anim && isRunning != prevIsRunning)
        {
            anim.SetBool(runBoolParam, isRunning);
            prevIsRunning = isRunning;
        }

        // flip only when direction sign changes
        int dirSign = isRunning ? (moveInput > 0f ? 1 : -1) : 0;
        if (dirSign != 0 && dirSign != prevDirSign)
        {
            FaceDirection(dirSign);
        }
        prevDirSign = dirSign;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f) return; 
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        // Clamp position inside camera
        ClampToScreen();
        
    }
    
    private void ClampToScreen()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // World position of camera edges
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z - cam.transform.position.z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z - cam.transform.position.z));

        // Measure player's half width in world units
        float halfWidth = 0.5f; // fallback
        SpriteRenderer sr = visual != null ? visual.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            halfWidth = sr.bounds.extents.x;
        }
        else
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) halfWidth = col.bounds.extents.x;
        }

        Vector3 pos = transform.position;

        // Clamp inside screen using half width offset
        pos.x = Mathf.Clamp(pos.x, min.x + halfWidth, max.x - halfWidth);
        // optional: limit Y as well if needed
        // pos.y = Mathf.Clamp(pos.y, min.y + halfHeight, max.y - halfHeight);

        transform.position = pos;
    }
    private void FaceDirection(float dir)
    {
        if (dir > 0f && !facingRight) Flip();
        else if (dir < 0f && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Transform target = visual != null ? visual : transform;
        Vector3 s = target.localScale;
       // s.x = Mathf.Approximately(s.x, 0f) ? 1f : -s.x;
       s.x = Mathf.Abs(s.x) * (facingRight ? 1f : -1f);
        target.localScale = s;
    }

    private bool IsPointerOverUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
#else
        if (EventSystem.current == null) return false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                return true;
        }
        return false;
#endif
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("block"))
        {
            gm.PlayerHit();
            Destroy(other.gameObject);
        }
    }

        public void IncreaseSpeed(float level)
    {
        float speedGain = 0.3f + 0.05f * level;  
        moveSpeed += speedGain;
        Debug.Log("Player speed now " + moveSpeed);
    }


    void SnapToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5f, LayerMask.GetMask("ground"));
        if (hit.collider != null)
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + GetComponent<Collider2D>().bounds.extents.y;
            transform.position = pos;
        }
    }

}


 