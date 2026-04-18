using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] UnityEvent onClick;

    private void OnMouseDown()
    {
        if (GameManager.Instance.lockMovingIfActive.Any(x => x.activeInHierarchy))
            return;

        onClick.Invoke();
    }
}
