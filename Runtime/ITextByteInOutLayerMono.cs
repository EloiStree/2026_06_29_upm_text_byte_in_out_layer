using System;

namespace Eloi.TBIO
{
    public interface ITextByteInOutLayerMono
    {
        public void SendByteToServer(byte[] byteArray);
        public void SendTextToServer(string text);
        public void SendByteToClient(byte[] byteArray);
        public void SendTextToClient(string text);

        public void AddListenerOnByteSendToServer(Action<byte[]> listener);
        public void AddListenerOnTextSendToServer(Action<string> listener);
        public void AddListenerOnByteReceivedFromServer(Action<byte[]> listener);
        public void AddListenerOnTextReceivedFromServer(Action<string> listener);
        public void RemoveListenerOnByteSendToServer(Action<byte[]> listener);
        public void RemoveListenerOnTextSendToServer(Action<string> listener);
        public void RemoveListenerOnByteReceivedFromServer(Action<byte[]> listener);
        public void RemoveListenerOnTextReceivedFromServer(Action<string> listener);

    }

}