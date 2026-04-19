using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

	public void PlaySpellVFX(SpellData spell, float duration)
	{
		if (!vfxSetup.TryGetValue(spell, out VFXInfo info)) return;
		foreach (var vfx in info.effectsToEnable) vfx.Play();
		StartCoroutine(Co_StopVFX(info.effectsToEnable, duration));
	}

	IEnumerator Co_StopVFX(List<VisualEffect> effects, float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (var vfx in effects) vfx.Stop();
	}

#if UNITY_EDITOR
	[Button]
	void DevAutofillVFXSetup()
	{
		var guids = UnityEditor.AssetDatabase.FindAssets("t:SpellData");
		foreach (var guid in guids)
		{
			var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			var spell = UnityEditor.AssetDatabase.LoadAssetAtPath<SpellData>(path);
			if (spell != null && !vfxSetup.ContainsKey(spell))
				vfxSetup[spell] = new VFXInfo();
		}
		UnityEditor.EditorUtility.SetDirty(this);
	}
#endif
}
