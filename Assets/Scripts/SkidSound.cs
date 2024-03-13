using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidSound : MonoBehaviour
{
    public void PlaySkidSound(){
        SoundManager.Instance.PlaySkidSound();
    }
}
