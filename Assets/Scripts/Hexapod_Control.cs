using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexapod_Control : MonoBehaviour
{
    public GameObject hexapod;
    private Body_Control body_control;

    public GameObject objective;

    private const int NumberOfSets = 6;
    private const int NeuronsPerSet = 5;
    private const int AngleInputs = 3;
    //private const int Threshold = 0;
    private double[,] neuronStates = new double[6, 3] { { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
    public double[,,] intraNeuronWeights;
    public double[,] exoNeuronWeights;
    public double[,,] inputWeights;
    public float[,] angleInputs;

    public bool runSim = false;
    public float[,] actualJointTargets = new float[6, 3] { { 0.0f, 0.0f, 0.0f}, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f }, { 0.0f, 0.0f, 0.0f } };
    public float[,] feelers = new float[6, 2] { { 0.0f, 0.0f}, { 0.0f,0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f }, { 0.0f, 0.0f } };


    public Vector3 distanceToObjective;

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

                    actualJointTargets[i, j] += 0.0001f;     
                }

            }
            body_control.actualJointTargets = actualJointTargets;

            distanceToObjective = body_control.transform.position - objective.transform.position;
            Debug.Log(distanceToObjective);
        }
    }
    */
    // Update is called once per frame
    
    void Update()
    {

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
                        weightedSum += neuronStates[i,j] * intraNeuronWeights[i,j,k];
                    }
                }

                //Exo connections
                for (int k = 0; k < NumberOfSets; k++)
                {
                    //prevents self connections
                    if (j != k)
                    {
                        weightedSum += neuronStates[k,j] * exoNeuronWeights[k,j];
                    }
                }

                //Input connections
                for (int k = 0; k < 5; k++)
                {
                weightedSum += actualJointTargets[j, 0] * inputWeights[i, 0, j];
                weightedSum += actualJointTargets[j, 1] * inputWeights[i, 1, j];
                weightedSum += actualJointTargets[j, 2] * inputWeights[i, 2, j];
                weightedSum += feelers[j, 0] * inputWeights[i, 3, j];
                weightedSum += feelers[j, 1] * inputWeights[i, 4, j];

                }


                // Apply tanh activation function
                //this is more closer to what the literature uses.
                neuronStates[i,j] = System.Math.Tanh(weightedSum);
            }
        }
    }
    
    public void setJointTargets(double targetJoints)
    {
        for (int i = 0; i < NumberOfSets; i++)
        {
            for (int j = 0; j < NeuronsPerSet; j += 2)
            {
                body_control.actualJointTargets[i, j] = neuronStates[i][j];
            }
        }

    }
}
