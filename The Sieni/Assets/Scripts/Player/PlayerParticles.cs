using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField]
    private List<TypedParticle> particles;

    public static PlayerParticles main;

    private void Awake()
    {
        main = this;
    }

    public void PlayParticles(MoveObjectType type)
    {
        particles.Where(x => x.Type == type).FirstOrDefault()?.Particles.Play();
    }

}

[Serializable]
public class TypedParticle
{
    public MoveObjectType Type;
    public ParticleSystem Particles;
}