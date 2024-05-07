using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerReady;
    [SerializeField] Button playerReadyButton;
    [SerializeField] TMP_Text buttonName;
    [SerializeField] int team;

    public int Team { get { return team; } }
    public bool ReadyState { get; private set; }
    public Player Player { get { return player; } }
    Player player;
    PlayerProperty property;
    Action<PlayerEntry, int> changeTeamMethod;
    bool isMine;
    public void SetPlayer(Player player , PlayerProperty buttons, Action<PlayerEntry, int> _changeTeam , int teamType = 0)
    {
        this.player = player;
        playerName.text = player.NickName; //�÷��̾� �г��� ���
        changeTeamMethod = _changeTeam; //�׼� �Լ� ����
        isMine = player.IsLocal; //���� ��ü���� Ȯ��
        buttonName.text = isMine ? "�غ�" : "�Ӽ�"; //���� ��ü��� �غ� ǥ��, �ƴ϶�� �Ӽ� ǥ��
        ReadyState = player.GetProperty<bool>(DefinePropertyKey.READY);  //�������� ���� 
        team = teamType; //�� Ÿ�� ����

        ChangeReady(ReadyState); //�غ�������� ������Ʈ
        property = buttons;//��Ŭ�� ��ư
        if (teamType != 0) //�� Ÿ���� 0�� �ƴ϶��  �� �Ŵ����� ������Ʈ 
            player.JoinTeam((byte)teamType);
    }
    public void Ready()
    {
        //�غ� ��ư�� Ŭ���� ���

        if (isMine) //���� ��ü�� �����ٸ�
        {
            bool ready = player.GetProperty<bool>(DefinePropertyKey.READY);
            ready = !ready;
            player.SetProperty(DefinePropertyKey.READY, ready);
            PhotonNetwork.AutomaticallySyncScene = ready;
        }
        else //��� ��ü�� �����ٸ�
        {
            //�Ӽ�â ���
            property.gameObject.SetActive(true);
            //�Ӽ� ��� ����
            property.SetPlayer(player);
            //��ġ�� ���콺 ��ġ�� ����
            RectTransform rect = property.transform as RectTransform;
            if(rect != null)
                rect.position = Input.mousePosition;
        }
    }

    public void UpdateProperty(PhotonHashtable property)
    {
        //���� �����Ѵٸ�
        if (player.GetPhotonTeam() != null) //�� ������Ʈ
            ChangeTeam(player.GetPhotonTeam().Code);
        //�غ� ���� ������Ʈ
        bool ready = property.ContainsKey(DefinePropertyKey.READY) ?
           (bool)property[DefinePropertyKey.READY] : player.GetProperty<bool>(DefinePropertyKey.READY);
        ChangeReady(ready);
    }
    void ChangeReady(bool ready)
    {
        playerReady.text = ready ? "Ready" : "";
        ReadyState = ready;
    }
    void ChangeTeam(int changeTeamValue)
    {
        //�׼� �Լ� ȣ��
        changeTeamMethod.Invoke(this, changeTeamValue);
        //���� ����Ǿ��ٸ� ���ο� �� ����
        if(team != changeTeamValue)
            team = changeTeamValue;
    }
}
