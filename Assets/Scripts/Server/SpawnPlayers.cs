using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviourPun
{
     public Camera playerCamera1;  // Камера для игрока 1
    public Camera playerCamera2;  // Камера для игрока 2

    void Start()
    {
        // Проверяем, какой игрок присоединился
        if (photonView.IsMine)
        {
            // Активируем камеру для этого игрока
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            {
                playerCamera1.gameObject.SetActive(true);  // Включаем камеру игрока 1
                playerCamera2.gameObject.SetActive(false); // Выключаем камеру игрока 2
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            {
                playerCamera1.gameObject.SetActive(false); // Выключаем камеру игрока 1
                playerCamera2.gameObject.SetActive(true);  // Включаем камеру игрока 2
            }
        }
        else
        {
            // Если это не наш игрок, выключаем камеры
            playerCamera1.gameObject.SetActive(false);
            playerCamera2.gameObject.SetActive(false);
        }
    }
}
