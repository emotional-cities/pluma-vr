using NetMQ;
using NetMQ.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPublisher : MonoBehaviour
{
    public string PublisherBindAddress;
    protected PublisherSocket PubSocket;

    protected virtual void Awake()
    {
        AsyncIO.ForceDotNet.Force();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        PubSocket = new PublisherSocket();
        PubSocket.Bind(PublisherBindAddress);
    }

    protected virtual void OnApplicationQuit()
    {
        if (PubSocket != null)
        {
            PubSocket.Dispose();
        }
        NetMQConfig.Cleanup(false);
    }
}
