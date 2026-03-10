using UnityEngine;
using UnityEngine.Events;

public class HideOnTap : MonoBehaviour
{
    public GameObject rootToHide;   // assign this object or a parent panel
    public UnityEvent OnTapped;     // drag your GameManager.StartGame() here

    void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            OnTapped?.Invoke();
            (rootToHide ? rootToHide : gameObject).SetActive(false);
        }
    }
}
