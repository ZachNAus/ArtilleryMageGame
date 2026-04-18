using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbMenu : MonoBehaviour
{
    [SerializeField] Transform holder;
    [SerializeField] OrbJobTemplate template;

	private void OnEnable()
	{
		
	}

	void UpdateJobs()
	{
		holder.DestroyAllChildren();
	}
}
