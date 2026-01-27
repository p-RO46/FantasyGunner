using UnityEngine;

public class FollowPlayerPosition : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform 연결
    public float height = 20f; // 미니맵 카메라의 높이

    void LateUpdate()
    {
        if (player == null) return;

        // 플레이어의 X, Z 좌표는 유지하고, 높이(Y)만 고정된 위치로 이동
        Vector3 newPosition = player.position;
        newPosition.y += height;
        transform.position = newPosition;

        // 카메라의 회전을 고정 (항상 북쪽이 위를 향하게 하고 싶을 때)
        // 만약 플레이어가 보는 방향에 따라 미니맵이 회전하길 원하면 이 줄을 주석 처리하세요.
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
