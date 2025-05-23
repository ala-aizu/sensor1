using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public int minLux = 35;
    public int maxLux = 2894;
    public Light lightSource;
    public float sensorValue;
    public float multiplier = 10;

    void Start()
    {
        lightSource = GetComponent<Light>();
    }
    void Update()
    {
        if (KiwriousSerialReader.instance != null)
        {
            sensorValue = KiwriousSerialReader.instance
                ?.sensorData["LIGHT"]
                ?.values?["lux"]
                ?? 0;

            var lerpLux = Mathf.InverseLerp(minLux, maxLux, sensorValue);
            var lerpIntensity = Mathf.Lerp(0, 1, lerpLux);

            lightSource.intensity = lerpIntensity * multiplier;
        }
    }
}
