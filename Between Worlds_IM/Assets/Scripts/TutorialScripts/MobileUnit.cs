using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class MobileUnit : MonoBehaviour
{
    public GameObject Target;
    public GameObject[] Positions;

    [HideInInspector]
    public NavMeshAgent Agent;

    [HideInInspector]
    public Factory _factory; //used to tell this factory when it has successflly made another factory

    private Dictionary<GameObject, GameObject> Configure = new Dictionary<GameObject, GameObject>();

    [HideInInspector]
    public bool _reachedTarget = false;

    // Start is called before the first frame update
    private void Start()
    {
        Agent = this.GetComponent<NavMeshAgent>();
        Agent.SetDestination(Target.transform.position);
        foreach (GameObject pos in Positions)
        {
            Configure.Add(pos, null);
        }
    }

    public void StartConfigure(GameObject go)
    {
        Debug.Log("StartConfigure");
        List<GameObject> keys = Configure.Keys.ToList();
        foreach (GameObject key in keys)
        {
            //guard statement
            if (Configure[key] != null) { continue; } //go to the next loop iteration
            Configure[key] = go;
            go.GetComponentInParent<NavMeshAgent>().enabled = false;
            go.GetComponentInParent<MobileUnit>().enabled = false;
            go.GetComponent<CollisionDetection>().enabled = false;
            break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_reachedTarget)
        {
            bool allConfig = true;
            foreach (KeyValuePair<GameObject, GameObject> kvp in Configure)
            {
                //guard statement
                if (kvp.Value == null) { allConfig = false; continue; }
                if (kvp.Key.transform.position == kvp.Value.transform.position) { continue; }

                //move object into position
                GameObject cAgent = kvp.Value;
                Vector3 pos = kvp.Key.transform.position;
                Quaternion rot = kvp.Key.transform.rotation;
                cAgent.transform.position = Vector3.Lerp(cAgent.transform.position, pos, Time.deltaTime);
                cAgent.transform.rotation = Quaternion.Lerp(cAgent.transform.rotation, rot, Time.deltaTime);
                if (Vector3.Distance(cAgent.transform.position, pos) < 0.05f)
                {
                    cAgent.transform.position = pos;
                    cAgent.transform.rotation = rot;
                }
                allConfig = false;
            }

            if (allConfig) //if each agent is in position
            {
                List<GameObject> keys = Configure.Keys.ToList(); //get dictionary keys as list
                foreach (GameObject key in keys)
                {
                    GameObject go = Configure[key]; //get agent associated with this key
                    Destroy(go); //delete all agents in configure positions
                }

                if (_factory != null) { _factory.DestroyFactory(); }
                Manager.Instance.InstantiateFactory(Target);
                Destroy(gameObject); //destroy the agent this script is attached to
            }

            return; //exit so we don't do any more code in Update()
        }

        Debug.DrawLine(this.transform.position, Target.transform.position, Color.black);
        Debug.DrawRay(this.transform.position, this.transform.forward, Color.red);
        HasReachedTarget();
    }

    public void UpdateTarget(GameObject target)
    {
        //guard statement
        if (Agent.enabled == false) { return; } //if agent is disable just exit method

        Target = target;
        Agent = this.GetComponent<NavMeshAgent>();
        Agent.SetDestination(Target.transform.position);
    }

    public virtual void HasReachedTarget()
    {
        //test if agent has reached target
        if (!Agent.pathPending) //if agent is looking for target it hasn't reached target
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance) //agent is as close as it can get
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f) //if agent isn't moving
                {
                    Debug.Log("Target Reached!!!");
                    _reachedTarget = true;
                    Agent.enabled = false;
                }
            }
        }
    }
}