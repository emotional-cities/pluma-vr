using HP.Omnicept.Messaging.Messages;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GliaDataPublisher : DataPublisher
{
    public Transform Camera;
    public Vector3 hitPoint = new Vector3();
    public int MaxDistance;
    public LayerMask HitObjects;
    IEnumerable<byte> GetTimestampBytes(Timestamp timestamp)
    {
        return BitConverter.GetBytes(timestamp.HardwareTimeMicroSeconds)
            .Concat(BitConverter.GetBytes(timestamp.OmniceptTimeMicroSeconds))
            .Concat(BitConverter.GetBytes(timestamp.SystemTimeMicroSeconds));
    }

    IEnumerable<byte> GetEyeBytes(Eye eye)
    {
        return BitConverter.GetBytes(eye.Openness)
            .Concat(BitConverter.GetBytes(eye.OpennessConfidence))
            .Concat(BitConverter.GetBytes(eye.PupilDilation))
            .Concat(BitConverter.GetBytes(eye.PupilDilationConfidence))
            .Concat(BitConverter.GetBytes(eye.PupilPosition.X))
            .Concat(BitConverter.GetBytes(eye.PupilPosition.Y));
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
            var combinedGazeDirection = new Vector3(-et.CombinedGaze.X, et.CombinedGaze.Y, et.CombinedGaze.Z);

            RaycastHit hit;
            Debug.DrawRay(Camera.transform.position, (Camera.transform.rotation * combinedGazeDirection) * 100, UnityEngine.Color.red);
            //var ray = new Ray(Camera.transform.position, (Camera.transform.rotation * combinedGazeDirection));
            Ray ray = new Ray(Camera.transform.position, Camera.transform.TransformDirection(combinedGazeDirection));

            if (Physics.Raycast(ray, out hit,300, HitObjects))
            {
                //EyeHit.transform.position = hit.point;
                hitPoint = hit.point;
                //Debug.Log(combinedGazeDirection);
            }
            else
            {
                //EyeHit.transform.position = ray.GetPoint(MaxDistance);
                //Debug.Log("None");
                hitPoint= ray.GetPoint(MaxDistance);
            }
            byte[] positionData = BitConverter.GetBytes(hitPoint.x)
                .Concat(BitConverter.GetBytes(hitPoint.y))
                .Concat(BitConverter.GetBytes(hitPoint.z))
                .ToArray();
            
            byte[] gazeData = BitConverter.GetBytes(et.CombinedGaze.X)
                .Concat(BitConverter.GetBytes(et.CombinedGaze.Y))
                .Concat(BitConverter.GetBytes(et.CombinedGaze.Z))
                .Concat(GetEyeBytes(et.LeftEye))
                .Concat(GetEyeBytes(et.RightEye))
                .ToArray();
            byte[] allData = gazeData.Concat(positionData).ToArray();
            PubSocket.SendMoreFrame("EyeTracking") // Topic
                .SendMoreFrame(GetTimestampBytes(et.Timestamp).ToArray()) // Timestamp
                .SendFrame(allData); // Blob data
        }
    }

    public void HandleIMU(IMUFrame imu)
    {
        if (imu != null)
        {
            byte[] imuData = BitConverter.GetBytes(imu.Data[0].Acc.X)
                .Concat(BitConverter.GetBytes(imu.Data[0].Acc.Y))
                .Concat(BitConverter.GetBytes(imu.Data[0].Acc.Z))
                .Concat(BitConverter.GetBytes(imu.Data[0].Gyro.X))
                .Concat(BitConverter.GetBytes(imu.Data[0].Gyro.Y))
                .Concat(BitConverter.GetBytes(imu.Data[0].Gyro.Z))
                .ToArray();

            PubSocket.SendMoreFrame("IMU")
                .SendMoreFrame(GetTimestampBytes(imu.Timestamp).ToArray())
                .SendFrame(imuData);
        }
    }
}