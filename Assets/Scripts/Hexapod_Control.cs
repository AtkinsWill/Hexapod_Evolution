using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class Hexapod_Control : MonoBehaviour
{
    public GameObject hexapod;
    private Body_Control body_control;

    public GameObject goal;

    private const int NumberOfSets = 6;
    private const int NeuronsPerSet = 15;
    private const int AngleInputs = 3;
    //private const int Threshold = 0;
    private double[,] neuronStates = new double[6, 15] { 
        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f}, 
        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f}, 
        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,  0.0f, 0.0f, 0.0f },
    };
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
    public float currentTime;
    public float val;
    public float theta = 0.5f;
    private float maximumJointAngle = Mathf.PI / 6f;
    private float maximumJointVelocity = 1; 
    public float timeStep = 1f;



    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;

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


    public float getNormalisedDistanceToGoal()
    {
        Vector3 vector3ToGoal = goal.transform.position - hexapod.transform.position;

        float distanceToGoal = Mathf.Sqrt((vector3ToGoal.x * vector3ToGoal.x) + (vector3ToGoal.z * vector3ToGoal.z));

        float normalisedDistance = ((distanceToGoal / 45f) - 1f) * -1f;
        if (normalisedDistance > 1)
        {
            normalisedDistance = 1;
        }
        if (normalisedDistance < -1)
        {
            normalisedDistance = -1;
        }
        //print(normalisedDistance);
        return normalisedDistance;
    }

    public float[,] getNormalisedJointPositions()
    {
        float[,] jointAngles = hexapod.GetComponent<Body_Control>().currentJointTargets;
        float[,] normalisedJointAngles = new float[6, 3]{   { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f },
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
               // print((float)neuronStates[i, k]);
                body_control.currentJointTargets[i, j] = (float)neuronStates[i, k] * maximumJointAngle;
                //Debug.Log("Joint angle");

                // Debug.Log(neuronStates[i, k]);
                // Debug.Log(maximumJointAngle);
                // Debug.Log(body_control.currentJointTargets[i, j]);
                k += 2;
            }
        }

    }


    public void setIntraNeuronWeights(double[,,] intraNW)
    {
       // Debug.Log(gameObject.GetInstanceID());
       // Debug.Log(intraNW[2, 2, 2]);
        intraNeuronWeights = intraNW;
        //PrintWeightsCSV(intraNW, "CSVdataOutput/something" + intraNeuronWeights[2, 2, 2] + ".csv");

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
       // Debug.Log(gameObject.GetInstanceID());
      //  Debug.Log(intraNeuronWeights[2, 2, 2]);
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



    void Update()
    {
        currentTime += Time.deltaTime;
        //   Debug.Log("This is in hexapod control update");
        //   Debug.Log(gameObject.GetInstanceID());
        //   Debug.Log(intraNeuronWeights[2, 2, 2]);
        // Add a matrix to store the membrane potentials for each neuron
        double[,] membranePotentials = new double[NumberOfSets, NeuronsPerSet];

        normalisedJointPositions = getNormalisedJointPositions();
        //print(normalisedJointPositions[1, 1]);
        normalisedDistanceToGoal = getNormalisedDistanceToGoal();
        normalisedAngleToGoal = getNormalisedAngleToGoal();
        // Introduce a time step for updating the membrane potentials

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
                //for (int k = 0; k < 5; k++)
                //{
                weightedSum += (normalisedJointPositions[i, 0] + 1) * inputWeights[i, 0, j];
                weightedSum += (normalisedJointPositions[i, 1] + 1) * inputWeights[i, 1, j];
                weightedSum += (normalisedJointPositions[i, 2] + 1) * inputWeights[i, 2, j];
                //weightedSum += (normalisedAngleToGoal + 1) * inputWeights[i, 3, j];
                weightedSum += (normalisedDistanceToGoal + 1) * inputWeights[i, 4, j];
                val = Mathf.Sin(4f * currentTime / 3.14f);
                weightedSum += (val) * inputWeights[i, 3, j];

                //}


                // Apply tanh activation function
                //this is more closer to what the literature uses.
                ////neuronStates[i, j] = System.Math.Tanh(weightedSum);
                // Debug.Log(string.Format("{0} {1}", i, j));
                // Debug.Log(neuronStates[i, j]);
                //   print("weighted sum");
                //   print(weightedSum);
                // Update the membrane potential for neuron i, j using Hopfield dynamics
                membranePotentials[i, j] += timeStep * (-membranePotentials[i, j] + weightedSum);
            //    print("membrane");
            //    print(membranePotentials[i, j]);
            //    print("theta");
            //    print(theta);
             //   print("calc");

                // Calculate the neuron state using the tanh function and the membrane potential (sigmoid)
                neuronStates[i, j] = System.Math.Tanh(membranePotentials[i, j] - theta);
                //print(neuronStates[i, j]);
                //neuronStates[i, j] = 1f/(1f+ Math.Exp( theta - membranePotentials[i, j]));
                //PrintWeightsCSV2D(neuronStates, "CSVdataOutput/something" + neuronStates[ 2, 2] + ".csv");
            }
        }
        setJointTargets();
    }

    public void resetTime()
    {
        currentTime = 0;
    }

    void PrintWeightsCSV(double[,,] weights, string fileName)
    {
        int dim1 = weights.GetLength(0);
        int dim2 = weights.GetLength(1);
        int dim3 = weights.GetLength(2);

        using (StreamWriter writer = new StreamWriter(fileName))
        {
            for (int i = 0; i < dim1; i++)
            {
                writer.WriteLine($"Set {i + 1}:");

                for (int j = 0; j < dim2; j++)
                {
                    string row = "";

                    for (int k = 0; k < dim3; k++)
                    {
                        row += weights[i, j, k].ToString("F2");

                        // Add a comma separator between values, except for the last value in the row
                        row += (k < dim3 - 1) ? "," : "";
                    }

                    writer.WriteLine(row);
                }

                writer.WriteLine("");
            }
        }
    }


    void PrintWeightsCSV2D(double[,] weights, string fileName)
    {
        int dim1 = weights.GetLength(0);
        int dim2 = weights.GetLength(1);

        using (StreamWriter writer = new StreamWriter(fileName))
        {
            for (int i = 0; i < dim1; i++)
            {
                string row = "";

                for (int j = 0; j < dim2; j++)
                {
                    row += weights[i, j].ToString("F2");

                    // Add a comma separator between values, except for the last value in the row
                    row += (j < dim2 - 1) ? "," : "";
                }

                writer.WriteLine(row);
            }
        }
    }

}