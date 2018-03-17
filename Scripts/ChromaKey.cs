using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ChromaKeyRenderer), PostProcessEvent.AfterStack, "Custom/ChromaKey")]
public class ChromaKey : PostProcessEffectSettings
{
	[Tooltip("The color of the screen")]
    public ColorParameter screenColor = new ColorParameter { value = Color.green };

    [Range(0f, 0.2f), Tooltip("Chroma key effect distance. If the difference between the hue of a pixel and 'Screen Color' is smaller than 'Distance' then it will be replaced with 'Replace Texture' ")]
    public FloatParameter distance = new FloatParameter { value = 0.01f };

    [Tooltip("The texture the screen's pixels may be replaced with")]
    public TextureParameter replaceTexture = new TextureParameter { value = null};
}

public sealed class ChromaKeyRenderer : PostProcessEffectRenderer<ChromaKey>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ChromaKey"));
        sheet.properties.SetFloat("_Distance", settings.distance);
		sheet.properties.SetColor("_ScreenColor", settings.screenColor);
        if(settings.replaceTexture.value != null)
            sheet.properties.SetTexture("_ReplaceTex", settings.replaceTexture);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}