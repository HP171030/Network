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

    [SerializeField] Button passApplyButton;  //��й�ȣ �缳�� ��ư
    [SerializeField] Button cancleButton; //â�ݱ� ��ư

    private void Awake()
    {
        passApplyButton.onClick.AddListener(PassApply);
        cancleButton.onClick.AddListener(Cancel);
    }
    void Cancel()
    {
        gameObject.SetActive(false);
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
    private void SetInteractable(bool interactable)
    {
        passInputField.interactable = interactable;
        confirmInputField.interactable = interactable;
        passApplyButton.interactable = interactable;
    }

}
