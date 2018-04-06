using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;

/// <summary>
/// Writes a line of data after every trial, giving information on the trial.
/// </summary>
public class DataHandler : MonoBehaviour {

    // stores the data for writing to file at end of task
    List<Data> continuousData = new List<Data>();

	/// <summary>
    /// Subscribe to data writing events.
    /// </summary>
	void Awake () {
        SkittlesGame.OnRecordContinuousData += recordContinuousTrial;
	}

    /// <summary>
    /// Write all data to a file and unsubscribe from data writing event.
    /// </summary>
    void OnDisable()
    {
        WriteTrialFile();
    }

    // Records continuous data into the data list
    private void recordContinuousTrial(float time, Vector3 ballPosition)
    {
        continuousData.Add(new Data(time, ballPosition));
    }

    /// <summary>
    /// A class that stores info on each trial relevant to data recording. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class Data
    {
        public readonly float time;
        public readonly Vector3 ballPosition;

        public Data(float time, Vector3 ballPosition)
        {
            this.time = time;
            this.ballPosition = ballPosition;
        }
    }

 
    /// <summary>
    /// Writes the Trial File to a CSV
    /// </summary>
    private void WriteTrialFile()
    {
        // Write all entries in data list to file
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/TrialData" + "ParticipantID" + ".csv"))
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
            foreach (Data d in continuousData)
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

    }

    
}
