using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] UnityEvent onClick;

    private void OnMouseDown()
    {
        onClick.Invoke();
    }
}
