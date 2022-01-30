using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    private bool _prefab = false;
    public GameObject Prefab1;
    public GameObject Prefab2;
    public GameObject[] Prefabs;
    public int MakeLimit = 6; //maximum agents before destruction
    private int _makeCount = 0; //each time we make an agent, add to count
    public GameObject Target;

    public float MakeRate = 2.0f;

    private float _lastMake = 0;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //guard statement
        if (Target == null) { return; }

        //destroy factory when limit reached
        if (_makeCount >= MakeLimit)
        {
            //Destroy(gameObject);
        }

        _lastMake += Time.deltaTime; //_lastMake = _lastMake + Time.deltaTime;
        if (_lastMake > MakeRate)
        {
            //Debug.Log("Make");
            _lastMake = 0; //reset time counter
            _makeCount++; //increase agent make count by one

            GameObject prefab = Prefabs[Random.Range(0, Prefabs.Length)]; //random prefab
            GameObject go = Instantiate(prefab, this.transform.position, Quaternion.identity);
            MobileUnit mu = go.GetComponent<MobileUnit>();
            mu.Target = Manager.Instance.GetTarget();
            mu._factory = this;
        }
    }

    public void DestroyFactory()
    {
        Manager.Instance.RemoveTargetFromList(Target);
        Destroy(this.gameObject);
    }
}