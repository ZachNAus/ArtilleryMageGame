using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell")]
public class SpellData : ScriptableObject
{
	public new string name = "";

	public List<KeyCode> inputs = new List<KeyCode>();

	public GameObject visualEffects;
}
