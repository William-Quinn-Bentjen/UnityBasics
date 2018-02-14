using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visibility : MonoBehaviour
{
    public bool VisibleAtStart = false;
    private Renderer rend;
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (VisibleAtStart)
        {
            rend.enabled = true;
        }
        else
        {
            rend.enabled = false;
        }
    }
    //set
    public void isVisible(OnOffToggle set)
    {
        if (set == OnOffToggle.On)
        {
            rend.enabled = true;
        }
        else if (set == OnOffToggle.Off)
        {
            rend.enabled = false;
        }
        else if (set == OnOffToggle.Toggle)
        {
            rend.enabled = !rend.enabled;
        }
        else
        {
            Debug.Log(set + " Not a supported visibility setting for a triggerzone");
        }
    }
    //get
    public bool isVisible()
    {
        return rend.enabled;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
