using System;
using UnityEngine;

public class JoustResolver : MonoBehaviour
{
    [SerializeField] Egg _eggprefab;


    bool _dead;

    void OnCollisionEnter2D(Collision2D other){
        if(_dead) return;
        var opponent = other.collider.gameObject.GetComponentInParent<JoustResolver>();
        if(!opponent) return;

        if(!LostJoust(opponent)) return;

        _dead = true;

        if(RemovedEnemy()) return;
        ScoreManager.Instance.KillPlayer(this.gameObject);
        }

    bool RemovedEnemy()
    {
        if(!TryGetComponent<EnemyInputManager>(out var enemyInputManager)) return false;
        SpawnEgg();
        enemyInputManager.enabled = false;
        FindObjectOfType<Enemymanager>().RemoveEnemy(enemyInputManager);
        return true;
    }

    private void SpawnEgg()
    {
        if(!_eggprefab) return;
        var egg = Instantiate(_eggprefab,transform.position,Quaternion.identity);
        if(!egg.TryGetComponent<Rigidbody2D>(out var rb)) return;
        var forceDirection = (transform.eulerAngles == Vector3.zero) ? Vector3.right : Vector3.left;
        rb.AddForce(forceDirection * 2f,ForceMode2D.Impulse);
    }

    bool LostJoust(JoustResolver opponent)
    {
        return (opponent.transform.position.y - transform.position.y > 0.25f);
    }
}
