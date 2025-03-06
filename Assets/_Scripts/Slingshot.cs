using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slingshot : MonoBehaviour
{

    [SerializeField] private LineRenderer rubber;
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;
    [SerializeField] private Configuration configuration;
    public AudioSource releaseSound;

    //Fields set in the Inspector
    [Header("Inscribed")]
    public GameObject       projectilePrefab;
    public float            velocityMult = 10f;
    public GameObject       projLinePrefab;

    //field set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3    launchPos;
    public GameObject projectile;
    public bool       aimingMode;

    void Start() {
        rubber.SetPosition(0, firstPoint.position); //sets first point
        rubber.SetPosition(2, secondPoint.position); //sets second point

    }
    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint"); //search for child of slingshot names launchpoint and returns its trasnform
        launchPoint = launchPointTrans.gameObject; //gets GO associated w/ transform and assigns to launchPoint
        launchPoint.SetActive( false );
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter() {
        //print ("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit() {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {
        //The player pressed the mouse button while over Slingshot
        aimingMode = true;
        //Instantiate a projectile
        projectile = Instantiate( projectilePrefab ) as GameObject;
        //Start it at the launchPoint
        projectile.transform.position = launchPos;
        //Set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;

    }

    void Update() 
    {
        //If slingshot is not in aimmode, dont run code
        if (!aimingMode) return;

        //Get mouse position in 2D screen coords
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint( mousePos2D );
        
        //Find delta from launchPos to the mousePos
        Vector3 mouseDelta = mousePos3D - launchPos;
        //Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        //Move projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        if (Input.GetMouseButton(0)) {
            rubber.SetPosition(1, projPos); //GetMousePositionInWorld()
        }
        if (Input.GetMouseButtonUp(0) ) { //Mouse has been released
            releaseSound.Play();                
            AudioSource windSound = projectile.GetComponent<AudioSource>();
            if (windSound != null) {windSound.Play();}
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -mouseDelta * velocityMult;

            //Switch to slingshot view immediately before setting POI
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile; // Set _Maincamera POI
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            
            MissionDemolition.SHOT_FIRED();
        }
    }

    Vector3 GetMousePositionInWorld(){
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z += Camera.main.transform.position.z;
        Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
        if (mousePositionInWorld.magnitude > configuration.Radius) {
            mousePositionInWorld.Normalize();
            mousePositionInWorld *= configuration.Radius;
        }
        return mousePositionInWorld;
    }

    
}
