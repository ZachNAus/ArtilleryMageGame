using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbMenu : MonoBehaviour
{
    [SerializeField] Transform holder;
    [SerializeField] OrbJobTemplate template;

	private void OnEnable()
	{
		UpdateJobs();

		RequestManager.Instance.OnRequestAdded += UpdateJobs;
		RequestManager.Instance.OnRequestRemoved += UpdateJobs;
	}
	private void OnDisable()
	{
		RequestManager.Instance.OnRequestAdded -= UpdateJobs;
		RequestManager.Instance.OnRequestRemoved -= UpdateJobs;
	
		holder.DestroyAllChildren();
	}

	void UpdateJobs(RequestData d)
	{
		UpdateJobs();
	}

	void UpdateJobs()
	{
		holder.DestroyAllChildren();

		foreach(var job in RequestManager.Instance.currentlyActiveRequests)
		{
			var t = Instantiate(template, holder);
			t.Setup(job.request);
		}
	}
}
