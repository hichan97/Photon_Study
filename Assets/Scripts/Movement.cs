using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Movement : MonoBehaviour, IPunObservable
{
    //컴포넌트 개시 처리용 변수
    private CharacterController _controller;
    private Transform _transform;
    private Animator _animator;
    private Camera _camera;

    //가상 Plnae에 레이캐스팅 변수
    private Plane _plane;
    private Ray _ray;
    private Vector3 _hitPoint;

    private PhotonView _pv;

    private CinemachineVirtualCamera _vCam;

    //이동 속도
    public float _moveSpeed = 10.0f;

    //수신된 위치와 회전값을 저장할 변수
    private Vector3 _receivePos;
    private Quaternion _ReceiveRot;
    //수신된 좌표로 이동 및 회전 속도의 민감도
    public float damping = 10.0f;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;

        _pv = GetComponent<PhotonView>();
        _vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        //PhotonView가 자신의 것일 경우 시네머신 가상카메라 설정
        if(_pv.IsMine)
        {
            _vCam.Follow = transform;
            _vCam.LookAt = transform;
        }

        //가상의 바닥을 주인공 위치를 기준으로 생성
        _plane = new Plane(transform.up, transform.position);
    }
    private void Update()
    {
        if (_pv.IsMine)
        {
            Move();
            Turn();
        }
        else
        {
            //수신된 좌표로 보간한 이동처리
            transform.position = Vector3.Lerp(transform.position,
                                              _receivePos,
                                              Time.deltaTime * damping);
            //수신된 회전값으로 보간한 회전 처리
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                                _ReceiveRot,
                                                Time.deltaTime * damping);
        }
    }

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");
    
    public void Move()
    {
        Vector3 _forward = _camera.transform.forward;
        Vector3 _right = _camera.transform.right;
        _forward.y = 0.0f;
        _right.y = 0.0f;

        Vector3 _moveDir = (_forward * v) + (_right * h);
        _moveDir.Set(_moveDir.x, 0.0f, _moveDir.z);

        _controller.SimpleMove(_moveDir * _moveSpeed);

        float forward = Vector3.Dot(_moveDir, transform.forward);
        float strafe = Vector3.Dot(_moveDir, transform.right);

        _animator.SetFloat("Forward", forward);
        _animator.SetFloat("Strafe", strafe);

    }

    public void Turn()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        float _enter = 0.0f;

        _plane.Raycast(_ray, out _enter);
        _hitPoint = _ray.GetPoint(_enter);

        Vector3 _lookDir = _hitPoint + transform.position;
        _lookDir.y = 0;

        transform.localRotation = Quaternion.LookRotation(_lookDir);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //자신의 로컬 캐릭터인 경우 자신의 데이터를 다른 네트워크 유저에게 송신
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _receivePos = (Vector3)stream.ReceiveNext();
            _ReceiveRot = (Quaternion)stream.ReceiveNext();
        }
        
    }
}
