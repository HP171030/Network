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
            ShowInfo("비밀번호가 일치하지 않습니다.");
            SetInteractable(true);
        }
        string newPassword = passInputField.text;
        FireBaseManager.Auth.CurrentUser.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowInfo("비밀번호 재설정이 취소되었습니다.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "비밀번호 변경이 실패하였습니다.");
                SetInteractable(true);
                return;
            }
            ShowInfo("비밀번호 변경이 완료되었습니다.");
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
                ShowInfo("계정 삭제가 취소되었습니다.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "계정 삭제가 실패하였습니다..");
                SetInteractable(true);
                return;
            }
            ShowInfo("계정 삭제가 완료되었습니다.");
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
