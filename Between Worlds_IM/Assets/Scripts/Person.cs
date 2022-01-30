using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Person : MonoBehaviour
{
    public string[] TargetTypes = new string[] { "PARK", "CORNERS" };
    public GameObject[] Targets;
    public float ReachedTargetDistance = 3;
    public bool ShuffleTargets = true;

    [HideInInspector]
    public NavMeshAgent Agent;

    [HideInInspector]
    public bool _reachedTarget = false;

    private int t = 0;
    private GameObject target = null;
    private Vector3 position;

    // Start is called before the first frame update
    private void Start()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        //grab targets using tags
        if (Targets.Length == 0)
        {
            //get all game objects tagged with "Target"
            Targets = GameObject.FindGameObjectsWithTag("Target");

            //make new list of targest that has the correlating target type name
            List<GameObject> targetList = new List<GameObject>();
            foreach (GameObject go in Targets) //search all "Target" game objects
            {
                foreach (string targetName in TargetTypes)
                {
                    if (go.name.Contains(targetName)) //if GameObject has the same name as targetName, add to list
                    {
                        targetList.Add(go);
                    }
                }
            }
            Targets = targetList.ToArray(); //Convert List to Array, because other code is still using array
        }

        //shuffle targets
        if (ShuffleTargets) { Targets = Shuffle(Targets); }

        //Setup Agent
        Agent = GetComponent<NavMeshAgent>(); //set the agent variable to this game object's navmesh
        t = 0;
        target = Targets[t];
        Agent.SetDestination(target.transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        //only continue if Agent is working
        if (!Agent.enabled) { return; }

        //if target has moved, let's update the Agent's position
        if (target.transform.position != position)
        {
            position = target.transform.position;
            Agent.SetDestination(position);
        }

        //see agent's next destination
        //Debug.DrawLine(transform.position, Agent.steeringTarget, Color.black);
        //Debug.DrawLine(transform.position, Agent.pathEndPosition, Color.cyan);
        //Debug.DrawRay(Agent.pathEndPosition, Vector3.up * 40, Color.red);
        //Debug.DrawRay(target.transform.position, Vector3.up * 40, Color.yellow);

        //change target once it is reached
        float distanceToTarget = Vector3.Distance(Agent.transform.position, target.transform.position);
        if (ReachedTargetDistance > distanceToTarget) //have we reached our target
        {
            t++;
            if (t == Targets.Length) { t = 0; }
            target = Targets[t];
            Agent.SetDestination(target.transform.position);
        }

        //make sure agent has path
        //Debug.Log(gameObject.name + " : " + Agent.hasPath);
        if (!Agent.hasPath) //cath agent error when agent doesn't resume
        {
            position = target.transform.position;
            Agent.SetDestination(position);
        }

        //make sure path can be reached
        NavMeshPath navMeshPath = new NavMeshPath();
        //create path and check if it can be done
        // and check if navMeshAgent can reach its target
        /*/
        if (Agent.CalculatePath(target.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            Agent.SetPath(navMeshPath);
        }
        else
        {
            //Fail condition here
        }
        //*/
    }

    /// <summary>
    /// Randomly Shuffle Array of GameObjects
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    private GameObject[] Shuffle(GameObject[] objects)
    {
        GameObject tempGO;
        for (int i = 0; i < objects.Length; i++)
        {
            //Debug.Log("i: " + i);
            int rnd = Random.Range(0, objects.Length);
            tempGO = objects[rnd];
            objects[rnd] = objects[i];
            objects[i] = tempGO;
        }
        return objects;
    }
}