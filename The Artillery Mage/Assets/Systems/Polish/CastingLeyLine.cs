using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingLeyLine : MonoBehaviour
{
	[System.Serializable]
	class RunePart
	{
		public Transform transform;
		public float rotateSpeed;
	}

	[System.Serializable]
	class RuneDetail
	{
		public List<RunePart> parts = new List<RunePart>();
	}

	[SerializeField] List<RuneDetail> runes = new List<RuneDetail>();

	private void Update()
	{
		for (int i = 0; i < runes.Count; i++)
		{
			bool active = SpellCaster.Instance.activelyCasting.Count > 0 || SpellCaster.Instance.IsCastingDelay;
			bool spinning = SpellCaster.Instance.activelyCasting.Count > i || SpellCaster.Instance.IsCastingDelay;

			foreach (RunePart part in runes[i].parts)
			{
				part.transform.gameObject.SetActive(active);

				if (spinning)
					part.transform.Rotate(new Vector3(0, 0, part.rotateSpeed * Time.deltaTime));
			}
		}
	}
}
