using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    // Создание комнаты
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        Debug.Log("Попытка создать комнату: " + createInput.text);
    }

    // Присоединение к комнате
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
        Debug.Log("Попытка присоединиться к комнате: " + joinInput.text);
    }

    // Метод вызывается после подключения к мастер-серверу Photon
    public override void OnConnectedToMaster()
    {
        Debug.Log("Подключение к мастер-серверу успешно!");
    }

    // Метод вызывается при успешном присоединении к комнате
    public override void OnJoinedRoom()
    {
        Debug.Log("Успешно присоединились к комнате!");
        PhotonNetwork.LoadLevel("OutdoorsScene");
    }

    // Метод вызывается, если не удалось присоединиться к комнате
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Ошибка при присоединении к комнате: " + message);
    }

    // Метод вызывается, если не удалось создать комнату
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Ошибка при создании комнаты: " + message);
    }
}
