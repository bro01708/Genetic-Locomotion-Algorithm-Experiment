using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour

{
   public Transform agent; // reference to agent to be tracked
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float deadzone;
    private GameObject[] agentArray;
    private static CameraController instance;
    private CameraController() { }
    private int bestAgent;
    private float bestPos;

    public static CameraController Instance
    {

        get
        {
            if (instance == null)
            {
                instance = new CameraController();
            }
            return instance;
        }
    }
 
    public void Start()
    {
        AssignAgents();
    }

    public void AssignAgents()
    {
        agentArray = GameObject.FindGameObjectsWithTag("Agent").OrderBy(g => g.transform.GetSiblingIndex()).ToArray();
    }


    void LateUpdate()
    {
        AssignAgents();
        IdentifyFurthestAgent();
        Vector3 targetPos = agent.transform.GetChild(0).transform.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
        transform.position = smoothPos;

    }

    void IdentifyFurthestAgent()
    {
        // set to minus as nothing will appear there anyway, all will be better than that
        float furthestpos = -10;
        //index of agent list containing furthest agent
        int indexOfFurthest = 0;
        for (int i = 0; i < agentArray.Length; i++)
        {
            //Debug.Log(agentArray[i].transform.GetChild(0).transform.position.x);
            if (agentArray[i].transform.GetChild(0).transform.position.x > furthestpos)
            {
                //Debug.Log("New winner");
                indexOfFurthest = i;
                furthestpos = agentArray[i].transform.GetChild(0).transform.position.x;
                if (furthestpos > BestPos)
                {
                    BestAgent = i;
                    BestPos = furthestpos;
                }


            }
        }
        agent = agentArray[indexOfFurthest].transform;
    }

    public GameObject[] AgentArray
    {
        get
        {
            return agentArray;
        }

        set
        {
            agentArray = value;
        }
    }

    public int BestAgent
    {
        get
        {
            return bestAgent;
        }

        set
        {
            bestAgent = value;
        }
    }

    public float BestPos
    {
        get
        {
            return bestPos;
        }

        set
        {
            bestPos = value;
        }
    }
}