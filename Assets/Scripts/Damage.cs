using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    //��� �� ���� ó���� ���� MeshRenderer ������Ʈ �迭
    private Renderer[] _renderers;

    //ĳ���� �ʱ� ����ġ
    private int _initHp = 100;
    //���� ĳ���� Hp
    public int _currHP = 100;

    private Animator _anim;
    private CharacterController _cc;

    //�ִϸ����� �信 ������ �Ķ������ �ؽð� ����
    private readonly int _hasDie = Animator.StringToHash("Die");
    private readonly int _hasRespawn = Animator.StringToHash("Respawn");

    private void Awake()
    {
        //ĳ���� ���� ��� ������ ������Ʈ ���� �� �迭�� �Ҵ�
        _renderers = GetComponentsInChildren<Renderer>();
        _anim = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();

        //���� ����ġ�� �ʱ� ����ġ�� �ʱ�ȭ
        _currHP = _initHp;
    }

    public void OnCollisionEnter(Collision coll)
    {
        //hp�� 0���� ũ�� �浹ü�� �±װ� Bullet�� ��쿡 ����ġ ����
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



