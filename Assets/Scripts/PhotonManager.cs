using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string _version = "1.0";

    private string _userId = "SC";

    void Awake()
    {
        //������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.AutomaticallySyncScene = true;    //������ ���ο� ���� �ε����� �� �ش� �뿡 ������ �޵� �����ڵ鿡�Ե� �ڵ����� �ش���� �ε����ִ� ���
        //���� ���� ����
        PhotonNetwork.GameVersion = _version;
        //���� ������ �г��� ����
        PhotonNetwork.NickName = _userId;

        Debug.Log(PhotonNetwork.SendRate);  //�������� ������ �ʴ� ���� Ƚ�� �⺻ �ʴ� 30ȸ

        PhotonNetwork.ConnectUsingSettings();   //���� ���� ����
    }

    //������ ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();  //�κ� �����ϴ� �Լ�
    }

    //�κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLooby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();     //���� ���ִ� �뿡 ���� ����
    }

    //������ �� ������ �������� ���(�� ���� X) ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        RoomOptions _ro = new RoomOptions();
        _ro.MaxPlayers = 20;                    //�ִ� ������ �� ���� �÷� Max 20��
        _ro.IsOpen = true;                      //���� ����
        _ro.IsVisible = true;                   //���⵵ ����

        PhotonNetwork.CreateRoom("My Room", _ro);   //�� ����
    }

    //�� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Rooom");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    //�뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
    
        foreach(var player in PhotonNetwork.CurrentRoom.Players)        //���� ����� ���� ��ȸ
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        //��ġ ���� ������ �迭�� ����
        Transform[] _points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int _idx = Random.Range(1, _points.Length);

        //��Ʈ��ũ �� ĳ���� ����
        PhotonNetwork.Instantiate("Player", _points[_idx].position, _points[_idx].rotation, 0);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //�� ������ ���� (������ ���� �ְų� �ٸ� ������ ����)�ϸ� ȣ��Ǵ� �Լ�
        Debug.Log("�� ���� ����!");
    }
}
