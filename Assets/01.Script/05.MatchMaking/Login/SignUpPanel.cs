using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviourShowInfo
{
    [SerializeField] LoginPanelController panelController;
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passInputField;
    [SerializeField] TMP_InputField confirmInputField;

    [SerializeField] Button cancelButton;
    [SerializeField] Button signUpButton;

    private void Awake()
    {
        cancelButton.onClick.AddListener(Cancel);
        signUpButton.onClick.AddListener(SignUp);
    }
    public void OnEnable()
    {
        SetInteractable(true);
    }
    public void SignUp()
    {
        string email = emailInputField.text;
        string password = passInputField.text;
        string confirm = confirmInputField.text;
        if (confirm != password)
        {
            ShowInfo($"��й�ȣ�� ����ġ�մϴ�.");
            return;
        }
        SetInteractable(false);
        FireBaseManager.Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread((task) => {
            if (task.IsCanceled)
            {
                ShowInfo($"���� ������ ��ҵǾ����ϴ�.");
                SetInteractable(true);
                return;
            }
            if (task.IsFaulted)
            {
                ShowError(task.Exception.InnerExceptions, "����� �� �����ϴ�.");
                SetInteractable(true);
                return;
            }
            Firebase.Auth.AuthResult result = task.Result;
            panelController.SetActivePanel(LoginPanelController.Panel.Login);
            ShowInfo("������ �����Ǿ����ϴ�.");
            SetInteractable(true);
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }

    public void Cancel()
    {
        panelController.SetActivePanel(LoginPanelController.Panel.Login);
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passInputField.interactable = interactable;
        confirmInputField.interactable = interactable;
        cancelButton.interactable = interactable;
        signUpButton.interactable = interactable;
    }
}
