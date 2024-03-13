using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public void PlayFootstepSound(){
        SoundManager.Instance.PlayRunSound();
    }
}
