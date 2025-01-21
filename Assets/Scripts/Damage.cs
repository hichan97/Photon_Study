using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    //사망 후 투명 처리를 위한 MeshRenderer 컴포넌트 배열
    private Renderer[] _renderers;

    //캐릭터 초기 생명치
    private int _initHp = 100;
    //현재 캐릭터 Hp
    public int _currHP = 100;

    private Animator _anim;
    private CharacterController _cc;

    //애니메이터 뷰에 생성한 파라미터의 해시값 추출
    private readonly int _hasDie = Animator.StringToHash("Die");
    private readonly int _hasRespawn = Animator.StringToHash("Respawn");

    private void Awake()
    {
        //캐릭터 모델의 모든 렌더러 컴포넌트 추출 후 배열에 할당
        _renderers = GetComponentsInChildren<Renderer>();
        _anim = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();

        //현재 생명치를 초기 생명치로 초기화
        _currHP = _initHp;
    }

    public void OnCollisionEnter(Collision coll)
    {
        //hp가 0보다 크고 충돌체의 태그가 Bullet인 경우에 생명치 차감
        if(_currHP > 0 && coll.collider.CompareTag("BULLET"))
        {
            _currHP -= 20;
            if (_currHP <= 0)
                StartCoroutine(PlayerDie());
        }
    }

    IEnumerator PlayerDie()
    {
        _cc.enabled = false;

        _anim.SetBool(_hasRespawn, false);
        _anim.SetTrigger(_hasDie);

        yield return new WaitForSeconds(3.0f);

        _anim.SetBool(_hasRespawn, true);

        SetPlayerVisible(false);

        yield return new WaitForSeconds(1.5f);

        Transform[] _points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int _idx = Random.Range(1, _points.Length);
        transform.position = _points[_idx].position;

        _currHP = 100;
        SetPlayerVisible(true);

        _cc.enabled = true;
    }

    void SetPlayerVisible(bool isVisible)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = isVisible;
        }
    }
}



