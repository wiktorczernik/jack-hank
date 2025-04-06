using System.Collections;
using UnityEngine;

public class ExplosionVisuals : MonoBehaviour
{
    [SerializeField] KeyCode key;
    [SerializeField] ParticleSystem[] _particles;
    [SerializeField] float[] _delays;
    [SerializeField] AnimationCurve[] _dissolveSliders1;
    [SerializeField] AnimationCurve[] _dissolveSliders2;

    IEnumerator Dissolve(Material material, float maxTime, AnimationCurve dissolveSlider1, AnimationCurve dissolveSlider2)
    {
        float time = 0;
        while (time < maxTime)
        {
            time += Time.deltaTime;
            material.SetFloat("_DissolveSlider1", dissolveSlider1.Evaluate(time / maxTime));
            material.SetFloat("_DissolveSlider2", dissolveSlider2.Evaluate(time / maxTime));
            yield return new WaitForEndOfFrame();
        }
        material.SetFloat("_DissolveSlider1", 1);
        material.SetFloat("_DissolveSlider2", 1);
    }
    IEnumerator PlayParticle(int index)
    {
        var particle = _particles[index];
        var delay = _delays[index];
        var dissolveSlider1 = _dissolveSliders1[index];
        var dissolveSlider2 = _dissolveSliders2[index];

        yield return new WaitForSeconds(delay);
        particle.Play();
        var renderer = particle.GetComponent<ParticleSystemRenderer>();
        StartCoroutine(Dissolve(renderer.material, particle.main.startLifetime.constantMin, dissolveSlider1, dissolveSlider2));
    }
    
    public void Init()
    {
        for (int i = 0; i < _particles.Length; ++i)
        {
            StartCoroutine(PlayParticle(i));
        }
    }
}
