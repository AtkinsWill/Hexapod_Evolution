using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Control : MonoBehaviour
{

    public List<float> jointAngleTargets;
    public Vector3 limbDimensions;
    //public GameObject hip;
    public GameObject knee;
    Vector3 hipEuler;
    Vector3 kneeEuler;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //jointAngleTargets
        /*
        var hipRollMotor = GetComponent<ArticulationBody>().xDrive;
        var hipYawMotor = GetComponent<ArticulationBody>().yDrive;
        var kneeRollMotor = knee.transform.GetComponent<ArticulationBody>().xDrive;
        if (kneeRollMotor != null)
        {
            kneeRollMotor.target = jointAngleTargets[2];
        }
        else
        {
            Debug.Log("Problem");
        }
        

        hipRollMotor.target = jointAngleTargets[0];
        hipYawMotor.target = jointAngleTargets[1];
        

        gameObject.GetComponent<ArticulationBody>().xDrive = hipRollMotor;
        gameObject.GetComponent<ArticulationBody>().yDrive = hipYawMotor;
        knee.GetComponent<ArticulationBody>().yDrive = kneeRollMotor;

        */
        GetComponent<ArticulationBody>().SetDriveTargets(jointAngleTargets);
    }
}
