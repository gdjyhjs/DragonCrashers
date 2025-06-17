using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitVFXDisplayManager : MonoBehaviour
{

    [Header("打击特效对象池")]
    public ObjectPoolBehaviour objectPool;

    public void ShowHitVFX(Transform hitTransform)
    {
        GameObject hitVFXObject = objectPool.GetPooledObject();
        hitVFXObject.transform.SetPositionAndRotation(hitTransform.position, hitTransform.rotation);
        hitVFXObject.SetActive(true);
    }
}