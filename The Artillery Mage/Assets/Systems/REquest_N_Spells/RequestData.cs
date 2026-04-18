using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationList
{
    CropsArea = 0,
    Castle = 1,
    Village = 2,
    Forest = 3,
    DemonGate = 4,
    Volcano = 5,
    ETC = 6,
    ETCETC = 7
}

[CreateAssetMenu(menuName = "Request")]
public class RequestData : ScriptableObject
{
    public new string name = "";

    public string personRequesting;
    [TextArea(3,10)]
    public string requestDetails;
    public float timeToComplete;
    [Space]

    public LocationList desiredLocation;

    public List<SpellData> spellsThatWork = new List<SpellData>();
    public List<SpellData> instantFailSpells = new List<SpellData>();
}
