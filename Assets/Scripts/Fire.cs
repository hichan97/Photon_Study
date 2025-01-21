using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviour
{
    public Transform _firePos;
    public GameObject _bulletPrefab;

    private ParticleSystem _muzzleFlash;
    //왼쪽 마우스 버튼 클릭 이벤트 저장

    private PhotonView _pv;
    private bool isMouseClick => Input.GetMouseButton(0);

    private void Start()
    {
        //포톤뷰 컴포넌트 연결
        _pv = GetComponent<PhotonView>();

        _muzzleFlash = _firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        //로컬 유저 여부와 마우스 왼쪽 버튼을 클릭했을 때 총알 발사
        if(_pv.IsMine && isMouseClick)
        {
            FireBullet();
            //RPC로 원격지에 있는 함수 호출
            _pv.RPC("FireBullet", RpcTarget.Others, null);
        }
    }

    [PunRPC]
    public void FireBullet()
    {
        //총구 이펙트가 실행중이 아닌경우 총구 이펙트 효과 실행
        if(!_muzzleFlash.isPlaying) _muzzleFlash.Play(true);

        GameObject bullet = Instantiate(_bulletPrefab,
                                        _firePos.position,
                                        _firePos.rotation);
    }
}
