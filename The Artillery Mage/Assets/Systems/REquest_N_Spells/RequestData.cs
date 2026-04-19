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
    public string personRequesting;
    [TextArea(3,10)]
    public string requestDetails;
    public float timeToComplete;
    
    [Header("Reqs")]
    public LocationList desiredLocation;

    public MathEquation percentGoodEquation;
    [Range(0,1f)]
    public float percentGoodNeeded;


    [Header("Outcomes")]
    public List<Outcome> outcomes = new List<Outcome>();

    [System.Serializable]
    public class Outcome
    {
        public SpellData spell;
        public int goodUnitsAlter;
        public int badUnitsAlter;
    }
}
