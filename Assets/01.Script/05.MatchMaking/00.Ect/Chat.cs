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

    [SerializeField] Button chatResetButton; //��ȭâ �ʱ�ȭ ��ư
    [SerializeField] TMP_InputField inputField; //�Է�â
    [SerializeField] RectTransform content; //��ȭ��� �θ�
    [SerializeField] ScrollRect scrollRect; //��ũ��
    [SerializeField] TMP_Text chatTextPrefab; //��ȭ�� ������ ��ü
    [SerializeField] ChatType chatTarget; //���� Ÿ��
    Player currentMessageTarget; //���� Ÿ��

    private void Awake()
    {
        inputField.onSubmit.AddListener(SendChat); //�����Ҷ� �����ϵ��� �̺�Ʈ �ٿ��ִ´�.
        gameObject.GetOrAddComponent<PhotonView>(); //����䰡 ���� ��� ����並 ���δ�.
        chatResetButton.onClick.AddListener(RemoveEntry); //�ʱ�ȭ ��ư�� �ʱ�ȭ �Լ� �̺�Ʈ�� �ٿ��ִ´�.
    }
    private void OnEnable()
    {
        chatTarget = ChatType.ALL; //����Ÿ���� ��ä�� ����
        //�շ� �޽��� ����
        photonView.RPC("AddMessage", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.NickName}�� �շ��Ͽ����ϴ�.", ChatType.NEW, (byte)0);
    }
    private void OnDisable()
    {
        RemoveEntry();  //��� ��ȭ��� ����(��ȭ ��ü �ı�)
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
        //if (chatTarget == ChatType.TEAM) //�� ��ȭ ����̰�
        //    if((int)teamCode != (int)PhotonNetwork.LocalPlayer.GetPhotonTeam().Code) //���� �ٸ��ٸ� ����
        //        return;

        TMP_Text newMessage = Instantiate(chatTextPrefab);

        switch (chatType) //Ÿ�Կ� ���� ��Ʈ ���� ����
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
                newMessage.fontStyle |= FontStyles.Bold; //��Ʈ�� �β��� ����
                break;
        }
        newMessage.text = message;  //���� �Է�
        newMessage.transform.SetParent(content); //��ü�� �������� �ڽ����� ����
        RectTransform rect = newMessage.transform as RectTransform;
        if (rect != null) // �������� 1�� ����
            rect.localScale = Vector3.one;
        scrollRect.verticalScrollbar.value = 0; //��ũ�� �並 �� ������ ����
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
        //�� �ڵ带 �����´�.
        byte code = PhotonNetwork.LocalPlayer.GetPhotonTeam().Code;
        //�ش� �ڵ��� �� �ɹ��� �����´�.
        PhotonTeamsManager.Instance.TryGetTeamMembers(code, out Player[] teams);
        if (teams != null)
        {
            foreach (Player target in teams) //���� ����� ���� ������ �Ѵ�.
            {
                photonView.RPC("AddMessage", target, chat, ChatType.TEAM, code);
            }
        }
    }

    void RemoveEntry()
    {
        for (int i = 0; i < content.childCount; i++)
            Destroy(content.GetChild(i).gameObject);
    }
}
