using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Movement : MonoBehaviour, IPunObservable
{
    //������Ʈ ���� ó���� ����
    private CharacterController _controller;
    private Transform _transform;
    private Animator _animator;
    private Camera _camera;

    //���� Plnae�� ����ĳ���� ����
    private Plane _plane;
    private Ray _ray;
    private Vector3 _hitPoint;

    private PhotonView _pv;

    private CinemachineVirtualCamera _vCam;

    //�̵� �ӵ�
    public float _moveSpeed = 10.0f;

    //���ŵ� ��ġ�� ȸ������ ������ ����
    private Vector3 _receivePos;
    private Quaternion _ReceiveRot;
    //���ŵ� ��ǥ�� �̵� �� ȸ�� �ӵ��� �ΰ���
    public float damping = 10.0f;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;

        _pv = GetComponent<PhotonView>();
        _vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        //PhotonView�� �ڽ��� ���� ��� �ó׸ӽ� ����ī�޶� ����
        if(_pv.IsMine)
        {
            _vCam.Follow = transform;
            _vCam.LookAt = transform;
        }

        //������ �ٴ��� ���ΰ� ��ġ�� �������� ����
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
            //���ŵ� ��ǥ�� ������ �̵�ó��
            transform.position = Vector3.Lerp(transform.position,
                                              _receivePos,
                                              Time.deltaTime * damping);
            //���ŵ� ȸ�������� ������ ȸ�� ó��
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
        //�ڽ��� ���� ĳ������ ��� �ڽ��� �����͸� �ٸ� ��Ʈ��ũ �������� �۽�
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
