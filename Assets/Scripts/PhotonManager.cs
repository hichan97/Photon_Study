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

    //�������� �Է��� Input Field
    public TMP_InputField _userIF;

    public TMP_InputField _roomNameIF;

    //�� ��Ͽ� ���� �����͸� �����ϱ� ���� ��ųʸ� �ڷ���
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    //�� ����� ǥ���� ������
    private GameObject roomItemPrefab;
    // RoomItem �������� �߰��� ScrollContent
    public Transform scrollContent;

    void Awake()
    {
        //������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.AutomaticallySyncScene = true;    //������ ���ο� ���� �ε����� �� �ش� �뿡 ������ �޵� �����ڵ鿡�Ե� �ڵ����� �ش���� �ε����ִ� ���
        //���� ���� ����
        PhotonNetwork.GameVersion = _version;
        //���� ������ �г��� ����
        //PhotonNetwork.NickName = _userId;

        //�������� ������ �ʴ� ���� Ƚ�� �⺻ �ʴ� 30ȸ
        Debug.Log(PhotonNetwork.SendRate);

        //RoomItem Prefab Load
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();   //���� ���� ����
        }
    }

    public void Start()
    {
        //����� �������� �ε�
        _userId = PlayerPrefs.GetString("User_ID", $"User_{Random.Range(1, 21):00}");
        _userIF.text = _userId;

        //���� ������ �г��� ���
        PhotonNetwork.NickName = _userId;
    }

    //�������� �����ϴ� ����
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

        //������ ����
        PlayerPrefs.SetString("USER_ID", _userId);
        //���� ������ �г��� ���
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
        //PhotonNetwork.JoinRandomRoom();     //���� ���ִ� �뿡 ���� ����
    }

    

    //������ �� ������ �������� ���(�� ���� X) ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");
        //���� �����ϴ� �Լ� ����
        OnMakeRoomClick();

        //RoomOptions _ro = new RoomOptions();
        //_ro.MaxPlayers = 20;                    //�ִ� ������ �� ���� �÷� Max 20��
        //_ro.IsOpen = true;                      //���� ����
        //_ro.IsVisible = true;                   //���⵵ ����

        //�� ����
        //PhotonNetwork.CreateRoom("My Room", _ro);   //�� ����
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
        //Transform[] _points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        //int _idx = Random.Range(1, _points.Length);

        //��Ʈ��ũ �� ĳ���� ����
        //PhotonNetwork.Instantiate("Player", _points[_idx].position, _points[_idx].rotation, 0);

        //������ Ŭ���̾�Ʈ�� ��쿡 �뿡 ������ �� ���� ���� �ε��Ѵ�.
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ������ RoomItem �������� ������ �ӽú���
        GameObject tempRoom = null;

        foreach (var roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == true)
            {  // ���� ������ ���
                // ��ųʸ����� �� �̸����� �˻��� ����� RoomItem �����ո� ����
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom); // RoomItem ������ ����        
                rooms.Remove(roomInfo.Name); // ��ųʸ����� �ش� �� �̸��� �����͸� ����
            }
            else // �� ������ ����� ���
            {
                // �� �̸��� ��ųʸ��� ���� ��� ���� �߰�
                if (rooms.ContainsKey(roomInfo.Name) == false)
                {
                    // RoomInfo �������� scrollContent ������ ����
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    // �� ������ ǥ���ϱ� ���� RoomInfo ���� ����
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;

                    // ��ųʸ� �ڷ����� ������ �߰�
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else // �� �̸��� ��ųʸ��� ���� ��쿡 �� ������ ����
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;  //�� ���� ����
                }
            }
            Debug.Log($"Room={roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
        }
    }

    #region UI_BUTTON_EVENT

    public void OnLoginClick()
    {
        //������ ����
        SetUserId();

        //�������� ������ ������ ����
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        //������ ����
        SetUserId();

        //���� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;

        //�����
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }
    #endregion

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //�� ������ ���� (������ ���� �ְų� �ٸ� ������ ����)�ϸ� ȣ��Ǵ� �Լ�
        Debug.Log("�� ���� ����!");
    }

}
