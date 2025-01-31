using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string _version = "1.0";

    private string _userId = "SC";

    //유저명을 입력할 Input Field
    public TMP_InputField _userIF;

    public TMP_InputField _roomNameIF;

    //룸 목록에 대한 데이터를 저장하기 위한 딕셔너리 자료형
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    //룸 목록을 표시할 프리팹
    private GameObject roomItemPrefab;
    // RoomItem 프리팹이 추가될 ScrollContent
    public Transform scrollContent;

    void Awake()
    {
        //마스터 클라이언트의 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;    //방장이 새로운 씬을 로딩했을 때 해당 룸에 입장한 달든 접속자들에게도 자동으로 해당씬을 로딩해주는 기능
        //게임 버전 설정
        PhotonNetwork.GameVersion = _version;
        //접속 유저의 닉네임 설정
        //PhotonNetwork.NickName = _userId;

        //서버와의 데이터 초당 전송 횟수 기본 초당 30회
        Debug.Log(PhotonNetwork.SendRate);

        //RoomItem Prefab Load
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();   //포톤 서버 접속
        }
    }

    public void Start()
    {
        //저장된 유저명을 로드
        _userId = PlayerPrefs.GetString("User_ID", $"User_{Random.Range(1, 21):00}");
        _userIF.text = _userId;

        //접속 유저의 닉네임 등록
        PhotonNetwork.NickName = _userId;
    }

    //유저명을 설정하는 로직
    public void SetUserId()
    {
        if(string.IsNullOrEmpty(_userIF.text))
        {
            _userId = $"USER_{Random.Range(1, 21):00}";

        }
        else
        {
            _userId =_userIF.text;
        }

        //유저명 저장
        PlayerPrefs.SetString("USER_ID", _userId);
        //접속 유저의 닉네임 등록
        PhotonNetwork.NickName = _userId;
    }

    public string SetRoomName()
    {
        if( string.IsNullOrEmpty(_roomNameIF.text))
        {
            _roomNameIF.text = $"ROOM_{Random.Range(1, 101):000}";
        }

        return _roomNameIF.text;
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
        //PhotonNetwork.JoinRandomRoom();     //생성 되있는 룸에 랜덤 입장
    }

    

    //랜덤한 룸 입장이 실패했을 경우(방 생성 X) 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");
        //룸을 생성하는 함수 실행
        OnMakeRoomClick();

        //RoomOptions _ro = new RoomOptions();
        //_ro.MaxPlayers = 20;                    //최대 접속자 수 무료 플랜 Max 20명
        //_ro.IsOpen = true;                      //오픈 여부
        //_ro.IsVisible = true;                   //노출도 여부

        //룸 생성
        //PhotonNetwork.CreateRoom("My Room", _ro);   //방 생성
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
        //Transform[] _points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        //int _idx = Random.Range(1, _points.Length);

        //네트워크 상에 캐릭터 생성
        //PhotonNetwork.Instantiate("Player", _points[_idx].position, _points[_idx].rotation, 0);

        //마스터 클라이언트인 경우에 룸에 입장한 후 전투 씬을 로딩한다.
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 삭제된 RoomItem 프리팹을 저장할 임시변수
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == true)
            {  // 룸이 삭제된 경우
                // 딕셔너리에서 룸 이름으로 검색해 저장된 RoomItem 프리팹를 추출
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom); // RoomItem 프리팹 삭제        
                rooms.Remove(roomInfo.Name); // 딕셔너리에서 해당 룸 이름의 데이터를 삭제
            }
            else // 룸 정보가 변경된 경우
            {
                // 룸 이름이 딕셔너리에 없는 경우 새로 추가
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    // RoomInfo 프리팹을 scrollContent 하위에 생성
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    // 룸 정보를 표시하기 위해 RoomInfo 정보 전달
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;

                    // 딕셔너리 자료형에 데이터 추가
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else // 룸 이름이 딕셔너리에 없는 경우에 룸 정보를 갱신
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;  //룸 정보 전달
                }
            }
            Debug.Log($"Room={roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
        }
    }

    #region UI_BUTTON_EVENT

    public void OnLoginClick()
    {
        //유저명 저장
        SetUserId();

        //무작위로 추출한 룸으로 입장
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        //유저명 저장
        SetUserId();

        //룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;

        //룸생성
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }
    #endregion

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //룸 생성에 실패 (동일한 룸이 있거나 다른 이유로 실패)하면 호출되는 함수
        Debug.Log("룸 생성 실패!");
    }

}
