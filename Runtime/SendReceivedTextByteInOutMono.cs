using UnityEngine;

namespace Eloi.TBIO
{
    public class SendReceivedTextByteInOutMono : MonoBehaviour { 
    
        public AbstractTextByteInOutLayerMono m_textByteInOutLayer;

        [TextArea(2, 10)]
        [SerializeField] public string m_sendTextToServer;
        [SerializeField] public byte[] m_sendByteToServer;
        [TextArea(2, 10)]
        [SerializeField] public string m_receivedTextFromServer;
        [SerializeField] public byte[] m_receivedByteFromServer;

        [SerializeField] public int m_sendTextToServerSize = 0;
        [SerializeField] public int m_sendByteToServerSize = 0;
        [SerializeField] public int m_receivedTextFromServerSize = 0;
        [SerializeField] public int m_receivedByteFromServerSize = 0;

            
        public int m_maxTextSizeInInspector = 1000;

        private void OnEnable()
        {
            m_textByteInOutLayer.AddListenerOnTextSendToServer(OnTextSendToServer);
            m_textByteInOutLayer.AddListenerOnByteSendToServer(OnByteSendToServer);
            m_textByteInOutLayer.AddListenerOnTextReceivedFromServer(OnTextReceivedFromServer);
            m_textByteInOutLayer.AddListenerOnByteReceivedFromServer(OnByteReceivedFromServer);

        }

        private void OnDisable()
        {
            m_textByteInOutLayer.RemoveListenerOnTextSendToServer(OnTextSendToServer);
            m_textByteInOutLayer.RemoveListenerOnByteSendToServer(OnByteSendToServer);
            m_textByteInOutLayer.RemoveListenerOnTextReceivedFromServer(OnTextReceivedFromServer);
            m_textByteInOutLayer.RemoveListenerOnByteReceivedFromServer(OnByteReceivedFromServer);
        }

        private void OnTextSendToServer(string text)
        {
            if (text == null)
                text = "";

            if (text.Length > m_maxTextSizeInInspector)
                text = text.Substring(0, m_maxTextSizeInInspector);
            m_sendTextToServer = text;
            m_sendTextToServerSize = text.Length;
        }
        private void OnByteSendToServer(byte[] byteArray)
        {
            if (byteArray == null)
                byteArray = new byte[0];    

            m_sendByteToServer = byteArray;
            m_sendByteToServerSize = byteArray.Length;

        }
        private void OnTextReceivedFromServer(string text)
        {
            if (text == null)
                text = "";

            if (text.Length > m_maxTextSizeInInspector)
                text = text.Substring(0, m_maxTextSizeInInspector);
            m_receivedTextFromServer = text;
            m_receivedTextFromServerSize = text.Length;
        }
        private void OnByteReceivedFromServer(byte[] byteArray)
        {
            if (byteArray==null)
                byteArray = new byte[0];

            m_receivedByteFromServer = byteArray;
            m_receivedByteFromServerSize = byteArray.Length;
        }
    }

}