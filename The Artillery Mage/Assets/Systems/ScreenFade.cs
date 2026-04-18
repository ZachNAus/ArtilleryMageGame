using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance { get; private set; }

	[SerializeField] Image img;

	private void Awake()
	{
		Instance = this;
	}

	public void FadeToBlack()
	{
		img.DOFade(1, .75f);
	}
	public void FadeFromBlack()
	{
		img.DOFade(0, .75f);
	}
}
