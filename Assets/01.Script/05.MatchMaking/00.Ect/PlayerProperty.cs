using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperty : MonoBehaviour
{
    Chat chat;
    Player player;
    [SerializeField] RoomPanel roomManager;
    [SerializeField] Button whisper;
    [SerializeField] Button getOut;
    [SerializeField] Button teamChange;
    public bool isMaster { get { return roomManager.isMaster; } }
    private void Awake()
    {
        whisper.onClick.AddListener(Whisper);
        getOut.onClick.AddListener(GetOut);
        teamChange.onClick.AddListener(TeamChange);
    }
    private void Start()
    {
        chat = chat != null ? chat : FindObjectOfType<Chat>();
    }
    private void OnEnable()
    {
        if(isMaster)
        {
            //�Ӽ���ư�� Ŭ���� Ŭ���̾�Ʈ�� �����Ͷ�� �߹�� �� ���� ��ư�� ��Ȱ��ȭ�Ǿ��ִٸ� Ȱ��ȭ�Ѵ�.
            if (getOut.gameObject.activeSelf == false)
                getOut.gameObject.SetActive(true);
            if (teamChange.gameObject.activeSelf == false)
                teamChange.gameObject.SetActive(true);
        }
        else
        {
            //�Ӽ���ư�� Ŭ���� Ŭ���̾�Ʈ�� �����Ͱ� �ƴ϶�� �߹�� �� ���� ��ư�� Ȱ��ȭ�Ǿ��ִٸ� ��Ȱ��ȭ�Ѵ�.
            if (getOut.gameObject.activeSelf)
                getOut.gameObject.SetActive(false);
            if (teamChange.gameObject.activeSelf)
                teamChange.gameObject.SetActive(false);
        }
    }
    public void SetPlayer(Player _player)
    {
        if (_player != null)
            player = _player;
        else
            gameObject.SetActive(false);
    }
    public void SetChat(Chat _chat)
    {
        chat = _chat;
    }

    void Whisper()
    {
        //ä���� ���� ��븦 �����Ѵ�.
        chat.SendTarget(player);
        gameObject.SetActive(false);
    }

    void GetOut()
    {
        //�߹��Ų��.
        PhotonNetwork.CloseConnection(player);
        gameObject.SetActive(false);
    }

    void TeamChange()
    {
        //�ִ� �����ο��� ���� �����Ѵ�.
        int halfCount = PhotonNetwork.CurrentRoom.MaxPlayers >> 1;
        int num = 1;
        //���� �� �ڵ尡 1�̸� ������ �� �ڵ带 2�� �����Ѵ�.
        if (num == player.GetPhotonTeam().Code)
            num = 2;
        //������ ���� �ο��� �����´�.
        int teamCount = PhotonTeamsManager.Instance.GetTeamMembersCount((byte)num);
        //������ ���� �ο��� á���� �� ������ �õ����� �ʴ´�.
        //���� �ο��� ��á���� �� ������ �õ��Ѵ�.
        if (halfCount > teamCount)
            player.SwitchTeam((byte)num);
        gameObject.SetActive(false);
    }
}
