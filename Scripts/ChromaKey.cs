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
    public ChromeKeyParameter chromeKeySettings = new ChromeKeyParameter { value = null};
}

public sealed class ChromaKeyRenderer : PostProcessEffectRenderer<ChromaKey>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ChromaKey"));
        if (settings.chromeKeySettings.value == null)
            return;

        var fromTexture = new RenderTargetIdentifier(context.GetScreenSpaceTemporaryRT());
        context.command.BlitFullscreenTriangle(context.source, fromTexture);
        var swap = fromTexture;
        var toTexture = new RenderTargetIdentifier(context.GetScreenSpaceTemporaryRT());
        for (int i = 0; i<settings.chromeKeySettings.value.Length; i++)
        {
            AddCommand(sheet, settings.chromeKeySettings.value[i], context,fromTexture,toTexture);
            swap = toTexture;
            toTexture = fromTexture;
            fromTexture = swap;
        }
        context.command.BlitFullscreenTriangle(fromTexture, context.destination);
    }

    public void AddCommand(PropertySheet sheet,ChromeKeySettings settings, PostProcessRenderContext context, RenderTargetIdentifier fromTexture,RenderTargetIdentifier toTexture)
    {
        sheet.properties.SetFloat("_Distance", settings.distance);
        sheet.properties.SetColor("_ScreenColor", settings.screenColor);
        if (settings.replaceTexture != null)
            sheet.properties.SetTexture("_ReplaceTex", settings.replaceTexture);
        context.command.BlitFullscreenTriangle(fromTexture, toTexture, sheet, 0);
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
    public Texture replaceTexture = null;

}

[Serializable]
public class ChromeKeyParameter : ParameterOverride<ChromeKeySettings[]>
{
    /*public override void Interp(ChromeKeySettings[] from, ChromeKeySettings[] to, float t)
    {
        int num = Math.Min(from == null?0:from.Length, to == null ? 0 : to.Length);
        for (int i = 0; i < num; i++)
        {
            to[i].screenColor = Color.Lerp(from[i].screenColor, to[i].screenColor, t);
            to[i].distance = Mathf.Lerp(from[i].distance, to[i].distance, t);
        }
    }*/
}