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
    public float currentTime = 0;

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
        currentTime += Time.deltaTime;

        if (currentTime > 2f)
        {
            gameObject.GetComponent<ArticulationBody>().immovable = false;
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

            // GetComponent<ArticulationBody>().SetDriveTargets(currentJointAngles);
            GetComponent<ArticulationBody>().SetDriveTargetVelocities(currentJointAngles);
        }

    }

    public float[,] getJointAngles()
    {
        List<float> positions = new List<float> { 21, 22, 15, 16, 12, 13, 18, 19, 6, 7, 9, 10};

        float[] jointPositions = new float[12];
        ArticulationReducedSpace jointAngle;
        float[,] jointAngles = new float[6, 2] { { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f } };
        //artic
        for (int i = 0; i < 6; i++)
        {
            ArticulationBody hip = gameObject.transform.GetChild(i).GetComponent<ArticulationBody>();
            jointAngle = hip.jointPosition;

            jointAngles[i, 0] =( jointAngle[0] % 360)/360;
            jointAngles[i, 1] = (jointAngle[1] % 360) /360;

            //jointAngles[i, j] = jointOrder[(i * 3) + j];
            // gameObject.GetComponent<ArticulationBody>().GetDriveTargets)

        }
        return jointAngles;
    }

    public float getBodyAngle()
    {
        float angle = gameObject.transform.rotation.x;
        if (angle < 0)
        {
            angle = angle * -1;
        }
        angle = angle / 180;

        return angle;

    }
}

/*
 *     
*/