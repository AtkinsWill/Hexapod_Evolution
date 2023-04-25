using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg_Control : MonoBehaviour
{

    public Vector3 jointAngleTargets;
    public Vector3 limbDimensions;
    //public GameObject hip;
    public GameObject knee;


    // Start is called before the first frame update
    void Start()
    {
        jointAngleTargets = new Vector3(0.0f, 0.0f, 0.0f);
        limbDimensions = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        jointAngleTargets += new Vector3(0.1f, 0.1f, 0.1f);
        Vector3 hipEuler = new Vector3(transform.localRotation.x + jointAngleTargets[0], transform.localRotation.y + jointAngleTargets[1], transform.localRotation.z);
        Vector3 kneeEuler = new Vector3(jointAngleTargets[2], 0, 0);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(hipEuler), 0.5f);
        knee.transform.localRotation = Quaternion.RotateTowards(knee.transform.localRotation, Quaternion.Euler(kneeEuler), 0.5f);



    }
}
