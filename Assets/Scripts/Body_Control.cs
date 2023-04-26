using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Control : MonoBehaviour
{
    public Leg_Control leg_control;


    // Start is called before the first frame update
    void Start()
    {
        leg_control = GetComponent<Leg_Control>();
 
    }

    // Update is called once per frame
    void Update()
    {
        leg_control.actualJointTargets[1, 1] += 0.001f;
        leg_control.actualJointTargets[2, 2] += 0.001f;
        leg_control.actualJointTargets[4, 1] += 0.001f;
        leg_control.actualJointTargets[5, 0] += 0.001f;

    }
}
