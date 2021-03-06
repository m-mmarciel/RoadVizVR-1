﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropSpawnBehavior : MonoBehaviour, ISceneUIMenu
{

    public void init(params GameObject[] objRefs)
    {
        // catches the instance when we move a prop and then open Prop Spawn menu. This should be caught by the ActionMenu code
        if (CurrentPropManager.Instance.getPropBeingMoved() == true)
        {
            CurrentPropManager.Instance.revertMovedProp();
        }

        CurrentPropManager.Instance.setCurrentPropObj(CurrentPropManager.Instance.getPropNames().ToArray()[0]);
        ModifyController.Instance.setAddingProps(true);

        CurrentPropManager.Instance.setRotation(0);

        List<string> propNames = GameObject.Find("CurrentPropTracker").GetComponent<CurrentPropManager>().getPropNames();
        Dropdown dd = gameObject.transform.Find("PropSelectControls/PropSelect").GetComponent<Dropdown>();
        // add lane types to dropdown, then set current active
        dd.AddOptions(propNames);

        StartCoroutine(CurrentPropManager.Instance.clearErrantPropObjects());
    }

    public void handleRotateCW()
    {
        CurrentPropManager.Instance.rotateCW();
    }

    public void handleRotateCCW()
    {
        CurrentPropManager.Instance.rotateCCW();
    }

    public void handlePropSelectChange()
    {
        List<string> propNames = GameObject.Find("CurrentPropTracker").GetComponent<CurrentPropManager>().getPropNames();
        int currentPropIndex = gameObject.transform.Find("PropSelectControls/PropSelect").GetComponent<Dropdown>().value;

        CurrentPropManager.Instance.setCurrentPropObj(propNames.ToArray()[currentPropIndex]);
    }

    public void handleClose()
    {
        ModifyController.Instance.setAddingProps(false);
        UIManager.Instance.closeCurrentUI();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
