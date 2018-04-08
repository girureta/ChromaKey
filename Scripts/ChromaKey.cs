using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ChromaKeyRenderer), PostProcessEvent.AfterStack, "Custom/ChromaKey")]
public class ChromaKey : PostProcessEffectSettings
{
    public BoolParameter chainKeys = new BoolParameter { value = false };
    public ChromeKeyParameter keys = new ChromeKeyParameter { value = null};
}

public sealed class ChromaKeyRenderer : PostProcessEffectRenderer<ChromaKey>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ChromaKey"));
        if (settings.keys.value == null)
            return;

        var compoundTexture = context.GetScreenSpaceTemporaryRT();
        context.command.BlitFullscreenTriangle(context.source, compoundTexture);
        for (int i = 0; i<settings.keys.value.Length; i++)
        {
            AddCommand(sheet, settings.keys.value[i], context, context.source, context.destination, compoundTexture);
        }
    }

    public void AddCommand(PropertySheet sheet,ChromeKeySettings settings, PostProcessRenderContext context, RenderTargetIdentifier sourceTexture, RenderTargetIdentifier targetTexture, RenderTexture compoundTexture)
    {
        sheet.properties.SetFloat("_Distance", settings.distance);
        sheet.properties.SetColor("_ScreenColor", settings.screenColor);
        if (settings.backgroundTexture != null)
            sheet.properties.SetTexture("_BackgroundTex", settings.backgroundTexture);
        sheet.properties.SetTexture("_CompoundTex", compoundTexture);
        context.command.BlitFullscreenTriangle(sourceTexture, targetTexture, sheet, 0);
        context.command.BlitFullscreenTriangle(targetTexture,compoundTexture);
    }
}

[Serializable]
public class ChromeKeySettings
{
    [Tooltip("The color of the screen")]
    [SerializeField]
    public Color screenColor = Color.green;

    [Range(0f, 0.2f), Tooltip("Chroma key effect distance. If the difference between the hue of a pixel and 'Screen Color' is smaller than 'Distance' then it will be replaced with 'Replace Texture' ")]
    [SerializeField]
    public float distance = 0.01f;

    [Tooltip("The texture the screen's pixels may be replaced with")]
    [SerializeField]
    public Texture backgroundTexture = null;

}

[Serializable]
public class ChromeKeyParameter : ParameterOverride<ChromeKeySettings[]>
{
}