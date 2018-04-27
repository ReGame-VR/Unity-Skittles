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

        SkittlesGame.OnRecordContinuousData += recordContinuousTrial;
        SkittlesGame.OnRecordTrialData += recordTrial;
        VirtualSkittlesGame.OnRecordContinuousData += recordContinuousTrial;
        VirtualSkittlesGame.OnRecordTrialData += recordTrial;
     
	}

    /// <summary>
    /// Write all data to a file
    /// </summary>
    void OnDisable()
    {
        WriteContinuousFile();
        WriteTrialFile();
    }

    // Records continuous data into the data list
    private void recordContinuousTrial(float time, int trialNum, Vector3 ballPosition, Vector3 wristPosition, Vector3 armPosition)
    {
        continuousData.Add(new ContinuousData(time, trialNum, ballPosition, CoPtoCM(Wii.GetCenterOfBalance(0)), wristPosition, armPosition));
    }

    // Records trial data into the data list
    private void recordTrial(float time, int curTrial, Vector3 ballPosition, Vector3 wristPosition,
        float errorDistance, float ballVelocity, Vector3 poleTopPosition, float ropePoleAngle)
    {
        trialData.Add(new TrialData(time, curTrial, ballPosition, wristPosition, errorDistance, ballVelocity,
            poleTopPosition, ropePoleAngle));
    }

    /// <summary>
    /// A class that stores info continuously. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class ContinuousData
    {
        public readonly float time;
        public readonly int trialNum;
        public readonly Vector3 ballPosition;
        public readonly Vector2 cop;
        public readonly Vector3 wristPosition;
        public readonly Vector3 armPosition;

        public ContinuousData(float time, int trialNum, Vector3 ballPosition, Vector2 cop, Vector3 wristPosition, Vector3 armPosition)
        {
            this.time = time;
            this.trialNum = trialNum;
            this.ballPosition = ballPosition;
            this.cop = cop;
            this.wristPosition = wristPosition;
            this.armPosition = armPosition;
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

        public readonly Vector3 poleTopPosition;
        public readonly float ropePoleAngle;

        public TrialData(float time, int curTrial, Vector3 ballPosition, Vector3 wristPosition,
            float errorDistance, float ballVelocity, Vector3 poleTopPosition, float ropePoleAngle)
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
            this.poleTopPosition = poleTopPosition;
            this.ropePoleAngle = ropePoleAngle;
        }
    }


    /// <summary>
    /// Writes the Continuous File to a CSV
    /// </summary>
    private void WriteContinuousFile()
    {

        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid + VersionString());
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + VersionString() + "/Continuous" + pid + ".csv"))
        {
            Debug.Log("Writing continuous data to file");
            
            // write header
            CsvRow header = new CsvRow();
            header.Add("Time");
            header.Add("Trial Number");
            header.Add("Ball X");
            header.Add("Ball Y");
            header.Add("Ball Z");
            header.Add("COP X");
            header.Add("COP Y");
            header.Add("Wrist X");
            header.Add("Wrist Y");
            header.Add("Wrist Z");
            header.Add("Arm X");
            header.Add("Arm Y");
            header.Add("Arm Z");

            writer.WriteRow(header);

            // write each line of data
            foreach (ContinuousData d in continuousData)
            {
                CsvRow row = new CsvRow();

                row.Add(d.time.ToString());
                row.Add(d.trialNum.ToString());
                row.Add(d.ballPosition.x.ToString());
                row.Add(d.ballPosition.y.ToString());
                row.Add(d.ballPosition.z.ToString());
                row.Add(d.cop.x.ToString());
                row.Add(d.cop.y.ToString());
                row.Add(d.wristPosition.x.ToString());
                row.Add(d.wristPosition.y.ToString());
                row.Add(d.wristPosition.z.ToString());
                row.Add(d.armPosition.x.ToString());
                row.Add(d.armPosition.y.ToString());
                row.Add(d.armPosition.z.ToString());

                writer.WriteRow(row);
            }
        }
        SkittlesGame.OnRecordContinuousData -= recordContinuousTrial;
        VirtualSkittlesGame.OnRecordContinuousData -= recordContinuousTrial;
    }


    /// <summary>
    /// Writes the Trial File to a CSV
    /// </summary>
    private void WriteTrialFile()
    {

        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid + VersionString());
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + VersionString() + "/Trial" + pid + ".csv"))
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
            header.Add("Ball Velocity m/s");
            header.Add("Pole Top X");
            header.Add("Pole Top Y");
            header.Add("Pole Top Z");
            header.Add("Rope-Pole Angle (Degrees)");
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
                row.Add(d.poleTopPosition.x.ToString());
                row.Add(d.poleTopPosition.y.ToString());
                row.Add(d.poleTopPosition.z.ToString());
                row.Add(d.ropePoleAngle.ToString());

                writer.WriteRow(row);
            }
        }
        SkittlesGame.OnRecordTrialData -= recordTrial;
        VirtualSkittlesGame.OnRecordTrialData -= recordTrial;
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

    /// <summary>
    /// Converts COP ratio to be in terms of cm to match PE task.
    /// </summary>
    /// <param name="posn"> The current COB posn, not in terms of cm </param>
    /// <returns> The posn, in terms of cm </returns>
    public static Vector2 CoPtoCM(Vector2 posn)
    {
        return new Vector2(posn.x * 43.3f / 2f, posn.y * 23.6f / 2f);
    }
}
