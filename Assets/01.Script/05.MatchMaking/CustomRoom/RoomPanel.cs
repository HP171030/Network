using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] string gameSceneName;
    [SerializeField] Button startButton;
    [SerializeField] Button leaveButton;

    [SerializeField] RectTransform redTeam;
    [SerializeField] RectTransform blueTeam;

    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] TextMeshProUGUI roomNameTxt;
    List<PlayerEntry> playerList;
    [SerializeField] PlayerProperty playerProperty;
    Room currentRoom;
    [SerializeField] Chat chat;
    int halfCount;
    const int RED = 2;
    const int BLUE = 1;
    bool isEnterGame;
    public bool isMaster { get; private set; }
    private void Awake()
    {
        playerList = new List<PlayerEntry>();
        
        startButton.onClick.AddListener(StartGame);
        leaveButton.onClick.AddListener(LeaveRoom);
    }
    private void OnEnable()
    {
        isEnterGame = false;
        currentRoom = PhotonNetwork.CurrentRoom;
        //�� �̸� ǥ��
        roomNameTxt.text = currentRoom.Name;
        //���� �ִ� �ο��� ����ؼ� �����´�.
        halfCount = (currentRoom.MaxPlayers >> 1);
        isMaster = PhotonNetwork.IsMasterClient;
        //���� �÷��� ��ư ���������� ��� Ȱ��ȭ
        startButton.gameObject.SetActive(isMaster);
        //�÷��̾� ����Ʈ �ʱ�ȭ
        playerList.Clear();

        //���� �÷��̾� ������Ƽ �ʱ�ȭ
        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.READY, false);
        PhotonNetwork.LocalPlayer.SetProperty(DefinePropertyKey.LOAD, false);

        //�÷��̾� ����Ʈ�� ���鼭 �÷��̾� ��Ʈ�� ����
        foreach (Player player in PhotonNetwork.PlayerList)
            PlayerSet(player);

        //��Ŭ�� ��Ͽ� ä�� ����
        playerProperty.SetChat(chat);
        //��Ŭ�� ��� ��Ȱ��ȭ
        playerProperty.gameObject.SetActive(false);

    }
    private void OnDisable()
    {
        //������ ���鼭 �÷��̾� ��Ʈ�� �ı�
        ClearRoomData(redTeam);
        ClearRoomData(blueTeam);
    }
    void ClearRoomData( RectTransform team)
    {
        for (int i = 0; i < team.childCount; i++)
            Destroy(team.GetChild(i).gameObject);
    }

    void PlayerSet(Player newPlayer)
    {
        //�θ�ü�� ��������� ����(�ӽ÷�)
        Transform parent = blueTeam;
        //�ӽ� ���ڵ� ����
        int teamType = 0;

        if (newPlayer.IsLocal) //���� �÷��̾����� Ȯ��
        {
            if (PhotonNetwork.IsMasterClient) //�������� Ȯ��
                newPlayer.JoinTeam(new PhotonTeam() { Code = BLUE}); //��������� ����
        }
        else
        {
            //�� �ڵ带 �����´�.
            teamType = newPlayer.GetPhotonTeam().Code;
            //���� ������̸� �θ�ü�� ��������� �ƴϸ� ���������� ����
            parent = (teamType == BLUE) ? blueTeam : redTeam;
            //���� �����ͼ� ������ ����(�̷��� ���Ŀ� ���� ���� �� �Ŵ����� �����Ͱ� ������Ʈ��(�ڵ� ����ȭ�� ���� ����))
            newPlayer.JoinTeam(newPlayer.GetPhotonTeam());
        }
        //��ü�� �����ؼ� �ش� �θ�ü�� �ڽ����� ����
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, parent);
        //��ü�� �ʱ� �����͸� ����
        playerEntry.SetPlayer(newPlayer, playerProperty, ChangeTeam, teamType);
        //�÷��̾� ��Ͽ� �߰�
        playerList.Add(playerEntry);
    }
    public void PlayerLeftRoom(Player otherPlayer)
    {
        //�÷��̾� ����Ʈ���� �÷��̾� ����
        RemovePlayer(otherPlayer, playerList);
        //�÷��̾ �����ٴ� �޽��� ���
        chat.LeftPlayer(otherPlayer);
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        //�÷��̾� ��ü �����ؼ� ������� �߰�
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, blueTeam);
        //�÷��̾� ��ü�� �ʱ� ������ ����
        playerEntry.SetPlayer(newPlayer, playerProperty, ChangeTeam);
        //�÷��̾� ����Ʈ�� �߰�
        playerList.Add(playerEntry);
        //������ Ŭ���̾�Ʈ�� �÷��̾ ������ �� ����
        if (PhotonNetwork.IsMasterClient)
            NewPlayerEnterSetTeam(newPlayer);
    }
    void NewPlayerEnterSetTeam(Player newPlayer)
    {
        //������� �������� �� �ο��� Ʃ�ÿ� �߰�
        (int blueCount, int redCount) count = (PhotonTeamsManager.Instance.GetTeamMembersCount(BLUE), PhotonTeamsManager.Instance.GetTeamMembersCount(RED));
        int teamType = RED;
        //�ӽ÷� ������ ����
        //�������� ������� �ο��� �ٸ� ���
        if (count.redCount != count.blueCount) //�� ���� ���� ��Ÿ������ ���� (���ο��� ���� ��� ���������� �߰�)
            teamType = (count.redCount > count.blueCount) ? BLUE : RED;
        //�÷��̾��� ���� ����
        newPlayer.JoinTeam(new PhotonTeam() { Code = (byte)teamType });
    }

    void RemovePlayer(Player otherPlayer, List<PlayerEntry> playerList)
    {
        //�÷��̾� ����Ʈ�� ���鼭 
        for (int i = 0; i < playerList.Count; i++)
        {
            //���ͳѹ��� ���� ��� 
            if (playerList[i].Player.ActorNumber == otherPlayer.ActorNumber)
            {
                //��Ʈ�� ��ü �ı� �� ����Ʈ���� �ش� ��ü ����
                Destroy(playerList[i].gameObject);
                playerList.RemoveAt(i);
                return;
            }
        }
    }

    public void TeamChange(int num)
    {
        num += BLUE;
        if (num == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code)
            return;
        int teamCount = PhotonTeamsManager.Instance.GetTeamMembersCount((byte)num);
        if(halfCount > teamCount)
            PhotonNetwork.LocalPlayer.SwitchTeam((byte)num);
    }

    void StartGame()
    {
        if (isEnterGame)
            return;
        isEnterGame = true;
        PhotonNetwork.LoadLevel(gameSceneName);

    }
    public void PlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        //�÷��̾� ����Ʈ�� ���鼭 
        foreach (PlayerEntry player in playerList)
        {
            //�÷��̾� ���Ͱ� ���� ���
            if (player.Player.ActorNumber == targetPlayer.ActorNumber)
            {
                //�ش� �÷��̾� ������Ʈ
                player.UpdateProperty(changedProps);
                break;
            }
        }
        //��� �÷��̾ �غ�Ǿ����� Ȯ��
        AllPlayerReadyCheck();
    }

    void AllPlayerReadyCheck()
    {
        //������ Ŭ���̾�Ʈ�� �ƴϸ� ����
        if (PhotonNetwork.IsMasterClient == false)
            return;
        //�÷��̾� ����Ʈ�� ���鼭
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //�غ� �ȵǾ�������
            if (player.GetProperty<bool>(DefinePropertyKey.READY) == false)
            {
                //��Ȱ��ȭ �� ����
                startButton.interactable = false;
                return;
            }
        }
        //Ȱ��ȭ
        startButton.interactable = true;
    }
    void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void MasterClientSwitched(Player newMasterClient)
    {
        //������ Ŭ���̾�Ʈ�� ���� �÷��̾�� ���� ��ư Ȱ��ȭ
        if (newMasterClient.IsLocal)
        {
            startButton.gameObject.SetActive(true);
            isMaster = true;
        }
    }

    void ChangeTeam(PlayerEntry playerEntry, int team)
    {
        //�÷��̾� ��ü�� ���� �̵��Ϸ��� ���� ���� ��� ����
        if (playerEntry.Team == team)
            return;

        //�����͸� ����
        CheckChangedTeamCount(playerEntry, team);
        //�̵��Ϸ��� ���� ������̸� �θ�ü�� ��� �ƴϸ� ���� ����
        Transform teamTransform = (team == BLUE) ? blueTeam : redTeam;
        //�θ�ü ������ �̵�
        playerEntry.transform.SetParent(teamTransform);
    }

    void CheckChangedTeamCount(PlayerEntry player, int team)
    {
        //������ Ŭ���̾�Ʈ�� ���
        if (PhotonNetwork.IsMasterClient)
        {
            //�̵��Ϸ��� ���� �ο��� �ʰ��ߴ��� Ȯ��
            int count = PhotonTeamsManager.Instance.GetTeamMembersCount((byte)team);
            // ���� �ο��� ������ ���� ���
            if (count > halfCount)
                //���� ������ �̵�
                player.Player.SwitchTeam((byte)player.Team);
        }
    }

}
