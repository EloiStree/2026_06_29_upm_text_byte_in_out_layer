using Eloi.TBIO;
using System.Collections;
using UnityEngine;

public class V001EloiMoveDroneMono : MonoBehaviour
{
    public TextByteInOutLayerMono m_playerAbstractLayerTBIO;

    public string m_lastReceivedText;
    public byte [] m_lastReceivedBytes;

    public Vector3[] m_playersPosition;
    public Vector3[] m_pointsPosition;
    public int[] m_playersIndex;
    public int m_playerIndex = 0;
    public Vector3 m_currentPosition;
    public Vector3 m_currentEulerRotation;
    public Vector3 m_lastSpawnPosition;
    public Vector3 m_lastLocalSpawnPosition;
    public float m_leftRightAngle;
    public float m_leftRightPercent;
    public float m_downTopAngle;

    private void Awake()
    {
        m_playerAbstractLayerTBIO.AddListenerOnByteReceivedFromServer(ReceivedBytes);
        m_playerAbstractLayerTBIO.AddListenerOnTextReceivedFromServer(ReceivedText);
    }

    public void PushInteger(int integer)
    {
        m_playerAbstractLayerTBIO.SendByteToServer(System.BitConverter.GetBytes(integer));
    }


    public int PercentTo99(float percent11) {
        if (percent11==0f)
            return 0;
        return 1+(int)( ( (percent11+1f) / 2f ) * 98f);
    }

    public float m_angleClamp = 30f;
    public float m_moveClamp = 0.3f;
    public int m_lastSentInteger = 0;

    public float m_xPercent;
    public float m_yPercent;
    public float m_zPercent;

    IEnumerator Start()
    {
        while (true)
        {

            Vector3 currentPosition = m_currentPosition;
            Quaternion currentRotation = Quaternion.Euler(m_currentEulerRotation);
            Vector3 target = m_lastSpawnPosition;
            Vector3 localtarget = Quaternion.Inverse(currentRotation)* (target - currentPosition);
            Vector3 localZX = new Vector3(localtarget.x, 0, localtarget.z);
            Vector3 localYX = new Vector3(localtarget.x, localtarget.y, 0);

            float xPercent = Mathf.Clamp(localtarget.x / m_moveClamp, -1f, 1f);
            float yPercent = Mathf.Clamp(localtarget.y / m_moveClamp, -1f, 1f);
            float zPercent = Mathf.Clamp(localtarget.z / m_moveClamp, -1f, 1f);
            m_xPercent=xPercent;
            m_yPercent=yPercent;
            m_zPercent=zPercent;

            Debug.DrawLine(Vector3.zero, localtarget, Color.red);
            Debug.DrawLine(currentPosition, target, Color.green);

            m_lastLocalSpawnPosition = localtarget;

            int isTargetUp =0 ;
            int isTargetRight =0 ;
            int isTargetFront =0 ;

            isTargetRight =PercentTo99(xPercent);
            isTargetUp =PercentTo99(yPercent);
            isTargetFront =PercentTo99(zPercent);

            Vector3 localTargetZX = new Vector3(localtarget.x, 0, localtarget.z);
            float leftRight = Vector3.SignedAngle(Vector3.forward, localTargetZX, Vector3.up);
            m_leftRightAngle = leftRight;

            float leftRightPercent =Mathf.Clamp( leftRight / m_angleClamp, -1f, 1f);
            m_leftRightPercent = leftRightPercent;

            int gamepad = 1800000000;
            gamepad += PercentTo99(-leftRightPercent) * 1000000;
            gamepad += PercentTo99(isTargetUp ) * 10000;
            gamepad += PercentTo99(isTargetRight ) * 100;
            gamepad += PercentTo99(isTargetFront ) * 1;
            m_lastSentInteger = gamepad;
            PushInteger(gamepad);


            //PushInteger(1899000000);
            //yield return new WaitForSeconds(1);
            //PushInteger(1801000000);
            //yield return new WaitForSeconds(1);
            //PushInteger(1800990000);
            //yield return new WaitForSeconds(1);
            //PushInteger(1800010000);
            //yield return new WaitForSeconds(1);
            //PushInteger(1800009900);
            //yield return new WaitForSeconds(1);
            //PushInteger(1800000100);
            //yield return new WaitForSeconds(1);
            //PushInteger(180000099);
            //yield return new WaitForSeconds(1);
            //PushInteger(180000001);
            //yield return new WaitForSeconds(1);


            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ReceivedText(string text)
    {
        m_lastReceivedText = text;
        if (text.StartsWith("SPAWN:"))
        {
            string[] parts = text.Substring(6).Split(':');
            if (parts.Length >= 3)
            {
                if (float.TryParse(parts[0], out float posX) &&
                    float.TryParse(parts[1], out float posY) &&
                    float.TryParse(parts[2], out float posZ) )
                {
                    m_lastSpawnPosition = new Vector3(posX, posY, posZ);
                }
            }
        }

        else if (text.StartsWith("PE:"))
        {
            string[] parts = text.Substring(3).Split(':');
            if (parts.Length >= 6)
            {
                if (float.TryParse(parts[0], out float posX) &&
                    float.TryParse(parts[1], out float posY) &&
                    float.TryParse(parts[2], out float posZ) &&
                    float.TryParse(parts[3], out float rotX) &&
                    float.TryParse(parts[4], out float rotY) &&
                    float.TryParse(parts[5], out float rotZ))
                {
                    m_currentPosition = new Vector3(posX, posY, posZ);
                    m_currentEulerRotation = new Vector3(rotX, rotY, rotZ);
                }
            }
        }

        else if (text.StartsWith("I:"))
        {
            string indexPart = text.Substring(2);
            if (int.TryParse(indexPart, out int index))
            {
                m_playerIndex = index;
            }
        }

        else if (text.StartsWith("POINTS:"))
        {
            string[] parts = text.Substring(7).Split(':');
            int elementCount = parts.Length / 3;
            if (m_pointsPosition == null || m_pointsPosition.Length != elementCount)
            {
                m_pointsPosition = new Vector3[elementCount];
            }
            for (int i = 0; i < elementCount; i++)
            {
                if (float.TryParse(parts[i * 3], out float posX) &&
                    float.TryParse(parts[i * 3 + 1], out float posY) &&
                    float.TryParse(parts[i * 3 + 2], out float posZ))
                {
                    m_pointsPosition[i] = new Vector3(posX, posY, posZ);
                }
            }
        }
        else if (text.StartsWith("P_ALL:"))
        {
            string[] parts = text.Substring(6).Split(':');
            int elementCount = parts.Length / 3;
            if (m_playersPosition == null || m_playersPosition.Length != elementCount)
            {
                m_playersPosition = new Vector3[elementCount];
            }
            for (int i = 0; i < elementCount; i++)
            {
                if (float.TryParse(parts[i * 3], out float posX) &&
                    float.TryParse(parts[i * 3 + 1], out float posY) &&
                    float.TryParse(parts[i * 3 + 2], out float posZ))
                {
                    m_playersPosition[i] = new Vector3(posX, posY, posZ);
                }
            }
        }
        else if (text.StartsWith("I_ALL:"))
        {
            string[] parts = text.Substring(6).Split(':');
            int elementCount = parts.Length;
            if (m_playersIndex == null || m_playersIndex.Length != elementCount)
            {
                m_playersIndex = new int[elementCount];
            }
            for (int i = 0; i < elementCount; i++)
            {
                if (int.TryParse(parts[i], out int index))
                {
                    m_playersIndex[i] = index;
                }
                else {
                    m_playersIndex[i] = -1;
                }
            }
        }
    }

public void ReceivedBytes(byte[] bytes)
    {
        m_lastReceivedBytes = bytes;
    }

    
}
