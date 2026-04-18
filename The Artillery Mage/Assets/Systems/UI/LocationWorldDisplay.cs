using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationWorldDisplay : MonoBehaviour
{
	[SerializeField] LocationList location;

    [SerializeField] TextMeshProUGUI liberationText;

	private void Update()
	{
		float percent = GameManager.Instance.GetGoodPercent(location);

		liberationText.SetText($"{Mathf.Floor(percent*100)}% Liberated");
	}
}
