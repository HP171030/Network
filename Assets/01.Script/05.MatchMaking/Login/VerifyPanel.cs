using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VerifyPanel : MonoBehaviourShowInfo
{
    [SerializeField] LoginPanelController panelController;
    [SerializeField] Button logoutButton;
    [SerializeField] Button sendButton;
    [SerializeField] TMP_Text sendButtonText;

    [SerializeField] int sendMailCooltime;
    private void Awake()
    {
        logoutButton.onClick.AddListener(Logout);
        sendButton.onClick.AddListener(SendVerifyMail);
    }
    private void Logout()
    {
        FireBaseManager.Auth.SignOut();
        panelController.SetActivePanel(LoginPanelController.Panel.Login);
    }

  

    private void SendVerifyMail()
    {
        SetInteractable(false);
        FireBaseManager.Auth.CurrentUser.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCanceled)
            {
                ShowInfo("���� ���� ��ҵǾ����ϴ�.");
                SetInteractable(true);
                return;
            }
            else if(task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions,"���� ���� �����Ͽ����ϴ�."); 
                SetInteractable(true);
                return;
            }
            ShowInfo("���� ���� �����Ͽ����ϴ�.");
            SetInteractable(true);
        });
    }

    Coroutine verifyCo;
    private void OnEnable()
    {
        if (FireBaseManager.Auth == null)
            return;
        /*verifyCo = */
        StartCoroutine(VerifyCheckRoutine());
    }
    void OnDisable()
    {
        StopAllCoroutines();
        //StopCoroutine(verifyCo);
    }
    IEnumerator VerifyCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(4);
            FireBaseManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    ShowInfo("ReloadAsync canceled");
                }
                else if (task.IsFaulted)
                {
                    ShowError(task.Exception.InnerExceptions, "������ �����Ͽ����ϴ�.");
                }
                else if (FireBaseManager.Auth.CurrentUser.IsEmailVerified)
                {
                    ShowInfo("�����Ǿ����ϴ�.");
                    panelController.SetActivePanel(LoginPanelController.Panel.CreateNickName);
                }
                return;
            });
        }
    }

    private void SetInteractable(bool interactable)
    {
        logoutButton.interactable = interactable;
        sendButton.interactable = interactable;
    }
}
