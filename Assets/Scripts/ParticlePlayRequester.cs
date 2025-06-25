using UnityEngine;

public class ParticlePlayRequester : MonoBehaviour
{
    public ParticleSystem[] particles;

    public void Play()
    {
        foreach (var particle in particles)
            particle.Play();
    }
    public void Stop()
    {
        foreach (var particle in particles)
            particle.Stop();
    }
}
