using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;

/// <summary>
/// Writes a line of data after every trial, giving information on the trial.
/// </summary>
public class DataHandler : MonoBehaviour {

    // stores the data for writing to file at end of task
    List<Data> data = new List<Data>();

	/// <summary>
    /// Subscribe to data writing events.
    /// </summary>
	void Awake () {
        SkittlesGame.OnRecordData += recordTrial;
	}

    /// <summary>
    /// Write all data to a file and unsubscribe from data writing event.
    /// </summary>
    void OnDisable()
    {
        // Finally, write the file that displays continuous CoP/CoM
        WriteTrialFile();
    }

    // Records trial data
    private void recordTrial(Vector3 ballPosition)
    {
        data.Add(new Data(ballPosition));
    }

    /// <summary>
    /// A class that stores info on each trial relevant to data recording. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class Data
    {
        public readonly Vector3 ballPosition;

        public Data(Vector3 ballPosition)
        {
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
            Debug.Log("Writing trial data to file");
            
            // write header
            CsvRow header = new CsvRow();
            header.Add("Ball X");
            header.Add("Ball Y");
            header.Add("Ball Z");
            writer.WriteRow(header);

            // write each line of data
            foreach (Data d in data)
            {
                CsvRow row = new CsvRow();

                row.Add(d.ballPosition.x.ToString());
                row.Add(d.ballPosition.y.ToString());
                row.Add(d.ballPosition.z.ToString());

                writer.WriteRow(row);
            }
        }

        SkittlesGame.OnRecordData -= recordTrial;

    }

    
}
