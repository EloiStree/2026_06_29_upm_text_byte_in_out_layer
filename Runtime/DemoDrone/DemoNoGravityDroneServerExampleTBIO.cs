using Eloi.TBIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class DemoNoGravityDroneServerExampleTBIO : MonoBehaviour
{
    public List<AbstractTextByteInOutLayerMono> m_playerAbstractLayerTBIO = new List<AbstractTextByteInOutLayerMono>();
    public List<NoGravityBasicFpvAndTelloDroneMono> m_drones = new List<NoGravityBasicFpvAndTelloDroneMono>();
    public List<Transform> m_pointsToCapture = new List<Transform>();
    public List<int> m_playerIndex = new List<int>() { -1,-2,-3,-4, -5, -6, -7, -8, -9, -10, -11, -12 };
    public List<int> m_playerCatchPoints = new List<int>() { 0,0,0,0,0,0,0,0,0,0,0,0,0};


    public float m_refreshGameInformation= 1f;
    public float m_refreshPlayerInformation = 0.1f;

    public float m_distanceToCapturePoint = 0.15f;
    public Transform m_randomPointAnchor;
    public float m_randomPointRange = 5f;

    public void Awake()
    {
        foreach (var point in m_pointsToCapture)
        {
            SetRandomPointTo(point);
        }

        for (int i = 0; i < m_playerAbstractLayerTBIO.Count; i++)
        {
            AbstractTextByteInOutLayerMono playerLayer = m_playerAbstractLayerTBIO[i];
            if (playerLayer != null)
            {
                int playerIndex = m_playerIndex[i];
                int playerListIndex = i;
                playerLayer.AddListenerOnByteSendToServer((byte[] bytes) =>
                {
                    PlayerByteToPlayerListIndex(playerListIndex, bytes);
                });
                playerLayer.AddListenerOnTextSendToServer((string text) =>
                {
                    if (text=="TELLO" || text =="STABE")
                    {
                        m_drones[playerListIndex].SetStableMode();
                    }
                    else if (text == "FPV"|| text == "ACROBATIC")
                    {
                        m_drones[playerListIndex].SetAcrobaticMode();
                    }
                });
            }
        }
    }

    public void PlayerByteToPlayerListIndex(int playerListIndex, byte[] bytes) {
        m_drones[playerListIndex].SetWith1899887766(bytes);
    }

    private float m_timeSinceLastGameInformationRefresh = 0f;
    private float m_timeSinceLastPlayerInformationRefresh = 0f;

    public void Update()
    {
        foreach (var drone in m_drones)
        {
            Vector3 dronePosition = drone.transform.position;
            for (int i = 0; i < m_pointsToCapture.Count; i++)
            {
                Vector3 capturePointPosition = m_pointsToCapture[i].position;
                float distance = Vector3.Distance(dronePosition, capturePointPosition);
                if (distance <= m_distanceToCapturePoint)
                {
                    m_playerCatchPoints[i]++;
                    SetRandomPointTo(m_pointsToCapture[i]);
                }
            }
        }

        m_timeSinceLastGameInformationRefresh += Time.deltaTime;
        m_timeSinceLastPlayerInformationRefresh += Time.deltaTime;
        
        if (m_timeSinceLastGameInformationRefresh >= m_refreshGameInformation)
        {
            PushGameInformation();
            m_timeSinceLastGameInformationRefresh = 0f;
        }

        if (m_timeSinceLastPlayerInformationRefresh >= m_refreshPlayerInformation)
        {
            PushPlayerInformation();
            m_timeSinceLastPlayerInformationRefresh = 0f;
        }

    }

    public void PushGameInformation() {

        string allPointPositions = "POINTS:";
        List<string> allPointPositionsList = new List<string>();
        for (int i = 0; i < m_pointsToCapture.Count; i++)
        {
            Vector3 pointPosition = m_pointsToCapture[i].position;
            allPointPositionsList.Add($"{pointPosition.x:F2}:{pointPosition.y:F2}:{pointPosition.z:F2}");
        }
        allPointPositions += string.Join(":", allPointPositionsList);
        SendTextToAll(allPointPositions);

        string allPlayerIndexes = "I_ALL:" + string.Join(":", m_playerIndex);
        SendTextToAll(allPlayerIndexes);
    }

    public void PushPlayerInformation() {

        string allPlayerPositions = "P_ALL:";
        List<string> allPlayerPositionsList = new List<string>();
        for (int i = 0; i < m_drones.Count; i++)
        {
            Vector3 playerPosition = m_drones[i].transform.position;
            allPlayerPositionsList.Add($"{playerPosition.x:F2}:{playerPosition.y:F2}:{playerPosition.z:F2}");
        }
        allPlayerPositions+=  string.Join(":", allPlayerPositionsList);
        SendTextToAll(allPlayerPositions);

        for (int i = 0; i < m_playerIndex.Count; i++)
        {
            int playerIndex = m_playerIndex[i];
            AbstractTextByteInOutLayerMono playerLayer = m_playerAbstractLayerTBIO[i];
            if (playerLayer != null)
            {
                playerLayer.SendTextToClient($"I:{playerIndex}");
                Vector3 playerPosition = m_drones[i].transform.position;
                Vector3 playerRotation = m_drones[i].transform.eulerAngles;
                string message = $"PE:{playerPosition.x:F2}:{playerPosition.y:F2}:{playerPosition.z:F2}:{playerRotation.x:F2}:{playerRotation.y:F2}:{playerRotation.z:F2}";
                playerLayer.SendTextToClient(message);
                byte[] pointsData = new byte[4*6*sizeof(int)];
                // Convert the two vector3 to 6 following float
                BitConverter.GetBytes((int)playerPosition.x).CopyTo(pointsData, 0);
                BitConverter.GetBytes((int)playerPosition.y).CopyTo(pointsData, 4);
                BitConverter.GetBytes((int)playerPosition.z).CopyTo(pointsData, 8);
                BitConverter.GetBytes((int)playerRotation.x).CopyTo(pointsData, 12);
                BitConverter.GetBytes((int)playerRotation.y).CopyTo(pointsData, 16);
                BitConverter.GetBytes((int)playerRotation.z).CopyTo(pointsData, 20);
                playerLayer.SendByteToClient(pointsData);
            }
        }
    }

    public void PushSpawn(Vector3 position)
    {
        string spawnText = $"SPAWN:{position.x:F2}:{position.y:F2}:{position.z:F2}";
        SendTextToAll(spawnText);
    }
    public void PushDespawn(Vector3 position)
    {
        string despawnText = $"DESPAWN:{position.x:F2}:{position.y:F2}:{position.z:F2}";
        SendTextToAll(despawnText);
    }

    public void SendTextToAll(string text)
    {
        foreach (var playerLayer in m_playerAbstractLayerTBIO)
        {
            if (playerLayer != null)
            {
                playerLayer.SendTextToClient(text);
            }
        }
    }
    public void SendBytesToAll(byte[] bytes)
    {
        foreach (var playerLayer in m_playerAbstractLayerTBIO)
        {
            if (playerLayer != null)
            {
                playerLayer.SendByteToClient(bytes);
            }
        }

    }

    public void PushSinglePlayerInformation(int playerIndex) { 
        for(int i=0; i < m_playerIndex.Count; i++)
        {
            if(m_playerIndex[i] == playerIndex)
            {
                AbstractTextByteInOutLayerMono playerLayer = m_playerAbstractLayerTBIO[i];
                if (playerLayer != null)
                {
                    Vector3 playerPosition = m_drones[i].transform.position;
                    Vector3 playerRotation = m_drones[i].transform.eulerAngles;
                    string message = $"PE:{playerPosition.x}:{playerPosition.y}:{playerPosition.z}:{playerRotation.x}:{playerRotation.y}:{playerRotation.z}";
                    playerLayer.SendTextToClient(message);
                    byte[] pointsData = new byte[4*6*sizeof(int)];
                    // Convert the two vector3 to 6 following float
                    BitConverter.GetBytes((int)playerPosition.x).CopyTo(pointsData, 0);
                    BitConverter.GetBytes((int)playerPosition.y).CopyTo(pointsData, 4);
                    BitConverter.GetBytes((int)playerPosition.z).CopyTo(pointsData, 8);
                    BitConverter.GetBytes((int)playerRotation.x).CopyTo(pointsData, 12);
                    BitConverter.GetBytes((int)playerRotation.y).CopyTo(pointsData, 16);
                    BitConverter.GetBytes((int)playerRotation.z).CopyTo(pointsData, 20);
                    playerLayer.SendByteToClient(pointsData);
                    playerLayer.SendTextToClient($"I:{playerIndex}");
                }
                return;
            }
        }

        for (int i = 0; i < m_playerAbstractLayerTBIO.Count; i++)
        {
            AbstractTextByteInOutLayerMono playerLayer = m_playerAbstractLayerTBIO[i];
            if (playerLayer != null)
            {
                Vector3 playerPosition = m_drones[i].transform.position;
                Vector3 playerRotation = m_drones[i].transform.eulerAngles;
                string message = $"PE:{playerPosition.x}:{playerPosition.y}:{playerPosition.z}:{playerRotation.x}:{playerRotation.y}:{playerRotation.z}";
                playerLayer.SendTextToClient(message);
                byte[] pointsData = new byte[4*6*sizeof(int)];
                // Convert the two vector3 to 6 following float
                BitConverter.GetBytes((int)playerPosition.x).CopyTo(pointsData, 0);
                BitConverter.GetBytes((int)playerPosition.y).CopyTo(pointsData, 4);
                BitConverter.GetBytes((int)playerPosition.z).CopyTo(pointsData, 8);
                BitConverter.GetBytes((int)playerRotation.x).CopyTo(pointsData, 12);
                BitConverter.GetBytes((int)playerRotation.y).CopyTo(pointsData, 16);
                BitConverter.GetBytes((int)playerRotation.z).CopyTo(pointsData, 20);
            }
        }

    }


    private void SetRandomPointTo(Transform transform)
    {
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-m_randomPointRange, m_randomPointRange),
            UnityEngine.Random.Range(0.5f, 2f),
            UnityEngine.Random.Range(-m_randomPointRange, m_randomPointRange)
        );
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = m_randomPointAnchor.position + randomOffset;
        transform.position = newPosition;

        PushDespawn(currentPosition);
        PushSpawn(newPosition);
    }
}
