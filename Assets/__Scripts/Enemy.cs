using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float speed = 10f;   //The speed in m/s
    public float fireRate = 0.3f; //Seconds/shot (Unused)
    public float health = 100;

    public bool ______________;

    public Bounds bounds;   //The bounds of this and its child
    public Vector3 boundsCenterOffset;  //Dist of bounds.center

    void Awake()
    {
        InvokeRepeating("CheckOffscreen", 0f, 2f);
    }

    void Update()
    {
        Move();
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos= tempPos;
    }

    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }

        set
        {
            this.transform.position = value;
        }
    }

    void CheckOffscreen()
    {
        //If bounds are still their default value

        if (bounds.size == Vector3.zero)
        {
            //then set them
            bounds = Utils.CombineBoundsofChildren(this.gameObject);
            //Also find the diff between bounds.center & transform.position
            boundsCenterOffset = bounds.center - transform.position;
        }

        //Every time, update the bounds to the current postion 
        bounds.center = transform.position + boundsCenterOffset;

        //Check to see whether the bounds are completely offscreen
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
        
        if (off != Vector3.zero)
        {
            //If this enemy has gone off the bottom edge of the screen
            if (off.y < 0)
            {
                //then destroy it
                Destroy(this.gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch(other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //Enemies don't take damage unless they're onscreen
                //THis stops the player from shooting them before they are visible
                bounds.center = transform.position + boundsCenterOffset;
                if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero)
                {
                    Destroy(other);
                    break;
                }
                //Hurt this Enemy
                //Get the damage amount from the Projectile.type & Main.W_Defs
                health -= Main.W_DEFS[p.type].damageOnHit;
                if (health <= 0)
                {
                    //Destroy this enemy
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }
}
