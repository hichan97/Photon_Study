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
        //마스터 클라이언트의 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        //게임 버전 설정
        PhotonNetwork.GameVersion = _version;
        //접속 유저의 닉네임 설정
        PhotonNetwork.NickName = _userId;

        Debug.Log(PhotonNetwork.SendRate);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLooby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        RoomOptions _ro = new RoomOptions();
        _ro.MaxPlayers = 20;
        _ro.IsOpen = true;
        _ro.IsVisible = true;

        PhotonNetwork.CreateRoom("My Room", _ro);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Rooom");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
    
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        //위치 출현 정보를 배열에 저장
        Transform[] _points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int _idx = Random.Range(1, _points.Length);

        //네트워크 상에 캐릭터 생성
        PhotonNetwork.Instantiate("Player", _points[_idx].position, _points[_idx].rotation, 0);
    }
}
