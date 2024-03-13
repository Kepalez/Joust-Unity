using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] Enemymanager _enemyManager;
    [SerializeField] Spawner _playerSpawner;
    [SerializeField] float _respawnPlayerDelay = 2f;

    public void PlayerMountDespawned(){
        if(ScoreManager.Instance.Lives > 0){
            _playerSpawner.SpawnEnemy();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayStartSound();
        StartGame();   
    }

    void StartGame(){
        if(!FindObjectOfType<PlayerInputManager>()){
            _playerSpawner.SpawnPlayer();
        }
        ScoreManager.Instance.ResetScore();
        SubsribeToEvents();
        StartWave();
    }

    private void SubsribeToEvents()
    {
        ScoreManager.Instance.OnLivesChanged += OnLivesChanged;
        _enemyManager.EnemyDestroyed += OnEnemyDestroyed;
    }

    void UnsubscribeToEvents(){
        ScoreManager.Instance.OnLivesChanged -= OnLivesChanged;
        _enemyManager.EnemyDestroyed -= OnEnemyDestroyed;
    }

    private void OnEnemyDestroyed()
    {
        if(_enemyManager.EnemiesRemaining < 1){
            NextWave();
        }
    }

    void StartWave(){
        _enemyManager.SpawnEnemies(ScoreManager.Instance.Wave * 2 + 1);
    }

    private void NextWave()
    {
        ScoreManager.Instance.NextWave();
        StartWave();
    }

    private void OnLivesChanged(int lives,bool loose)
    {
        if(lives < 1){
            _enemyManager.DestroyAllEnemies();
            Debug.Log("Game Over");
            SceneManager.LoadScene("Game Over",LoadSceneMode.Single);
            return;
        }
        if(loose) StartCoroutine(RespawnPlayer());
    }
    
    IEnumerator RespawnPlayer(){
        yield return new WaitForSeconds(_respawnPlayerDelay);
        _playerSpawner.SpawnPlayer();
    }
}
