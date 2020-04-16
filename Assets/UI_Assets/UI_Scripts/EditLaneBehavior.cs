﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditLaneBehavior : MonoBehaviour, ISceneUIMenu
{
    private const float BASE_CHANGE_FT = 0.5f;
    public Text widthText;

    public GameObject workingLaneReference; // consider removing this variable?
    private BasicLane basicLaneScriptReference;

    //private Lane workingLane;

    public void init(GameObject laneRef)
    {
        this.workingLaneReference = laneRef;
        this.basicLaneScriptReference = workingLaneReference.GetComponent<BasicLane>();
        if(workingLaneReference == null || basicLaneScriptReference == null)
        {
            Debug.Log("Tried to set working reference, but failed.");
        }
        updateWidthField();

        resolveButtonActivationStates();
    }

    private void resolveButtonActivationStates()
    {
        Road rd = GameObject.Find("Road").GetComponent<Road>();
        gameObject.transform.Find("Delete").GetComponent<Button>().interactable = !rd.isAtMinSize();
    }

    // Provides a check that we have a lane to reference before proceding
    private bool requireWorkingLaneReference()
    {
        if (workingLaneReference != null)
        {
            return true;
        } else
        {
            Debug.LogError("Function requires a lane reference, but does not have one.");
            return false;
        }
    }

    // TODO
    public void handleSwitchDirectionToggle()
    {
        Debug.Log("Lane direction switched");
    }

    // Nathan implemented this
    // should properly call removeLane and destroy this menu
    public void handleDeleteSelect()
    {
        Debug.Log("Delete button selected.");
        removeLane();
    }

    // Nathan partially completed this
    public void handleLaneTypeChange()
    {
        Debug.Log("Lane type change selected. *There is something weird with the height of shoulders!");
        // we will need to change the line below to something more substantial
        // once we get more lane types involved - maybe create a helper function to handle this
        GameObject.Find("Road").GetComponent<Road>().setLaneType(workingLaneReference, "BusLane");
    }

    // Kasey wrote this
    // increases lane width
    public void handleIncreaseLaneWidth()
    {
        requireWorkingLaneReference();

        float width = basicLaneScriptReference.getLaneWidth();
        width += UnitConverter.convertFeetToMeters(BASE_CHANGE_FT);

        if (width <= basicLaneScriptReference.getMaxWidth())
        {
            basicLaneScriptReference.setLaneWidth(width);
            updateWidthField();
            Debug.Log("Lane width increased to: " + UnitConverter.convertMetersToFeet(width).ToString() + "ft.");
        }
        else
        {
            Debug.Log("Tried to increment width, but maximum width reached.");
        }
    }

    // Kasey wrote this
    // decreases lane width
    public void handleDecreaseLaneWidth()
    {
        requireWorkingLaneReference();

        float width = basicLaneScriptReference.getLaneWidth();
        width -= UnitConverter.convertFeetToMeters(BASE_CHANGE_FT);

        if (basicLaneScriptReference.getMinWidth() <= width)
        {
            basicLaneScriptReference.setLaneWidth(width);
            updateWidthField();
            Debug.Log("Lane width decreased to: " + UnitConverter.convertMetersToFeet(width).ToString() + "ft.");
        }
        else
        {
            Debug.Log("Tried to decrement width, but minimum width reached.");
        }
    }

    private void updateWidthField()
    {
        // TODO: Trim the decimal places
        float laneWidth = basicLaneScriptReference.getLaneWidth();
        double laneWidthFeet = UnitConverter.convertMetersToFeet(laneWidth);
        widthText.text = laneWidthFeet.ToString("0.0") + "ft";
    }

    public void closeUI()
    {
        Destroy(this.gameObject);
    }

    // Nathan wrote this
    // should remove the lane referenced by this menu
    public void removeLane() 
    {
        GameObject.Find("Road").GetComponent<Road>().removeLane(workingLaneReference);
        closeUI();
    }

}
