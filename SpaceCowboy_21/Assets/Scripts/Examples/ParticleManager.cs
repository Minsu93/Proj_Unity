using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem[] effects;
    ParticlePool[] particlePool;

    void Start()
    {

        if (effects.Length <= 0)
            return; 

        particlePool = new ParticlePool[effects.Length];
        for(int i = 0; i < effects.Length; i++)
        {
            particlePool[i] = new ParticlePool(effects[i], this.transform, 5);
        }

    }

    //리스트에서 순서 찾기
    public void GetParticle(ParticleSystem particle, Vector3 pos, Quaternion rot)
    {
        int index = 0;

        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i] == particle)
            {
                index = i;
            }
        }

        playParticle(index, pos, rot);
    }

    public void GetParticle(ParticleSystem particle, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        int index = 0;

        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i] == particle)
            {
                index = i;
            }
        }

        playParticle(index, pos, rot, scale);
    }

    //해당 파티클 출력하기
    void playParticle(int index, Vector3 particlePos, Quaternion particleRot)
    {
        ParticleSystem particleToPlay = particlePool[index].getAvailabeParticle();

        if (particleToPlay != null)
        {
            if (particleToPlay.isPlaying)
                particleToPlay.Stop();

            particleToPlay.transform.position = particlePos;
            particleToPlay.transform.rotation = particleRot;
            particleToPlay.Play();
        }

    }
    void playParticle(int index , Vector3 particlePos, Quaternion particleRot, Vector3 particleScale)
    {
        ParticleSystem particleToPlay = particlePool[index].getAvailabeParticle();

        if (particleToPlay != null)
        {
            if (particleToPlay.isPlaying)
                particleToPlay.Stop();

            particleToPlay.transform.position = particlePos;
            particleToPlay.transform.rotation = particleRot;
            particleToPlay.transform.localScale = particleScale;
            particleToPlay.Play();
        }

    }
}
