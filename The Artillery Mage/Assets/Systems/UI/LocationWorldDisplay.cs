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

	[SerializeField] Image goodUnitsBar;
	[SerializeField] Image badUnitsBar;

	[SerializeField] GameObject changeIndicatorPrefab;
	[SerializeField] Transform indicatorHolder;
	[SerializeField] float tweenDistance = 60f;
	[SerializeField] float tweenDuration = 1f;

	float _lastPercent = -1f;
	int _lastGood = -1, _lastBad = -1;

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

		GameManager.Instance.GetUnits(location, out int good, out int bad);

		if (_lastPercent >= 0f && percent != _lastPercent)
			SpawnChangeIndicator(percent > _lastPercent, good - _lastGood, bad - _lastBad);

		_lastPercent = percent;
		_lastGood = good;
		_lastBad = bad;

		int maxUnits = GameManager.Instance.GetMaxBadUnits(location);
		if (goodUnitsBar != null) goodUnitsBar.fillAmount = maxUnits > 0 ? good / (float)maxUnits : 0f;
		if (badUnitsBar != null)  badUnitsBar.fillAmount  = maxUnits > 0 ? bad  / (float)maxUnits : 0f;
	}

	void SpawnChangeIndicator(bool increased, int deltaGood, int deltaBad)
	{
		if (changeIndicatorPrefab == null || indicatorHolder == null) return;

		var inst = Instantiate(changeIndicatorPrefab, indicatorHolder);
		var img = inst.GetComponentInChildren<Image>();
		var rect = inst.GetComponent<RectTransform>();
		var label = inst.GetComponentInChildren<TMPro.TextMeshProUGUI>();

		img.color = increased ? Color.green : Color.red;

		if (!increased)
			img.transform.localEulerAngles = new Vector3(0f, 0f, 180f);

		if (label != null)
		{
			var lines = new System.Text.StringBuilder();
			if (deltaBad != 0)  lines.AppendLine($"Demons: {deltaBad}");
			if (deltaGood != 0) lines.Append($"Humans: {deltaGood}");
			label.SetText(lines.ToString());
		}

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
