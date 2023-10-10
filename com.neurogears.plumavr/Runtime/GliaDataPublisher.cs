using HP.Omnicept.Messaging.Messages;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GliaDataPublisher : DataPublisher
{
    IEnumerable<byte> GetTimestampBytes(Timestamp timestamp)
    {
        return BitConverter.GetBytes(timestamp.HardwareTimeMicroSeconds)
            .Concat(BitConverter.GetBytes(timestamp.OmniceptTimeMicroSeconds))
            .Concat(BitConverter.GetBytes(timestamp.SystemTimeMicroSeconds));
    }

    public void HandleHeartRate(HeartRate hr)
    {
        if (hr != null)
        {
            byte[] heartRateData = BitConverter.GetBytes(hr.Rate);

            PubSocket.SendMoreFrame("HeartRate")
                .SendMoreFrame(GetTimestampBytes(hr.Timestamp).ToArray())
                .SendFrame(heartRateData);
        }
    }

    public void HandleMouthCameraImage(CameraImage camImage)
    {
        if (camImage != null)
        {
            if (camImage.SensorInfo.Location == "Mouth")
            {
                byte[] cameraImageData = BitConverter.GetBytes(camImage.Width)
                    .Concat(BitConverter.GetBytes(camImage.Height))
                    .Concat(camImage.ImageData)
                    .ToArray();

                PubSocket.SendMoreFrame("Mouth")
                    .SendMoreFrame(GetTimestampBytes(camImage.Timestamp).ToArray())
                    .SendFrame(cameraImageData);
            }
        }
    }

    public void HandleEyeTracking(EyeTracking et)
    {
        if (et != null)
        {
            byte[] gazeData = BitConverter.GetBytes(et.CombinedGaze.X)
                .Concat(BitConverter.GetBytes(et.CombinedGaze.Y))
                .Concat(BitConverter.GetBytes(et.CombinedGaze.Z))
                .ToArray();

            PubSocket.SendMoreFrame("EyeTracking") // Topic
                .SendMoreFrame(GetTimestampBytes(et.Timestamp).ToArray()) // Timestamp
                .SendFrame(gazeData); // Blob data
        }
    }

    public void HandleIMU(IMUFrame imu)
    {
        if (imu != null)
        {
            byte[] imuData = BitConverter.GetBytes(imu.Data[0].Acc.X)
                .Concat(BitConverter.GetBytes(imu.Data[0].Acc.Y))
                .Concat(BitConverter.GetBytes(imu.Data[0].Acc.Z))
                .ToArray();

            PubSocket.SendMoreFrame("IMU")
                .SendMoreFrame(GetTimestampBytes(imu.Timestamp).ToArray())
                .SendFrame(imuData);
        }
    }
}