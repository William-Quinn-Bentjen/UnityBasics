using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTo : MonoBehaviour {
    //list/queue to lerp to
    private List<LerpData> LerpStack = new List<LerpData>();
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    public Transform mid;
    public Transform end;
    private Vector3 endPosition;
    private Quaternion endRotation;
    private Vector3 endScale;
    private float Duration;
    private float TravelTime;
    private bool Lerping = false;
    public struct LerpData
    {
        public Transform Destination;
        public float LerpDuration;
    }
    // Use this for initialization
    void Start () {
        LerpStack = new List<LerpData>();
        AddToStack(mid, 5);
        AddToStack(end, 5);
    }
	
	// Update is called once per frame
	void Update () {
        LerpUpdate();
        if (Lerping == false && LerpStack.Count >= 1)
        {
            LerpUpdate();
        }
	}
    private void LerpUpdate()
    {
        if (Lerping == true)
        {
            TravelTime += Time.deltaTime;
            if (TravelTime >= Duration)
            {
                //stop lerp and make sure gameobject got to end position
                Lerping = false;
                gameObject.GetComponent<Transform>().position = endPosition;
                gameObject.GetComponent<Transform>().rotation = endRotation;
                gameObject.GetComponent<Transform>().localScale = endScale;
                //remove from stack of lerps
                LerpStack.RemoveAt(0);
            }
            else
            {
                Debug.Log(gameObject.transform.position);
                gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, TravelTime / Duration);
                gameObject.transform.rotation = Quaternion.Lerp(startRotation, endRotation, TravelTime / Duration);
                gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, TravelTime / Duration);
            }
        }
        if (Lerping == false && LerpStack.Count >= 1)
        {
            LerpToTransform(LerpStack[0].Destination, LerpStack[0].LerpDuration);
        }
    }
    private void LerpToTransform(Transform end, float duration)
    {
        Duration = duration;
        TravelTime = 0;
        startPosition = gameObject.transform.position;
        startRotation = gameObject.transform.rotation;
        startScale = gameObject.transform.localScale;
        endPosition = end.position;
        endRotation = end.rotation;
        endScale = end.localScale;
        Lerping = true;
    }
    public void AddToStack(Transform end, float duration)
    {
        LerpData temp;
        temp.Destination = end;
        temp.LerpDuration = duration;
        LerpStack.Add(temp);
        if (Lerping != true) { Lerping = true; }
    }
    public void ClearStack()
    {
        LerpStack.Clear();
        Lerping = false;
    }
    public bool IsLerping()
    {
        return Lerping;
    }
}
