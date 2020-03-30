﻿// BasicLane.cs
// parent class of all lane types
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLane : MonoBehaviour
{
    // class fields
    [SerializeField] private const float DEFAULT_LANE_WIDTH_FT = 12.0f;
    //[SerializeField] protected GameObject laneEditPrefab;
    //[SerializeField] protected GameObject editLaneDialogue;
    //[SerializeField] protected GameObject insertButton;
    [SerializeField] protected GameObject asphalt;
    //[SerializeField] protected Vector3 lanePosition;
    [SerializeField] protected int laneIndex;
    [SerializeField] protected string laneType;
    [SerializeField] protected float currentLaneWidth;
    [SerializeField] protected float maxWidth;
    [SerializeField] protected float minWidth;
    [SerializeField] protected GameObject leftStripe;
    [SerializeField] protected GameObject rightStripe;
    [SerializeField] protected bool vehicleLane;
    [SerializeField] protected bool nonVehicleAsphalt;
    [SerializeField] protected bool nonAsphalt;

    protected GameObject road;
    protected Road roadScript;

    // Nathan inserted start so we could use road functions more easily
    void Start()
    {
        //road = GameObject.Find("Road");
        //roadScript = (Road)road.GetComponent("Road");

        // this was causing a bug. not sure why
        //setLaneWidth(UnitConverter.convertFeetToMeters(DEFAULT_LANE_WIDTH_FT));

        road = GameObject.Find("Road");
        roadScript = (Road)road.GetComponent("Road");
    }

    // setLaneWidth() sets the width of a lane
    // new_width is a floating point number used to create
    // the new width of the lane
    public void setLaneWidth(float newWidth)
    {
        // basically just moved Luke's code from Road.cs
        // Steps:
        //       1. retrieve the transforms of the lane components and
        //          store them in temporary Vector3 objects
        //       2. create a positional adjustment for the lines and the 
        //          insertion button as well as surrounding lanes
        //       3. adjust the positions of the surrounding lanes
        //       4. adjust the temporary vectors accordingly
        //       5. update the transforms with the new Vector3 values
        // step 1
       
        
        
        Vector3 laneSize = asphalt.transform.localScale;

        //Vector3 buttonPos = insertButton.transform.localPosition;
        // step 2
        float adjustment = (newWidth - laneSize.z) / 2;
        // step 3
        //GameObject road = GameObject.Find("Road");
        // reference script that controls the road's behavior
        //Road roadScript = (Road)road.GetComponent("Road");
        // adjust the lane positions around the lane we are modifying
        roadScript.adjustRoadAroundLane(gameObject, adjustment);
        // step 4
        laneSize.z = newWidth;
        //buttonPos.z += adjustment;
        // step 5
        GetComponent<PropManager>().updateRelationalValues();
        asphalt.transform.localScale = laneSize;
        GetComponent<PropManager>().repositionProps();
        
        Renderer asphaltRenderer = asphalt.GetComponent<Renderer>();
        asphaltRenderer.material.SetTextureScale("_MainTex", new Vector2(100, newWidth));
        //insertButton.transform.localPosition = buttonPos;
        currentLaneWidth = asphalt.transform.localScale.z;

        // set new stripe locations
        adjustStripePositions();
    }

    // Nathan wrote this
    // retrieves the current lane width
    public float getLaneWidth()
    {
        return currentLaneWidth;
    }

    // Nathan wrote this
    // returns the lane's maximum width
    public float getMaxWidth()
    {
        return maxWidth;
    }

    // Nathan wrote this
    // returns the lane's minimum width
    public float getMinWidth()
    {
        Debug.Log("Min Width is " + minWidth.ToString() + ".");
        return minWidth;
    }

    // setLanePosition() shifts a lane along the road
    // adjustment is the amount by which the lane's position
    // is to be adjusted
    public void setLanePosition(float adjustment)
    {
        // create a temp vec, store value of lane's position transform in it,
        // adjust the temp vec's z value by the adjustment, store temp vec
        // in the lane's position transform
        Vector3 tempVec = gameObject.transform.localPosition;
        tempVec.z += adjustment;
        this.gameObject.transform.localPosition = tempVec;
        //lanePosition = gameObject.transform.localPosition;

        // set new stripe locations
        adjustStripePositions();  
    }

    // Nathan wrote this
    // retrieves the lane's current position
    public Vector3 getLanePosition()
    {
        //Debug.Log(gameObject.transform.localPosition);
        //Debug.Log(lanePosition);
        //Debug.Assert(lanePosition == gameObject.transform.localPosition);
        return gameObject.transform.localPosition;
    }

    // Nathan wrote this
    // changes the lane index
    public void setLaneIndex(int newIndex)
    {
        laneIndex = newIndex;
    }

    // Nathan wrote this
    // retrieves the lane index
    public int getLaneIndex()
    {
        return laneIndex;
    }

    // Nathan wrote this
    // sets the lane's current type
    public void setLaneType(string newTypeName)
    {
        laneType = newTypeName;
    }

    // Nathan wrote this
    // retrieves lane's current type
    public string getLaneType()
    {
        return laneType;
    }

    // Nathan wrote this
    // sets a stripe's orientation to the lane
    // parameter stripe is the stripe which we are
    // setting the orientation of
    // parameter stripeOrientation is its
    // new orientation
    public void setStripeOrientation(GameObject stripe, string stripeOrientation)
    {
        Stripe stripeScriptReference = null;
        // first of all, check for null
        // if the stripe object is not null, set the stripes accordingly
        if (stripe != null)
        {
            stripeScriptReference = (Stripe)stripe.GetComponent("Stripe");
            if (stripeOrientation == "left")
            {
                // the stripe is now this lane's "left" stripe
                leftStripe = stripe;
                // this lane is now the stripe's "right" lane
                stripeScriptReference.setLaneOrientation(this.gameObject, "right");
                stripeScriptReference.setStripePosition(gameObject.transform.localPosition, -currentLaneWidth / 2);
                leftStripe.transform.parent = gameObject.transform;
            }
            else if (stripeOrientation == "right")
            {
                // the stripe is now this lane's "right" stripe
                rightStripe = stripe;
                // this lane is now the stripe's "left" lane
                stripeScriptReference.setLaneOrientation(this.gameObject, "left");
                stripeScriptReference.setStripePosition(gameObject.transform.localPosition, currentLaneWidth / 2);
                rightStripe.transform.parent = gameObject.transform;
            }
            // error case
            else
            {
                Debug.Log("NOT A VALID STRIPE ORIENTATION");
                Debug.Assert(false);
            }
        }
        // if the stripe is null, then do one of the following:
        else
        {
            // if left stripe is specified, make it null
            if (stripeOrientation == "left")
            {
                leftStripe = null;
            }
            // if right stripe is specified, make it null
            else if (stripeOrientation == "right")
            {
                rightStripe = null;
            }
            // otherwise, make them both null
            else
            {
                leftStripe = null;
                rightStripe = null;
            }
        }
    }

    // Nathan wrote this
    // retrieves the specified stripe
    public GameObject getStripe(string stripe)
    {
        if (stripe == "left")
        {
            return leftStripe;
        }
        else if (stripe == "right")
        {
            return rightStripe;
        }
        else
        {
            throw new System.ArgumentException("Stripe value unknown. Did you mean 'left' or 'right?'");
        }
    }

    // Nathan wrote this
    // determines if the current lane is a vehicle lane (is not by default)
    public bool isVehicleLane()
    {
        return vehicleLane;
    }

    // Nathan wrote this
    // determines if the current lane is a non-vehicle lane with asphalt
    // these include shoulders and parking lanes
    public bool isNonVehicleAsphaltLane() 
    {
        return nonVehicleAsphalt;
    }

    // Nathan wrote this
    // determines if the current lane is a non-asphalt type lane
    // this includes sidewalks, curbs, gutters, medians, and grass divisions
    public bool isNonAsphaltLane() 
    {
        return nonAsphalt;
    }

    // Nathan wrote this
    // adjusts the positions of stripes
    private void adjustStripePositions()
    {
        if (this.leftStripe != null) 
        {
            this.leftStripe.GetComponent<Stripe>().setStripePosition(gameObject.transform.localPosition, -currentLaneWidth / 2);
        }
        if (this.rightStripe != null)
        {
            this.rightStripe.GetComponent<Stripe>().setStripePosition(gameObject.transform.localPosition, currentLaneWidth / 2);
        }
    }
   
}

