using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class LocationWorldDisplay : MonoBehaviour
{
	[SerializeField] LocationList location;

    [SerializeField] TextMeshProUGUI liberationText;

	[SerializeField] SpellsToVFX vfxSetup = new SpellsToVFX();

	[System.Serializable]
	public class SpellsToVFX : SerializableDictionary<SpellData, VFXInfo> { }

	[System.Serializable]
	public class VFXInfo
	{
		public List<VisualEffect> effectsToEnable;
	}

	private void Update()
	{
		float percent = GameManager.Instance.GetGoodPercent(location);

		liberationText.SetText($"{Mathf.Floor(percent*100)}% Liberated");
	}
}
