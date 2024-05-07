using System.Collections.ObjectModel;
using System;
using UnityEngine;

public class LoginPanelController : MonoBehaviour
{
    public enum Panel { Login, SignUp, Verify, Reset , CreateNickName }

    [SerializeField] InfoPanel infoPanel;
    [SerializeField] LoginPanel loginPanel;
    [SerializeField] SignUpPanel signUpPanel;
    [SerializeField] ResetPanel resetPanel;
    [SerializeField] VerifyPanel verifyPanel;
    [SerializeField] CreateNamePanel setNamePanel;

    private void Start()
    {
        Manager.Pool.CreatePool(infoPanel, 1, 1); //�˸� â�� ������Ʈ Ǯ������ ����(��𼭳� ������ �� �ְ�)
        SetActivePanel(Panel.Login);
    }

    public void SetActivePanel(Panel panel)
    {
        signUpPanel.gameObject.SetActive(panel == Panel.SignUp);
        resetPanel.gameObject.SetActive(panel == Panel.Reset);
        setNamePanel.gameObject.SetActive(panel == Panel.CreateNickName);
        verifyPanel.gameObject.SetActive(panel == Panel.Verify);
    }

}
