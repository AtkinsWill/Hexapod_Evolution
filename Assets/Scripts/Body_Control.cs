using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Control : MonoBehaviour
{
    public List<float> currentJointAngles;
    public float[,] currentJointTargets = new float[6, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
    //public float[,] currentJointAngles = new float[6, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
    public int DoF;

    private int[] jointOrder = new int[18] { 21, 22, 29, 15, 16, 27, 12, 13, 26, 18, 19, 28, 6, 7, 24, 9, 10, 25 };

    public float maxAngleChangePerUpdate = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        DoF = 30;
        currentJointAngles = new List<float>(DoF);
        for (int i = 0; i < DoF; i++)
        {
            currentJointAngles.Add(0.0f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        /*
        for (int legJoint = 0; legJoint < 6; legJoint++)
        {
            int dofOfJoint = (4 * legJoint) + 6;
            currentJointAngles[dofOfJoint] = currentJointTargets[legJoint, 0];
            currentJointAngles[dofOfJoint + 1] = currentJointTargets[legJoint, 1];
            currentJointAngles[dofOfJoint + 3] = currentJointTargets[legJoint, 2];

        }

        GetComponent<ArticulationBody>().SetDriveTargets(currentJointAngles);
        */


        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                float currentJointAngle = currentJointAngles[jointOrder[(i * 3) + j]];

                if (currentJointAngle < currentJointTargets[i, j])
                {
                    if (currentJointTargets[i, j] - currentJointAngle <= maxAngleChangePerUpdate)
                    {
                        currentJointAngle = currentJointTargets[i, j];
                    }
                    else
                    {
                        currentJointAngle += maxAngleChangePerUpdate;
                    }
                }
                if (currentJointAngle > currentJointTargets[i, j])
                {
                    if (currentJointAngle - currentJointTargets[i, j] <= maxAngleChangePerUpdate)
                    {
                        currentJointAngle = currentJointTargets[i, j];
                    }
                    else
                    {
                        currentJointAngle -= maxAngleChangePerUpdate;
                    }
                }
                currentJointAngles[jointOrder[(i * 3) + j]] = currentJointAngle;

            }
        }

        /*
        currentJointAngles[21] = currentJointTargets[0, 0] ;
        currentJointAngles[22] = currentJointTargets[0, 1] ;
        currentJointAngles[29] = currentJointTargets[0, 2] ;

        currentJointAngles[15] = currentJointTargets[1, 0] ;
        currentJointAngles[16] = currentJointTargets[1, 1] ;
        currentJointAngles[27] = currentJointTargets[1, 2] ;

        currentJointAngles[12] = currentJointTargets[2, 0] ;
        currentJointAngles[13] = currentJointTargets[2, 1] ;
        currentJointAngles[26] = currentJointTargets[2, 2] ;

        currentJointAngles[18] = currentJointTargets[3, 0] ;
        currentJointAngles[19] = currentJointTargets[3, 1] ;
        currentJointAngles[28] = currentJointTargets[3, 2] ;

        currentJointAngles[6] = currentJointTargets[4, 0] ;
        currentJointAngles[7] = currentJointTargets[4, 1] ;
        currentJointAngles[24] = currentJointTargets[4, 2] ;

        currentJointAngles[9] = currentJointTargets[5, 0] ;
        currentJointAngles[10] = currentJointTargets[5, 1] ;
        currentJointAngles[25] = currentJointTargets[5, 2] ;
        */

        GetComponent<ArticulationBody>().SetDriveTargets(currentJointAngles);
    }

}
