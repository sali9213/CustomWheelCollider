using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Logger : MonoBehaviour
{
    [SerializeField] private Wheels wheels;
    [SerializeField] private TextMeshProUGUI log;
    // Update is called once per frame
    void Update()
    {
        if(wheels.WC[0].GetGroundHit (out WheelHit hit))
        {
            log.text = wheels.WC[0].motorTorque + " " + wheels.WC[1].brakeTorque + " " + hit.forwardSlip;
        }

    }
}
