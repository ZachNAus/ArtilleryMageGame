using UnityEngine;
using DG.Tweening;

public class RequestNotifier : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip notificationSound;
    [SerializeField] GameObject objectToEnable;
    [SerializeField] GameObject objectToShake;

    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakeStrength = 0.3f;
    [SerializeField] int shakeVibrato = 10;

    private void Start()
    {
        RequestManager.Instance.OnRequestAdded += OnRequestAdded;
    }


    private void OnRequestAdded(RequestData request)
    {
        if (notificationSound != null)
            audioSource.PlayOneShot(notificationSound);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        if (objectToShake != null)
            objectToShake.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, fadeOut: false);
    }
}
