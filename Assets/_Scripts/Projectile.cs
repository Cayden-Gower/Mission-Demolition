using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof(Rigidbody) )] //Ensures any GO this script is attached to has Rigidbody componnet

public class Projectile : MonoBehaviour
{
    const int LOOKBACK_COUNT = 10; //Stores max length that we want to allow deltas list to reach
    static List<Projectile> PROJECTILES = new List<Projectile>();

    [SerializeField]
    private bool _awake = true; //Can be seen in inspector, and other scripts can read it, but they can't change it
    
    public bool awake {         //^^ Does this
        get { return _awake; } 
        private set { _awake = value; }
    }

    private Vector3 prevPos;

    // This private list stores the history of projectiles move distance
    private List<float> deltas = new List<float>();
    private Rigidbody rigid;
    private AudioSource windSound;


    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        awake = true;
        prevPos = new Vector3(1000,10000,0);
        deltas.Add( 1000 ); // set initial values in prevPos to a position and distance far from actual projectile position to give proj. some time to move before it is removed from list
        windSound = GetComponent<AudioSource>();
        PROJECTILES.Add( this );
    }

    void FixedUpdate() 
    {
        if ( rigid.isKinematic || !awake) return; // if rigidbody is still kinematic the rest of fixedupdate doesnt execute

        Vector3 deltaV3 = transform.position - prevPos;
        deltas.Add( deltaV3.magnitude ); // calculate distance btwn current position and last position, then store distance in the list.
        prevPos = transform.position;

        //Limit lookback
        while ( deltas.Count > LOOKBACK_COUNT ) { //more than LOOKBACK_COUNT elements in the list, remove the 0th element (oldest) until only LOOKBACK_COUNT exist
            deltas.RemoveAt( 0 );
        }

        //Iterate over deltas and find the greatest one
        float maxDelta = 0;
        foreach (float f in deltas) {
            if (f > maxDelta ) maxDelta = f;
        }

        //If projectile hasn't moved more than sleepThreshold
        if (maxDelta <= Physics.sleepThreshold) { //if max distance in deltas is lower than sleep threshold, tell rigibody to sleep
            //Set awake to false and put rigidbody to sleep
            awake = false;
            rigid.Sleep();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (windSound != null && windSound.isPlaying){
            windSound.Stop();
        }
    }
    private void OnDestroy() {
        PROJECTILES.Remove ( this );
    }

    static public void DESTROY_PROJECTILES() {
        foreach (Projectile p in PROJECTILES) {
            Destroy (p.gameObject);
        }
    }
}
