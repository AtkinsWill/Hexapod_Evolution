using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Control : MonoBehaviour
{
    public List<float> fullJointTargets;
    public List<float> actualJointTargets;
    public GameObject[] legs;
    public int DoF;
    public ArticulationBody artiulationBody;

    // Start is called before the first frame update
    void Start()
    {
        
        artiulationBody = GetComponent<ArticulationBody>();
        DoF = 30;
        fullJointTargets = new List<float>(DoF);
        for (int i = 0; i < DoF; i++)
        {
            fullJointTargets.Add(0.0f);
        }
        actualJointTargets = new List<float>(3);
        for (int i = 0; i < 3; i++)
        {
            actualJointTargets.Add(0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int legJoint = 6; legJoint < DoF; legJoint += 4)
        {
            Debug.Log(legJoint);
            fullJointTargets[legJoint] = actualJointTargets[0];
            fullJointTargets[legJoint+1] = actualJointTargets[1];
            fullJointTargets[legJoint+3] = actualJointTargets[2];

        }
        
        artiulationBody.SetDriveTargets(fullJointTargets);
 
    }
}
