﻿// BasicLane.cs
// parent class of all lane types
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicLane : MonoBehaviour
{
    // class fields
    [SerializeField] private const float DEFAULT_LANE_WIDTH_FT = 12.0f;
    [SerializeField] protected GameObject asphalt;
    [SerializeField] protected int laneIndex;
    [SerializeField] protected float currentLaneWidth;
    [SerializeField] protected float maxWidth;
    [SerializeField] protected float minWidth;
    [SerializeField] protected GameObject leftStripe;
    [SerializeField] protected GameObject rightStripe;
    [SerializeField] protected bool vehicleLane;
    [SerializeField] protected bool nonVehicleAsphalt;
    [SerializeField] protected bool nonAsphalt;

    Material asphaltMaterial;

    //The direction of the lane
    //0 means from positive to negative x,
    //1 means from negative to positive x
    [SerializeField] private int direction = 0; //Zero by default

    private void Awake()
    {
        asphaltMaterial = asphalt.GetComponent<Renderer>().material;
    }

    protected GameObject road;
    protected Road roadScript;

    void Start()
    {
        asphaltMaterial.SetTextureScale("_MainTex", new Vector2(100, asphalt.transform.localScale.z));
    }

    // Max wrote this
    // changes the direction of the lane's traffic 
    // (meaning the way in which a spawned vehicle will move), as well as the direction
    // (that signs point)
    // will also set the direction of the signs
    public void setDirection(int newDirection)
    {
        //If it is a valid direction specified for both variables
        if (newDirection == 1 || newDirection == 0)
        {
            //Update the directional variable
            direction = newDirection;

            //Update the sign, which are children, first by finding them, then by running its update.
            //They are named "sign", "sign 2", and "sign 3"
            //This code is a little ugly but I only need to use it for 3 objects, so is good enough.
            GameObject signReference = this.transform.Find("sign").gameObject;
            signReference.GetComponent<signDirection>().updateRotation();

            //Update sign 2
            signReference = this.transform.Find("sign 2").gameObject;
            signReference.GetComponent<signDirection>().updateRotation();

            //Update sign 3
            signReference = this.transform.Find("sign 3").gameObject;
            signReference.GetComponent<signDirection>().updateRotation();
        }

        //If the direction is invalid
        else
        {
            throw new System.ArgumentException("Error, bad direction.");
        }
    }

    // Nathan wrote this
    // retrieves the direction of the lane
    public int getDirection()
    {
        return direction;
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
        // step 2
        float adjustment = (newWidth - laneSize.z) / 2;
        // step 3
        GameObject road = GameObject.Find("Road");
        // reference script that controls the road's behavior
        Road roadScript = (Road)road.GetComponent("Road");
        // adjust the lane positions around the lane we are modifying
        roadScript.adjustRoadAroundLane(gameObject, adjustment);
        // step 4
        laneSize.z = newWidth;
        // step 5
        asphalt.transform.localScale = laneSize;

        // updates lane texture width
        asphaltMaterial.SetTextureScale("_MainTex", new Vector2(100, newWidth));

        currentLaneWidth = asphalt.transform.localScale.z;
        if (!isVehicleLane())
        {
            GetComponent<PropManager>().repositionProps(adjustment);
        }
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
        // set new stripe locations
        adjustStripePositions();  
    }

    // Nathan wrote this
    // retrieves the lane's current position
    public Vector3 getLanePosition()
    {
        return gameObject.transform.localPosition;
    }

    // Nathan wrote this
    // retrieves lane's current type
    public string getLaneType()
    {
        string laneType = gameObject.name;
        while (laneType.EndsWith("(Clone)"))
        {
            laneType = laneType.Substring(0, laneType.Length - 7);
        }
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
                throw new System.ArgumentException("Invalid stripe orientation.");
            }
        }
        // if the stripe is null, then do one of the following:
        else
        {
            // if left stripe is specified, make it null
            if (stripeOrientation == "left")
            {
                Destroy(leftStripe);
                leftStripe = null;
            }
            // if right stripe is specified, make it null
            else if (stripeOrientation == "right")
            {
                Destroy(rightStripe);
                rightStripe = null;
            }
            // otherwise, make them both null
            else
            {
                Destroy(leftStripe);
                Destroy(rightStripe);
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
    // loads a saved lane's data
    public void loadLaneAtts(LaneData savedLane)
    {
        // just need to reassign each of the lane's attributes (position, width, stripes, etc.)
        // not sure we need position (the lanes will be inserted either way, and should all be
        // the correct type and in the correct order without resetting this)
        // also not sure we need to set type, max/min width, or the booleans
        // as these should all be set properly by inserting the saved lane type
        // the only true variables are the width and the stripes
        setLaneWidth(savedLane.loadLaneWidth());
        // stripes could be a little more complicated
        // first, load in the data for both stripes
        
        StripeData leftStripeData = savedLane.loadStripeData("left");
        StripeData rightStripeData = savedLane.loadStripeData("right");
        // if the stripes are not null, load in their data
        // otherwise, just set their orientation to null
        if(leftStripeData != null)
        {
            // subcase: not sure if we need it, but just in case
            if(leftStripe != null)
            {
                Stripe leftStripeScriptReference = (Stripe)leftStripe.GetComponent("Stripe");
                leftStripeScriptReference.loadStripeAtts(leftStripeData);
            }
            else
            {
                Debug.Log("This almost certainly should never happen. INSIDE WEIRD LOADING CASE.");
            }
        }
        else
        {
            setStripeOrientation(null, "left");
        }
        if(rightStripeData != null)
        {
            // again, not sure we need this subcase
            if(rightStripe != null)
            {
                Stripe rightStripeScriptReference = (Stripe)rightStripe.GetComponent("Stripe");
                rightStripeScriptReference.loadStripeAtts(rightStripeData);
            }
            else
            {
                Debug.Log("This almost certainly should never happen. INSIDE WEIRD LOADING CASE.");
            }
        }
        else
        {
            setStripeOrientation(null, "right");
        }
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

