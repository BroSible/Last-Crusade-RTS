using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour
{
    private float interval = 1f;

    void Start()
    {
        InvokeRepeating(nameof(GetGold), interval, interval);
    }

    public void GetGold()
    {
        ResourceManager.Instance.IncreaseResource(ResourceManager.ResourcesType.Gold, 1);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(GetGold));
    }
}