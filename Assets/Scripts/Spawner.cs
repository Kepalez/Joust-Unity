using System.Collections; 
using UnityEngine;

public class Spawner : MonoBehaviour{
    [SerializeField] BirdMover _playerPrefab, _enemyPrefab;
    [SerializeField] AudioClip _spawnPlayerSound, _spawnEnemySound;
    [SerializeField] SpriteRenderer _renderer;

    [ContextMenu("Spawn Player")]
    public void SpawnPlayer(){
        SoundManager.Instance.PlayAudioClip(_spawnPlayerSound);
        var _player = Instantiate(_playerPrefab,transform.position,Quaternion.identity);
        _player.Init(Random.Range(0f,100f) > 50f ? Vector3.right : Vector3.left);
        transform.localScale *= 0.1f;
        StartCoroutine(_player.ShowSpawnEffect(_spawnPlayerSound.length));
        StartCoroutine(ShowSpawnEffect(_spawnPlayerSound.length));
    }

    [ContextMenu("Spawn Enemy")]
    public GameObject SpawnEnemy(){
        SoundManager.Instance.PlayAudioClip(_spawnEnemySound);
        var _enemy = Instantiate(_enemyPrefab,transform.position,Quaternion.identity);
        _enemy.Init(Random.Range(0f,100f) > 50f ? Vector3.right : Vector3.left);
        transform.localScale *= 0.1f;
        StartCoroutine(_enemy.ShowSpawnEffect(_spawnEnemySound.length));
        StartCoroutine(ShowSpawnEffect(_spawnEnemySound.length));
        return _enemy.gameObject;
    }


    IEnumerator ShowSpawnEffect(float spawnLength){
        float time = spawnLength;
        while(time >= 0){
            time -= Time.deltaTime;

            _renderer.material.color = Random.ColorHSV();
            yield return null;
        }
        _renderer.material.color = Color.white;
    }
}
