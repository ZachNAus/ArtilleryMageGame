using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingLeyLine : MonoBehaviour
{
    [System.Serializable]
    class RuneDetail
	{
        public Transform rune;

        public float rotateSpeed;
	}

    [SerializeField] List<RuneDetail> runes = new List<RuneDetail>();

	private void Update()
	{
		for (int i = 0; i < runes.Count; i++)
		{
				runes[i].rune.gameObject.SetActive(SpellCaster.Instance.activelyCasting.Count > 0);

			if (SpellCaster.Instance.activelyCasting.Count > i)
			{
				runes[i].rune.Rotate(new Vector3(0,0, runes[i].rotateSpeed * Time.deltaTime));
			}
		}
	}
}
