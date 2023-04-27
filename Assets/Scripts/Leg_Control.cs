using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Control : MonoBehaviour
{
    public List<float> fullJointTargets;
    public float[,] actualJointTargets = new float[6, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
    public int DoF;

    // Start is called before the first frame update
    void Start()
    {
        DoF = 30;
        fullJointTargets = new List<float>(DoF);
        for (int i = 0; i < DoF; i++)
        {
            fullJointTargets.Add(0.0f);
        }

    }

    // Update is called once per frame
    void Update()
    {
 
        for (int legJoint = 0; legJoint < 6; legJoint++)
        {
            int dofOfJoint = (4 * legJoint) + 6; 
            fullJointTargets[dofOfJoint] = actualJointTargets[legJoint, 0];
            fullJointTargets[dofOfJoint + 1] = actualJointTargets[legJoint, 1];
            fullJointTargets[dofOfJoint + 3] = actualJointTargets[legJoint, 2];

        }

        GetComponent<ArticulationBody>().SetDriveTargets(fullJointTargets);

    }
}
