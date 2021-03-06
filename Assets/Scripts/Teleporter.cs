﻿using System.Collections;
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
    [Tooltip("Stops velocity when teleporting out")]
    public bool StopVelociyOnSend;
    [Tooltip("Stops rotational velocity when sending and or receiving")]
    public TeleportAction StopAngularVelociy;
    [Tooltip("When the teleporter receives a traveler what should it do with the traveler's velocity")]
    public ReceiveVelocityBehavior ReceiveVelocity;
    [Tooltip("If enabled triggerzone will teleport thing that enters it.")]
    public bool SendOnEnter = true;
    [Tooltip("If enabled triggerzone will teleport thing that stays in it.")]
    public bool SendOnStay = true;
    [Tooltip("If enabled triggerzone will teleport thing that exits it.\nThis Could be used for a way to keep players within an area or be teleported back when they try to leave")]
    public bool SendOnExit = true;
    [Tooltip("When a two way teleporter receives a traveler should the traveler have to leave and reenter the triggerzone to teleport again?")]
    public bool RequireReentryToTeleportAgain = true;
    [Tooltip("When someone teleports here \nif true will set the travelers rotation to the destination's rotation\nif false the traveler keeps there rotation")]
    public bool OnReceiveSetRotation = false;
    [Tooltip("When Receiving Accept any Traveler as long as they are on the same channel no matter their tag")]
    public bool ReceiveAnyTag;
    [Tooltip("if left empty will turn on Receive Any Tag when Start() is called\nTo Have it not take anything in make an unused Tag or change the channel")]
    public List<string> ReceiveTagWhitelist;

    
    //public bool StopAngularVelociyOnSend;
    //public bool StopAngularVelociyOnReceive;
    //public bool StopVelociyOnSend;
    //public bool StopVelociyOnReceive;

    //EnteredObject.GetComponent<Rigidbody>().velocity = Vector3.zero;    
    //EnteredObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    //private
    //used to ignore travelers after jumping to a two way node (require exit to teleport again)
    private List<GameObject> ReceivedList = new List<GameObject>();
    // Use this for initialization
    void Start () {

        //make sure there is a destination on a reciver or two way
        if ((TeleporterType != TeleporterBehaviorMode.Sender) && ReceiveDestination == null)
        {
            ReceiveDestination = gameObject.GetComponent<Transform>();
        }
        //tag whitelist enabled if left blank
        if (TeleporterType != TeleporterBehaviorMode.Sender && ReceiveTagWhitelist.Count == 0)
        {
            ReceiveAnyTag = true; 
        }
    }
    //CONSTRUCTON ZONE
    enum TeleporCheckResult
    {
        Successful,
        WrongChannel,
        NotInWhiteList,
        WrongType,
        SenderIsDestination
    }
    public void TeleportSend(GameObject Traveler)
    {
        if (ReceivedList.Contains(Traveler) == false)
        {
            List<GameObject> AvailableDestinations = new List<GameObject>();
            foreach (GameObject Destination in GameObject.FindGameObjectsWithTag("TeleporterNode"))
            {
                //make sure we aren't sending to the teleporter we sent from 
                if (Destination != gameObject)
                {
                    TeleporCheckResult result = ChannelAndTagCheck(Traveler, Destination.GetComponent<Teleporter>());
                    Debug.Log(result);
                    if (result == TeleporCheckResult.Successful)
                    {
                        AvailableDestinations.Add(Destination);
                    }
                }
            }
            if (AvailableDestinations.Count > 0)
            {
                if (StopVelociyOnSend == true)
                {
                    Traveler.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                //call teleporthere on a random teleporter that was successful in channel and tag check
                AvailableDestinations[Random.Range(0, AvailableDestinations.Count - 1)].GetComponent<Teleporter>().TeleportHere(Traveler);
            }
        }
    }

    private TeleporCheckResult ChannelAndTagCheck(GameObject Traveler, Teleporter DestinationTeleporter)
    {
        if (DestinationTeleporter != gameObject.GetComponent<Teleporter>())
        {
            if (DestinationTeleporter.TeleporterType == TeleporterBehaviorMode.Sender)
            {
                return TeleporCheckResult.WrongType;
            }
            else
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
                            if (WhiteListTag == Traveler.tag)
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
        }
        else
        {
            return TeleporCheckResult.SenderIsDestination;
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
    //called on teleport here
    void ResetRotation(GameObject Traveler)
    {
        //local
        if (ReceiveVelocity == ReceiveVelocityBehavior.NoVelocity)
        {
            Traveler.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.LocalUp)
        {
            Traveler.GetComponent<Rigidbody>().velocity = this.transform.up * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.LocalDown)
        {
            Traveler.GetComponent<Rigidbody>().velocity = (this.transform.up * -1) * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.LocalForward)
        {
            Traveler.GetComponent<Rigidbody>().velocity = this.transform.forward * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.LocalBackward)
        {
            Traveler.GetComponent<Rigidbody>().velocity = (this.transform.forward * -1) * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.LocalRight)
        {
            Traveler.GetComponent<Rigidbody>().velocity = this.transform.right * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.LocalLeft)
        {
            Traveler.GetComponent<Rigidbody>().velocity = (this.transform.right * -1) * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        //world 
        else if (ReceiveVelocity == ReceiveVelocityBehavior.WorldUp)
        {
            Traveler.GetComponent<Rigidbody>().velocity = Vector3.up * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.WorldDown)
        {
            Traveler.GetComponent<Rigidbody>().velocity = (Vector3.up * -1) * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.WorldForward)
        {
            Traveler.GetComponent<Rigidbody>().velocity = Vector3.forward * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.WorldBackward)
        {
            Traveler.GetComponent<Rigidbody>().velocity = (Vector3.forward * -1) * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.WorldRight)
        {
            Traveler.GetComponent<Rigidbody>().velocity = Vector3.right * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else if (ReceiveVelocity == ReceiveVelocityBehavior.WorldLeft)
        {
            Traveler.GetComponent<Rigidbody>().velocity = (Vector3.right * -1) * Traveler.GetComponent<Rigidbody>().velocity.magnitude;
        }
        if (StopAngularVelociy == TeleportAction.OnReceive || StopAngularVelociy == TeleportAction.OnBoth)
        {
            Traveler.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
    public enum ReceiveVelocityBehavior
    {
        Unchanged,
        NoVelocity,
        LocalUp,
        LocalDown,
        LocalLeft,
        LocalRight,
        LocalForward,
        LocalBackward,
        WorldUp,
        WorldDown,
        WorldLeft,
        WorldRight,
        WorldForward,
        WorldBackward,

    }
    //END ZONE
    //returns successfull in teleporting and finding a channel or tag
    public void TeleportHere(GameObject Traveler)
    {
        //find teleporters with the same channel
        //look for tags/receive any tag to be enabled
        //velocity changes 

        ResetRotation(Traveler);
        
        Debug.Log(Traveler.GetComponent<Rigidbody>().velocity.magnitude);
        //rotation changes and travel
        if (OnReceiveSetRotation == true)
        {
            Traveler.GetComponent<Transform>().SetPositionAndRotation(ReceiveDestination.position, ReceiveDestination.rotation);
        }
        else
        {
            Traveler.GetComponent<Transform>().SetPositionAndRotation(ReceiveDestination.position, Traveler.GetComponent<Transform>().rotation);
        }
        if (TeleporterType == TeleporterBehaviorMode.TwoWay)
        {
            if (RequireReentryToTeleportAgain == true)
            {
                ReceivedList.Add(Traveler);
            }
        }
    }
    // Update is called once per frame
    void Update () {
        if (ReceivedList.Count > 0)
        {
            if (SendTriggerZone.GetInteractors(TriggerState.Exit).Count > 0)
            {
                foreach (GameObject Traveler in SendTriggerZone.GetInteractors(TriggerState.Exit))
                {
                    ReceivedList.Remove(Traveler);
                }
            }
        }
        if (TeleporterType != TeleporterBehaviorMode.Receiver)
        {
            if (SendTriggerZone != null)
            {
                if (SendOnEnter == true)
                {
                    if (SendTriggerZone.GetInteractors(TriggerState.Enter).Count > 0)
                    {
                        foreach (GameObject Traveler in SendTriggerZone.GetInteractors(TriggerState.Enter))
                        {
                            TeleportSend(Traveler);
                        }
                    }
                }
                if (SendOnStay == true)
                {
                    if (SendTriggerZone.GetInteractors(TriggerState.Stay).Count > 0)
                    {
                        foreach (GameObject Traveler in SendTriggerZone.GetInteractors(TriggerState.Stay))
                        {
                            TeleportSend(Traveler);
                        }
                    }
                }
                if (SendOnExit == true)
                {
                    if (SendTriggerZone.GetInteractors(TriggerState.Exit).Count > 0)
                    {
                        foreach (GameObject Traveler in SendTriggerZone.GetInteractors(TriggerState.Exit))
                        {
                            TeleportSend(Traveler);
                        }
                    }
                }
            }
        }
    }
}
