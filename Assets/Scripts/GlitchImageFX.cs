using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlitchImageFX : MonoBehaviour
{
    public static Material material;

    static MonoBehaviour instance;

    void Awake()
    {
        //Get string from shader properties/info in the inspector.
        if (material == null)
        {
            //Debug.Log("GlitchImageFX Material is null. Trying to add GlitchMat...");
            if (Resources.Load("Materials/GlitchMat") as Material != null)
            {
                //Debug.Log("GlitchMat Added!");
                material = Resources.Load("Materials/GlitchMat") as Material;
            }
            else Debug.Log("GlitchMat failure.");
        }

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    public void Glitch(float strength, Material mat)
    {
        //Get string from shader properties/info in the inspector.
        mat.SetFloat("_Strength", strength);
    }

}
