using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartRateUpdate : MonoBehaviour
{
    public WebSocketClient webSocketClient;

    [Header("Heart Rate Settings")]
    [SerializeField] private float currentHeartRate = 0;
    [SerializeField] private float minHeartRate = 40f;
    [SerializeField] private float maxHeartRate = 200f;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color elevatedColor = Color.red;
    [SerializeField] private Color noSignalColor = Color.gray;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float noSignalPulseSpeed = 1f;
    
    private Vector3 originalScale;
    private Renderer heartRenderer;
    private Material heartMaterial;

    // Start is called before the first frame update
    void Start()
    {
        webSocketClient = GameObject.FindWithTag("ws")?.GetComponent<WebSocketClient>();
        if (webSocketClient == null)
        {
            Debug.LogError("WebSocketClient not found. Please assign it in the inspector or ensure the GameObject with 'ws' tag has the WebSocketClient component.");
            return;
        }
        webSocketClient.OnHeartRateUpdate += OnHeartRateUpdate;

        // Get the renderer component
        heartRenderer = GetComponent<Renderer>();
        if (heartRenderer != null)
        {
            heartMaterial = heartRenderer.material;
        }
        
        // Store original scale for pulsing effect
        originalScale = transform.localScale;
    }

    private void OnHeartRateUpdate(int hr)
    {
        this.SetHeartRate(hr);
    }

    // Update is called once per frame
    void Update()
    {
        // Update heart rate visualization
        UpdateHeartRateVisualization();
    }

    public void SetHeartRate(int newHeartRate)
    {
        currentHeartRate = newHeartRate;
        // currentHeartRate = Mathf.Clamp(newHeartRate, minHeartRate, maxHeartRate);
    }

    private void UpdateHeartRateVisualization()
    {
        if (heartMaterial != null)
        {
            if (currentHeartRate <= 0)
            {
                // No signal state - static gray color, no pulsing
                heartMaterial.color = noSignalColor;
                transform.localScale = originalScale;
            }
            else
            {
                // Normal heart rate visualization
                float normalizedHeartRate = Mathf.InverseLerp(minHeartRate, maxHeartRate, currentHeartRate);
                Color currentColor = Color.Lerp(normalColor, elevatedColor, normalizedHeartRate);
                heartMaterial.color = currentColor;

                // Create pulsing effect based on heart rate
                float pulse = Mathf.Sin(Time.time * pulseSpeed * (currentHeartRate / 60f)) * 0.5f + 0.5f;
                float scale = Mathf.Lerp(1f, pulseScale, pulse);
                transform.localScale = originalScale * scale;
            }
        }
    }
}
