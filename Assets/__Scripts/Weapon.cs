﻿using System.Collections;
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
    static public Transform PROJECTILE_ANCHOR;

    [SerializeField]
    private WeaponType _type = WeaponType.blaster;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShot;

    void Awake()
    {
        collar = transform.Find("Collar").gameObject; 
    }

    void Start ()
    {
        //Call SetType() properly for the default_type
        SetType(_type);
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        //Find the fireDelegate of the parent
        GameObject parentGO = transform.parent.gameObject;
        if(parentGO.tag == "Hero")
        {
            Hero.S.fireDelegate += Fire;
        }
	}

    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collar.GetComponent<Renderer>().material.color = def.color;
        lastShot = 0;       //You can always fire immediately after _type is set.
    }

    public void Fire()
    {
        //If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        //IF it hasn't been enough time between shots, return
        if (Time.time - lastShot < def.delayBetweenShots)
        {
            return;
        }

        Projectile p;
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                break;

            case WeaponType.spread:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(-.2f, 0.9f, 0) * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(.2f, 0.9f, 0) * def.velocity;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate(def.projectilePrefab) as GameObject;
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.parent = PROJECTILE_ANCHOR;
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShot = Time.time;
        return (p);
    }
}
