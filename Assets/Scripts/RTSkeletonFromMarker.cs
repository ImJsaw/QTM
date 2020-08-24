using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTSkeletonFromMarker : MonoBehaviour {

    public bool isTest = false;
    private bool isInit = false;
    public string SkeletonName = "SR";
    public Transform test = null;

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

    private mSkeleton loadSk = null;

    private mSkeleton getSkeleton(string SkeletonName) {
        if (isTest)
            return getSkFromMk();
        Skeleton sk = rtClient.GetSkeleton(SkeletonName);
        if (sk == null)
            return null;
        return new mSkeleton(sk);
    }

    void FixedUpdate() {
        skeleton = null;
        if (rtClient == null)
            rtClient = RTClient.GetInstance();
        skeleton = getSkeleton(SkeletonName);
        updateSkeleton();
    }

    private void loadSke() {
        if (loadSk == null) {
            loadSk = new mSkeleton();
            loadSk.Name = "SR";
            loadSk._seg = Utils.load<qtmRecord>("Ske.txt").records[0];
            Debug.Log("Load sk");
        }
    }

    private mSkeleton getSkFromMk() {
        loadSke();
        setSkeletonPosition(loadSk);
        setSkeletonRotation(loadSk);
        return loadSk;
    }

    private void setSkeletonPosition(mSkeleton sk) {
        foreach (var segment in sk._seg) {
            switch (segment.Value.Name) {
                case "Hips":
                    segment.Value.Position = new Vector3(-test.position.z, test.position.y, test.position.x);
                    break;
                default:
                    break;
            }
        }
    }

    private void setSkeletonRotation(mSkeleton sk) {

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

    private void updateSkeleton() {
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
                gameObject.transform.localPosition = segment.Value.Position;
                gameObject.transform.localRotation = segment.Value.Rotation;
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
