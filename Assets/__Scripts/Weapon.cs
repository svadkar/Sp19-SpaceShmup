using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is an enum of the various possible wepaon types
//It also includes a "shield" type to allow a shield power-up
//Items marked [NI] below are Not Implemented in the Book
public enum WeaponType
{
    none,                   //THe default/no weapon
    blaster,                //A simple blaster
    spread,                 //Two shots simultaneously
    phaser,                 //Shots that move in waves [NI]
    missile,                //Homing missile [NI]
    laser,                  //Damage over time [NI]
    shield,                 //Raise shieldLevel
}

//The Weapon Definition class allows you to set the porepeties
//  of a specific weapon the Inspector. Main has an array
//  of WeaponDefinitions that makes this possible.
[System.Serializable]
//System.Serializable tells Unity to try to view WeaponDefinition
//  in the Inspector pane. It doesn't work for everything, but it
//  will work for simple classes like this!
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;                               //The letter to show on the power-up
    public Color color = Color.white;                   //Color of Collar & power-up
    public GameObject projectilePrefab;                 //Prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;                       //Amount of damage caused
    public float continuousDamage = 0;                  //Damage per second (laser)
    public float delayBetweenShots = 0;
    public float velocity = 20;                         //Speed of projectile
}

//Note: Weapon prefabs, colors, etc. are set in the class Main.

public class Weapon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
