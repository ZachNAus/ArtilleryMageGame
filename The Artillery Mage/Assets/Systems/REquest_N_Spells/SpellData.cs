using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "Spell")]
public class SpellData : ScriptableObject
{
	public new string name = "";

	public List<KeyCode> inputs = new List<KeyCode>();

	[Header("FX")]
	public float particleAliveTime;

	public List<AudioClip> audio;
}
