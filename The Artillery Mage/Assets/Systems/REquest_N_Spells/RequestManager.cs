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
	public event Action<RequestData> OnRequestFailed;
	public event Action<RequestData> OnRequestPassed;

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
		));

		var rand = pool.GetRandom();

		ActiveRequestData r = new ActiveRequestData()
		{
			request = rand,
			timeLeft = rand.timeToComplete
		};

		currentlyActiveRequests.Add(r);
		OnRequestAdded?.Invoke(rand);

		timeTillNextSpawn = UnityEngine.Random.Range(requestPopupTime.x, requestPopupTime.y);
	}

	private void Update()
	{
		timeTillNextSpawn -= Time.deltaTime;

		if (timeTillNextSpawn <= 0)
			SpawnRequest();

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
		bool fuckedUp = false;

		foreach (var r in currentlyActiveRequests)
		{
			if(r.request.desiredLocation == location)
			{
				if (r.request.spellsThatWork.Contains(spell))
				{
					cleanup = r;
					break;
				}
				else if (r.request.instantFailSpells.Contains(spell))
				{
					cleanup = r;
					fuckedUp = true;
					break;
				}
			}
		}

		if (cleanup != null)
		{
			currentlyActiveRequests.Remove(cleanup);
			if (fuckedUp)
				OnRequestFailed?.Invoke(cleanup.request);
			else
				OnRequestPassed?.Invoke(cleanup.request);
		
			OnRequestRemoved?.Invoke(cleanup.request);
		}
	}
}
