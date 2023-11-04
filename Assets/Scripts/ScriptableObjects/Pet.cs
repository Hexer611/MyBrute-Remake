using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pet 1", menuName = "ScriptableObjects/Pet", order = 3)]
public class Pet : ScriptableObject
{
    public string petName;
    public BruteBase petPrefab;
}
