using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] List<SpellData> spellPool = new List<SpellData>();
    [SerializeField] LocationDict locations = new LocationDict();
    [SerializeField] LocationDisplayMap locationDisplays = new LocationDisplayMap();

    [SerializeField] MovementSystem movement;

    [System.Serializable]
    class LocationDict : SerializableDictionary<LocationList, Transform> { }
    [System.Serializable]
    class LocationDisplayMap : SerializableDictionary<LocationList, LocationWorldDisplay> { }

    public static SpellCaster Instance { get; private set; }

    [ReadOnly]
    public List<KeyCode> activelyCasting = new List<KeyCode>();

    public event Action<KeyCode> OnInputAdded;
    public event Action OnCastingCleared;
    public event Action<LocationList, SpellData> OnCastedSpell;
    public event Action<SpellData, float> OnCastDelayStarted;

    static readonly Dictionary<KeyCode, KeyCode> keyAliases = new Dictionary<KeyCode, KeyCode>
    {
        { KeyCode.W, KeyCode.UpArrow },
        { KeyCode.A, KeyCode.LeftArrow },
        { KeyCode.S, KeyCode.DownArrow },
        { KeyCode.D, KeyCode.RightArrow },
    };

    HashSet<KeyCode> watchedKeys = new HashSet<KeyCode>();
    float timeSinceLastInput = 0f;
    const float castingTimeout = 2f;
    [SerializeField] float castDelay = 1f;
    bool isCastingDelay = false;
    public bool IsCastingDelay => isCastingDelay;

    void Awake()
    {
        Instance = this;

        MovementSystem.OnArrived += x => ClearCasting();
    }

    void Start()
    {
        foreach (SpellData spell in spellPool)
            foreach (KeyCode key in spell.inputs)
			{
                if(watchedKeys.Contains(key) == false)
                    watchedKeys.Add(key);
			}
    }

    public void Update()
    {
        if (movement.CurrentLocation.canCast && !movement.Moving && !isCastingDelay)
        {
            KeyCode? pressed = null;
            foreach (KeyCode key in watchedKeys)
            {
                if (Input.GetKeyDown(key)) { pressed = key; break; }
            }
            if (pressed == null)
            {
                foreach (var alias in keyAliases)
                {
                    if (Input.GetKeyDown(alias.Key) && watchedKeys.Contains(alias.Value))
                    {
                        pressed = alias.Value;
                        break;
                    }
                }
            }
            if (pressed.HasValue)
            {
                activelyCasting.Add(pressed.Value);
                timeSinceLastInput = 0f;
                OnInputAdded?.Invoke(pressed.Value);
                CheckSpells();
            }

            if (activelyCasting.Count > 0)
            {
                if (timeSinceLastInput >= castingTimeout)
                    ClearCasting();
                else
                    timeSinceLastInput += Time.deltaTime;
            }
        }
    }

    void CheckSpells()
    {
        foreach (SpellData spell in spellPool)
        {
            if (activelyCasting.SequenceEqual(spell.inputs))
            {
                ClearCasting();
                isCastingDelay = true;
                OnCastDelayStarted?.Invoke(spell, castDelay);
                StartCoroutine(Co_CastDelay(spell));
                return;
            }
        }

        bool anyPrefix = spellPool.Any(spell => IsPrefix(activelyCasting, spell.inputs));
        if (!anyPrefix)
            ClearCasting();
    }

    bool IsPrefix(List<KeyCode> candidate, List<KeyCode> full)
        => candidate.Count <= full.Count
        && candidate.SequenceEqual(full.Take(candidate.Count));

    void ClearCasting()
    {
        StopAllCoroutines();
        isCastingDelay = false;
        activelyCasting.Clear();
        timeSinceLastInput = 0f;
        OnCastingCleared?.Invoke();
    }

    LocationList GetClosestLookedAtLocation()
    {
        LocationList closest = default;
        float smallestAngle = float.MaxValue;

        foreach (var kvp in locations)
        {
            Vector3 dirToLocation = (kvp.Value.position - movement.transform.position).normalized;
            float angle = Vector3.Angle(movement.transform.forward, dirToLocation);
            if (angle < smallestAngle)
            {
                smallestAngle = angle;
                closest = kvp.Key;
            }
        }

        return closest;
    }

    void CastSpell(SpellData spell)
    {
        LocationList location = GetClosestLookedAtLocation();

        if (locationDisplays.TryGetValue(location, out LocationWorldDisplay display))
            display.PlaySpellVFX(spell, spell.particleAliveTime);

        OnCastedSpell?.Invoke(location, spell);
    }

    IEnumerator Co_CastDelay(SpellData spell)
    {
        yield return new WaitForSeconds(castDelay);
        isCastingDelay = false;
        CastSpell(spell);
    }

#if UNITY_EDITOR
    [Button]
    void DevAutofillSpellPool()
    {
        spellPool.Clear();
        var guids = UnityEditor.AssetDatabase.FindAssets("t:SpellData");
        foreach (var guid in guids)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<SpellData>(path);
            if (asset != null) spellPool.Add(asset);
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
