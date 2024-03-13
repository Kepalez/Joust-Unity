using System;
using UnityEngine;

public class ScoreManager : SingletonMonoBehavior<ScoreManager>
{
    public event Action<int> OnScoreChanged = delegate(int i){};
    public event Action<int,bool> OnLivesChanged = delegate(int i, bool loose){};

    public static int Score;
    public int Lives;
    public int Wave;
    [SerializeField] int _eggValue = 250, killValue = 500, _extraLifeInterval = 20000;
    [SerializeField] EggCollectorEffect _eggCollectorPrefab;

    int _enemiesKilled, _nextExtraLifeAt;

    public void ResetScore(){
        Wave = 0;
        Lives = 3;
        Score = 0;
        _enemiesKilled = 0;
        _nextExtraLifeAt = _extraLifeInterval;
        OnScoreChanged(Score);
        OnLivesChanged(Lives,false);
        NextWave();
    }

    public void ScoreEgg(Vector3 position){
        var effect = Instantiate(_eggCollectorPrefab,position,Quaternion.identity);
        effect.GetComponent<TextMesh>().text = _eggValue.ToString();
        SoundManager.Instance.PlayEggSound();
        AddPoints(_eggValue);
    }

    public void ScoreKill(){
        SoundManager.Instance.PlayKillSound();
        AddPoints(++_enemiesKilled*killValue);
    }

    public void KillPlayer(GameObject player){
        SoundManager.Instance.PlayKillSound();
        Destroy(player);
        LoseLife();
    }

    private void LoseLife()
    {
        Lives--;
        OnLivesChanged(Lives,true);
    }

    private void AddPoints(int points)
    {
        Score += points;
        OnScoreChanged(Score);
        if(Score >= _nextExtraLifeAt){
            GainLife();
            
        }
    }

    private void GainLife()
    {
        Lives++;
        _nextExtraLifeAt += _extraLifeInterval;
        OnLivesChanged(Lives,false);
    }

    public void NextWave()
    {
        ++Wave;
        _enemiesKilled = 0;
    }

}
