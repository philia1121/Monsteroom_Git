using UnityEngine;

public class BigDog : MonoBehaviour
{
    public Transform target; // B 物件
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;
    //public TextMesh dogX;
    //public TextMesh dogY;
    //public TextMesh dogZ;
    void Update()
    {
        //dogX.text = "X : " + followX.ToString();
        //dogY.text = "Y : " + followY.ToString();
        //dogZ.text = "Z : " + followZ.ToString();
        if (target == null) return;

        // 取得目前的旋轉（歐拉角）
        Vector3 currentRotation = transform.rotation.eulerAngles;
        Vector3 targetRotation = target.rotation.eulerAngles;

        // 根據選項決定要不要覆蓋
        float x = followX ? targetRotation.x : currentRotation.x;
        float y = followY ? targetRotation.y : currentRotation.y;
        float z = followZ ? targetRotation.z : currentRotation.z;

        // 設定新的旋轉
        transform.rotation = Quaternion.Euler(x, y, z);
        this.transform.position = target.transform.position;
    }
}
