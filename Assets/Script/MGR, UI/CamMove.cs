using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    [SerializeField] private GameObject Target;


    void LateUpdate()
    {
        if(Target.transform.position.y >= 0)
            transform.position = new Vector3(0, Target.transform.position.y, -10);
    }
}
