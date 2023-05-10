using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Continuous_Control : MonoBehaviour
{
    public List<float> currentJointAngles;
    public float[,] currentJointTargets = new float[6, 2] { { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f}, { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f} };
    //public float[,] currentJointAngles = new float[6, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
    public int DoF;

    private int[] jointOrder = new int[12] { 21, 22, 29, 15, 16, 27, 12, 13, 26, 18, 19, 28 };
    // knees , 6, 7, 24, 9, 10, 25

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



        currentTime += Time.deltaTime;

        if (currentTime > 2f)
        {
            gameObject.GetComponent<ArticulationBody>().immovable = false;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
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

            GetComponent<ArticulationBody>().SetDriveTargets(currentJointAngles);
        }

    }
}
