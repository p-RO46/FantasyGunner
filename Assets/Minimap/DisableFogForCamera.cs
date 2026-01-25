using UnityEngine;
using UnityEngine.Rendering;

public class DisableFogForCamera : MonoBehaviour
{
    private Camera targetCamera;

    void OnEnable()
    {
        targetCamera = GetComponent<Camera>();
        // 렌더링 시작 전/후 이벤트 구독
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    // 카메라가 그리기 직전에 실행
    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == targetCamera)
        {
            RenderSettings.fog = false; // 안개 끄기
        }
    }

    // 카메라가 그리기를 마친 후 실행
    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == targetCamera)
        {
            RenderSettings.fog = true; // 안개 다시 켜기 (다른 카메라를 위해)
        }
    }
}