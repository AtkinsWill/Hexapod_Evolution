using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation_Control : MonoBehaviour
{
    public int numHexapods;
    public GameObject[] hexapods;
    public GameObject hexapodPrefab;
    float angle;
    public float[] weights;
    public float simulationTime;
    public float currentTime;
    

    // Start is called before the first frame update
    void Start()
    {
        hexapods = new GameObject[numHexapods];
        weights = new float[numHexapods];
        for (int i = 0; i < numHexapods; i++)
        {
            weights[i] = i + 1;
        }
        
        spawnHexapods();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        
        if (currentTime > simulationTime)
        {
            evaluate();
        }
        else
        {
            runIteration();
        }
        

    }

    void runIteration()
    {
        angle += 0.0001f;
        int i = 0;
        foreach (GameObject hexapod in hexapods)
        {
            hexapod.GetComponent<Body_Control>().setJointTargets(angle * weights[i]);
            i++;
        }
    }

    void evaluate()
    {
        destroyHexapods();
        for (int i = 0; i < numHexapods; i++)
        {
            Debug.Log(weights);
            weights[i] = Random.Range(1f, 10);
        }

        spawnHexapods();

    }

    void spawnHexapods()
    {
        for (int i = 0; i < numHexapods; i++)
        {
            GameObject hexapod = Instantiate(hexapodPrefab, new Vector3(0, 3, (float)i * 15), Quaternion.identity) as GameObject;
            hexapods[i] = hexapod;
        }
        angle = 0;
        currentTime = 0;


    }

    void destroyHexapods()
    {
        for (int i = 0; i < numHexapods; i++)
        {
            Destroy(hexapods[i]);
        }
    }
}
