using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] lockMovingIfActive;

	public float CurrentMana { get; private set; } = 100;

	[SerializeField] float manaDrainPerSecond = 0.5f;

	[SerializeField] float demonGrowthTimer = 10;
	float timeTillDemonGrowth;

	public bool GameStarted { get; private set; }

	[SerializeField]
	[ReadOnly]
	ActiveLocations activeLocations = new ActiveLocations();

	[SerializeField]
	List<LocationInfo> locationInfo = new();

	[System.Serializable]
	public class ActiveLocations : SerializableDictionary<LocationList, ActiveLocationInfo> { }

	[System.Serializable]
	public class ActiveLocationInfo
	{
		public int goodUnits;
		public int badUnits;

		public float PercentGood
		{
			get
			{
				int total = goodUnits + badUnits;
				return total == 0 ? 0f : (float)goodUnits / total;
			}
		}
	}

	[System.Serializable]
	public class LocationInfo
	{
		public LocationList location;
		public int maxBadUnits;
		public LocationList whereDoDemonsExpand;

		public int startingGoodUnits;
		public int startingBadUnits;
	}

	LocationInfo GetLocationInfo(LocationList loc) =>
		locationInfo.Find(x => x.location == loc);


	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		timeTillDemonGrowth = demonGrowthTimer;

		foreach (var info in locationInfo)
		{
			activeLocations[info.location] = new ActiveLocationInfo
			{
				goodUnits = info.startingGoodUnits,
				badUnits  = info.startingBadUnits
			};
		}

		RequestManager.Instance.OnRequestPassed += OnRequestPassed;
	}

	void OnRequestPassed(RequestData request, RequestData.Outcome outcome)
	{
		AlterUnits(outcome.locationToAltarUnits, true, outcome.goodUnitsAlter);
		AlterUnits(outcome.locationToAltarUnits, false, outcome.badUnitsAlter);
	}

	private void Update()
	{
		CurrentMana -= manaDrainPerSecond * Time.deltaTime;

		if (GameStarted)
		{
			timeTillDemonGrowth -= Time.deltaTime;

			if (timeTillDemonGrowth <= 0)
			{
				ExpandDemons();
			}
		}
	}

	void ExpandDemons()
	{
		timeTillDemonGrowth = demonGrowthTimer;

		foreach (var kvp in activeLocations)
		{
			LocationList zone = kvp.Key;
			ActiveLocationInfo loc = kvp.Value;

			LocationInfo info = GetLocationInfo(zone);
			if (info == null) continue;

			if (loc.PercentGood < 0.5f)
			{
				if (loc.badUnits < info.maxBadUnits)
				{
					loc.badUnits++;
				}
				else if (loc.goodUnits > 0)
				{
					loc.goodUnits--;
				}

				if (loc.goodUnits > 0 && Random.value < 0.3f)
				{
					loc.goodUnits--;
				}
			}

			if (loc.PercentGood < 0.2f)
			{
				LocationList expandTarget = info.whereDoDemonsExpand;
				LocationInfo expandInfo = GetLocationInfo(expandTarget);
				if (activeLocations.ContainsKey(expandTarget) &&
					expandInfo != null &&
					activeLocations[expandTarget].badUnits < expandInfo.maxBadUnits)
				{
					activeLocations[expandTarget].badUnits++;
				}
			}
		}

		// Lose if Castle is >80% demon
		if (activeLocations.ContainsKey(LocationList.Castle) &&
			activeLocations[LocationList.Castle].PercentGood < 0.2f)
		{
			LoseGame();
		}
	}

	public void AlterUnits(LocationList location, bool goodUnits, int amount)
	{
		if (!activeLocations.ContainsKey(location)) return;

		if (goodUnits)
			activeLocations[location].goodUnits = Mathf.Max(0, activeLocations[location].goodUnits + amount);
		else
			activeLocations[location].badUnits = Mathf.Max(0, activeLocations[location].badUnits + amount);
	}

	public float GetGoodPercent(LocationList location)
	{
		return activeLocations[location].PercentGood;
	}

	[Button]
	public void StartGame()
	{
		GameStarted = true;
	}

	void LoseGame()
	{

	}
}
