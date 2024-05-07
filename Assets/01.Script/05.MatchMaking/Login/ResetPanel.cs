using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResetPanel : MonoBehaviourShowInfo
{
    [SerializeField] LoginPanelController panelController;
    [SerializeField] TMP_InputField emailInputField;

    [SerializeField] Button sendButton;
    [SerializeField] Button cancelButton;

    private void Awake()
    {
        sendButton.onClick.AddListener(SendResetMail);
        cancelButton.onClick.AddListener(Cancel);
    }
    private void SendResetMail()
    {
        SetInteractable(false);
        string email = emailInputField.text;
        FireBaseManager.Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("��й�ȣ �ʱ�ȭ ���� ������ ��ҵǾ����ϴ�.");
                SetInteractable(true);
            }
            else if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "���� ���� ���ۿ� �����Ͽ����ϴ�.");
                SetInteractable(true);
            }

            panelController.SetActivePanel(LoginPanelController.Panel.Login);
            ShowInfo("���� ������ ���۵Ǿ����ϴ�.");
            SetInteractable(true);
        });
    }

    private void Cancel()
    {
        panelController.SetActivePanel(LoginPanelController.Panel.Login);
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        sendButton.interactable = interactable;
        cancelButton.interactable = interactable;
    }

}
