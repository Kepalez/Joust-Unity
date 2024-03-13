using System;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] TMP_Text _score;
    [SerializeField] Transform _playerLivesContainer;
    [SerializeField] GameObject _playerLifePrefab;


    void Start(){
        ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
        ScoreManager.Instance.OnLivesChanged += OnLivesChanged;
        
    }

    void OnDisable(){
        ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
        ScoreManager.Instance.OnLivesChanged -= OnLivesChanged;
        RemoveExtraPlayerLifeIcons(0);
    }

    private int RemoveExtraPlayerLifeIcons(int v)
    {
        int childCount = _playerLivesContainer.childCount;
        while(childCount > v){
            childCount--;
            Destroy(_playerLivesContainer.GetChild(childCount).gameObject);
        }
        return childCount;
    }

    void AddMissingPlayerLifeIcons(int lives, int childCount){
        while(childCount < lives){
            
            Instantiate(_playerLifePrefab,_playerLivesContainer);
            childCount++;
        }
    }
    private void OnLivesChanged(int lives,bool loose)
    {
        var childCount = RemoveExtraPlayerLifeIcons(lives);
        AddMissingPlayerLifeIcons(lives,childCount);
    }

    private void OnScoreChanged(int score)
    {
        _score.text = score.ToString();
    }
}
