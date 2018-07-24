using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;
using System.IO;

// A class that records data relevant to an explore condition in a skittles game
public class ExplorationRecording : MonoBehaviour {

    private string pid = GlobalControl.Instance.participantID;

    // List of data to be recorded
    private List<ForcedData> forcedData = new List<ForcedData>();
    private List<RewardData> rewardData = new List<RewardData>();
    private List<StabilityData> stabilityData = new List<StabilityData>();

    public void AddForcedData(float time, int obstacleNum, Vector3 obstaclePos, int trialWhenSpawned)
    {
        forcedData.Add(new ForcedData(time, obstacleNum, obstaclePos, trialWhenSpawned));
    }

    public void AddRewardData(float time, Vector3 avgPosition, Vector3 avgVelocity, int trialWhenCreated)
    {
        rewardData.Add(new RewardData(time, avgPosition, avgVelocity, trialWhenCreated));
    }

    public void AddStabilityData(float time, int trialWhenCreated)
    {
        stabilityData.Add(new StabilityData(time, trialWhenCreated));
    }

    class ForcedData
    {
        public readonly float time;
        public readonly int obstacleNum;
        public readonly Vector3 obstaclePos;
        public readonly int trialWhenSpawned;

        public ForcedData(float time, int obstacleNum, Vector3 obstaclePos, int trialWhenSpawned)
        {
            this.time = time;
            this.obstacleNum = obstacleNum;
            this.obstaclePos = obstaclePos;
            this.trialWhenSpawned = trialWhenSpawned;
        }
    }

    class RewardData
    {
        public readonly float time;
        public readonly Vector3 avgPosition;
        public readonly Vector3 avgVelocity;
        public readonly int trialWhenCreated;

        public RewardData(float time, Vector3 avgPosition, Vector3 avgVelocity, int trialWhenCreated)
        {
            this.time = time;
            this.avgPosition = avgPosition;
            this.avgVelocity = avgVelocity;
            this.trialWhenCreated = trialWhenCreated;
        }
    }

    class StabilityData
    {
        public readonly float time; // The time that the user achieved stability
        public readonly int trialWhenCreated; // The trial when the user achieved stability

        public StabilityData(float time, int trialWhenCreated)
        {
            this.time = time;
            this.trialWhenCreated = trialWhenCreated;
        }
    }

    private void WriteForcedFile()
    {
        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid + "Virtual");
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "Virtual/Exploration" + pid + ".csv"))
        {
            Debug.Log("Writing exploration data to file");

            // write header
            CsvRow header = new CsvRow();
            header.Add("Time");
            header.Add("Obstacle Number");
            header.Add("Obstacle X");
            header.Add("Obstacle Y");
            header.Add("Obstacle Z");
            header.Add("Trial When Spawned");
            writer.WriteRow(header);

            // write each line of data
            foreach (ForcedData d in forcedData)
            {
                CsvRow row = new CsvRow();
                row.Add(d.time.ToString());
                row.Add(d.obstacleNum.ToString());
                row.Add(d.obstaclePos.x.ToString());
                row.Add(d.obstaclePos.y.ToString());
                row.Add(d.obstaclePos.z.ToString());
                row.Add(d.trialWhenSpawned.ToString());
                writer.WriteRow(row);
            }
        }
    }

    private void WriteRewardFile()
    {
        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid + "Virtual");
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "Virtual/Exploration" + pid + ".csv"))
        {
            Debug.Log("Writing exploration data to file");

            // write header
            CsvRow header = new CsvRow();
            header.Add("Time");
            header.Add("Blocked Position X");
            header.Add("Blocked Position Y");
            header.Add("Blocked Position Z");
            header.Add("Blocked Velocity X");
            header.Add("Blocked Velocity Y");
            header.Add("Blocked Velocity Z");
            header.Add("Trial When Spawned");
            writer.WriteRow(header);

            // write each line of data
            foreach (RewardData d in rewardData)
            {
                CsvRow row = new CsvRow();
                row.Add(d.time.ToString());
                row.Add(d.avgPosition.x.ToString());
                row.Add(d.avgPosition.y.ToString());
                row.Add(d.avgPosition.z.ToString());
                row.Add(d.avgVelocity.x.ToString());
                row.Add(d.avgVelocity.y.ToString());
                row.Add(d.avgVelocity.z.ToString());
                row.Add(d.trialWhenCreated.ToString());
                writer.WriteRow(row);
            }
        }
    }

    private void WriteStabilityFile()
    {
        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid + "Virtual");
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "Virtual/Exploration" + pid + ".csv"))
        {
            Debug.Log("Writing exploration data to file");

            // write header
            CsvRow header = new CsvRow();
            header.Add("Time");
            header.Add("Trial When Stability Achieved");
            writer.WriteRow(header);

            // write each line of data
            foreach (StabilityData d in stabilityData)
            {
                CsvRow row = new CsvRow();
                row.Add(d.time.ToString());
                row.Add(d.trialWhenCreated.ToString());
                writer.WriteRow(row);
            }
        }
    }

    void OnDisable()
    {
        if (GlobalControl.Instance.explorationMode == GlobalControl.ExplorationMode.FORCED)
        {
            WriteForcedFile();
        }
        else if (GlobalControl.Instance.explorationMode == GlobalControl.ExplorationMode.REWARD_BASED)
        {
            WriteRewardFile();
        }
        else
        {
            WriteStabilityFile();
        }
    }
}
