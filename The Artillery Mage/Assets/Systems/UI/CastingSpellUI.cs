using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastingSpellUI : MonoBehaviour
{
	[SerializeField] Transform holder;
	[SerializeField] GameObject iconTemplate;

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
		MovementSystem.OnArrived += SetActiveStatus;

		gameObject.SetActive(false);
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
}
