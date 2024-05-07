using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditUserDataPanel : MonoBehaviourShowInfo
{
    [SerializeField] TMP_InputField passInputField;
    [SerializeField] TMP_InputField confirmInputField;
    [SerializeField] Button passApplyButton;
    [SerializeField] Button cancleButton;

    private void Awake()
    {
        passApplyButton.onClick.AddListener(PassApply);
        cancleButton.onClick.AddListener(() => { gameObject.SetActive(false); });
    }
    private void PassApply()
    {
        SetInteractable(false);
        if (passInputField.text != confirmInputField.text)
        {
            ShowInfo("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            SetInteractable(true);
        }
        string newPassword = passInputField.text;
        FireBaseManager.Auth.CurrentUser.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("��й�ȣ �缳���� ��ҵǾ����ϴ�.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "��й�ȣ ������ �����Ͽ����ϴ�.");
                SetInteractable(true);
                return;
            }
            ShowInfo("��й�ȣ ������ �Ϸ�Ǿ����ϴ�.");
            SetInteractable(true);
        });
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
        });
    }
    private void SetInteractable(bool interactable)
    {
        passInputField.interactable = interactable;
        confirmInputField.interactable = interactable;
        passApplyButton.interactable = interactable;
    }

}
