using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastingSpellUI : MonoBehaviour
{
	[SerializeField] Transform holder;
	[SerializeField] GameObject iconTemplate;

	[SerializeField] Image castBar;

	[SerializeField] IconDict iconDict = new IconDict();

	[System.Serializable]class IconDict : SerializableDictionary<KeyCode, RandSprites> { }

	[System.Serializable] class RandSprites
	{
		public Sprite[] sprites;
	}

	private void Start()
	{
		SpellCaster.Instance.OnInputAdded += UpdateInputs;
		SpellCaster.Instance.OnCastingCleared += UpdateInputs;
		SpellCaster.Instance.OnCastDelayStarted += StartCastBar;
		MovementSystem.Instance.OnArrived += SetActiveStatus;

		gameObject.SetActive(false);
	}

	void OnDestroy()
	{
		SpellCaster.Instance.OnInputAdded -= UpdateInputs;
		SpellCaster.Instance.OnCastingCleared -= UpdateInputs;
		SpellCaster.Instance.OnCastDelayStarted -= StartCastBar;
		MovementSystem.Instance.OnArrived -= SetActiveStatus;
	}

	void SetActiveStatus(string id)
	{
		UpdateInputs();

		if (id == "Roof")
			gameObject.SetActive(true);
		else
			gameObject.SetActive(false);
	}

	void UpdateInputs(KeyCode x)
	{
		UpdateInputs();
	}

	void UpdateInputs()
	{
		StopAllCoroutines();
		castBar.fillAmount = 0f;

		holder.DestroyAllChildren();
		foreach(var key in SpellCaster.Instance.activelyCasting)
		{
			var inst = Instantiate(iconTemplate, holder);
			var img = inst.GetComponentInChildren<Image>();
			img.sprite = iconDict[key].sprites.GetRandom();

			inst.transform.localScale = Vector3.one * Random.Range(0.94f, 1.06f);
			inst.transform.eulerAngles = new Vector3(0,0, Random.Range(-2f,2f));
		}
	}

	void StartCastBar(SpellData spell, float duration)
	{
		StopAllCoroutines();
		StartCoroutine(Co_FillCastBar(duration));
	}

	IEnumerator Co_FillCastBar(float duration)
	{
		float elapsed = 0f;
		castBar.fillAmount = 0f;
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			castBar.fillAmount = elapsed / duration;
			yield return null;
		}
		castBar.fillAmount = 1f;
		castBar.fillAmount = 0f;
	}
}
