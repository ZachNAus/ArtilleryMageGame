using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] lockMovingIfActive;

	public float CurrentMana { get; private set; } = 100;

	[SerializeField] float manaDrainPerSecond = 0.5f;

	[SerializeField] float demonGrowthTimer = 10;
	float timeTillDemonGrowth;

	[SerializeField] float goodGrowthTimer = 20f;
	float timeTillGoodGrowth;

	public bool GameStarted { get; private set; }
	public bool GameFinished { get; private set; }

	[SerializeField] UnityEvent onWin;
	[SerializeField] UnityEvent onLose;

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
		public LocationList whereDoGoodiesExpand;

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
		timeTillGoodGrowth = goodGrowthTimer;

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
		foreach (var effect in outcome.effects)
		{
			AlterUnits(effect.locationToAltarUnits, true, effect.goodUnitsAlter);
			AlterUnits(effect.locationToAltarUnits, false, effect.badUnitsAlter);
		}
		CheckWinLose();
	}

	private void Update()
	{
		if (!GameFinished)
			CurrentMana -= manaDrainPerSecond * Time.deltaTime;

		if (GameStarted && !GameFinished)
		{
			timeTillDemonGrowth -= Time.deltaTime;
			timeTillGoodGrowth -= Time.deltaTime;

			if (timeTillDemonGrowth <= 0)
			{
				ExpandDemons();
			}

			if (timeTillGoodGrowth <= 0)
			{
				timeTillGoodGrowth = goodGrowthTimer;
				ExpandGoodForces();
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

		CheckWinLose();
	}

	void ExpandGoodForces()
	{
		foreach (var kvp in activeLocations)
		{
			ActiveLocationInfo loc = kvp.Value;
			LocationInfo info = GetLocationInfo(kvp.Key);
			if (info == null) continue;

			if (loc.PercentGood >= 0.8f && loc.goodUnits < info.maxBadUnits)
				loc.goodUnits++;
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

	public void GetUnits(LocationList location, out int good, out int bad)
	{
		good = bad = 0;
		if (!activeLocations.ContainsKey(location)) return;
		good = activeLocations[location].goodUnits;
		bad = activeLocations[location].badUnits;
	}

	public int GetMaxBadUnits(LocationList location)
	{
		var info = GetLocationInfo(location);
		return info != null ? info.maxBadUnits : 10;
	}

	[Button]
	public void StartGame()
	{
		GameStarted = true;
	}

	void CheckWinLose()
	{
		if (GameFinished) return;

		if (activeLocations.ContainsKey(LocationList.Castle) &&
			activeLocations[LocationList.Castle].PercentGood < 0.2f)
		{
			LoseGame();
			return;
		}

		if (activeLocations.ContainsKey(LocationList.DemonGate) &&
			activeLocations[LocationList.DemonGate].PercentGood >= 0.8f)
		{
			WinGame();
		}
	}

	void WinGame()
	{
		GameFinished = true;
		onWin.Invoke();
	}

	void LoseGame()
	{
		GameFinished = true;
		onLose.Invoke();
	}

	public void RestartGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(
			UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
	}
}
