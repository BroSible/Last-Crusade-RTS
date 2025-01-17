using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
     public GameObject cameraPrefab; // Префаб камеры игрока
    public Transform[] spawnPoints; // Точки спавна камер
    public PlacementSystem placementSystem; // Ссылка на систему размещения

    void Start()
    {
        SpawnPlayerCamera();
    }

    void SpawnPlayerCamera()
    {
        if (PhotonNetwork.IsConnected && cameraPrefab != null)
        {
            int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Vector3 spawnPosition = spawnPoints[spawnIndex % spawnPoints.Length].position;
            Quaternion spawnRotation = cameraPrefab.transform.rotation; // Используем ротацию префаба
            GameObject playerCamera = PhotonNetwork.Instantiate(cameraPrefab.name, spawnPosition, spawnRotation);

            // Включаем камеру только для локального игрока
            if (playerCamera.TryGetComponent(out Camera cam))
            {
                cam.enabled = true;
                //placementSystem.SetActiveCamera(cam); // Устанавливаем активную камеру в системе размещения
            }
            else
            {
                Camera camInChildren = playerCamera.GetComponentInChildren<Camera>();
                if (camInChildren != null)
                {
                    camInChildren.enabled = true;
                    //placementSystem.SetActiveCamera(camInChildren);
                }
            }
        }
    }
}

