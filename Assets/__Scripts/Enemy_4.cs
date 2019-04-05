using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Part is another serializable data storage class jsut like WeaponDefinition
[System.Serializable]
public class Part
{
    //These three fields need to be defined in the Inspector pane
    public string name;                 //The name of this part
    public float health;                //The amount of health this part has
    public string[] protectedBy;        //The other parts that protect this

    //These two fields are set automatically in Start();
    //Caching like this makes it faster and easier to find these later
    public GameObject go;           //The GameObject of this part
    public Material mat;            //The Material to show damage.
}

public class Enemy_4 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
