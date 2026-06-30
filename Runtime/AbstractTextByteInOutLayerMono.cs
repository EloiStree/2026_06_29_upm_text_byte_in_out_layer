using System;
using UnityEngine;

namespace Eloi.TBIO
{
    public abstract class AbstractTextByteInOutLayerMono : MonoBehaviour, ITextByteInOutLayerMono
    {
        public abstract void SendByteToServer(byte[] byteArray);
        public abstract void SendTextToServer(string text);
        public abstract void SendByteToClient(byte[] byteArray);
        public abstract void SendTextToClient(string text);

        public abstract void AddListenerOnByteSendToServer(Action<byte[]> listener);
        public abstract void AddListenerOnTextSendToServer(Action<string> listener);
        public abstract void AddListenerOnByteReceivedFromServer(Action<byte[]> listener);
        public abstract void AddListenerOnTextReceivedFromServer(Action<string> listener);
        public abstract void RemoveListenerOnByteSendToServer(Action<byte[]> listener);
        public abstract void RemoveListenerOnTextSendToServer(Action<string> listener);
        public abstract void RemoveListenerOnByteReceivedFromServer(Action<byte[]> listener);
        public abstract void RemoveListenerOnTextReceivedFromServer(Action<string> listener);
    }

}