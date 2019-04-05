using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is actually OUTSIDE of the Utils class
public enum BoundsTest
{
    center,         //Is the center of the Gameobject on screen
    onScreen,       //Are the bounds entirely on screen
    offScreen,      //Are the bounds entirely off screen
}

public class Utils : MonoBehaviour {

    //============================= Bounds Functions =============================\\

    //Creates bounds that encapsulate of the two Bound passed in,
    public static Bounds BoundsUnion (Bounds b0, Bounds b1)
    {
        //If the size of one of the bounds is Vector3.zero, ignore that one
        if (b0.size == Vector3.zero && b1.size != Vector3.zero)
        {
            return (b1);
        }
        else if (b0.size != Vector3.zero && b1.size == Vector3.zero)
        {
            return (b0);
        }
        else if (b0.size == Vector3.zero && b1.size == Vector3.zero)
        {
            return (b0);
        }

        //Stretch b0 to include the b1.min and b1.max
        b0.Encapsulate(b1.min);
        b0.Encapsulate(b1.max);
        return (b0);
    }

    public static Bounds CombineBoundsofChildren (GameObject go)
    {
        //Create an empty Bounds b
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);

        //If this GameObject has a Renderer Component
        if(go.GetComponent<Renderer>() != null)
        {
            //Expand b to contain the Renderer's Bounds
            b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
        }

        //If this GameObject has a Collider Component
        if (go.GetComponent<Collider>() != null)
        {
            //Expand b to contain the Collider's Bounds
            b = BoundsUnion(b, go.GetComponent<Collider>().bounds);
        }

        //Iterate through each child of this gameObject.transform
        foreach (Transform t in go.transform)
        {
            //Expand b to contain their Bounds as well
            b = BoundsUnion(b, CombineBoundsofChildren(t.gameObject));
        }

        return (b);
    }

    //Make a static read-only public property camBounds
    static public Bounds camBounds
    {
        get
        {
            //If _camBounds hasn't been set yet
            if(_camBounds.size == Vector3.zero)
            {
                //SetCameraBounds using the default Camera
                SetCameraBounds();
            }
            return (_camBounds);
        }
    }

    //This is the private static field that camBounds uses
    static private Bounds _camBounds;

    public static void SetCameraBounds (Camera cam = null)
    {
        //If no Camera was passed in, use the main camer
        if (cam == null) cam = Camera.main;
        //This makea couple important assumptions about the camera!:
        //  1. The camera is Orthographic
        //  2. The camera is at a set rotate of R:[0, 0, 0]

        //Make Vector3s at the topLeft and bottomRight of the Screen coords
        Vector3 topLeft = new Vector3(0, 0, 0);
        Vector3 bottomRight = new Vector3(Screen.width, Screen.height, 0);

        //Convert these to world coordinates
        Vector3 boundTLN = cam.ScreenToWorldPoint(topLeft);
        Vector3 boundBRF = cam.ScreenToWorldPoint(bottomRight);

        //Adjust the z to be at the near and far Camera clipping planes
        boundTLN.z += cam.nearClipPlane;
        boundBRF.z += cam.farClipPlane;

        //Find the center of the Bound
        Vector3 center = (boundTLN + boundBRF) / 2f;
        _camBounds = new Bounds(center, Vector3.zero);
        //Expand _camBounds to encapsulate the extents.
        _camBounds.Encapsulate(boundTLN);
        _camBounds.Encapsulate(boundBRF);
    }

    //Test to tsee whether Bounds are on a screen.
    public static Vector3 ScreenBoundsCheck (Bounds bnd, BoundsTest test = BoundsTest.center)
    {
        //Call the more generic BoundsInBoundsCheck with camBounds as bigB
        return (BoundsInBoundsCheck(camBounds, bnd, test));
    } 

    //Tests to see whether lilB is inside bigB
    public static Vector3 BoundsInBoundsCheck (Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen)
    {
        //'Get the center of lilB
        Vector3 pos = lilB.center;

        //Initialize the offset at [0, 0, 0]
        Vector3 off = Vector3.zero;

        switch (test)
        {
            //The center test determines what off (offset) would have to be applied to lilB to move its center back inside bigB
            case BoundsTest.center:
                //if the center is contained, return Vector3.zero
                if (bigB.Contains(pos))
                {
                    return (Vector3.zero);
                }
                //if not contained, find the offset
                if (pos.x > bigB.max.x)
                {
                    off.x = pos.x - bigB.max.x;
                }
                else if (pos.x < bigB.min.x)
                {
                    off.x = pos.x - bigB.min.x;
                }
                if (pos.y > bigB.max.y)
                {
                    off.y = pos.y - bigB.max.y;
                }
                else if (pos.y < bigB.min.y)
                {
                    off.y = pos.y - bigB.min.y;
                }
                if (pos.z > bigB.max.z)
                {
                    off.z = pos.z - bigB.max.z;
                }
                else if (pos.z < bigB.min.z)
                {
                    off.z = pos.z - bigB.min.z;
                }
                return (off);

            //The onScreen test determines what off would have to be applied to keep all of lilB inside bigB
            case BoundsTest.onScreen:
                //Find whether bigB contains all of lilB
                if (bigB.Contains (lilB.min) && bigB.Contains(lilB.max))
                {
                    return (Vector3.zero);
                }
                //if not, find the offset
                if (lilB.max.x > bigB.max.x)
                {
                    off.x = lilB.max.x - bigB.max.x;
                }
                else if (lilB.min.x < bigB.min.x)
                {
                    off.x = lilB.min.x - bigB.min.x;
                }
                if (lilB.max.y > bigB.max.y)
                {
                    off.y = lilB.max.y - bigB.max.y;
                }
                else if (lilB.min.y < bigB.min.y)
                {
                    off.y = lilB.min.y - bigB.min.y;
                }
                if (lilB.max.z > bigB.max.z)
                {
                    off.z = lilB.max.z - bigB.max.z;
                }
                else if (lilB.min.z < bigB.min.z)
                {
                    off.z = lilB.min.z - bigB.min.z;
                }
                return (off);

            //The offScreen test determines what off would need to be applied to move any tiny part of lilB inside of bigB
            case BoundsTest.offScreen:
                //find whether bigB contains any of lilB
                bool cMin = bigB.Contains(lilB.min);
                bool cMax = bigB.Contains(lilB.max);
                if (cMin || cMax)
                {
                    return (Vector3.zero);
                }
                // If not, find the offset
                if (lilB.min.x > bigB.max.x)
                {
                    off.x = lilB.min.x - bigB.max.x;
                }
                else if (lilB.max.x < bigB.min.x)
                {
                    off.x = lilB.max.x - bigB.min.x;
                }
                if (lilB.min.y > bigB.max.y)
                {
                    off.y = lilB.min.y - bigB.max.y;
                }
                else if (lilB.max.y < bigB.min.y)
                {
                    off.y = lilB.max.y - bigB.min.y;
                }
                if (lilB.min.z > bigB.max.z)
                {
                    off.z = lilB.min.z - bigB.max.z;
                }
                else if (lilB.max.z < bigB.min.z)
                {
                    off.z = lilB.max.z - bigB.min.z;
                }
                return (off);
        }

        return (Vector3.zero);
    }

    //============================= Tranform Functions =============================\\

    //This function will iteratively climb up the transform.parent
    //until it either finda parent with a tag != "Untagged" or no parent

    public static GameObject FindTaggedParent(GameObject go)
    {
        //If this gameOBject has a tag
        if (go.tag != "Untagged")
        {
            //then return this gameObject
            return (go);
        }

        //IF there is no parent of this Tranform
        if(go.transform.parent == null)
        {
            //We've reached the end of the line with no interesting tage so return null
            return (null);
        }

        //Otherwise, recursively clumb up the tree
        return (FindTaggedParent(go.transform.parent.gameObject));
    }

    //This version of the functuion handles things if a Transform is passed in
    public static GameObject FindTaggedParent(Transform t)
    {
        return (FindTaggedParent(t.gameObject));
    }

    //============================= Materials Functions =============================\\

    //Returns a list of all Material in this GameObject or its children
    static public Material[] GetAllMaterials(GameObject go)
    {
        List<Material> mats = new List<Material>();
        if(go.GetComponent<Renderer>() != null)
        {
            mats.Add(go.GetComponent<Renderer>().material);
        }
        foreach (Transform t in go.transform)
        {
            mats.AddRange(GetAllMaterials(t.gameObject));
        }
        return (mats.ToArray());
    }
}
