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
        PhotonNetwork.AutomaticallySyncScene = true;    //방장이 새로운 씬을 로딩했을 때 해당 룸에 입장한 달든 접속자들에게도 자동으로 해당씬을 로딩해주는 기능
        //게임 버전 설정
        PhotonNetwork.GameVersion = _version;
        //접속 유저의 닉네임 설정
        PhotonNetwork.NickName = _userId;

        Debug.Log(PhotonNetwork.SendRate);  //서버와의 데이터 초당 전송 횟수 기본 초당 30회

        PhotonNetwork.ConnectUsingSettings();   //포톤 서버 접속
    }

    //서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();  //로비에 접속하는 함수
    }

    //로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLooby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();     //생성 되있는 룸에 랜덤 입장
    }

    //랜덤한 룸 입장이 실패했을 경우(방 생성 X) 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        RoomOptions _ro = new RoomOptions();
        _ro.MaxPlayers = 20;                    //최대 접속자 수 무료 플랜 Max 20명
        _ro.IsOpen = true;                      //오픈 여부
        _ro.IsVisible = true;                   //노출도 여부

        PhotonNetwork.CreateRoom("My Room", _ro);   //방 생성
    }

    //룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Rooom");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    //룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
    
        foreach(var player in PhotonNetwork.CurrentRoom.Players)        //룸의 사용자 정보 조회
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        //위치 출현 정보를 배열에 저장
        Transform[] _points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int _idx = Random.Range(1, _points.Length);

        //네트워크 상에 캐릭터 생성
        PhotonNetwork.Instantiate("Player", _points[_idx].position, _points[_idx].rotation, 0);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //룸 생성에 실패 (동일한 룸이 있거나 다른 이유로 실패)하면 호출되는 함수
        Debug.Log("룸 생성 실패!");
    }
}
