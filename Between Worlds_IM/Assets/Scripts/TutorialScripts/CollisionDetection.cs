using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private MobileUnit _mobileUnit;

    // Start is called before the first frame update
    private void Start()
    {
        _mobileUnit = transform.parent.GetComponent<MobileUnit>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_mobileUnit._reachedTarget) { return; }
        GameObject go = other.gameObject;
        if (go.tag != Manager.Instance.AgentTag) { return; }
        _mobileUnit.StartConfigure(go);
    }
}