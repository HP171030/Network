using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviourShowInfo
{
    [SerializeField] GameObject editPanel;
    [SerializeField] GameObject mainPanel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text emailText;
    [SerializeField] TMP_Text idText;
    [SerializeField] Button logoutButton;
    [SerializeField] Button editButton;
    [SerializeField] Button cancleButton;

    private void Awake()
    {
        logoutButton.onClick.AddListener(Delete);
        editButton.onClick.AddListener(Edit);
        cancleButton.onClick.AddListener(Cancle);
    }
    private void OnEnable()
    {
        if (FireBaseManager.Auth.CurrentUser == null)
            return;
        editPanel.SetActive(false);
        mainPanel.SetActive(true);
        nameText.text = FireBaseManager.Auth.CurrentUser.DisplayName;
        emailText.text = FireBaseManager.Auth.CurrentUser.Email;
        idText.text = FireBaseManager.Auth.CurrentUser.UserId;
    }

    void Edit()
    {
        editPanel.SetActive(true);
    }

    void Cancle()
    {
        gameObject.SetActive(false);
    }

    private void Delete()
    {
        SetInteractable(false);
        FireBaseManager.Auth.CurrentUser.DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("���� ������ ��ҵǾ����ϴ�.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "���� ������ �����Ͽ����ϴ�..");
                SetInteractable(true);
                return;
            }
            ShowInfo("���� ������ �Ϸ�Ǿ����ϴ�.");
            SetInteractable(true);
            FireBaseManager.Auth.SignOut();
            PhotonNetwork.Disconnect();
        });
    }

    void SetInteractable(bool state)
    {
        logoutButton.interactable = state;
        editButton.interactable = state;
        cancleButton.interactable = state;
    }

}
