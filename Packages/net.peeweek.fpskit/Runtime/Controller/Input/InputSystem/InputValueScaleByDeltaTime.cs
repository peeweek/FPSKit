using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class InputValueScaleByDeltaTime : InputProcessor<Vector2>
{
    [Tooltip("The target framerate of your game")]
    public float TargetDeltaTime = 0.016666f;

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        float ratio = Time.deltaTime / TargetDeltaTime;
        return value / ratio;
    }

#if UNITY_EDITOR
    static InputValueScaleByDeltaTime()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<InputValueScaleByDeltaTime>();
    }

}
