using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexapod_Control : MonoBehaviour
{
    public GameObject hexapod;
    private Body_Control body_control;

    public GameObject goal;

    private const int NumberOfSets = 6;
    private const int NeuronsPerSet = 5;
    private const int AngleInputs = 3;
    //private const int Threshold = 0;
    private double[,] neuronStates = new double[6, 5] { { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, 
                                                        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f } };
    public float[,] normalisedJointPositions = new float[6, 3] {   { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, 
                                                                { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
    //public float[,] feelers = new float[6, 2] { { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f },
    //                                           { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f } };

    private double[,,] intraNeuronWeights;
    private double[,] exoNeuronWeights;
    private double[,,] inputWeights;

    //public bool runSim = false;

    public float normalisedDistanceToGoal;
    public float normalisedAngleToGoal;

    private float maximumJointAngle = Mathf.PI/4;

    // Start is called before the first frame update
    void Start()
    {
        

    }
    /*
    private void Update()
    {
        if (runSim)
        {
            body_control = hexapod.GetComponent<Body_Control>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    currentJointPositions[i, j] += 0.0001f;     
                }

            }
            body_control.currentJointPositions = currentJointPositions;

            distanceToGoal = body_control.transform.position - goal.transform.position;
            Debug.Log(distanceToGoal);
        }
    }
    */
    // Update is called once per frame
    
    void Update()
    {
        Debug.Log("This is in hexapod control update");
        Debug.Log(gameObject.GetInstanceID());
        Debug.Log(intraNeuronWeights[2, 2, 2]);
        normalisedJointPositions = getNormalisedJointPositions();
        //print(normalisedJointPositions[1, 1]);
        normalisedDistanceToGoal = getNoramlisedDistanceToGoal();
        normalisedAngleToGoal = getNormalisedAngleToGoal();
        //iterate through each set
        for (int i = 0; i < NumberOfSets; i++)
        {
            //get angle input for that leg


            //for each neuron in that set
            for (int j = 0; j < NeuronsPerSet; j++)
            {
                //sum set to 0
                double weightedSum = 0;



                //Intra connections
                for (int k = 0; k < NeuronsPerSet; k++)
                {
                    //prevents self connections
                    if (j != k)
                    {
                        weightedSum += neuronStates[i, j] * intraNeuronWeights[i, j, k];
                    }
                }

                //Exo connections
                for (int k = 0; k < NumberOfSets; k++)
                {
                    //prevents self connections
                    if (j != k)
                    {
                        weightedSum += neuronStates[k, j] * exoNeuronWeights[k, j];
                    }
                }

                //Input connections
                for (int k = 0; k < 5; k++)
                {
                    weightedSum += normalisedJointPositions[j, 0] * inputWeights[i, 0, j];
                    weightedSum += normalisedJointPositions[j, 1] * inputWeights[i, 1, j];
                    weightedSum += normalisedJointPositions[j, 2] * inputWeights[i, 2, j];
                    weightedSum += normalisedAngleToGoal * inputWeights[i, 3, j];
                    weightedSum += normalisedDistanceToGoal * inputWeights[i, 4, j];
                }


                // Apply tanh activation function
                //this is more closer to what the literature uses.
                neuronStates[i, j] = System.Math.Tanh(weightedSum);
               // Debug.Log(string.Format("{0} {1}", i, j));
               // Debug.Log(neuronStates[i, j]);
            }
        }
        setJointTargets();
    }

    public float getNormalisedAngleToGoal()
    {
        Vector3 vector3ToGoal = goal.transform.position - hexapod.transform.position;
        Vector2 vector2ToGoal = new Vector2(vector3ToGoal.x, vector3ToGoal.z);
        float rot = hexapod.transform.rotation.y;
        Vector2 facing = new Vector2(Mathf.Cos(rot * Mathf.Deg2Rad), Mathf.Sin(rot * Mathf.Deg2Rad));

        float angleToGoal = Vector2.SignedAngle(vector2ToGoal, facing);

        float normalisedAngle = angleToGoal / 180;

        return normalisedAngle;
    }


    public float getNoramlisedDistanceToGoal()
    {
        Vector3 vector3ToGoal = goal.transform.position - hexapod.transform.position;

        float distanceToGoal = Mathf.Sqrt((vector3ToGoal.x * vector3ToGoal.x) + (vector3ToGoal.z * vector3ToGoal.z));

        float normalisedDistance = distanceToGoal / 25;
        if (normalisedDistance > 1)
        {
            normalisedDistance = 1;
        }
        if (normalisedDistance < -1)
        {
            normalisedDistance = -1;
        }

        return normalisedDistance;
    }

    public float[,] getNormalisedJointPositions()
    {
        float[,] jointAngles = hexapod.GetComponent<Body_Control>().currentJointTargets;
        float[,] normalisedJointAngles = new float[6,3]{   { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f },
                                                           { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                normalisedJointAngles[i, j] = jointAngles[i, j] / maximumJointAngle;

            }
        }

        return normalisedJointAngles;
    }

    public void setJointTargets()
    {
        body_control = hexapod.GetComponent<Body_Control>();
        for (int i = 0; i < 6; i++)
        {
            int k = 0;
            for (int j = 0; j < 3; j++)
            {
                body_control.currentJointTargets[i, j] = (float)neuronStates[i, k] * maximumJointAngle;
                //Debug.Log("Joint angle");
                
               // Debug.Log(neuronStates[i, k]);
               // Debug.Log(maximumJointAngle);
               // Debug.Log(body_control.currentJointTargets[i, j]);
                k += 2;
            }
        }

    }


    public void setIntraNeuronWeights(double[,,] intraNW) {
        Debug.Log(gameObject.GetInstanceID());
        Debug.Log(intraNW[2, 2, 2]);
        intraNeuronWeights = intraNW;
    }
    public void setExoNeuronWeights(double[,] exoNW)
    {
        exoNeuronWeights = exoNW;
    }
    public void setInputWeights(double[,,] intputW)
    {
        inputWeights = intputW;
    }
    public double[,,] getIntraNeuronWeights()
    {
        return intraNeuronWeights;
    }
    public double[,] getExoNeuronWeights()
    {
        return exoNeuronWeights;
    }
    public double[,,] getInputWeights()
    {
        return inputWeights;
    }


}
