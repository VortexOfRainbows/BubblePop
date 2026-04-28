void GaussianBlur_float(UnityTexture2D Texture, float2 UV, float Blur, UnitySamplerState Sampler, float2 Texture_TexelSize, out float3 Out_RGB, out float Out_Alpha)
{
    float4 col = 0;
    float kernelSum = 0;
    int upper = Blur;
    int lower = -Blur;
    for (int x = lower; x <= upper; ++x)
    {
        for (int y = lower; y <= upper; ++y)
        {
            float2 offset = float2(Texture_TexelSize.x * x, Texture_TexelSize.y * y);
            col += Texture.Sample(Sampler, UV + offset);
            ++kernelSum;
        }
    }
    col /= kernelSum;
    Out_RGB = col.rgb;
    Out_Alpha = col.a;
}