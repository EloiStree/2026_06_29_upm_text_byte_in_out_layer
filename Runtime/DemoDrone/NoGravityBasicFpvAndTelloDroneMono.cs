using UnityEngine;

namespace Eloi.TBIO
{

    public class NoGravityBasicFpvAndTelloDroneMono : MonoBehaviour
    {
        public bool m_isTelloEasyMode = true;
        public NoGravityBasicFpvDroneMono m_fpv;
        public NoGravityBasicTelloDroneMono m_tello;

        public void SetStableMode() {

            m_isTelloEasyMode = true;
            if (m_fpv != null)
                m_fpv.enabled= false;
            if (m_tello != null)
                m_tello.enabled= true;
        }

        public void SetAcrobaticMode() { 
            
            m_isTelloEasyMode = false;
            if (m_fpv != null)
                m_fpv.enabled= true;
            if (m_tello != null)
                m_tello.enabled= false;
        }
        public void ToggleFlyingMode() {
            SetTelloModeState(!m_isTelloEasyMode);
        }

        public void SetTelloModeState(bool isEasyMode) { 
            if (isEasyMode)
                SetStableMode();
            else
                SetAcrobaticMode();
        }

        public void SetDoubleJoystick(Vector2 leftJoystick, Vector2 rightJoystick)
        {
            if (m_isTelloEasyMode)
            {
                    m_tello.SetDoubleJoystick(leftJoystick, rightJoystick);
                    m_fpv.SetDoubleJoystick(Vector2.zero, Vector2.zero);

            }
            else
            {
                    m_fpv.SetDoubleJoystick(leftJoystick, rightJoystick);
                    m_tello.SetDoubleJoystick(Vector2.zero, Vector2.zero);
            }
        }

        public void SetWith1899887766(byte[] valueLittleEndian)
        {
            if(m_isTelloEasyMode)
            {
                m_tello.SetWith1899887766(valueLittleEndian);
                m_fpv.SetWith1899887766(1800000000);
            }
            else
            {
                m_fpv.SetWith1899887766(valueLittleEndian);
                m_tello.SetWith1899887766(1800000000);
            }
        }
    }
}

