using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayTeleporter : MonoBehaviour
{
    //public
    //public bool TwoWayTeleporter = false;
    public TriggerZone Sender;
    public Transform Destination;
    public bool SendOnEnter = true;
    public bool SendOnStay = true;
    public bool SendOnExit = true;
    public bool StopAngularVelociyOnSend = false;
    public bool StopVelociyOnSend = false;
    //private
    // Use this for initialization
    void Start()
    {
        
    }
    void ResetVelocity(GameObject EnteredObject)
    {
        if (StopAngularVelociyOnSend == true)
        {
            EnteredObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        if (StopVelociyOnSend == true)
        {
            EnteredObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (SendOnEnter == true)
        {
            foreach (GameObject EnteredObject in Sender.GetInteractors(TriggerState.Enter))
            {
                EnteredObject.GetComponent<Transform>().SetPositionAndRotation(Destination.transform.position, Destination.transform.rotation);
                ResetVelocity(EnteredObject);
            }
        }

        if (SendOnStay == true)
        {
            foreach (GameObject EnteredObject in Sender.GetInteractors(TriggerState.Stay))
            {
                EnteredObject.GetComponent<Transform>().SetPositionAndRotation(Destination.transform.position, Destination.transform.rotation);
                ResetVelocity(EnteredObject);
            }
        }
        if (SendOnExit == true)
        {
            foreach (GameObject EnteredObject in Sender.GetInteractors(TriggerState.Exit))
            {
                EnteredObject.GetComponent<Transform>().SetPositionAndRotation(Destination.transform.position, Destination.transform.rotation);
                ResetVelocity(EnteredObject);
            }
        }
    }
}
