using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Unit Data", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public float health;
    public float speed;
    public float damage;
    public int range;
}
