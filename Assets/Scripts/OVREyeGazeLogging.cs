using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OVREyeGazeLogging : MonoBehaviour
{
    private OVREyeGaze _eyeGaze;
    private List<string> eyeTrackingData;
    private string filePath;

    private void Start()
    {
        //Access the OVREyeGaze component
        _eyeGaze = GetComponent<OVREyeGaze>();

        if (_eyeGaze == null )
        {
            Debug.LogError("OVREyeGaze component not found on this GameObject.");
            enabled = false;
            return;
        }

        //Initialize data logging
        eyeTrackingData = new List<string>();
        filePath = Path.Combine(Application.persistentDataPath, "ASD-EyeTrackingData.csv");

        eyeTrackingData.Add("Timestamp,GazeDirectionX,GazeDirectionY,GazeDirectionZ,RotationX," +
            "RotationY,RotationZ,RotationW,Confidence");

    }

    private void Update()
    {
        //Ensure eye tracking is enabled and data is valid
        if (!_eyeGaze.EyeTrackingEnabled || _eyeGaze.Confidence < _eyeGaze.ConfidenceThreshold) return;

        //Retrieve gaze direction and rotation
        Vector3 gazeDirection = _eyeGaze.transform.forward;
        Quaternion gazeRotation = _eyeGaze.transform.rotation;

        //Log data into buffer
        float timestamp = Time.time;
        string dataLine = $"{timestamp}, {gazeDirection.x}, {gazeDirection.y}, {gazeDirection.z}, " +
                          $"{gazeRotation.x}, {gazeRotation.y}, {gazeRotation.z}, {gazeRotation.w}, {_eyeGaze.Confidence}";
        eyeTrackingData.Add(dataLine);
    }

    private void OnApplicationQuit()
    {
        using (StreamWriter sw= new StreamWriter(filePath))
        {
            foreach (string dataLine in eyeTrackingData)
            {
                sw.WriteLine(dataLine);
            }
        }

        Debug.Log($"Eye tracking data written to {filePath}");
    }


}
