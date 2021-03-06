﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VehicleLanePropIndicator : MonoBehaviour
{
    protected GameObject currentPropPrefab;
    protected GameObject currentProp = null;

    protected GameObject asphalt;

    protected VRTK_InteractableObject linkedObject;

    protected Transform cursorTransform = null;

    private bool trackCursor = false;

    private void Awake()
    {
        asphalt = transform.Find("PrimaryAsphalt").gameObject;

        linkedObject = GetComponentInChildren<VRTK_InteractableObject>();
    }

    private void Update()
    {
        if (trackCursor == true)
        {
            if (currentProp != null)
            {
                currentProp.transform.position = cursorTransform.position - currentProp.GetComponent<Prop>().getCenterObjectOffset();
            }
            else
            {
                currentProp = Instantiate(currentPropPrefab, cursorTransform.position, Quaternion.identity);
                currentProp.GetComponent<Collider>().enabled = false;
                if (!currentProp.name.Equals("Empty(Clone)"))
                {
                    currentProp.GetComponent<VRTK_InteractObjectHighlighter>().Highlight(Color.red);
                }
            }
        }
        else
        {
            if (currentProp != null)
            {
                Destroy(currentProp);
            }
        }
    }

    protected virtual void OnEnable()
    {
        linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

        if (linkedObject != null)
        {
            linkedObject.InteractableObjectTouched += InteractableObjectTouched;
            linkedObject.InteractableObjectUntouched += InteractableObjectUntouched;
        }

    }

    protected virtual void OnDisable()
    {
        if (linkedObject != null)
        {
            linkedObject.InteractableObjectTouched -= InteractableObjectTouched;
            linkedObject.InteractableObjectUntouched -= InteractableObjectUntouched;
        }
    }

    protected virtual void InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
    {
        //Debug.Log("InteractableObjectTouched");
        // write touch script here

        // sets emission color to gray
        asphalt.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0.38f, 0.38f, 0.38f, 0f));

        trackCursor = true;
        cursorTransform = getCursor(sender, e).transform;
        currentPropPrefab = CurrentPropManager.Instance.getCurrentPropObj();

    }

    protected virtual void InteractableObjectUntouched(object sender, InteractableObjectEventArgs e)
    {
        //Debug.Log("InteractableObjectUntouched");
        // write un-touch script here

        // sets emission color to gray
        asphalt.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));

        trackCursor = false;
        cursorTransform = null;// getCursor(sender, e).transform;
        currentPropPrefab = null;

    }

    // this method returns the cursor that is touching the current object
    private GameObject getCursor(object sender, InteractableObjectEventArgs e)
    {
        //Debug.Log("getCursor");

        //Debug.Log(sender.ToString());
        GameObject controller = e.interactingObject;
        //Debug.Log("interacting object:::    " + controller.GetType());
        Component[] controllerComponents = controller.GetComponents<Component>();
        //Debug.Log("controller components ::::    " + controllerComponents.ToString());

        VRTK_StraightPointerRenderer pointerRenderer = null;

        foreach (Component c in controllerComponents)
        {
            //Debug.Log("components ::::    " + c.ToString());
            if (c.GetType() == typeof(VRTK_StraightPointerRenderer))
            {
                pointerRenderer = (VRTK_StraightPointerRenderer)c;
            }
        }

        if (pointerRenderer == null)
        {
            Debug.Log("Pointer renderer not found");
        }

        //GameObject cursor = pointerRenderer.getCursor();
        GameObject cursor = pointerRenderer.GetPointerObjects()[1];
        //Debug.Log("cursor transform local position:::: " + cursor.transform.position);

        if (cursor != null)
        {
            //Debug.Log("cursor.transform.position: " + cursor.transform.position);
            return cursor;
        }
        else
        {
            Debug.Log("cursor not found");
            return null;
        }
    }
}
