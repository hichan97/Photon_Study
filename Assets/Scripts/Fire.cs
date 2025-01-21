using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviour
{
    public Transform _firePos;
    public GameObject _bulletPrefab;

    private ParticleSystem _muzzleFlash;
    //���� ���콺 ��ư Ŭ�� �̺�Ʈ ����

    private PhotonView _pv;
    private bool isMouseClick => Input.GetMouseButton(0);

    private void Start()
    {
        //����� ������Ʈ ����
        _pv = GetComponent<PhotonView>();

        _muzzleFlash = _firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        //���� ���� ���ο� ���콺 ���� ��ư�� Ŭ������ �� �Ѿ� �߻�
        if(_pv.IsMine && isMouseClick)
        {
            FireBullet();
            //RPC�� �������� �ִ� �Լ� ȣ��
            _pv.RPC("FireBullet", RpcTarget.Others, null);
        }
    }

    [PunRPC]
    public void FireBullet()
    {
        //�ѱ� ����Ʈ�� �������� �ƴѰ�� �ѱ� ����Ʈ ȿ�� ����
        if(!_muzzleFlash.isPlaying) _muzzleFlash.Play(true);

        GameObject bullet = Instantiate(_bulletPrefab,
                                        _firePos.position,
                                        _firePos.rotation);
    }
}
