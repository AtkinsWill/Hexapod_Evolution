using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Control : MonoBehaviour
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
        /*
        for (int legJoint = 0; legJoint < 6; legJoint++)
        {
            int dofOfJoint = (4 * legJoint) + 6;
            fullJointTargets[dofOfJoint] = actualJointTargets[legJoint, 0];
            fullJointTargets[dofOfJoint + 1] = actualJointTargets[legJoint, 1];
            fullJointTargets[dofOfJoint + 3] = actualJointTargets[legJoint, 2];

        }

        GetComponent<ArticulationBody>().SetDriveTargets(fullJointTargets);
        */

        fullJointTargets[6] = actualJointTargets[4, 0];
        fullJointTargets[7] = actualJointTargets[4, 1];
        //fullJointTargets[8] = actualJointTargets[3, 0];
        fullJointTargets[9] = actualJointTargets[5, 0];
        fullJointTargets[10] = actualJointTargets[5, 1];
        //fullJointTargets[11] = actualJointTargets[3, 0];
        fullJointTargets[12] = actualJointTargets[2, 0];
        fullJointTargets[13] = actualJointTargets[2, 1];
        //fullJointTargets[14] = actualJointTargets[3, 0];
        fullJointTargets[15] = actualJointTargets[1, 0];
        fullJointTargets[16] = actualJointTargets[1, 1];
        //fullJointTargets[17] = actualJointTargets[3, 0];
        fullJointTargets[18] = actualJointTargets[3, 0];
        fullJointTargets[19] = actualJointTargets[3, 1];
        //fullJointTargets[20] = actualJointTargets[3, 0];
        fullJointTargets[21] = actualJointTargets[0, 0];
        fullJointTargets[22] = actualJointTargets[0, 1];
        //fullJointTargets[23] = actualJointTargets[3, 0];
        fullJointTargets[24] = actualJointTargets[4, 2];
        fullJointTargets[25] = actualJointTargets[5, 2];
        fullJointTargets[26] = actualJointTargets[2, 2];
        fullJointTargets[27] = actualJointTargets[1, 0];
        fullJointTargets[28] = actualJointTargets[3, 0];
        fullJointTargets[29] = actualJointTargets[0, 0];
   
        GetComponent<ArticulationBody>().SetDriveTargets(fullJointTargets);
    }

}
