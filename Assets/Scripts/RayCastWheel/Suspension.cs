using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension
{
    public float RestLength;
    public float SpringTravel;
    public float SpringStiffness;
    public float DamperStiffness;

    public float maxLength { get; private set; }
    public float minLength { get; private set; }
    public float springLength { get; private set; }
    public float springForce { get; private set; }
    private float lastLength;
    private float springVelocity;
    private float damperForce;


    public Suspension(float RestLength, float SpringTravel, float SpringStiffness, float DamperStiffness)
    {
        this.RestLength = RestLength;
        this.SpringTravel = SpringTravel;
        this.SpringStiffness = SpringStiffness;
        this.DamperStiffness = DamperStiffness;

        minLength = RestLength - SpringTravel;
        maxLength = RestLength + SpringTravel;
    }

    public float CalculateSuspensionForce(RaycastHit hit, float wheelRadius)
    {
        lastLength = springLength;
        springLength = hit.distance - wheelRadius;
        springLength = Mathf.Clamp(springLength, minLength, maxLength);
        springVelocity = (lastLength - springLength) / Time.deltaTime;
        springForce = SpringStiffness * (RestLength - springLength);
        damperForce = DamperStiffness * springVelocity;
        return springForce + damperForce;
    }

    public void ExtendMax()
    {
        springLength = maxLength;
    }

}
