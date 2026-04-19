using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequestManager : MonoBehaviour
{
	public static RequestManager Instance { get; private set; }

	[SerializeField] List<RequestData> requestPool;

	[SerializeField] float startGameDelay = 12;

	[SerializeField] Vector2 requestPopupTime = new Vector2(20, 35);

	[ReadOnly]
	public List<ActiveRequestData> currentlyActiveRequests = new List<ActiveRequestData>();

	public event Action<RequestData> OnRequestAdded;

	public event Action<RequestData> OnRequestRemoved;

	public event Action<RequestData> OnRequestExpired;
	public event Action<RequestData, RequestData.Outcome> OnRequestPassed;

	[System.Serializable]
	public class ActiveRequestData
	{
		public RequestData request;

		public float timeLeft;
	}

	float timeTillNextSpawn;
	private void Awake()
	{
		Instance = this;

		timeTillNextSpawn = startGameDelay;
	}

	private void Start()
	{
		SpellCaster.Instance.OnCastedSpell += OnSpellCasted;
	}

	public void SpawnRequest()
	{
		var pool = new List<RequestData>();
		pool.AddRange(requestPool.Where(x =>
			currentlyActiveRequests.All(y => y.request.desiredLocation != x.desiredLocation)
			&& GameManager.Instance.GetGoodPercent(x.desiredLocation).MeetsEquation(x.percentGoodEquation, x.percentGoodNeeded)
			&& HasDemons(x.desiredLocation)
		));

		if (pool.Count > 0)
		{
			var rand = pool.GetRandom();

			ActiveRequestData r = new ActiveRequestData()
			{
				request = rand,
				timeLeft = rand.timeToComplete
			};

			currentlyActiveRequests.Add(r);
			OnRequestAdded?.Invoke(rand);

			if (requestPopupTime.y > requestPopupTime.x && UnityEngine.Random.value > 0.5f)
				requestPopupTime.y -= 1;
			else
				requestPopupTime.x -= 1;
		}

		timeTillNextSpawn = UnityEngine.Random.Range(requestPopupTime.x, requestPopupTime.y);
	}

	private void Update()
	{
		if (GameManager.Instance.GameStarted && !GameManager.Instance.GameFinished)
		{
			timeTillNextSpawn -= Time.deltaTime;

			if (timeTillNextSpawn <= 0)
				SpawnRequest();
		}

		if (GameManager.Instance.GameFinished) return;

		var cleanup = new List<ActiveRequestData>();
		foreach (var r in currentlyActiveRequests)
		{
			r.timeLeft -= Time.deltaTime;

			if (r.timeLeft <= 0)
			{
				cleanup.Add(r);
			}
		}

		foreach (var c in cleanup)
		{
			currentlyActiveRequests.Remove(c);

			OnRequestExpired?.Invoke(c.request);
			OnRequestRemoved?.Invoke(c.request);
		}
	}

	void OnSpellCasted(LocationList location, SpellData spell)
	{
		ActiveRequestData cleanup = null;
		RequestData.Outcome triggeredOutcome = null;

		foreach (var r in currentlyActiveRequests)
		{
			if (r.request.desiredLocation == location)
			{
				var outcome = r.request.outcomes.Find(o => o.spell == spell);
				if (outcome != null)
				{
					cleanup = r;
					triggeredOutcome = outcome;
					break;
				}
			}
		}

		if (cleanup != null)
		{
			currentlyActiveRequests.Remove(cleanup);
			OnRequestPassed?.Invoke(cleanup.request, triggeredOutcome);
			OnRequestRemoved?.Invoke(cleanup.request);
		}
	}

	bool HasDemons(LocationList location)
	{
		GameManager.Instance.GetUnits(location, out int _, out int bad);
		return bad > 0;
	}

#if UNITY_EDITOR
	[Button]
	void DevAutofillRequestPool()
	{
		requestPool = new List<RequestData>();
		var guids = UnityEditor.AssetDatabase.FindAssets("t:RequestData");
		foreach (var guid in guids)
		{
			var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<RequestData>(path);
			if (asset != null) requestPool.Add(asset);
		}
		UnityEditor.EditorUtility.SetDirty(this);
	}
#endif
}
