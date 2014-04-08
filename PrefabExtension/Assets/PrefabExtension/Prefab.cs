using UnityEngine;
using System.Collections;

[System.Serializable]
public class Prefab
{
	public GameObject gameObject { get { return _gameObject; } }
	// This field is assigned from Editor.
	[SerializeField]
	GameObject _gameObject;
}
