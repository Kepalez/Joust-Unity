using System;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] float _boundary = 6.6f;

    void Update(){
        if(Mathf.Abs(transform.position.x) > _boundary){
            Teleport();
        }
    }

    private void Teleport()
    {
        var position = transform.position;
        position.x = transform.position.x > 0 ? -6.4f : 6.4f;
        transform.position = position;
        //transform.eulerAngles = transform.position.x > 0 ? new Vector3(0f,180f,0f) : Vector3.zero;
    }
}
