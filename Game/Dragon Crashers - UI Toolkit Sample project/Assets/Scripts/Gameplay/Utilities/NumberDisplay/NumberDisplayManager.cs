using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberDisplayManager : MonoBehaviour
{
    [Header("数字对象池")]
    public ObjectPoolBehaviour objectPool;

    [Header("位置随机偏移")]
    public Vector3 positionRandomOffsetRange;

    public void ShowNumber(int numberAmount, Transform numberTransform, Color numberColor)
    {
        GameObject numberObject = objectPool.GetPooledObject();

        Vector3 newPosition = numberTransform.position + RandomOffsetRange(positionRandomOffsetRange);

        numberObject.GetComponent<NumberDisplayBehaviour>().SetupDisplay(numberAmount, newPosition, numberColor);
        numberObject.SetActive(true);
    }

    Vector3 RandomOffsetRange(Vector3 rangeVectors)
    {

        return new Vector3(RandomInRange(rangeVectors.x),
                            RandomInRange(rangeVectors.y),
                            RandomInRange(rangeVectors.x)
        );
    }

    float RandomInRange(float rangeValue)
    {
        return Random.Range(-rangeValue, rangeValue);
    }

}