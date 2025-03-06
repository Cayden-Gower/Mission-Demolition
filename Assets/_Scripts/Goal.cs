using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Renderer) )]

public class Goal : MonoBehaviour
{
    static public bool goalMet = false;

    void OnTriggerEnter ( Collider other ) {
        //When trigger is hit check to see if its a ball
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null) {
            Goal.goalMet = true;
            //Set alpha of color to higher opacity
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 0.75f;
            mat.color = c;
        }
    }
}
