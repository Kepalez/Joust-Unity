using System;
using UnityEngine;

public class EggCollector : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other){
        Debug.Log("Huevo");
        if(IsEgg(other, out var egg)){
            CollectEgg(egg);
        }
    }

    private void CollectEgg(Egg egg)
    {
        ScoreManager.Instance.ScoreEgg(egg.transform.position);
        Destroy(egg.gameObject);
    }

    private bool IsEgg(Collision2D other, out Egg egg)
    {
        egg = other.collider.gameObject.GetComponentInParent<Egg>();
        return egg;
    }
}
