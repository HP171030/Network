using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class CreateNamePanel : MonoBehaviourShowInfo
{
    [SerializeField] LoginPanelController panelController;
    [SerializeField] TMP_InputField nameInputField;

    [SerializeField] Button nameApplyButton;
    [SerializeField] Button backButton;

    private void Awake()
    {
        nameApplyButton.onClick.AddListener(NameApply);
        backButton.onClick.AddListener(GameStart);
    }
    private void NameApply()
    {
        SetInteractable(false);
        UserProfile profile = new UserProfile();
        profile.DisplayName = nameInputField.text;
        profile.PhotoUrl = FireBaseManager.Auth.CurrentUser.PhotoUrl;
        FireBaseManager.Auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task => 
        {
            if(task.IsCanceled)
            {
                ShowInfo("�г��� ������ ��ҵǾ����ϴ�.");
                SetInteractable(true);
                return;
            }
            if(task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "�г��� ������ �����Ͽ����ϴ�.");
                SetInteractable(true);
                return;
            }ShowInfo("�г��� ������ �����Ǿ����ϴ�.");
            SetInteractable(true);
        });
    }


    private void GameStart()
    {
        if(FireBaseManager.Auth.CurrentUser.DisplayName.IsNullOrEmpty())
            ShowInfo("�г����� �Է����ּ���.");
        else
        {
            gameObject.SetActive(false);
            Photon.Pun.PhotonNetwork.ConnectUsingSettings();
        }
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
            }ShowInfo("���� ������ �Ϸ�Ǿ����ϴ�.");
            SetInteractable(true);
            FireBaseManager.Auth.SignOut();
            panelController.SetActivePanel(LoginPanelController.Panel.Login);
        });
    }

    private void SetInteractable(bool interactable)
    {
        nameInputField.interactable = interactable;
        nameApplyButton.interactable = interactable;
        backButton.interactable = interactable;
    }

}
