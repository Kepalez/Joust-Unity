using System.Collections;
using UnityEngine;

public class EggCollectorEffect : MonoBehaviour
{
    [SerializeField] TextMesh _renderer;
    [SerializeField] float _fadeDuration = 2f, _fadeDelay = 0.25f;

    IEnumerator Start(){
        yield return new WaitForSeconds(_fadeDelay);
        var color = _renderer.color;
        while(color.a > 0.0f){
            var fadeSpeed = 1.0f/ _fadeDuration;
            color.a -= fadeSpeed * Time.deltaTime;
            _renderer.color = color;
            transform.position += Vector3.up * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

}
