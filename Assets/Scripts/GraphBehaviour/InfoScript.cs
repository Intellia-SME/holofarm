using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoScript : MonoBehaviour
{

    public Text position, connectivity, alerts, timestamp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //all values must be handleded here for live
        position.text = GameObject.Find("UI").GetComponent<RectTransform>().position.ToString();
        timestamp.text = DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
        alerts.text = "New Incoming Data";
    }

    private void Awake()
    {
        Utilities.FunctionPeriodic.Create(() => {

            int choice = UnityEngine.Random.Range(0, 4);
            switch (choice)
            {
                case 1:
                    connectivity.text = "Cellular Network";
                    break;
                case 2:
                    connectivity.text = "WiFi Network";
                    break;
                default:
                    connectivity.text = "Satelite Connection";
                    break;
            }
        
        }, .9f);
    }
}
