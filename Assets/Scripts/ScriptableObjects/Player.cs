using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player 1", menuName = "ScriptableObjects/Player", order = 1)]
public class Player : ScriptableObject
{
    public float HP;
    public float Strength;
    public float Speed;
    public float Agility;

    public Color bodyColor;

    public Weapon[] weapons;
}
