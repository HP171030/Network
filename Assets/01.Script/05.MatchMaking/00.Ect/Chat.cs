using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviourPun
{
    public enum ChatType { ALL, TARGET, TEAM, NEW, END }

    [SerializeField] Button chatResetButton;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] RectTransform content;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] TMP_Text chatTextPrefab;
    [SerializeField] ChatType chatTarget;
    Player currentMessageTarget;

    private void Awake()
    {
        inputField.onSubmit.AddListener(SendChat);
        gameObject.GetOrAddComponent<PhotonView>();
        chatResetButton.onClick.AddListener(RemoveMessageEntry);
    }
    private void OnEnable()
    {
        chatTarget = ChatType.ALL;
        photonView.RPC("AddMessage", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.NickName}�� �շ��Ͽ����ϴ�.", ChatType.NEW, (byte)0);
    }
    private void OnDisable()
    {
        RemoveMessageEntry();
    }

    public void LeftPlayer(Player otherPlayer)
    {
        //���� �ӼӸ� ��밡 ���� �����ٸ�
        if (currentMessageTarget.ActorNumber == otherPlayer.ActorNumber)
        {
            //Ÿ���� ���� ä�� Ÿ���� ��üä������ �����Ѵ�.
            currentMessageTarget = null;
            chatTarget = ChatType.ALL;
        }
        AddMessage($"{otherPlayer.NickName}�� �����Ͽ����ϴ�.", Chat.ChatType.NEW, (byte)0);
    }
    public void SendTarget(Player target)
    {
        //Ÿ���� ����ִٸ� �����.
        if (currentMessageTarget == null)
            currentMessageTarget = target;
        else //Ÿ���� ���� �����Ǿ��ִ� Ÿ�ٰ� �����ϴٸ� ����.
            currentMessageTarget = currentMessageTarget.ActorNumber == target.ActorNumber ? null : target;
        //���� Ÿ���� ����������� ��üä������ �ִٸ� �ӼӸ��� �����Ѵ�.
        chatTarget = currentMessageTarget == null ? ChatType.ALL : ChatType.TARGET;
    }
    void SendChat(string chat)
    {
        //������ ������.
        SendMessageToTarget(chat);
        //�Է�â�� ���� �ٽ� Ȱ��ȭ ���·� �����Ѵ�.
        inputField.text = "";
        inputField.ActivateInputField();
    }

    [PunRPC]
    public void AddMessage(string message, ChatType chatType, byte teamCode = 0)
    {
        //��ȭâ�� �����Ѵ�.
        TMP_Text newMessage = Instantiate(chatTextPrefab);
        //ui transform������Ʈ�� �����´�.
        RectTransform rect = newMessage.transform as RectTransform;
        //
        if (rect != null)
            rect.localScale = Vector3.one;

        switch (chatType)
        {
            case ChatType.ALL:
                newMessage.color = Color.black;
                break;
            case ChatType.TARGET:
                newMessage.color = Color.red;
                break;
            case ChatType.TEAM:
                newMessage.color = Color.blue;
                break;
            case ChatType.NEW:
                newMessage.color = Color.yellow;
                newMessage.fontStyle |= FontStyles.Bold;
                break;
        }
        newMessage.text = message;
        newMessage.transform.SetParent(content);
        scrollRect.verticalScrollbar.value = 0;
    }


    // Ŭ���̾�Ʈ ������ ���Ͽ� ��û�� ������ ����
    void SendMessageToTarget(string chat)
    {
        byte defaultParam = default(byte);
        switch (chatTarget)
        {
            case ChatType.ALL:
                photonView.RPC("AddMessage", RpcTarget.All, chat, ChatType.ALL, defaultParam);
                break;
            case ChatType.TARGET:
                photonView.RPC("AddMessage", currentMessageTarget, chat, ChatType.TARGET, defaultParam);
                break;
            case ChatType.TEAM:
                SendTeam(chat);
                break;
        }
    }
    void SendTeam(string chat)
    {
        byte code = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        PhotonTeamsManager.Instance.TryGetTeamMembers(code, out Player[] teams);
        if (teams != null)
        {
            foreach (Player target in teams)
            {
                photonView.RPC("AddMessage", target, chat, ChatType.TEAM, code);
            }
        }
    }

    void RemoveMessageEntry()
    {
        for (int i = 0; i < content.childCount; i++)
            Destroy(content.GetChild(i).gameObject);
    }
}
