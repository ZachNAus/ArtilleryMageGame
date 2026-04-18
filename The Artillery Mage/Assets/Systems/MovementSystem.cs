using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
	public static MovementSystem Instance { get; private set; }

    [SerializeField] float camRotateSpeed = 180;

    [System.Serializable]
    public class LocationData
	{
		public string id;

        //Where we go
        public Transform target;

		public Transform midPoint;

        public bool canRotate;

        public bool canCast;
	}

    [SerializeField] LocationData[] locations;

    public LocationData CurrentLocation { get; private set; }
	public bool Moving { get; private set; }

	private void Awake()
	{
		Instance = this;

		GoToLocation(locations[0], true);
	}

	public void GoToLocation(string id)
	{
		GoToLocation(locations.FirstOrDefault(x => x.id == id), false);
	}

    void GoToLocation(LocationData location, bool instant)
	{
		CurrentLocation = location;
		if (instant)
		{
            transform.position = location.target.position;
            transform.forward = location.target.forward;
		}
		else
		{
			Moving = true;
			ScreenFade.Instance.FadeToBlack();
			Quaternion lookRotation = Quaternion.LookRotation(CurrentLocation.midPoint.position - transform.position);
			transform.DORotateQuaternion(lookRotation, 1.25f);
			transform.DOMove(CurrentLocation.midPoint.position, 2).OnComplete(() =>
			{
				ScreenFade.Instance.FadeFromBlack();
				transform.position = CurrentLocation.target.position;
				transform.forward = CurrentLocation.target.forward;

				Moving = false;
			});
		}
	}

	private void Update()
	{
		if (CurrentLocation.canRotate && !Moving)
		{
			float mouseX = Input.GetAxis("Mouse X");
			transform.Rotate(Vector3.up, mouseX * camRotateSpeed * Time.deltaTime, Space.World);
		}
	}
}
