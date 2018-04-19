using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;
using System.IO;

/// <summary>
/// Writes a line of data after every trial, giving information on the trial.
/// </summary>
public class DataHandler : MonoBehaviour {

    // stores the data for writing to file at end of task
    List<ContinuousData> continuousData = new List<ContinuousData>();
    List<TrialData> trialData = new List<TrialData>();

    private string pid = GlobalControl.Instance.participantID;

	/// <summary>
    /// Subscribe to data writing events.
    /// </summary>
	void Awake () {
        if (GlobalControl.Instance.isRealLife)
        {
            SkittlesGame.OnRecordContinuousData += recordContinuousTrial;
            SkittlesGame.OnRecordTrialData += recordTrial;
        }
        else
        {
            VirtualSkittlesGame.OnRecordContinuousData += recordContinuousTrial;
            VirtualSkittlesGame.OnRecordTrialData += recordTrial;
        }

	}

    /// <summary>
    /// Write all data to a file and unsubscribe from data writing event.
    /// </summary>
    void OnDisable()
    {
        WriteContinuousFile();
        WriteTrialFile();
    }

    // Records continuous data into the data list
    private void recordContinuousTrial(float time, Vector3 ballPosition)
    {
        continuousData.Add(new ContinuousData(time, ballPosition));
    }

    // Records trial data into the data list
    private void recordTrial(float time, int curTrial, Vector3 ballPosition, Vector3 wristPosition,
        float errorDistance, float ballVelocity)
    {
        trialData.Add(new TrialData(time, curTrial, ballPosition, wristPosition, errorDistance, ballVelocity));
    }

    /// <summary>
    /// A class that stores info continuously. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class ContinuousData
    {
        public readonly float time;
        public readonly Vector3 ballPosition;

        public ContinuousData(float time, Vector3 ballPosition)
        {
            this.time = time;
            this.ballPosition = ballPosition;
        }
    }

    /// <summary>
    /// A class that stores info on each trial relevant to data recording. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class TrialData
    {
        public readonly float time;
        public readonly bool rightHanded;
        public readonly int curTrial;
        public readonly Vector3 ballPosition;
        public readonly Vector3 wristPosition;
        public readonly float handToBallDistance;

        public readonly bool targetHit;
        public readonly float errorDistance;
        public readonly float ballVelocity;

        public TrialData(float time, int curTrial, Vector3 ballPosition, Vector3 wristPosition,
            float errorDistance, float ballVelocity)
        {
            this.time = time;
            this.curTrial = curTrial;
            this.ballPosition = ballPosition;
            this.wristPosition = wristPosition;
            this.handToBallDistance = Vector3.Distance(ballPosition, wristPosition);
            if (errorDistance == 0f)
            {
                this.targetHit = true;
            }
            else
            {
                this.targetHit = false;
            }
            this.errorDistance = errorDistance;
            this.ballVelocity = ballVelocity;
        }
    }


    /// <summary>
    /// Writes the Continuous File to a CSV
    /// </summary>
    private void WriteContinuousFile()
    {

        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid + VersionString());
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "/Continuous" + pid + ".csv"))
        {
            Debug.Log("Writing continuous data to file");
            
            // write header
            CsvRow header = new CsvRow();
            header.Add("Time");
            header.Add("Ball X");
            header.Add("Ball Y");
            header.Add("Ball Z");
            writer.WriteRow(header);

            // write each line of data
            foreach (ContinuousData d in continuousData)
            {
                CsvRow row = new CsvRow();

                row.Add(d.time.ToString());
                row.Add(d.ballPosition.x.ToString());
                row.Add(d.ballPosition.y.ToString());
                row.Add(d.ballPosition.z.ToString());

                writer.WriteRow(row);
            }
        }
        SkittlesGame.OnRecordContinuousData -= recordContinuousTrial;
        VirtualSkittlesGame.OnRecordContinuousData -= recordContinuousTrial;
    }


    /// <summary>
    /// Writes the Continuous File to a CSV
    /// </summary>
    private void WriteTrialFile()
    {

        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid + VersionString());
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "/Trial" + pid + ".csv"))
        {
            Debug.Log("Writing trial data to file");

            // write header
            CsvRow header = new CsvRow();
            header.Add("Time");
            header.Add("Right Hand Dominant?");
            header.Add("Current Trial");
            header.Add("Ball X");
            header.Add("Ball Y");
            header.Add("Ball Z");
            header.Add("Wrist X");
            header.Add("Wrist Y");
            header.Add("Wrist Z");
            header.Add("Hand-Ball Distance");
            header.Add("Target Hit?");
            header.Add("Error Distance");
            header.Add("Ball Velocity");
            writer.WriteRow(header);

            // write each line of data
            foreach (TrialData d in trialData)
            {
                CsvRow row = new CsvRow();

                row.Add(d.time.ToString());
                if (GlobalControl.Instance.rightHanded)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
                }
                row.Add(d.curTrial.ToString());
                row.Add(d.ballPosition.x.ToString());
                row.Add(d.ballPosition.y.ToString());
                row.Add(d.ballPosition.z.ToString());
                row.Add(d.wristPosition.x.ToString());
                row.Add(d.wristPosition.y.ToString());
                row.Add(d.wristPosition.z.ToString());
                row.Add(d.handToBallDistance.ToString());
                if (d.targetHit)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
                }
                row.Add(d.errorDistance.ToString());
                row.Add(d.ballVelocity.ToString());

                writer.WriteRow(row);
            }
        }
        SkittlesGame.OnRecordContinuousData -= recordContinuousTrial;
        VirtualSkittlesGame.OnRecordContinuousData -= recordContinuousTrial;
    }

    // Returns the string denoting version of the game (real life or virtual)
    // Used to label data.
    private string VersionString()
    {
        if (GlobalControl.Instance.isRealLife)
        {
            return "RealLife";
        }
        else
        {
            return "Virtual";
        }
    }
}
