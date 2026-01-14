using System.Collections;
using UnityEngine;

public class GunSetting : MonoBehaviour
{
    public WeaponData weaponData;

    public bool isRunning = false;
    public bool isMoving = false;

    private Vector3 origin;

    private void Start()
    {
        origin = transform.localPosition;
    }

    private void Update()
    {
        //if(runCoroutine == null && isRunning) runCoroutine = StartCoroutine(Run());
    }

    void SetModel()
    {
        GetComponent<MeshRenderer>().material = weaponData.material;
        GetComponent<MeshFilter>().mesh = weaponData.mesh;
    }

    public void SetWeaponData(WeaponData w)
    {
        weaponData = w;
        SetModel();
    }

    /*
    public IEnumerator Run()
    {
        float y = 0.1f;
        while (isRunning && isMoving)
        {
            if (transform.localPosition.y > origin.y + 0.2f || transform.localPosition.y < origin.y)
            {
                y = -y;
            }
            transform.localPosition += new Vector3(0, y, 0);
            yield return Util.WaitForSecond(0.03f);
        }
        runCoroutine = null;
        transform.localPosition = origin;
    }
    */
}
