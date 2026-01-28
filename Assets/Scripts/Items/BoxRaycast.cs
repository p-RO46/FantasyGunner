using UnityEngine;

public class BoxRaycast : MonoBehaviour
{
    public bool nearPlayer = false;
    public BoxManager boxManager;
    private Transform player;

    public float range = 10f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    void Update()
    {
        if (nearPlayer)
        {
            Ray ray = new Ray(player.transform.position, player.transform.forward);
            RaycastHit hit;

            int boxMask = ~(1 << LayerMask.NameToLayer("Boxdetect"));

            if (Physics.Raycast(ray, out hit, range, boxMask))
            {
                if (hit.transform.root == transform.root)
                {
                    boxManager.CanOpen(true);
                }
                else
                {
                    boxManager.CanOpen(false);
                }
            }
            else
            {
                boxManager.CanOpen(false);
            }

            Debug.DrawRay(ray.origin, ray.direction * range, Color.blue, 1.0f);
        }
    }
}
