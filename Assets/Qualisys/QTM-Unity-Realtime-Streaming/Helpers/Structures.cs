// Unity SDK for Qualisys Track Manager. Copyright 2015-2018 Qualisys AB
//
using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace QualisysRealTime.Unity {
    // Class for 6DOF with unity data types
    public class SixDOFBody {
        public SixDOFBody() { }
        public string Name;
        public Vector3 Position;
        public Quaternion Rotation;
        public Color Color;
    }

    // Class for labeled markers with unity data types
    public class LabeledMarker {
        public LabeledMarker() { }
        public string Name;
        public Vector3 Position;
        public Color Color;
        public float Residual;
    }

    public class UnlabeledMarker {
        public uint Id;
        public Vector3 Position;
        public float Residual;
    }

    // Class for user bones
    public class Bone {
        public Bone() { }
        public string From;
        public LabeledMarker FromMarker;
        public string To;
        public LabeledMarker ToMarker;
        public Color Color = Color.yellow;
    }

    // Class for gaze vectors
    public class GazeVector {
        public GazeVector() { }
        public string Name;
        public Vector3 Position;
        public Vector3 Direction;
    }

    public class AnalogChannel {
        public string Name;
        public float[] Values;
    }

    [Serializable]
    public class Segment {
        public string Name;
        public uint Id;
        public uint ParentId;
        //current transform
        public virtual Vector3 Position { get { return _pos.v3(); } set { _pos.setPos(value); } }
        private SerializablePos _pos = new SerializablePos(Vector3.zero);
        public virtual Quaternion Rotation { get { return _rot.Quat(); } set { _rot.setRot(value); } }
        private SerializableRot _rot = new SerializableRot(Quaternion.identity);
        //T pose transform (initial calibration transform)
        public virtual Vector3 TPosition { get { return _tpos.v3(); } set { _tpos.setPos(value); } }
        private SerializablePos _tpos = new SerializablePos(Vector3.zero);
        public virtual Quaternion TRotation { get { return _trot.Quat(); } set { _trot.setRot(value); } }
        private SerializableRot _trot = new SerializableRot(Quaternion.identity);
    }

    [Serializable]
    public class Skeleton {
        public string Name;
        public Dictionary<uint, Segment> Segments = new Dictionary<uint, Segment>();
    }

    [Serializable]
    public class mSkeleton {
        public mSkeleton() {
            Name = "null mSkeleton";
        }
        public mSkeleton(Skeleton sk) {
            Name = sk.Name;
            _seg = sk.Segments.ToList();
        }
        public string Name;
        public Dictionary<uint, Segment> Segments {
            get {
                return _seg.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            set {
                _seg = value.ToList();
            }
        }
        private List<KeyValuePair<uint, Segment>> _seg = new Dictionary<uint, Segment>().ToList();
    }

    //custom class to avoid vector3 issue

    [Serializable]
    public class SerializableTransform {
        public SerializableTransform(Vector3 position, Quaternion rotation) {
            _pos = new SerializablePos(position);
            _rot = new SerializableRot(rotation);
        }
        public SerializableTransform() {
            _pos = new SerializablePos(new Vector3(0, 0, 0));
            _rot = new SerializableRot(Quaternion.identity);
        }
        private SerializablePos _pos;
        private SerializableRot _rot;
        public Vector3 pos { get { return _pos.v3(); } set { _pos.setPos(value); } }
        public Quaternion rot { get { return _rot.Quat(); } set { _rot.setRot(value); } }
    }

    [Serializable]
    public class SerializablePos {
        public float x;
        public float y;
        public float z;

        public SerializablePos(Vector3 vector) {
            setPos(vector);
        }

        public Vector3 v3() {
            return new Vector3(x, y, z);
        }

        public void setPos(Vector3 vector) {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
    }

    [Serializable]
    public class SerializableRot {
        public float x;
        public float y;
        public float z;
        public float w;


        public SerializableRot(Quaternion r) {
            setRot(r);
        }

        public Quaternion Quat() {
            return new Quaternion(x, y, z, w);
        }

        public void setRot(Quaternion r) {
            x = r.x;
            y = r.y;
            z = r.z;
            w = r.w;
        }
    }


}
