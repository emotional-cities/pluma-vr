using NetMQ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionDataPublisher : DataPublisher
{
    // Update is called once per frame
    void Update()
    {
        long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);

        byte[] positionData = BitConverter.GetBytes(transform.position.x)
            .Concat(BitConverter.GetBytes(transform.position.y))
            .Concat(BitConverter.GetBytes(transform.position.z))
            .ToArray();

        byte[] forwardData = BitConverter.GetBytes(transform.forward.x)
            .Concat(BitConverter.GetBytes(transform.forward.y))
            .Concat(BitConverter.GetBytes(transform.forward.z))
            .ToArray();

        byte[] allData = positionData.Concat(forwardData).ToArray();

        PubSocket.SendMoreFrame("Position")
            .SendMoreFrame(BitConverter.GetBytes(timestamp))
            .SendFrame(allData);
    }
}
