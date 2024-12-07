using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int heatlh;

    internal void TakeDamage(int damageToInflict)
    {
        heatlh -= damageToInflict;
    }
}
