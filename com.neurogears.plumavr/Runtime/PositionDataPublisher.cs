using NetMQ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDataPublisher : DataPublisher
{
    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;

        PubSocket.SendMoreFrame("Position")
            .SendMoreFrame(BitConverter.GetBytes(position.x))
            .SendMoreFrame(BitConverter.GetBytes(position.y))
            .SendMoreFrame(BitConverter.GetBytes(position.z));

        PubSocket.SendMoreFrame("ForwardVector")
            .SendMoreFrame(BitConverter.GetBytes(forward.x))
            .SendMoreFrame(BitConverter.GetBytes(forward.y))
            .SendMoreFrame(BitConverter.GetBytes(forward.z));
    }
}
