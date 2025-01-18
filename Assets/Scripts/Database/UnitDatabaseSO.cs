using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitDatabaseSO : ScriptableObject
{
    public List<UnitData> units;
    
}

[System.Serializable]
public class UnitData
{
    [field: SerializeField]
    public int ID;
    [field: SerializeField]
    public string Name { get;  set; }

    [field: SerializeField]

    [TextArea(3, 10)]
    public string description;

    [SerializeField]
    public int maxHealth;
    [SerializeField]
    public int unitDamage;
    [SerializeField]
    public int speedUnit;
    [SerializeField]
    public float attackingDistance;
    [SerializeField]
    public float stopAttackingDistance;

}
