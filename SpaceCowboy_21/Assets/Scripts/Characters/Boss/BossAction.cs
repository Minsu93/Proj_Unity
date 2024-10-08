using DamageNumbersPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAction : MonoBehaviour, ITarget, IHitable, IKickable
{

    //1. 체력은 BossUI에서 읽어서 화면에 출력함. 
    //2. 플레이어 위치를 (필요하다면)감지할 수 있는 EnemyBrain
    //3. Update에서 여러가지 패턴에 따른 행동.
    /// A - 플레이어 주변의 랜덤 장소로 이동한다. 이동을 마치고 플레이어 방향 사격
    /// B - 플레이어 주변의 랜덤 장소로 이동한다. 충전 시간을 조금 가진다. 전방위 사격.
    /// C - 플레이어 주변의 랜덤 장소로 이동한다. 엄폐물을 사방으로 뿌린다. 엄폐물은 플레이어가 닿으면 피해를 주고 사라진다. 총으로 맞춰서 제거할 수 있다. 
    /// D - 전화를 건다. 일반 해적을 2마리 소환한다.  -> 아이템을 확정으로 드랍하는 유닛.
    /// E - (필살기, 조건 : Phase 2) 제자리에서 기를 모은다. 플레이어를 추적하는 로켓을 발사한다. 
    /// 각각의 패턴은 쿨타임이 있다. 
    //4. Die시 웨이브가 클리어된다. 

    bool bossActivate = false;
    Health bossHealth;
    Collider2D coll;
    [SerializeField] GameObject projHitObj;
    [SerializeField] DamageNumber dmgNum;
    [SerializeField] ParticleSystem hitEffect;    //맞았을 때 효과

    public event Action<float> BossUIUpdateEvent;   //보스 HP UI 업데이트용 이벤트 -> BossSpawner 할당
    public event Action BossDieEvent;   //보스 잡았을 때 스테이지 클리어 이벤트 -> BossSpawner 할당

    //보스의 패턴들.
    [Header("Patterns")]
    [SerializeField] AttackPatternSet[] patternPresets;

    [ContextMenu("GetPatterns")]
    void GetChildPatterns()
    {
        AttackPattern[] atps = GetComponentsInChildren<AttackPattern>();
        patternPresets = new AttackPatternSet[atps.Length];
        for(int i = 0; i < atps.Length; i++)
        {
            AttackPatternSet preset = new AttackPatternSet();
            preset.pattern = atps[i];
            patternPresets[i] = preset;
        }
    }

    private void Awake()
    {
        bossHealth = GetComponent<Health>();
        coll = GetComponent<Collider2D>();
        projHitObj.SetActive(false);
    }

    //보스 등장 연출(시작 애니메이션)실행
    public void BossAwake()
    {
        InitializeBoss();
    }

    //보스 유닛 활성화
    public void InitializeBoss()
    {
        //비로소 활성화 시작. 
        projHitObj.SetActive(true);
        bossHealth.ResetHealth();
        
        for (int i = 0; i < patternPresets.Length; i++)
        {
            patternPresets[i].InitializePreset();
        }

        //bossActivate = true;
        StartCoroutine(updateRoutine());
    }

    IEnumerator updateRoutine()
    {
        while (true)
        {
            for(int i = 0; i < patternPresets.Length; i++)
            {
                if (patternPresets[i].pattern.IsCoolTimeOver())
                {
                    if (patternPresets[i].pattern.CheckCondition())
                    {
                        Debug.LogFormat("패턴 {0} : {1} 실행", i, patternPresets[i].pattern.name);
                        yield return StartCoroutine(patternPresets[i].pattern.StartPattern());
                        break;
                    }
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }




    #region 데미지 입음, Die Event
    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (bossHealth.AnyDamage(damage))
        {
            //데미지 num 생성.
            dmgNum.Spawn(hitPoint, damage);
            //UI업데이트
            if (BossUIUpdateEvent != null) BossUIUpdateEvent(bossHealth.HealthPercent);

            if (!bossHealth.IsDead())
            {
                //맞는 효과 
                if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);
            }
            else
            {
                WhenDieEvent();
            }
        }
    }

    public void Kicked(Vector2 hitPos)
    {
        throw new System.NotImplementedException();
    }

    public virtual void WhenDieEvent()
    {
        //충돌 처리 비활성화
        projHitObj.SetActive(false);
        bossActivate = false;
        StopAllCoroutines();

        if (BossDieEvent != null) BossDieEvent();
        //폭발 애니메이션 
        //이 오브젝트 제거?
        //WaveManager에 보스 클리어 전달
        WaveManager.instance.MonsterDisapper();
    }


    #endregion

    public Collider2D GetCollider()
    {
        return coll;
    }

}

[Serializable]
public class AttackPatternSet
{
    public AttackPattern pattern;
    public float coolTime;

    public void InitializePreset()
    {
        pattern.coolTime = this.coolTime;
    }
}

