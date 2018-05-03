using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;
using System.IO;

// A class to record the joint positions of an IK System
public class IKRecording : MonoBehaviour {

    private string pid = GlobalControl.Instance.participantID;

    // The GameObjects that mark the position on the IK body
    [SerializeField]
    private GameObject Head;
    [SerializeField]
    private GameObject Neck;
    [SerializeField]
    private GameObject SpineShoulder;
    [SerializeField]
    private GameObject SpineMid;
    [SerializeField]
    private GameObject SpineBase;

    [SerializeField]
    private GameObject RightShoulder;
    [SerializeField]
    private GameObject RightElbow;
    [SerializeField]
    private GameObject RightWrist;
    [SerializeField]
    private GameObject RightHand;
    [SerializeField]
    private GameObject RightHandTip;
    [SerializeField]
    private GameObject RightThumb;

    [SerializeField]
    private GameObject LeftShoulder;
    [SerializeField]
    private GameObject LeftElbow;
    [SerializeField]
    private GameObject LeftWrist;
    [SerializeField]
    private GameObject LeftHand;
    [SerializeField]
    private GameObject LeftHandTip;
    [SerializeField]
    private GameObject LeftThumb;

    [SerializeField]
    private GameObject RightHip;
    [SerializeField]
    private GameObject RightKnee;
    [SerializeField]
    private GameObject RightAnkle;
    [SerializeField]
    private GameObject RightFoot;

    [SerializeField]
    private GameObject LeftHip;
    [SerializeField]
    private GameObject LeftKnee;
    [SerializeField]
    private GameObject LeftAnkle;
    [SerializeField]
    private GameObject LeftFoot;

    // The list of joint data entries that is being built up once per frame
    // and will eventually be printed to CSV
    private List<JointData> data = new List<JointData>();

    void OnDisable()
    {
        WriteToFile();
    }

    // Adds a line of joint data to the growing list of joint data. This should be
    // called once per frame while a trial is active
    public void AddJointData(int curTrial)
    {
        data.Add(new JointData(Time.time, curTrial, Head, Neck, SpineShoulder,
            SpineMid, SpineBase, RightShoulder,
            LeftShoulder, RightElbow, LeftElbow,
            RightWrist, LeftWrist, RightHand,
            LeftHand, RightThumb, LeftThumb,
            RightHandTip, LeftHandTip, RightHip,
            LeftHip, RightKnee, LeftKnee,
            RightAnkle, LeftAnkle, RightFoot,
            LeftFoot));
    }

    public void WriteToFile()
    {
        Directory.CreateDirectory(@"Data/" + pid + "Virtual");
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "Virtual/IKData" + pid + ".csv"))
        {
            Debug.Log("Writing IK Recording to file");

            // write header
            CsvRow header = new CsvRow();

            header.Add("Time");
            header.Add("Current Trial");

            header.Add("Head X"); header.Add("Head Y"); header.Add("Head Z");
            header.Add("Neck X"); header.Add("Neck Y"); header.Add("Neck Z");

            header.Add("SpineShoulder X"); header.Add("SpineShoulder Y"); header.Add("SpineShoulder Z");
            header.Add("SpineMid X"); header.Add("SpineMid Y"); header.Add("SpineMid Z");
            header.Add("SpineBase X"); header.Add("SpineBase Y"); header.Add("SpineBase Z");

            header.Add("RightShoulder X"); header.Add("RightShoulder Y"); header.Add("RightShoulder Z");
            header.Add("LeftShoulder X"); header.Add("LeftShoulder Y"); header.Add("LeftShoulder Z");
            header.Add("RightElbow X"); header.Add("RightElbow Y"); header.Add("RightElbow Z");
            header.Add("LeftElbow X"); header.Add("LeftElbow Y"); header.Add("LeftElbow Z");
            header.Add("RightWrist X"); header.Add("RightWrist Y"); header.Add("RightWrist Z");
            header.Add("LeftWrist X"); header.Add("LeftWrist Y"); header.Add("LeftWrist Z");
            header.Add("RightHand X"); header.Add("RightHand Y"); header.Add("RightHand Z");
            header.Add("LeftHand X"); header.Add("LeftHand Y"); header.Add("LeftHand Z");
            header.Add("RightThumb X"); header.Add("RightThumb Y"); header.Add("RightThumb Z");
            header.Add("LeftThumb X"); header.Add("LeftThumb Y"); header.Add("LeftThumb Z");
            header.Add("RightHandTip X"); header.Add("RightHandTip Y"); header.Add("RightHandTip Z");
            header.Add("LeftHandTip X"); header.Add("LeftHandTip Y"); header.Add("LeftHandTip Z");

            header.Add("RightHip X"); header.Add("RightHip Y"); header.Add("RightHip Z");
            header.Add("LeftHip X"); header.Add("LeftHip Y"); header.Add("LeftHip Z");
            header.Add("RightKnee X"); header.Add("RightKnee Y"); header.Add("RightKnee Z");
            header.Add("LeftKnee X"); header.Add("LeftKnee Y"); header.Add("LeftKnee Z");
            header.Add("RightAnkle X"); header.Add("RightAnkle Y"); header.Add("RightAnkle Z");
            header.Add("LeftAnkle X"); header.Add("LeftAnkle Y"); header.Add("LeftAnkle Z");
            header.Add("RightFoot X"); header.Add("RightFoot Y"); header.Add("RightFoot Z");
            header.Add("LeftFoot X"); header.Add("LeftFoot Y"); header.Add("LeftFoot Z");

            writer.WriteRow(header);

            // write each line of data
            foreach (JointData d in data)
            {
                CsvRow row = new CsvRow();

                row.Add(d.time.ToString());
                row.Add(d.curTrial.ToString());

                row.Add(d.Head.x.ToString()); row.Add(d.Head.y.ToString()); row.Add(d.Head.z.ToString());
                row.Add(d.Neck.x.ToString()); row.Add(d.Neck.y.ToString()); row.Add(d.Neck.z.ToString());

                row.Add(d.SpineShoulder.x.ToString()); row.Add(d.SpineShoulder.y.ToString()); row.Add(d.SpineShoulder.z.ToString());
                row.Add(d.SpineMid.x.ToString()); row.Add(d.SpineMid.y.ToString()); row.Add(d.SpineMid.z.ToString());
                row.Add(d.SpineBase.x.ToString()); row.Add(d.SpineBase.y.ToString()); row.Add(d.SpineBase.z.ToString());

                row.Add(d.RightShoulder.x.ToString()); row.Add(d.RightShoulder.y.ToString()); row.Add(d.RightShoulder.z.ToString());
                row.Add(d.LeftShoulder.x.ToString()); row.Add(d.LeftShoulder.y.ToString()); row.Add(d.LeftShoulder.z.ToString());
                row.Add(d.RightElbow.x.ToString()); row.Add(d.RightElbow.y.ToString()); row.Add(d.RightElbow.z.ToString());
                row.Add(d.LeftElbow.x.ToString()); row.Add(d.LeftElbow.y.ToString()); row.Add(d.LeftElbow.z.ToString());
                row.Add(d.RightWrist.x.ToString()); row.Add(d.RightWrist.y.ToString()); row.Add(d.RightWrist.z.ToString());
                row.Add(d.LeftWrist.x.ToString()); row.Add(d.LeftWrist.y.ToString()); row.Add(d.LeftWrist.z.ToString());
                row.Add(d.RightHand.x.ToString()); row.Add(d.RightHand.y.ToString()); row.Add(d.RightHand.z.ToString());
                row.Add(d.LeftHand.x.ToString()); row.Add(d.LeftHand.y.ToString()); row.Add(d.LeftHand.z.ToString());
                row.Add(d.RightThumb.x.ToString()); row.Add(d.RightThumb.y.ToString()); row.Add(d.RightThumb.z.ToString());
                row.Add(d.LeftThumb.x.ToString()); row.Add(d.LeftThumb.y.ToString()); row.Add(d.LeftThumb.z.ToString());
                row.Add(d.RightHandTip.x.ToString()); row.Add(d.RightHandTip.y.ToString()); row.Add(d.RightHandTip.z.ToString());
                row.Add(d.LeftHandTip.x.ToString()); row.Add(d.LeftHandTip.y.ToString()); row.Add(d.LeftHandTip.z.ToString());

                row.Add(d.RightHip.x.ToString()); row.Add(d.RightHip.y.ToString()); row.Add(d.RightHip.z.ToString());
                row.Add(d.LeftHip.x.ToString()); row.Add(d.LeftHip.y.ToString()); row.Add(d.LeftHip.z.ToString());
                row.Add(d.RightKnee.x.ToString()); row.Add(d.RightKnee.y.ToString()); row.Add(d.RightKnee.z.ToString());
                row.Add(d.LeftKnee.x.ToString()); row.Add(d.LeftKnee.y.ToString()); row.Add(d.LeftKnee.z.ToString());
                row.Add(d.RightAnkle.x.ToString()); row.Add(d.RightAnkle.y.ToString()); row.Add(d.RightAnkle.z.ToString());
                row.Add(d.LeftAnkle.x.ToString()); row.Add(d.LeftAnkle.y.ToString()); row.Add(d.LeftAnkle.z.ToString());
                row.Add(d.RightFoot.x.ToString()); row.Add(d.RightFoot.y.ToString()); row.Add(d.RightFoot.z.ToString());
                row.Add(d.LeftFoot.x.ToString()); row.Add(d.LeftFoot.y.ToString()); row.Add(d.LeftFoot.z.ToString());

                writer.WriteRow(row);
            }
        }
    }

    // A class that stores joint data to be eventually written to CSV file
    class JointData
    {
        public readonly float time;
        public readonly int curTrial;

        public readonly Vector3 Head;
        public readonly Vector3 Neck;

        public readonly Vector3 SpineShoulder;
        public readonly Vector3 SpineMid;
        public readonly Vector3 SpineBase;

        public readonly Vector3 RightShoulder;
        public readonly Vector3 LeftShoulder;
        public readonly Vector3 RightElbow;
        public readonly Vector3 LeftElbow;
        public readonly Vector3 RightWrist;
        public readonly Vector3 LeftWrist;
        public readonly Vector3 RightHand;
        public readonly Vector3 LeftHand;
        public readonly Vector3 RightThumb;
        public readonly Vector3 LeftThumb;
        public readonly Vector3 RightHandTip;
        public readonly Vector3 LeftHandTip;

        public readonly Vector3 RightHip;
        public readonly Vector3 LeftHip;
        public readonly Vector3 RightKnee;
        public readonly Vector3 LeftKnee;
        public readonly Vector3 RightAnkle;
        public readonly Vector3 LeftAnkle;
        public readonly Vector3 RightFoot;
        public readonly Vector3 LeftFoot;

        public readonly bool marking;

        public JointData(float time, int curTrial, GameObject Head, GameObject Neck, GameObject SpineShoulder,
            GameObject SpineMid, GameObject SpineBase, GameObject RightShoulder,
            GameObject LeftShoulder, GameObject RightElbow, GameObject LeftElbow,
            GameObject RightWrist, GameObject LeftWrist, GameObject RightHand,
            GameObject LeftHand, GameObject RightThumb, GameObject LeftThumb,
            GameObject RightHandTip, GameObject LeftHandTip, GameObject RightHip,
            GameObject LeftHip, GameObject RightKnee, GameObject LeftKnee,
            GameObject RightAnkle, GameObject LeftAnkle, GameObject RightFoot,
            GameObject LeftFoot)
        {
            this.time = time;
            this.curTrial = curTrial;

            this.Head = Head.transform.position;
            this.Neck = Neck.transform.position;
            this.SpineShoulder = SpineShoulder.transform.position;
            this.SpineMid = SpineMid.transform.position;
            this.SpineBase = SpineBase.transform.position;

            this.RightShoulder = RightShoulder.transform.position;
            this.LeftShoulder = LeftShoulder.transform.position;
            this.RightElbow = RightElbow.transform.position;
            this.LeftElbow = LeftElbow.transform.position;
            this.RightWrist = RightWrist.transform.position;
            this.LeftWrist = LeftWrist.transform.position;
            this.RightHand = RightHand.transform.position;
            this.LeftHand = LeftHand.transform.position;
            this.RightThumb = RightThumb.transform.position;
            this.LeftThumb = LeftThumb.transform.position;
            this.RightHandTip = RightHandTip.transform.position;
            this.LeftHandTip = LeftHandTip.transform.position;

            this.RightHip = RightHip.transform.position;
            this.LeftHip = LeftHip.transform.position;
            this.RightKnee = RightKnee.transform.position;
            this.LeftKnee = LeftKnee.transform.position;
            this.RightAnkle = RightAnkle.transform.position;
            this.LeftAnkle = LeftAnkle.transform.position;
            this.RightFoot = RightFoot.transform.position;
            this.LeftFoot = LeftFoot.transform.position;

        }
    }
}
