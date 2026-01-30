using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Cinemachineをインポート
using Unity.Cinemachine;

public class CameraSwitch : MonoBehaviour{
    // Cinemachine Cameraをアタッチ
    public CinemachineCamera camera1;

    // プレイヤーがトリガーに入ったらカメラの優先度を上げる
    void OnTriggerStay(Collider other){
        if (other.tag == "Player"){
            camera1.Priority = 100;
        }
    }

    // プレイヤーがトリガーから出たらカメラの優先度を下げる
    void OnTriggerExit(Collider other){
        if (other.tag == "Player"){
            camera1.Priority = 10;
        }
    }
}
