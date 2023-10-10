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
            byte[] heartRateData = GetTimestampBytes(hr.Timestamp).Concat(BitConverter.GetBytes(hr.Rate)).ToArray();

            PubSocket.SendMoreFrame("HeartRate")
                .SendFrame(heartRateData);
        }
    }

    public void HandleMouthCameraImage(CameraImage camImage)
    {
        if (camImage != null)
        {
            if (camImage.SensorInfo.Location == "Mouth")
            {
                byte[] cameraImageData = GetTimestampBytes(camImage.Timestamp)
                    .Concat(BitConverter.GetBytes(camImage.Width))
                    .Concat(BitConverter.GetBytes(camImage.Height))
                    .Concat(camImage.ImageData)
                    .ToArray();

                PubSocket.SendMoreFrame("Mouth")
                    .SendFrame(cameraImageData);
            }
        }
    }

    public void HandleEyeTracking(EyeTracking et)
    {
        if (et != null)
        {
            byte[] gazeData = GetTimestampBytes(et.Timestamp)
                .Concat(BitConverter.GetBytes(et.CombinedGaze.X))
                .Concat(BitConverter.GetBytes(et.CombinedGaze.Y))
                .Concat(BitConverter.GetBytes(et.CombinedGaze.Z))
                .ToArray();

            PubSocket.SendMoreFrame("EyeTracking")
                .SendFrame(gazeData);
        }
    }

    public void HandleIMU(IMUFrame imu)
    {
        if (imu != null)
        {
            byte[] imuData = GetTimestampBytes(imu.Timestamp)
                .Concat(BitConverter.GetBytes(imu.Data[0].Acc.X))
                .Concat(BitConverter.GetBytes(imu.Data[0].Acc.Y))
                .Concat(BitConverter.GetBytes(imu.Data[0].Acc.Z))
                .ToArray();

            PubSocket.SendMoreFrame("IMU")
                .SendFrame(imuData);
        }
    }
}