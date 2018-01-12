using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLift : MonoBehaviour {
    //public
    public float force;
    public ForceMode ForceType = ForceMode.Impulse;
    public TriggerZone GravityLiftTriggerZone;
    public TriggerZone GravityLiftZone;
    public bool LaunchOnEnter = true;
    public bool LaunchOnStay = true;
    public bool LaunchOnExit = true;
    //private

    // Use this for initialization
    void Start () {
        Debug.Log(GravityLiftTriggerZone);
	}
	void LaunchObject(GameObject EnteredObject)
    {
        EnteredObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.up * force, ForceType);
    }
	// Update is called once per frame
	void Update () {
        if (LaunchOnEnter == true)
        {
            foreach (GameObject EnteredObject in GravityLiftZone.GetInteractors(TriggerState.Enter))
            {
                LaunchObject(EnteredObject);
            }
        }
        if (LaunchOnStay == true)
        {
            foreach (GameObject EnteredObject in GravityLiftZone.GetInteractors(TriggerState.Stay))
            {
                LaunchObject(EnteredObject);
            }
        }
        if (LaunchOnExit == true)
        {
            foreach (GameObject EnteredObject in GravityLiftZone.GetInteractors(TriggerState.Exit))
            {
                LaunchObject(EnteredObject);
            }
        }
    }
}
