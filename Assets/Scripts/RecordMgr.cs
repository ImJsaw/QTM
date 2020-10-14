using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordMgr : MonoBehaviour {

    public RTSkeletonPost skeScript = null;
    public InputField fileName = null;
    public Text hint = null;
    private string hintTxt = "";

    public void OnRecordTrigger() {
        if (skeScript != null)
            skeScript.OnRecordTrigger(fileName.text);
    }

    public void OnSaveCalibrateTrigger() {
        if (skeScript != null)
            skeScript.saveCalibrate(fileName.text);
    }

    public void OnReadCalibrateTrigger() {
        if (skeScript != null)
            skeScript.readCalibrate(fileName.text);
    }

    public void OnReplay() {
        if(skeScript != null)
            skeScript.OnReplay(fileName.text);
    }

    public void OnRecordStop() {
        if(skeScript != null)
            skeScript.OnRecordStop();
    }

    public void setHint(string txt) {
        hintTxt = txt;
    }

    // Update is called once per frame
    void Update() {
        if (hint != null)
            hint.text = hintTxt;
    }
}
