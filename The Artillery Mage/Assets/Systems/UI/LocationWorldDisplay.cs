using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class LocationWorldDisplay : MonoBehaviour
{
	[SerializeField] LocationList location;

    [SerializeField] TextMeshProUGUI liberationText;

	[SerializeField] GameObject changeIndicatorPrefab;
	[SerializeField] Transform indicatorHolder;
	[SerializeField] float tweenDistance = 60f;
	[SerializeField] float tweenDuration = 1f;

	float _lastPercent = -1f;

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

		if (_lastPercent >= 0f && percent != _lastPercent)
			SpawnChangeIndicator(percent > _lastPercent);
		_lastPercent = percent;
	}

	void SpawnChangeIndicator(bool increased)
	{
		if (changeIndicatorPrefab == null || indicatorHolder == null) return;

		var inst = Instantiate(changeIndicatorPrefab, indicatorHolder);
		var img = inst.GetComponent<Image>();
		var rect = inst.GetComponent<RectTransform>();

		img.color = increased ? Color.green : Color.red;

		if (!increased)
			inst.transform.localEulerAngles = new Vector3(0f, 0f, 180f);

		rect.DOAnchorPosY(rect.anchoredPosition.y + tweenDistance * (increased ? 1f : -1f), tweenDuration)
			.SetEase(Ease.OutQuad);
		img.DOFade(0f, tweenDuration)
			.SetEase(Ease.InQuad)
			.OnComplete(() => Destroy(inst));
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
