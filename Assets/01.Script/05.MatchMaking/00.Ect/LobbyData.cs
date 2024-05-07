using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyData : MonoBehaviour
{
    public enum LobbyState { Random,END}
    bool[] stats;
    private void Awake()
        => stats = new bool[(int)LobbyState.END];
    public void SetLobbyState(LobbyState key,bool state)  //�ش� Ű�� ���� ���� ����
        => stats[(int)key] = state; 
    public bool GetLobbyState(LobbyState key)  //�ش� Ű�� ���� ���� ��ȯ
        => stats[(int)key];
    public void ResetState(LobbyState key) //�ش� Ű�� ���� ���� �ʱ�ȭ
        => stats[(int)key] = false;
}
