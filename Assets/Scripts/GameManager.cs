using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomName;
    public TMP_Text connectInfo;
    public Button exitBtn;

    void Awake()
    {
        CreatePlayer();
        //���� ���� ���� �� ǥ��
        SetRoomInfo();
        //Exit ��ư �̺�Ʈ ����
        exitBtn.onClick.AddListener(() => OnExitClick());
    }
    public void CreatePlayer()
    {
        //���� ��ġ ������ �迭�� ����
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        //��Ʈ��ũ�� ĳ���� ����
        PhotonNetwork.Instantiate("Player",
                                    points[idx].position,
                                    points[idx].rotation,
                                    0);
    }

    void SetRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }

    //Exit ��ư�� OnClick�� ������ �Լ�
    private void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    //���� �뿡�� �������� �� ȣ��Ǵ� �� �� �Լ�
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
    }


}
