using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeleporterBehaviorMode
{
    Sender,
    Receiver,
    TwoWay
}
public enum TeleportAction
{
    Never,
    OnSend,
    OnReceive,
    OnBoth
}
public enum TeleportAttempt
{
    Successful,
    UnsucessfulNoChannels,
    UnsucessfulNoChannelsWithTagsThatWork,
}
public class Teleporter : MonoBehaviour {

    public TeleporterBehaviorMode TeleporterType = TeleporterBehaviorMode.TwoWay;
    [Tooltip("Only teleporters with the same channel\nwill be able to teleport, please don't leave blank.")]
    public string Channel = "Default";
    [Tooltip("Where objects will be sent from\nThis can be left blank for a receiver\nThis can also be left blank to have other objects call the teleport function on this script.")]
    public TriggerZone SendTriggerZone;
    [Tooltip("If left blank will use it's position")]
    public Transform ReceiveDestination;
    [Tooltip("If enabled triggerzone will teleport thing that enters it.")]
    public bool SendOnEnter = true;
    [Tooltip("If enabled triggerzone will teleport thing that stays in it.")]
    public bool SendOnStay = true;
    [Tooltip("If enabled triggerzone will teleport thing that exits it.\nThis Could be used for a way to keep players within an area or be teleported back when they try to leave")]
    public bool SendOnExit = true;
    [Tooltip("When someone teleports here \nif true will set the travelers rotation to the destination's rotation\nif false the traveler keeps there rotation")]
    public bool OnReceiveSetRotation = false;
    [Tooltip("When Receiving Accept any Traveler as long as they are on the same channel no matter their tag")]
    public bool ReceiveAnyTag;
    [Tooltip("if left empty will turn on Receive Any Tag\nTo Have it not take anything in make an unused Tag or change the channel")]
    public List<string> ReceiveTagWhitelist;
    [Tooltip("Stops rotational velocity when teleporting in and or out")]
    public TeleportAction StopAngularVelociy;
    [Tooltip("Stops velocity when teleporting in and or out")]
    public TeleportAction StopVelociy;
    //public bool StopAngularVelociyOnSend;
    //public bool StopAngularVelociyOnReceive;
    //public bool StopVelociyOnSend;
    //public bool StopVelociyOnReceive;

    //EnteredObject.GetComponent<Rigidbody>().velocity = Vector3.zero;    
    //EnteredObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    //private
    // Use this for initialization
    void Start () {

        //make sure there is a destination on a reciver
        if (TeleporterType == TeleporterBehaviorMode.Receiver && ReceiveDestination == null)
        {
            Debug.Log("detected no receiver location\nDefaulted to the location of the teleporter\nWill be updated next on Update");
            ReceiveDestination = gameObject.GetComponent<Transform>();
        }
        //tag whitelist enabled if left blank
        if (TeleporterType != TeleporterBehaviorMode.Sender && ReceiveTagWhitelist == null)
        {
            ReceiveAnyTag = true; 
        }
    }
    //CONSTRUCTON ZONE
    enum TeleporCheckResult
    {
        Successful,
        WrongChannel,
        NotInWhiteList
    }
    public void TeleportSend(GameObject Traveler)
    {
        List<GameObject> AvailableDestinations = new List<GameObject>();
        foreach(GameObject Destination in GameObject.FindGameObjectsWithTag("TeleporterNode"))
        {
            if (Destination != gameObject)
            {
                TeleporCheckResult result = ChannelAndTagCheck(Traveler, Destination.GetComponent<Teleporter>());
                if (result == TeleporCheckResult.Successful)
                {
                    AvailableDestinations.Add(Destination);
                }
            }
        }
        if (AvailableDestinations.Count > 0)
        {
            //call teleporthere on a random teleporter that was successful in channel and tag check
            AvailableDestinations[Random.Range(0, AvailableDestinations.Count - 1)].GetComponent<Teleporter>().TeleportHere(Traveler);
        }
    }

    private TeleporCheckResult ChannelAndTagCheck(GameObject Traveler, Teleporter DestinationTeleporter)
    {
        if (DestinationTeleporter.GetChannel() == Channel)
        {
            if (DestinationTeleporter.GetReceivesAnyTag())
            {
                //good channel and teleporter accepts any tag
                return TeleporCheckResult.Successful;
            }
            else
            {
                foreach (string WhiteListTag in DestinationTeleporter.GetReceiveWhiteList())
                {
                    if (WhiteListTag == DestinationTeleporter.tag)
                    {
                        //good channel and traveler tag match
                        return TeleporCheckResult.Successful;

                    }
                }
                //not in whitelist
                return TeleporCheckResult.NotInWhiteList;
            }
        }
        else
        {
            //wrong channel
            return TeleporCheckResult.WrongChannel;
        }
    }
    public string GetChannel()
    {
        return Channel;
    }
    public bool GetReceivesAnyTag()
    {
        return ReceiveAnyTag;
    }
    public List<string> GetReceiveWhiteList()
    {
        return ReceiveTagWhitelist;
    }
    void ResetRotation(GameObject Traveler)
    {
        if (StopVelociy == TeleportAction.OnReceive || StopVelociy == TeleportAction.OnBoth)
        {
            Traveler.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        if (StopAngularVelociy == TeleportAction.OnReceive || StopAngularVelociy == TeleportAction.OnBoth)
        {
            Traveler.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
    //END ZONE
    //returns successfull in teleporting and finding a channel or tag
    public void TeleportHere(GameObject Traveler)
    {
        //find teleporters with the same channel
        //look for tags/receive any tag to be enabled
        //velocity changes 

        ResetRotation(Traveler);
        //rotation changes and travel
        if (OnReceiveSetRotation == true)
        {
            Traveler.GetComponent<Transform>().SetPositionAndRotation(ReceiveDestination.position, ReceiveDestination.rotation);
        }
        else
        {
            Traveler.GetComponent<Transform>().SetPositionAndRotation(ReceiveDestination.position, Traveler.GetComponent<Transform>().rotation);
        }
        //
    }
    // Update is called once per frame
    void Update () {
        if (TeleporterType != TeleporterBehaviorMode.Receiver)
        {
            if (SendTriggerZone != null)
            {
                if (SendOnEnter == true)
                {
                    foreach (GameObject Traveler in SendTriggerZone.GetInteractors(TriggerState.Enter))
                    {
                        //teleport
                        //find destination with tag and use the teleprt
                        //EnteredObject.GetComponent<Transform>().SetPositionAndRotation(Destination.transform.position, Destination.transform.rotation);
                        TeleportSend(Traveler);
                    }
                }
                if (SendOnStay == true)
                {
                    foreach (GameObject Traveler in SendTriggerZone.GetInteractors(TriggerState.Stay))
                    {
                        //teleport
                        //find destination with tag and use the teleprt
                        //EnteredObject.GetComponent<Transform>().SetPositionAndRotation(Destination.transform.position, Destination.transform.rotation);
                        TeleportSend(Traveler);
                    }
                }
                if (SendOnExit == true)
                {
                    foreach (GameObject Traveler in SendTriggerZone.GetInteractors(TriggerState.Exit))
                    {
                        //teleport
                        //find destination with tag and use the teleprt
                        //EnteredObject.GetComponent<Transform>().SetPositionAndRotation(Destination.transform.position, Destination.transform.rotation);
                        TeleportSend(Traveler);
                    }
                }
            }
        }
    }
}
