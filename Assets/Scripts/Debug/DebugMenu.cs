using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// DebugMenu.cs
// Phoebe Faith (1033478)
/// <summary>
/// Contains debug menu handling and connector logic
/// </summary>

public class DebugMenu : MonoBehaviour
{
    private MovementHandler movementHandler;
    private LocalPlayer localPlayer;
    
    private DetectionMiddleman[] detectionMiddleman;

    private void OnEnable()
    {
        if (FindObjectsByType<LocalPlayer>(FindObjectsSortMode.None).Length > 1)
        {
            throw new Exception("There should only be one LocalPlayer in your scene, and there is more than one. Please ensure the only LocalPlayer component in your scene is on the local controlling player.");
        }

        if (FindObjectsByType<MovementHandler>(FindObjectsSortMode.None).Length > 1)
        {
            throw new Exception("There should only be one MovementHandler in your scene, and there is more than one. Please ensure the only MovementHandler component in your scene is on the local controlling player.");
        }

        detectionMiddleman = GameObject.FindObjectsByType<DetectionMiddleman>(FindObjectsSortMode.None);

        localPlayer = FindObjectOfType<LocalPlayer>();
        movementHandler = FindObjectOfType<MovementHandler>();
        
        if(movementHandler) movementHandler.OnDebugInformationChanged += MovementHandlerDebug;
        else
        {
            throw new Exception("Missing MovementHandler in scene.");
        }
        if(localPlayer) localPlayer.OnBasePlayerDebugUpdate += BasePlayerDebug;
        else
        {
            throw new Exception("Missing LocalPlayer in scene.");
        }
    }
    
    private void OnDisable()
    {
        if(movementHandler) movementHandler.OnDebugInformationChanged -= MovementHandlerDebug;
        if(localPlayer) localPlayer.OnBasePlayerDebugUpdate -= BasePlayerDebug;
    }

    private void BasePlayerDebug(BasePlayer.BasePlayerDebug _info)
    {
        Debug.Log($"DebugMenu: BasePlayerDebug: {_info.health}");
    }
    
    private void MovementHandlerDebug(MovementHandler.MovementHandlerDebugInformation _info)
    {
        Debug.Log($"DebugMenu: MovementHandlerDebug: Received information: {_info.velocity}");
    }
}