using System;
using UnityEngine;

public class PlayerLookAt : MonoBehaviour
{
    public float range = 10f;
    private bool isNearObject = false;

    public Camera cam;
    private CaptureSpot NearCS = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CSdetect"))
        {
            NearCS = other.gameObject.GetComponent<CaptureSpot>();
            isNearObject = true;
            //NearCS.PlaySound();
            //Debug.Log("상호작용 가능 범위 진입");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CSdetect"))
        {
            isNearObject = false;
            //Debug.Log("범위 벗어남");
            if (NearCS != null) 
            { 
                //NearCS.StopSound();
                NearCS.Interactable(false); 
            }
            NearCS = null;
        }
    }
    void Update()
    { 
        if (isNearObject)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            int layerMask = ~(1 << LayerMask.NameToLayer("CSdetect"));

            if (Physics.Raycast(ray, out hit, range, layerMask))
            {
                // 현재 맞춘 타겟이 가지고 있는 스크립트를 가져옴
                CaptureSpot target = hit.collider.GetComponent<CaptureSpot>();
                if (target != null)
                {
                    target.Interactable(true);
                    NearCS = target;
                }
                else
                {
                    if (NearCS != null) NearCS.Interactable(false);
                }
            }
            else
            {
                // 공중을 보고 있을 때만 꺼줌
                if (NearCS != null) NearCS.Interactable(false);
            }

            Debug.DrawRay(ray.origin, ray.direction * range, Color.blue, 1.0f);
        }
    }
}
