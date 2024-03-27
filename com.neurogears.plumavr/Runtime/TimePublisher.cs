using NetMQ;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TimePublisher : DataPublisher
{
    protected override void Start()
    {
        base.Start();

        StartCoroutine(PublishTime());
    }

    IEnumerator PublishTime()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            yield return new WaitForEndOfFrame();
            long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);

            PubSocket.SendMoreFrame("Time")
                .SendFrame(BitConverter.GetBytes(timestamp));
        }


    }
}
