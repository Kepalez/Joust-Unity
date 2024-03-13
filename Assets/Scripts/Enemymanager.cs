using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemymanager : MonoBehaviour
{
    public event Action EnemyDestroyed = delegate{ };
    public int EnemiesRemaining;

    [SerializeField] Spawner[] _spawners;
    [SerializeField] float _spawnInterval = 1f;

    float _spawnTimer;
    int _spawner;

    WaitForSeconds _spawnDelay;
    readonly List<EnemyInputManager> _enemies = new List<EnemyInputManager>();

    public void SpawnEnemies(int enemiesToSpawn){
        _spawner = -1;
        StartCoroutine(SpawnEnemiesWithDelay(enemiesToSpawn));
        _spawnDelay = new WaitForSeconds(_spawnInterval);
    }

    public void RemoveEnemy(EnemyInputManager enemyInputManager){
        if(!_enemies.Contains(enemyInputManager)) return;
        EnemiesRemaining--;
        SoundManager.Instance.PlayKillSound();
        ScoreManager.Instance.ScoreKill();
        _enemies.Remove(enemyInputManager);
        Destroy(enemyInputManager.gameObject);
        EnemyDestroyed();
    }

    public void DestroyAllEnemies(){
        foreach(var _enemy in _enemies){
            Destroy(_enemy.gameObject);
        }
        _enemies.Clear();
    }

    IEnumerator SpawnEnemiesWithDelay(int enemiesToSpawn){
        EnemiesRemaining+=enemiesToSpawn;
        while(_enemies.Count < enemiesToSpawn){
            yield return _spawnDelay;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        var enemy = NextSpawner().SpawnEnemy();
        enemy.transform.SetParent(transform);
        _enemies.Add(enemy.GetComponent<EnemyInputManager>());
    }

    Spawner NextSpawner(){
        if(++_spawner >= _spawners.Length){
            _spawner = 0;
        }
        return _spawners[_spawner];
    }
}
