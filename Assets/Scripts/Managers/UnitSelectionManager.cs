using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set;}

    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    private UnitMovement unitMovement;

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask attackable;
    public bool attackCursorVisible;
    public GameObject groundMarker;

    private Camera camera;

    PhotonView view;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        camera = Camera.main;
        unitMovement = GetComponent<UnitMovement>();
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(view.IsMine)
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
                {
                    // multi select unit by left shift
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        MuiltiSelect(hit.collider.gameObject);
                    }

                    // just select unit by one click
                    else
                    {
                        SelectByClicking(hit.collider.gameObject);
                    }
                }
                
                // deselect unit, if you don't press left shift and don't click on unit
                else
                {
                    if(Input.GetKey(KeyCode.LeftShift) == false)
                    {
                        DeselectAll();
                    }
                }
            }


            // Marker
            if(Input.GetMouseButtonDown(1) && unitsSelected.Count > 0)
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    groundMarker.transform.position = hit.point;

                    groundMarker.SetActive(false);
                    groundMarker.SetActive(true);
                }
            }

            // Attack Target

            if(unitsSelected.Count > 0 && AtleastOneOffensiveUnit(unitsSelected))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hit, Mathf.Infinity, attackable))
                {
                    Debug.Log("Enemy hovered with mouse");

                    attackCursorVisible = true;
                    
                    if(Input.GetMouseButtonDown(1))
                    {
                        Transform target = hit.transform;

                        foreach(GameObject unit in unitsSelected)
                        {
                            if(unit.GetComponent<AttackController>())
                            {
                                unit.GetComponent<AttackController>().targetToAttack = target;
                            }
                        }
                    }
                }
            }

            else
            {
                attackCursorVisible = true;
            }

            CursorSelector();
        }
        
    }

    private void CursorSelector()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, Mathf.Infinity, clickable))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Selectable);
        }

        else if(Physics.Raycast(ray,out hit, Mathf.Infinity, attackable) && unitsSelected.Count > 0 && AtleastOneOffensiveUnit(unitsSelected))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Attackable);
        }

        else if(Physics.Raycast(ray,out hit, Mathf.Infinity, ground) && unitsSelected.Count > 0)
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Walkable);
        }

        else
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.None);
        }

    }

    private bool AtleastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
        foreach(GameObject unit in unitsSelected)
        {
            if(unit.GetComponent<AttackController>())
            {
                return true;
            }
        }
        return false;
    }

    private void MuiltiSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit,true);
        }

        else
        {
            unitsSelected.Remove(unit);
            SelectUnit(unit,false);
        }
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        unitsSelected.Add(unit);
        SelectUnit(unit,true);
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerSelectionIndicator(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit,false);
        }

        groundMarker.SetActive(false);
        unitsSelected.Clear();
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.Find("Indicator").gameObject.SetActive(isVisible);
    }

    internal void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit,true);
        }
    }
}
