using System;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.TBIO
{


    public class TextByteInOutLayerMono : AbstractTextByteInOutLayerMono
    {
        [SerializeField] UnityEvent<byte[]> m_onByteSendToServer=new UnityEvent<byte[]>();
        [SerializeField] UnityEvent<string> m_onTextSendToServer=new UnityEvent<string>();
        [SerializeField] UnityEvent<byte[]> m_onByteReceivedFromServer=new UnityEvent<byte[]>();
        [SerializeField] UnityEvent<string> m_onTextReceivedFromServer=new UnityEvent<string>();


        public override void SendByteToServer(byte[] byteArray)
        {
            m_onByteSendToServer?.Invoke(byteArray);
        }

        public override void SendTextToServer(string text)
        {
            m_onTextSendToServer?.Invoke(text);
        }

        public override void SendByteToClient(byte[] byteArray)
        {
            m_onByteReceivedFromServer?.Invoke(byteArray);
        }

        public override void SendTextToClient(string text)
        {
            m_onTextReceivedFromServer?.Invoke(text);
        }


        public override void AddListenerOnByteSendToServer(Action<byte[]> listener)
        {
            m_onByteSendToServer.AddListener(listener.Invoke);
        }
        public override void AddListenerOnTextSendToServer(Action<string> listener)
        {
            m_onTextSendToServer.AddListener(listener.Invoke);
        }

        public override void AddListenerOnByteReceivedFromServer(Action<byte[]> listener)
        {
            m_onByteReceivedFromServer.AddListener(listener.Invoke);
        }
        public override void AddListenerOnTextReceivedFromServer(Action<string> listener)
        {
            m_onTextReceivedFromServer.AddListener(listener.Invoke);
        }

        public override void RemoveListenerOnByteSendToServer(Action<byte[]> listener)
        {
            m_onByteSendToServer.RemoveListener(listener.Invoke);
        }

        public override void RemoveListenerOnTextSendToServer(Action<string> listener)
        {
            m_onTextSendToServer.RemoveListener(listener.Invoke);
        }

        public override void RemoveListenerOnByteReceivedFromServer(Action<byte[]> listener)
        {
            m_onByteReceivedFromServer.RemoveListener(listener.Invoke);
        }

        public override void RemoveListenerOnTextReceivedFromServer(Action<string> listener)
        {
            m_onTextReceivedFromServer.RemoveListener(listener.Invoke);
        }
    }

}