using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class RTSkeletonFromMarker : MonoBehaviour {

    public bool isTest = false;
    private bool isInit = false;
    public string SkeletonName = "SR";
    public Transform test1 = null;
    public Transform test2 = null;
    public Transform test3 = null;
    [SerializeField]
    public int id = 0;

    private Avatar mSourceAvatar;
    public Avatar DestinationAvatar;

    private HumanPose mHumanPose = new HumanPose();
    private GameObject mStreamedRootObject;
    private Dictionary<uint, GameObject> mQTmSegmentIdToGameObject;
    private Dictionary<string, string> mMecanimToQtmSegmentNames = new Dictionary<string, string>();

    private HumanPoseHandler mSourcePoseHandler;
    private HumanPoseHandler mDestiationPoseHandler;

    protected RTClient rtClient;
    private mSkeleton mQtmSkeletonCache;
    private mSkeleton skeleton = null;
    private mSkeleton tSkeleton = null;

    private int updateClick = 0;

    private bool isRecord = false;
    //temp for save record
    private qtmRecord Rec = new qtmRecord();

    //marker
    private List<LabeledMarker> markerData = null;
    private Dictionary<string, Vector3> dotData = new Dictionary<string, Vector3>();

    public void readCalibrate() {
        string filename = "MarkerCalibrate.txt";
        tSkeleton = new mSkeleton();
        tSkeleton.Name = SkeletonName;
        tSkeleton._seg = Utils.load<qtmRecord>(filename).records[0];
        Debug.Log("PLAYING");
    }

    public void calibrateFromPlaying() {
        Debug.Log("calibrate playing");
        tSkeleton = new mSkeleton();
        tSkeleton.Name = skeleton.Name;
        tSkeleton._seg = skeleton._seg;
    }

    public void saveCalibrate() {
        isRecord = true;
    }

    void Update() {
        updateClick++;
        updateClick %= 50;
        if (updateClick != 0) {
        //return;
        }
        //get marker data
        if (rtClient == null)
            rtClient = RTClient.GetInstance();
        markerData = rtClient.Markers;
        if (markerData == null || markerData.Count == 0)
            return;
        //update marker data to local dictionary
        for (int i = 0; i < markerData.Count; i++) {
            if (!float.IsNaN(markerData[i].Position.sqrMagnitude)) {
                if (dotData.ContainsKey(markerData[i].Name)) {
                    dotData[markerData[i].Name] = markerData[i].Position;
                } else {
                    Debug.Log("add" + markerData[i].Name);
                    dotData.Add(markerData[i].Name, markerData[i].Position);
                }
                //Debug.Log("Update marker" + markerData[i].Name);
            }
        }
        //update skeleton from marker data
        if (skeleton == null) {
            skeleton = new mSkeleton();
            skeleton.Name = "SR";
            skeleton._seg = Utils.load<qtmRecord>("Ske.txt").records[0];
            Debug.Log("Load sk");
        }
        setSkeletonPosition(skeleton);
        setSkeletonRotation(skeleton);
        //apply skeleton to model
        updateModel();
        //record
        if (isRecord) {
            Debug.Log("####RECORDING####");
            Rec.records.Add(skeleton._seg);
            isRecord = false;
            Rec.length = 1;
            //use multithread to avoid extreme lag
            Debug.Log("Save start");
            Utils.Save(Rec, "MarkerCalibrate.txt");
            Debug.Log("Save complete");
            readCalibrate();
        }
    }

    private void setSkeletonPosition(mSkeleton sk) {
        foreach (var segment in sk._seg) {
            List<Vector3> toAvg = new List<Vector3>();
            switch (segment.Value.Name) {
                case "Hips":
                    if (getDotData("LPSI") != Vector3.zero)
                        toAvg.Add(getDotData("LPSI"));
                    if (getDotData("RPSI") != Vector3.zero)
                        toAvg.Add(getDotData("RPSI"));
                    break;
                case "Spine":
                    if (getDotData("WaistLBack") != Vector3.zero)
                        toAvg.Add(getDotData("WaistLBack"));
                    if (getDotData("WaistRBack") != Vector3.zero)
                        toAvg.Add(getDotData("WaistRBack"));
                    break;
                case "Spine1":
                    if (getDotData("BackL") != Vector3.zero)
                        toAvg.Add(getDotData("BackL"));
                    if (getDotData("BackR") != Vector3.zero)
                        toAvg.Add(getDotData("BackR"));
                    break;
                case "Spine2":
                    if (getDotData("BackL") != Vector3.zero)
                        toAvg.Add(getDotData("BackL"));
                    if (getDotData("BackR") != Vector3.zero)
                        toAvg.Add(getDotData("BackR"));
                    //if (getDotData("XP") != Vector3.zero)
                    //    toAvg.Add(getDotData("XP"));
                    break;
                case "Neck":
                    if (getDotData("C7") != Vector3.zero)
                        toAvg.Add(getDotData("C7"));
                    break;
                case "Head":
                    if (getDotData("C7") != Vector3.zero)
                        toAvg.Add(getDotData("C7"));
                    if (getDotData("HeadL") != Vector3.zero)
                        toAvg.Add(getDotData("HeadL"));
                    if (getDotData("HeadR") != Vector3.zero)
                        toAvg.Add(getDotData("HeadR"));
                    break;
                //////down side
                case "RightUpLeg":
                    if (getDotData("RTRO") != Vector3.zero)
                        toAvg.Add(getDotData("RTRO"));
                    if (getDotData("RPSI") != Vector3.zero)
                        toAvg.Add(getDotData("RPSI"));
                    break;
                case "RightLeg":
                    if (getDotData("RKneeIn") != Vector3.zero)
                        toAvg.Add(getDotData("RKneeIn"));
                    if (getDotData("RKneeOut") != Vector3.zero)
                        toAvg.Add(getDotData("RKneeOut"));
                    break;
                case "RightFoot":
                    if (getDotData("RHeelBack") != Vector3.zero)
                        toAvg.Add(getDotData("RHeelBack"));
                    break;
                case "LeftUpLeg":
                    if (getDotData("LTRO") != Vector3.zero)
                        toAvg.Add(getDotData("LTRO"));
                    if (getDotData("LPSI") != Vector3.zero)
                        toAvg.Add(getDotData("LPSI"));
                    break;
                case "LeftLeg":
                    if (getDotData("LKneeIn") != Vector3.zero)
                        toAvg.Add(getDotData("LKneeIn"));
                    if (getDotData("LKneeOut") != Vector3.zero)
                        toAvg.Add(getDotData("LKneeOut"));
                    break;
                case "LeftFoot":
                    if (getDotData("LHeelBack") != Vector3.zero)
                        toAvg.Add(getDotData("LHeelBack"));
                    break;

                ////upside
                case "RightArm":
                    if (getDotData("RShoulderBack") != Vector3.zero)
                        toAvg.Add(getDotData("RShoulderBack"));
                    if (getDotData("RShoulderTop") != Vector3.zero)
                        toAvg.Add(getDotData("RShoulderTop"));
                    break;
                case "RightForeArm":
                    if (getDotData("RElbowOut") != Vector3.zero)
                        toAvg.Add(getDotData("RElbowOut"));
                    break;
                case "RightHand":
                    if (getDotData("RWristOut") != Vector3.zero)
                        segment.Value.Position = getDotData("RWristOut");
                    if (getDotData("RWristIn") != Vector3.zero)
                        segment.Value.Position = getDotData("RWristIn");
                    break;
                case "LeftArm":
                    if (getDotData("LShoulderTop") != Vector3.zero)
                        toAvg.Add(getDotData("LShoulderTop"));
                    break;
                case "LeftForeArm":
                    if (getDotData("LElbowOut") != Vector3.zero)
                        toAvg.Add(getDotData("LElbowOut"));
                    break;
                case "LeftHand":
                    if (getDotData("LWristOut") != Vector3.zero)
                        segment.Value.Position = getDotData("LWristOut");
                    if (getDotData("LWristIn") != Vector3.zero)
                        segment.Value.Position = getDotData("LWristIn");
                    break;

                default:
                    break;
            }
            if (Utils.GetMeanVector(toAvg) != Vector3.zero) {
                segment.Value.Position = Utils.GetMeanVector(toAvg);
            }

        }
    }

    private Vector3 getDotData(string name) {
        if (dotData.ContainsKey(SkeletonName + "_" + name)) {
            //Debug.Log("get" + SkeletonName + "_" + name);
            return dotData[SkeletonName + "_" + name];
        }
        Debug.Log("[marker]can't find " + SkeletonName + "_" + name);
        return Vector3.zero;
    }

    private void setSkeletonRotation(mSkeleton sk) {
        for (int i = 0; i < sk._seg.Count; i++) {
            var segment = sk._seg[i];
            Quaternion calibrateRot = Quaternion.identity;
            Quaternion quat = Quaternion.identity;

            switch (segment.Value.Name) {
                case "Spine":
                    if (getDotData("WaistLBack") != Vector3.zero && getDotData("WaistRBack") != Vector3.zero && getDotData("BackL") != Vector3.zero && getDotData("BackR") != Vector3.zero) {
                        Vector3 v = getDotData("WaistLBack") - getDotData("WaistRBack");
                        Vector3 v2 = getDotData("BackL") - getDotData("BackR");
                        quat = Quaternion.LookRotation(v, v2 - segment.Value.Position);
                    }
                    break;
                case "Hips":
                    if (getDotData("LPSI") != Vector3.zero && getDotData("RPSI") != Vector3.zero && getDotData("L4L5") != Vector3.zero) {
                        Vector3 v = getDotData("LPSI") - getDotData("RPSI");
                        quat = Quaternion.LookRotation(v, getDotData("L4L5") - segment.Value.Position);
                    }
                    break;
                case "LeftShoulder":
                case "RightShoulder":
                    if (getDotData("BackL") != Vector3.zero && getDotData("BackR") != Vector3.zero && getDotData("C7") != Vector3.zero) {
                        Vector3 v = getDotData("BackL") - getDotData("BackR");
                        quat = Quaternion.LookRotation(v, getDotData("C7") - (getDotData("BackL") + getDotData("BackR")/2));
                    }
                    break;
                case "Spine1":
                case "Spine2":
                    if (getDotData("BackL") != Vector3.zero && getDotData("BackR") != Vector3.zero && getDotData("C7") != Vector3.zero) {
                        Vector3 v = getDotData("BackL") - getDotData("BackR");
                        quat = Quaternion.LookRotation(v, getDotData("C7") - segment.Value.Position);
                    }
                    break;
                case "Neck":
                    if (getDotData("HeadL") != Vector3.zero && getDotData("HeadR") != Vector3.zero && getDotData("C7") != Vector3.zero ) {
                        Vector3 v = getDotData("HeadL")- getDotData("HeadR") ;
                        quat = Quaternion.LookRotation(v, (getDotData("HeadR") + getDotData("HeadL")) / 2 - getDotData("C7"));
                    }
                    break;
                case "Head":
                    if (getDotData("HeadL") != Vector3.zero && getDotData("HeadR") != Vector3.zero && getDotData("HeadBack") != Vector3.zero ) {
                        Vector3 v = getDotData("HeadL") - getDotData("HeadR");
                        quat = Quaternion.LookRotation(v, (getDotData("HeadR") + getDotData("HeadL")) / 2 - getDotData("HeadBack"));
                    }
                    break;

                //// down part
                case "RightUpLeg":
                    if (getDotData("RKneeIn") != Vector3.zero && getDotData("RKneeOut") != Vector3.zero) {
                        Vector3 v = getDotData("RKneeIn") - getDotData("RKneeOut");
                        quat = Quaternion.LookRotation(v, getDotData("RKneeOut") - segment.Value.Position);
                    }
                    break;
                case "RightLeg":
                    if (getDotData("RKneeIn") != Vector3.zero && getDotData("RKneeOut") != Vector3.zero) {
                        Vector3 v = getDotData("RKneeIn") - getDotData("RKneeOut");
                        quat = Quaternion.LookRotation(v, getDotData("RHeelBack") - segment.Value.Position);
                    }
                    break;
                case "RightFoot":
                    if (getDotData("RAnkleIn") != Vector3.zero && getDotData("RAnkleOut") != Vector3.zero) {
                        Vector3 v = getDotData("RAnkleIn") - getDotData("RAnkleOut");
                        quat = Quaternion.LookRotation(v, (getDotData("RAnkleIn") + getDotData("RAnkleOut")) / 2 - segment.Value.Position);
                    }
                    break;
                case "LeftUpLeg":
                    if (getDotData("LKneeIn") != Vector3.zero && getDotData("LKneeOut") != Vector3.zero) {
                        Vector3 v = getDotData("LKneeIn") - getDotData("LKneeOut");
                        quat = Quaternion.LookRotation(v,getDotData("LKneeOut") - segment.Value.Position);
                    }
                    break;
                case "LeftLeg":
                    if (getDotData("LKneeIn") != Vector3.zero && getDotData("LKneeOut") != Vector3.zero) {
                        Vector3 v = getDotData("LKneeIn") - getDotData("LKneeOut");
                        quat = Quaternion.LookRotation(v, getDotData("LHeelBack") - segment.Value.Position);
                    }
                    break;
                case "LeftFoot":
                    if (getDotData("LAnkleIn") != Vector3.zero && getDotData("LAnkleOut") != Vector3.zero) {
                        Vector3 v = getDotData("LAnkleIn") - getDotData("LAnkleOut");
                        quat = Quaternion.LookRotation(v, (getDotData("LAnkleIn") + getDotData("LAnkleOut")) / 2 - segment.Value.Position);
                    }
                    break;

                ///////up part
                case "RightArm":
                    if (getDotData("RArm") != Vector3.zero && getDotData("RShoulderTop") != Vector3.zero && getDotData("RElbowOut") != Vector3.zero) {
                        Vector3 v1 = getDotData("RArm") - getDotData("RShoulderTop");
                        Vector3 v2 = getDotData("RElbowOut") - getDotData("RShoulderTop");
                        Vector3 v = Vector3.Cross(v1, v2);
                        quat = Quaternion.LookRotation(v, getDotData("RElbowOut") - segment.Value.Position);
                    }
                    break;
                case "RightForeArm":
                    if (getDotData("RWristOut") != Vector3.zero && getDotData("RWristIn") != Vector3.zero && getDotData("RElbowOut") != Vector3.zero) {
                        Vector3 v = getDotData("RWristIn") - getDotData("RWristOut");
                        quat = Quaternion.LookRotation(v,(getDotData("RWristIn") + getDotData("RWristOut"))/2 - segment.Value.Position);
                    }
                    break;
                case "LeftArm":
                    if (getDotData("LArm") != Vector3.zero && getDotData("LShoulderTop") != Vector3.zero && getDotData("LElbowOut") != Vector3.zero) {
                        Vector3 v1 = getDotData("LArm") - getDotData("LShoulderTop");
                        Vector3 v2 = getDotData("LElbowOut") - getDotData("LShoulderTop");
                        Vector3 v = Vector3.Cross(v1, v2);
                        quat = Quaternion.LookRotation(v, getDotData("LElbowOut") - segment.Value.Position);
                    }
                    break;
                case "LeftForeArm":
                    if (getDotData("LWristOut") != Vector3.zero && getDotData("LWristIn") != Vector3.zero && getDotData("LElbowOut") != Vector3.zero) {
                        Vector3 v = getDotData("LWristIn") - getDotData("LWristOut");
                        quat = Quaternion.LookRotation(v, (getDotData("LWristIn") + getDotData("LWristOut")) / 2 - segment.Value.Position);
                    }
                    break;
                /////cannot get hand rot
                case "RightHand":
                    if (getDotData("RWristOut") != Vector3.zero && getDotData("RWristIn") != Vector3.zero && getDotData("RHandOut") != Vector3.zero) {
                        Vector3 v = getDotData("RWristIn") - getDotData("RWristOut");
                        quat = Quaternion.LookRotation(v, getDotData("RHandOut") - segment.Value.Position);
                    }
                    break;
                case "LeftHand":
                    if (getDotData("LWristOut") != Vector3.zero && getDotData("LWristIn") != Vector3.zero && getDotData("LHandOut") != Vector3.zero) {
                        Vector3 v = getDotData("LWristIn") - getDotData("LWristOut");
                        quat = Quaternion.LookRotation(v, getDotData("LHandOut") - segment.Value.Position);
                    }
                    break;

                default:
                    break;
            }
            if (tSkeleton != null) {
                calibrateRot = tSkeleton._seg[i].Value.Rotation;
                segment.Value.Rotation = quat * Quaternion.Inverse(calibrateRot);
                //Debug.Log(tSkeleton._seg[i].Value.Name + " TPOSE :" + tSkeleton._seg[i].Value.Rotation.x + "" + tSkeleton._seg[i].Value.Rotation.y + "" + tSkeleton._seg[i].Value.Rotation.z);
            } else {
                segment.Value.Rotation = Quaternion.LookRotation(Vector3.right)*quat;

            }
        }
    }

    private Quaternion dynQuat() {
        switch (id) {
            case 0:
                return Quaternion.LookRotation(Vector3.right, Vector3.up);
            case 1:
                return Quaternion.LookRotation(Vector3.left, Vector3.up);
            case 2:
                return Quaternion.LookRotation(Vector3.back, Vector3.up);
            case 3:
                return Quaternion.LookRotation(Vector3.right, Vector3.down);
            case 4:
                return Quaternion.LookRotation(Vector3.left, Vector3.down);
            case 5:
                return Quaternion.LookRotation(Vector3.forward, Vector3.down);
            case 6:
                return Quaternion.LookRotation(Vector3.back, Vector3.down);
            case 7:
                return Quaternion.LookRotation(Vector3.up, Vector3.forward);
            case 8:
                return Quaternion.LookRotation(Vector3.down, Vector3.forward);
            case 9:
                return Quaternion.LookRotation(Vector3.left, Vector3.forward);
            case 10:
                return Quaternion.LookRotation(Vector3.right, Vector3.forward);
            case 11:
                return Quaternion.LookRotation(Vector3.up, Vector3.back);
            case 12:
                return Quaternion.LookRotation(Vector3.down, Vector3.back);
            case 13:
                return Quaternion.LookRotation(Vector3.left, Vector3.back);
            case 14:
                return Quaternion.LookRotation(Vector3.right, Vector3.back);
            case 15:
                return Quaternion.LookRotation(Vector3.up, Vector3.left);
            case 16:
                return Quaternion.LookRotation(Vector3.down, Vector3.left);
            case 17:
                return Quaternion.LookRotation(Vector3.back, Vector3.left);
            case 18:
                return Quaternion.LookRotation(Vector3.forward, Vector3.left);
            case 19:
                return Quaternion.LookRotation(Vector3.up, Vector3.right);
            case 20:
                return Quaternion.LookRotation(Vector3.down, Vector3.right);
            case 21:
                return Quaternion.LookRotation(Vector3.back, Vector3.right);
            case 22:
                return Quaternion.LookRotation(Vector3.forward, Vector3.right);
            default:
                return Quaternion.identity;

        }
    }

    private void genSeg(mSkeleton sk) {
        Segment seg;

        seg = new Segment("Hips", 1, 0, new Vector3(0, 0.9673969f, 0));
        sk._seg.Add(new KeyValuePair<uint, Segment>(1, seg));

        seg = new Segment("Spine", 2, 1, new Vector3(0, 0.134412169f, 0.000665835047f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(2, seg));

        seg = new Segment("Spine1", 3, 2, new Vector3(0, 0.118435688f, 0.0112132384f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(3, seg));

        seg = new Segment("Spine2", 4, 3, new Vector3(0, 0.1539664f, 0.0168198589f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(4, seg));

        seg = new Segment("LeftShoulder", 5, 4, new Vector3(-0.03163761f, 0.124090888f, 0.01654551f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(5, seg));

        seg = new Segment("LeftArm", 6, 5, new Vector3(-0.163892642f, 0.004388696f, 0));
        sk._seg.Add(new KeyValuePair<uint, Segment>(6, seg));

        seg = new Segment("LeftForeArm", 7, 6, new Vector3(-0.267741978f, 0, 0));
        sk._seg.Add(new KeyValuePair<uint, Segment>(7, seg));

        seg = new Segment("LeftHand", 8, 7, new Vector3(-0.248353586f, 0, 0));
        sk._seg.Add(new KeyValuePair<uint, Segment>(8, seg));

        seg = new Segment("RightShoulder", 9, 4, new Vector3(0.030696474f, 0.124090888f, 0.01654551f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(9, seg));

        seg = new Segment("RightArm", 10, 9, new Vector3(0.163892642f, 0.004388696f, 0));
        sk._seg.Add(new KeyValuePair<uint, Segment>(10, seg));

        seg = new Segment("RightForeArm", 11, 10, new Vector3(0.267741978f, 0, 0));
        sk._seg.Add(new KeyValuePair<uint, Segment>(11, seg));

        seg = new Segment("RightHand", 12, 11, new Vector3(0.248353586f, 0, 0));
        sk._seg.Add(new KeyValuePair<uint, Segment>(12, seg));

        seg = new Segment("Neck", 13, 4, new Vector3(0, 0.1539664f, 0.0168198589f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(13, seg));

        seg = new Segment("Head", 14, 13, new Vector3(0.005832924f, 0.105543181f, 0.01740256f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(14, seg));

        seg = new Segment("LeftUpLeg", 15, 1, new Vector3(-0.100470617f, -0.0259915553f, 0.05359349f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(15, seg));

        seg = new Segment("LeftLeg", 16, 15, new Vector3(0, -0.387621045f, -0.00338271679f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(16, seg));

        seg = new Segment("LeftFoot", 17, 16, new Vector3(0, -0.446042448f, -0.0233760923f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(17, seg));

        seg = new Segment("LeftToeBase", 18, 17, new Vector3(0, -0.06377827f, 0.118218124f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(18, seg));

        seg = new Segment("RightUpLeg", 19, 1, new Vector3(0.100470617f, -0.0259915553f, 0.05359349f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(19, seg));

        seg = new Segment("RightLeg", 20, 19, new Vector3(0, -0.387621045f, -0.00338271679f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(20, seg));

        seg = new Segment("RightFoot", 21, 20, new Vector3(0, -0.446042448f, -0.0233760923f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(21, seg));

        seg = new Segment("RightToeBase", 22, 21, new Vector3(0, -0.06377827f, 0.118218124f));
        sk._seg.Add(new KeyValuePair<uint, Segment>(22, seg));

    }

    private void updateModel() {
        //update local skeleton data
        if (mQtmSkeletonCache != skeleton) {
            //Debug.Log("Flush skeleton cache");
            mQtmSkeletonCache = skeleton;
            if (mQtmSkeletonCache == null)
                return;
            checkInit();
        }

        if (mQtmSkeletonCache == null)
            return;

        // Update all the game objects
        foreach (var segment in mQtmSkeletonCache.Segments.ToList()) {
            GameObject gameObject;
            if (mQTmSegmentIdToGameObject.TryGetValue(segment.Key, out gameObject)) {
                gameObject.transform.position = segment.Value.Position;
                gameObject.transform.rotation = segment.Value.Rotation;
            }
        }
        if (mSourcePoseHandler != null && mDestiationPoseHandler != null) {
            mSourcePoseHandler.GetHumanPose(ref mHumanPose);
            mDestiationPoseHandler.SetHumanPose(ref mHumanPose);
        } else {
            Debug.Log("mSourcePoseHandler null");
        }
    }

    private void checkInit() {
        if (!isInit) {
            CreateMecanimToQtmSegmentNames(SkeletonName);
            if (mStreamedRootObject != null) {
                Destroy(mStreamedRootObject);
            }
            mStreamedRootObject = new GameObject(SkeletonName);
            mQTmSegmentIdToGameObject = new Dictionary<uint, GameObject>(mQtmSkeletonCache.Segments.Count);
            foreach (var segment in mQtmSkeletonCache.Segments.ToList()) {
                var gameObject = new GameObject(SkeletonName + "_" + segment.Value.Name);
                gameObject.transform.parent = segment.Value.ParentId == 0 ? mStreamedRootObject.transform : mQTmSegmentIdToGameObject[segment.Value.ParentId].transform;
                gameObject.transform.localPosition = segment.Value.TPosition;
                mQTmSegmentIdToGameObject[segment.Value.Id] = gameObject;
            }
            BuildMecanimAvatarFromQtmTPose();
            mStreamedRootObject.transform.SetParent(transform, false);
            mStreamedRootObject.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
            isInit = true;
        }
    }

    private void BuildMecanimAvatarFromQtmTPose() {
        var humanBones = new List<HumanBone>(mQtmSkeletonCache.Segments.Count);
        for (int index = 0; index < HumanTrait.BoneName.Length; index++) {
            var humanBoneName = HumanTrait.BoneName[index];
            if (mMecanimToQtmSegmentNames.ContainsKey(humanBoneName)) {
                var bone = new HumanBone() {
                    humanName = humanBoneName,
                    boneName = mMecanimToQtmSegmentNames[humanBoneName],
                };
                bone.limit.useDefaultValues = true;
                humanBones.Add(bone);
            }
        }

        // Set up the T-pose and game object name mappings.
        var skeletonBones = new List<SkeletonBone>(mQtmSkeletonCache.Segments.Count + 1);
        skeletonBones.Add(new SkeletonBone() {
            name = this.SkeletonName,
            position = Vector3.zero,
            rotation = Quaternion.identity,
            scale = Vector3.one,
        });

        // Create remaining T-Pose bone definitions from Qtm segments
        foreach (var segment in mQtmSkeletonCache.Segments.ToList()) {
            skeletonBones.Add(new SkeletonBone() {
                name = this.SkeletonName + "_" + segment.Value.Name,
                position = segment.Value.TPosition,
                rotation = Quaternion.identity,//segment.Value.TRotation,
                scale = Vector3.one,
            });
        }

        mSourceAvatar = AvatarBuilder.BuildHumanAvatar(mStreamedRootObject,
            new HumanDescription() {
                human = humanBones.ToArray(),
                skeleton = skeletonBones.ToArray(),
            }
        );
        if (mSourceAvatar.isValid == false || mSourceAvatar.isHuman == false) {
            this.enabled = false;
            return;
        }

        mSourcePoseHandler = new HumanPoseHandler(mSourceAvatar, mStreamedRootObject.transform);
        mDestiationPoseHandler = new HumanPoseHandler(DestinationAvatar, this.transform);
    }

    private void CreateMecanimToQtmSegmentNames(string skeletonName) {
        mMecanimToQtmSegmentNames.Clear();
        mMecanimToQtmSegmentNames.Add("RightShoulder", skeletonName + "_RightShoulder");
        mMecanimToQtmSegmentNames.Add("RightUpperArm", skeletonName + "_RightArm");
        mMecanimToQtmSegmentNames.Add("RightLowerArm", skeletonName + "_RightForeArm");
        mMecanimToQtmSegmentNames.Add("RightHand", skeletonName + "_RightHand");
        mMecanimToQtmSegmentNames.Add("LeftShoulder", skeletonName + "_LeftShoulder");
        mMecanimToQtmSegmentNames.Add("LeftUpperArm", skeletonName + "_LeftArm");
        mMecanimToQtmSegmentNames.Add("LeftLowerArm", skeletonName + "_LeftForeArm");
        mMecanimToQtmSegmentNames.Add("LeftHand", skeletonName + "_LeftHand");

        mMecanimToQtmSegmentNames.Add("RightUpperLeg", skeletonName + "_RightUpLeg");
        mMecanimToQtmSegmentNames.Add("RightLowerLeg", skeletonName + "_RightLeg");
        mMecanimToQtmSegmentNames.Add("RightFoot", skeletonName + "_RightFoot");
        mMecanimToQtmSegmentNames.Add("RightToeBase", skeletonName + "_RightToeBase");
        mMecanimToQtmSegmentNames.Add("LeftUpperLeg", skeletonName + "_LeftUpLeg");
        mMecanimToQtmSegmentNames.Add("LeftLowerLeg", skeletonName + "_LeftLeg");
        mMecanimToQtmSegmentNames.Add("LeftFoot", skeletonName + "_LeftFoot");
        mMecanimToQtmSegmentNames.Add("LeftToeBase", skeletonName + "_LeftToeBase");

        mMecanimToQtmSegmentNames.Add("Hips", skeletonName + "_Hips");
        mMecanimToQtmSegmentNames.Add("Spine", skeletonName + "_Spine");
        mMecanimToQtmSegmentNames.Add("Chest", skeletonName + "_Spine1");
        mMecanimToQtmSegmentNames.Add("UpperChest", skeletonName + "_Spine2");
        mMecanimToQtmSegmentNames.Add("Neck", skeletonName + "_Neck");
        mMecanimToQtmSegmentNames.Add("Head", skeletonName + "_Head");
        /*
        mMecanimToQtmSegmentNames.Add("Left Thumb Proximal", skeletonName + "_LeftHandThumb1");
        mMecanimToQtmSegmentNames.Add("Left Thumb Intermediate", skeletonName + "_LeftHandThumb2");
        mMecanimToQtmSegmentNames.Add("Left Thumb Distal", skeletonName + "_LeftHandThumb3");
        mMecanimToQtmSegmentNames.Add("Left Index Proximal", skeletonName + "_LeftHandIndex1");
        mMecanimToQtmSegmentNames.Add("Left Index Intermediate", skeletonName + "_LeftHandIndex2");
        mMecanimToQtmSegmentNames.Add("Left Index Distal", skeletonName + "_LeftHandIndex3");
        mMecanimToQtmSegmentNames.Add("Left Middle Proximal", skeletonName + "_LeftHandMiddle1");
        mMecanimToQtmSegmentNames.Add("Left Middle Intermediate", skeletonName + "_LeftHandMiddle2");
        mMecanimToQtmSegmentNames.Add("Left Middle Distal", skeletonName + "_LeftHandMiddle3");
        mMecanimToQtmSegmentNames.Add("Left Ring Proximal", skeletonName + "_LeftHandRing1");
        mMecanimToQtmSegmentNames.Add("Left Ring Intermediate", skeletonName + "_LeftHandRing2");
        mMecanimToQtmSegmentNames.Add("Left Ring Distal", skeletonName + "_LeftHandRing3");
        mMecanimToQtmSegmentNames.Add("Left Little Proximal", skeletonName + "_LeftHandPinky1");
        mMecanimToQtmSegmentNames.Add("Left Little Intermediate", skeletonName + "_LeftHandPinky2");
        mMecanimToQtmSegmentNames.Add("Left Little Distal", skeletonName + "_LeftHandPinky3");

        mMecanimToQtmSegmentNames.Add("Right Thumb Proximal", skeletonName + "_RightHandThumb1");
        mMecanimToQtmSegmentNames.Add("Right Thumb Intermediate", skeletonName + "_RightHandThumb2");
        mMecanimToQtmSegmentNames.Add("Right Thumb Distal", skeletonName + "_RightHandThumb3");
        mMecanimToQtmSegmentNames.Add("Right Index Proximal", skeletonName + "_RightHandIndex1");
        mMecanimToQtmSegmentNames.Add("Right Index Intermediate", skeletonName + "_RightHandIndex2");
        mMecanimToQtmSegmentNames.Add("Right Index Distal", skeletonName + "_RightHandIndex3");
        mMecanimToQtmSegmentNames.Add("Right Middle Proximal", skeletonName + "_RightHandMiddle1");
        mMecanimToQtmSegmentNames.Add("Right Middle Intermediate", skeletonName + "_RightHandMiddle2");
        mMecanimToQtmSegmentNames.Add("Right Middle Distal", skeletonName + "_RightHandMiddle3");
        mMecanimToQtmSegmentNames.Add("Right Ring Proximal", skeletonName + "_RightHandRing1");
        mMecanimToQtmSegmentNames.Add("Right Ring Intermediate", skeletonName + "_RightHandRing2");
        mMecanimToQtmSegmentNames.Add("Right Ring Distal", skeletonName + "_RightHandRing3");
        mMecanimToQtmSegmentNames.Add("Right Little Proximal", skeletonName + "_RightHandPinky1");
        mMecanimToQtmSegmentNames.Add("Right Little Intermediate", skeletonName + "_RightHandPinky2");
        mMecanimToQtmSegmentNames.Add("Right Little Distal", skeletonName + "_RightHandPinky3");
        */
    }

}
