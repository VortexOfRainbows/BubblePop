Shader "Shader Graphs/OutlineShader"
{
    Properties
    {
        [MainTexture][NoScaleOffset]_MainTex("Texture2D", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (1, 0, 0, 0)
        _OutlineSize("OutlineSize", Range(0, 1000)) = 20
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
        }
        
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
 		ColorMask [_ColorMask]

        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _OutlineColor;
        float _OutlineSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float
        {
        half4 uv0;
        };
        
        void SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(UnityTexture2D _Texture2D, float _OutlineSize, float2 _Dir, Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float IN, out float New_0)
        {
        UnityTexture2D _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D = _Texture2D;
        float4 _UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4 = IN.uv0;
        float2 _Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2 = _Dir;
        float _Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float = _OutlineSize;
        float _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float;
        Unity_Multiply_float_float(_Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float, 0.001, _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float);
        float2 _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2, (_Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float.xx), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2);
        float2 _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2;
        Unity_Add_float2((_UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4.xy), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2, _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2);
        float4 _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.tex, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.samplerstate, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.GetTransformedUV(_Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2) );
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_R_4_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.r;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_G_5_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.g;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_B_6_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.b;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.a;
        float _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float;
        Unity_Add_float(_SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float, float(0), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float);
        float _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        Unity_Step_float(float(1), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float, _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float);
        New_0 = _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4 = IN.uv0;
            float4 _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.tex, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.samplerstate, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.GetTransformedUV((_UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4.xy)) );
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_R_4_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.r;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_G_5_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.g;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_B_6_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.b;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.a;
            UnityTexture2D _Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6;
            _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6.uv0 = IN.uv0;
            float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D, _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float, float2 (1, 0), _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6, _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float);
            UnityTexture2D _Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60;
            _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60.uv0 = IN.uv0;
            float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D, _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float, float2 (-1, 0), _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float);
            float _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float, _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float);
            UnityTexture2D _Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a;
            _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a.uv0 = IN.uv0;
            float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D, _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float, float2 (0, -1), _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a, _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float);
            UnityTexture2D _Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9;
            _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9.uv0 = IN.uv0;
            float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D, _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float, float2 (0, 1), _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float);
            float _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float);
            float _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float;
            Unity_Add_float(_Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float, _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float);
            UnityTexture2D _Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5;
            _OutlineSubGraph_4ff2351332a74927963088c13b1940a5.uv0 = IN.uv0;
            float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D, _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float, float2 (0.5, 0.5), _OutlineSubGraph_4ff2351332a74927963088c13b1940a5, _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float);
            UnityTexture2D _Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2;
            _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2.uv0 = IN.uv0;
            float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D, _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float, float2 (0.5, -0.5), _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float);
            float _Add_a08e1ea940744c1da85b292963096547_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float, _Add_a08e1ea940744c1da85b292963096547_Out_2_Float);
            UnityTexture2D _Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb;
            _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb.uv0 = IN.uv0;
            float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D, _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float, float2 (-0.5, 0.5), _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb, _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float);
            UnityTexture2D _Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804;
            _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804.uv0 = IN.uv0;
            float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D, _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float, float2 (-0.5, -0.5), _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float);
            float _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float);
            float _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float;
            Unity_Add_float(_Add_a08e1ea940744c1da85b292963096547_Out_2_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float);
            float _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float;
            Unity_Add_float(_Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float, _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float);
            float _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float;
            Unity_Step_float(float(1), _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float, _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float);
            float _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float;
            Unity_Step_float(float(0.4), _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float);
            float _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float;
            Unity_Subtract_float(_Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float, _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float);
            float _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float;
            Unity_Clamp_float(_Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float, float(0), float(1), _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float);
            float _Subtract_ced5ff4c16c041d48bfdf75c32fa2f1d_Out_2_Float;
            Unity_Subtract_float(_SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float, _Subtract_ced5ff4c16c041d48bfdf75c32fa2f1d_Out_2_Float);
            float _Step_9abe0aad0dcc4d8cbd96f299e0151e86_Out_2_Float;
            Unity_Step_float(float(0), _Subtract_ced5ff4c16c041d48bfdf75c32fa2f1d_Out_2_Float, _Step_9abe0aad0dcc4d8cbd96f299e0151e86_Out_2_Float);
            float4 _Multiply_bacd830cc1ce456dbdc81d82d77a33a9_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4, (_Step_9abe0aad0dcc4d8cbd96f299e0151e86_Out_2_Float.xxxx), _Multiply_bacd830cc1ce456dbdc81d82d77a33a9_Out_2_Vector4);
            float4 _Property_e04ad3080dba4e7286c47eccf6bb029e_Out_0_Vector4 = _OutlineColor;
            float4 _Multiply_9b815cedb3114644a0a83a5233668176_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float.xxxx), _Property_e04ad3080dba4e7286c47eccf6bb029e_Out_0_Vector4, _Multiply_9b815cedb3114644a0a83a5233668176_Out_2_Vector4);
            float4 _Add_c158d86a66754802a15e7b297b5de109_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bacd830cc1ce456dbdc81d82d77a33a9_Out_2_Vector4, _Multiply_9b815cedb3114644a0a83a5233668176_Out_2_Vector4, _Add_c158d86a66754802a15e7b297b5de109_Out_2_Vector4);
            float _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            Unity_Add_float(_Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float, _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float);
            surface.BaseColor = (_Add_c158d86a66754802a15e7b297b5de109_Out_2_Vector4.xyz);
            surface.Alpha = _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            surface.AlphaClipThreshold = float(0);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _OutlineColor;
        float _OutlineSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float
        {
        half4 uv0;
        };
        
        void SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(UnityTexture2D _Texture2D, float _OutlineSize, float2 _Dir, Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float IN, out float New_0)
        {
        UnityTexture2D _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D = _Texture2D;
        float4 _UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4 = IN.uv0;
        float2 _Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2 = _Dir;
        float _Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float = _OutlineSize;
        float _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float;
        Unity_Multiply_float_float(_Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float, 0.001, _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float);
        float2 _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2, (_Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float.xx), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2);
        float2 _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2;
        Unity_Add_float2((_UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4.xy), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2, _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2);
        float4 _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.tex, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.samplerstate, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.GetTransformedUV(_Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2) );
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_R_4_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.r;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_G_5_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.g;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_B_6_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.b;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.a;
        float _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float;
        Unity_Add_float(_SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float, float(0), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float);
        float _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        Unity_Step_float(float(1), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float, _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float);
        New_0 = _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6;
            _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6.uv0 = IN.uv0;
            float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D, _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float, float2 (1, 0), _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6, _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float);
            UnityTexture2D _Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60;
            _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60.uv0 = IN.uv0;
            float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D, _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float, float2 (-1, 0), _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float);
            float _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float, _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float);
            UnityTexture2D _Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a;
            _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a.uv0 = IN.uv0;
            float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D, _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float, float2 (0, -1), _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a, _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float);
            UnityTexture2D _Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9;
            _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9.uv0 = IN.uv0;
            float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D, _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float, float2 (0, 1), _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float);
            float _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float);
            float _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float;
            Unity_Add_float(_Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float, _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float);
            UnityTexture2D _Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5;
            _OutlineSubGraph_4ff2351332a74927963088c13b1940a5.uv0 = IN.uv0;
            float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D, _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float, float2 (0.5, 0.5), _OutlineSubGraph_4ff2351332a74927963088c13b1940a5, _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float);
            UnityTexture2D _Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2;
            _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2.uv0 = IN.uv0;
            float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D, _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float, float2 (0.5, -0.5), _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float);
            float _Add_a08e1ea940744c1da85b292963096547_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float, _Add_a08e1ea940744c1da85b292963096547_Out_2_Float);
            UnityTexture2D _Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb;
            _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb.uv0 = IN.uv0;
            float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D, _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float, float2 (-0.5, 0.5), _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb, _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float);
            UnityTexture2D _Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804;
            _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804.uv0 = IN.uv0;
            float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D, _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float, float2 (-0.5, -0.5), _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float);
            float _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float);
            float _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float;
            Unity_Add_float(_Add_a08e1ea940744c1da85b292963096547_Out_2_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float);
            float _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float;
            Unity_Add_float(_Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float, _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float);
            float _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float;
            Unity_Step_float(float(1), _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float, _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float);
            UnityTexture2D _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4 = IN.uv0;
            float4 _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.tex, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.samplerstate, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.GetTransformedUV((_UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4.xy)) );
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_R_4_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.r;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_G_5_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.g;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_B_6_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.b;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.a;
            float _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float;
            Unity_Step_float(float(0.4), _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float);
            float _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float;
            Unity_Subtract_float(_Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float, _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float);
            float _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float;
            Unity_Clamp_float(_Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float, float(0), float(1), _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float);
            float _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            Unity_Add_float(_Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float, _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float);
            surface.Alpha = _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            surface.AlphaClipThreshold = float(0);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _OutlineColor;
        float _OutlineSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float
        {
        half4 uv0;
        };
        
        void SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(UnityTexture2D _Texture2D, float _OutlineSize, float2 _Dir, Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float IN, out float New_0)
        {
        UnityTexture2D _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D = _Texture2D;
        float4 _UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4 = IN.uv0;
        float2 _Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2 = _Dir;
        float _Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float = _OutlineSize;
        float _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float;
        Unity_Multiply_float_float(_Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float, 0.001, _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float);
        float2 _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2, (_Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float.xx), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2);
        float2 _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2;
        Unity_Add_float2((_UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4.xy), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2, _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2);
        float4 _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.tex, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.samplerstate, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.GetTransformedUV(_Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2) );
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_R_4_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.r;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_G_5_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.g;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_B_6_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.b;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.a;
        float _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float;
        Unity_Add_float(_SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float, float(0), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float);
        float _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        Unity_Step_float(float(1), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float, _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float);
        New_0 = _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6;
            _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6.uv0 = IN.uv0;
            float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D, _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float, float2 (1, 0), _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6, _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float);
            UnityTexture2D _Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60;
            _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60.uv0 = IN.uv0;
            float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D, _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float, float2 (-1, 0), _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float);
            float _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float, _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float);
            UnityTexture2D _Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a;
            _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a.uv0 = IN.uv0;
            float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D, _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float, float2 (0, -1), _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a, _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float);
            UnityTexture2D _Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9;
            _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9.uv0 = IN.uv0;
            float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D, _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float, float2 (0, 1), _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float);
            float _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float);
            float _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float;
            Unity_Add_float(_Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float, _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float);
            UnityTexture2D _Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5;
            _OutlineSubGraph_4ff2351332a74927963088c13b1940a5.uv0 = IN.uv0;
            float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D, _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float, float2 (0.5, 0.5), _OutlineSubGraph_4ff2351332a74927963088c13b1940a5, _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float);
            UnityTexture2D _Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2;
            _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2.uv0 = IN.uv0;
            float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D, _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float, float2 (0.5, -0.5), _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float);
            float _Add_a08e1ea940744c1da85b292963096547_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float, _Add_a08e1ea940744c1da85b292963096547_Out_2_Float);
            UnityTexture2D _Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb;
            _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb.uv0 = IN.uv0;
            float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D, _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float, float2 (-0.5, 0.5), _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb, _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float);
            UnityTexture2D _Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804;
            _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804.uv0 = IN.uv0;
            float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D, _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float, float2 (-0.5, -0.5), _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float);
            float _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float);
            float _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float;
            Unity_Add_float(_Add_a08e1ea940744c1da85b292963096547_Out_2_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float);
            float _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float;
            Unity_Add_float(_Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float, _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float);
            float _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float;
            Unity_Step_float(float(1), _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float, _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float);
            UnityTexture2D _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4 = IN.uv0;
            float4 _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.tex, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.samplerstate, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.GetTransformedUV((_UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4.xy)) );
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_R_4_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.r;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_G_5_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.g;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_B_6_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.b;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.a;
            float _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float;
            Unity_Step_float(float(0.4), _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float);
            float _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float;
            Unity_Subtract_float(_Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float, _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float);
            float _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float;
            Unity_Clamp_float(_Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float, float(0), float(1), _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float);
            float _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            Unity_Add_float(_Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float, _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float);
            surface.Alpha = _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            surface.AlphaClipThreshold = float(0);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float4 _OutlineColor;
        float _OutlineSize;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        struct Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float
        {
        half4 uv0;
        };
        
        void SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(UnityTexture2D _Texture2D, float _OutlineSize, float2 _Dir, Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float IN, out float New_0)
        {
        UnityTexture2D _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D = _Texture2D;
        float4 _UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4 = IN.uv0;
        float2 _Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2 = _Dir;
        float _Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float = _OutlineSize;
        float _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float;
        Unity_Multiply_float_float(_Property_d632c28be3a34b528db1cadd636c3f70_Out_0_Float, 0.001, _Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float);
        float2 _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2;
        Unity_Multiply_float2_float2(_Property_71e830657cbd4d7db890bf6b254c01d8_Out_0_Vector2, (_Multiply_4fc1defa938845c59424ac3bd469a045_Out_2_Float.xx), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2);
        float2 _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2;
        Unity_Add_float2((_UV_354217fb42e543f0934226cc5ef867ff_Out_0_Vector4.xy), _Multiply_eded421b2fcf49a68ee1e1bdbcc41ca5_Out_2_Vector2, _Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2);
        float4 _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.tex, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.samplerstate, _Property_2f787e14d3514dfab8393bc5adb80d64_Out_0_Texture2D.GetTransformedUV(_Add_32ee5488cfdb4c5eb4462b54ab95e267_Out_2_Vector2) );
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_R_4_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.r;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_G_5_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.g;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_B_6_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.b;
        float _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float = _SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_RGBA_0_Vector4.a;
        float _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float;
        Unity_Add_float(_SampleTexture2D_dca0c0fda7634b36b05feacbe3dcaa87_A_7_Float, float(0), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float);
        float _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        Unity_Step_float(float(1), _Add_440834d7a3004f94a51a623f674cafae_Out_2_Float, _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float);
        New_0 = _Step_5f19070689ad497e92a0f5d4e57d0d64_Out_2_Float;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4 = IN.uv0;
            float4 _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.tex, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.samplerstate, _Property_c89a875838344abbaf36ec7d271e77e4_Out_0_Texture2D.GetTransformedUV((_UV_eccc094e7667491cb680355fbdcd6337_Out_0_Vector4.xy)) );
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_R_4_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.r;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_G_5_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.g;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_B_6_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.b;
            float _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float = _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4.a;
            UnityTexture2D _Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6;
            _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6.uv0 = IN.uv0;
            float _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_28fb438753c54831a869116dd6887350_Out_0_Texture2D, _Property_cbab0f09da2c4646821c3152594c3c38_Out_0_Float, float2 (1, 0), _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6, _OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float);
            UnityTexture2D _Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60;
            _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60.uv0 = IN.uv0;
            float _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_1840da67a425486fbf046276a9c2ceb6_Out_0_Texture2D, _Property_0feb0453e8044b879e1c62491f1a8895_Out_0_Float, float2 (-1, 0), _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float);
            float _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_bb2c63595d64404e936c50a719c9f9a6_New_0_Float, _OutlineSubGraph_31a82e87d32348cab819fcac86dabc60_New_0_Float, _Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float);
            UnityTexture2D _Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a;
            _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a.uv0 = IN.uv0;
            float _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_b7d50b09efe94f829ec1d34f81818c4d_Out_0_Texture2D, _Property_2adca0c522ef450ba41af217ac274210_Out_0_Float, float2 (0, -1), _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a, _OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float);
            UnityTexture2D _Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9;
            _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9.uv0 = IN.uv0;
            float _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_bcdd09bb68de43a08c90f678e3711b89_Out_0_Texture2D, _Property_17d16bb3a043419196f24ce5aba2460a_Out_0_Float, float2 (0, 1), _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float);
            float _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_cfe49601056f42ea988876b3fa73087a_New_0_Float, _OutlineSubGraph_778b048ae4064bd4a3c77051aca87ab9_New_0_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float);
            float _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float;
            Unity_Add_float(_Add_1a882a8b82874e84b6549ff94697dafc_Out_2_Float, _Add_9decf2d1b6d3407e86c0fb9249743a09_Out_2_Float, _Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float);
            UnityTexture2D _Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5;
            _OutlineSubGraph_4ff2351332a74927963088c13b1940a5.uv0 = IN.uv0;
            float _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_e077e1569bc14e4f808bf7efc05d481f_Out_0_Texture2D, _Property_56c5d4882ffc49988c4cec2e7d230304_Out_0_Float, float2 (0.5, 0.5), _OutlineSubGraph_4ff2351332a74927963088c13b1940a5, _OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float);
            UnityTexture2D _Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2;
            _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2.uv0 = IN.uv0;
            float _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_84d4522fcb1141dcb9fe776925c829bd_Out_0_Texture2D, _Property_0ecab1da18c94520bfb22c7c4044d1cb_Out_0_Float, float2 (0.5, -0.5), _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float);
            float _Add_a08e1ea940744c1da85b292963096547_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_4ff2351332a74927963088c13b1940a5_New_0_Float, _OutlineSubGraph_7b1552092af1476b9dec571b9c632ec2_New_0_Float, _Add_a08e1ea940744c1da85b292963096547_Out_2_Float);
            UnityTexture2D _Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb;
            _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb.uv0 = IN.uv0;
            float _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_2c7ae49896bd4ddf868413bd3da6a83e_Out_0_Texture2D, _Property_31c874ad2cdb4601a5b602b959ce5311_Out_0_Float, float2 (-0.5, 0.5), _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb, _OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float);
            UnityTexture2D _Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float = _OutlineSize;
            Bindings_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804;
            _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804.uv0 = IN.uv0;
            float _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float;
            SG_OutlineSubGraph_3dc197836f4dde148b8c2e0b47a08cd9_float(_Property_57e62678a51f4ffdbe2e18f52601ff9e_Out_0_Texture2D, _Property_b3420125c1644f708c9a3b68efb96bf8_Out_0_Float, float2 (-0.5, -0.5), _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float);
            float _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float;
            Unity_Add_float(_OutlineSubGraph_9f872fdc0f524ddeba7b45ce5d62ebcb_New_0_Float, _OutlineSubGraph_151ac21d80b44c578ecf756eb0a61804_New_0_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float);
            float _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float;
            Unity_Add_float(_Add_a08e1ea940744c1da85b292963096547_Out_2_Float, _Add_505163fb489149b7a80bbe722e5cb3b8_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float);
            float _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float;
            Unity_Add_float(_Add_b8daf4b1fea34cd3b4c2d5a2b0cf03dd_Out_2_Float, _Add_bcbdd9d4d24f464f9ce3fe76a5608dc9_Out_2_Float, _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float);
            float _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float;
            Unity_Step_float(float(1), _Add_02ed600f56cb4731a49c72d681944509_Out_2_Float, _Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float);
            float _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float;
            Unity_Step_float(float(0.4), _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float);
            float _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float;
            Unity_Subtract_float(_Step_5d570c144a8c4bb89d0baa5e4245c298_Out_2_Float, _Step_f53f84242f5743dcbe9e11f98497c324_Out_2_Float, _Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float);
            float _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float;
            Unity_Clamp_float(_Subtract_dfedf53bdf3541d1871f96504afc7ae7_Out_2_Float, float(0), float(1), _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float);
            float _Subtract_ced5ff4c16c041d48bfdf75c32fa2f1d_Out_2_Float;
            Unity_Subtract_float(_SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float, _Subtract_ced5ff4c16c041d48bfdf75c32fa2f1d_Out_2_Float);
            float _Step_9abe0aad0dcc4d8cbd96f299e0151e86_Out_2_Float;
            Unity_Step_float(float(0), _Subtract_ced5ff4c16c041d48bfdf75c32fa2f1d_Out_2_Float, _Step_9abe0aad0dcc4d8cbd96f299e0151e86_Out_2_Float);
            float4 _Multiply_bacd830cc1ce456dbdc81d82d77a33a9_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_RGBA_0_Vector4, (_Step_9abe0aad0dcc4d8cbd96f299e0151e86_Out_2_Float.xxxx), _Multiply_bacd830cc1ce456dbdc81d82d77a33a9_Out_2_Vector4);
            float4 _Property_e04ad3080dba4e7286c47eccf6bb029e_Out_0_Vector4 = _OutlineColor;
            float4 _Multiply_9b815cedb3114644a0a83a5233668176_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float.xxxx), _Property_e04ad3080dba4e7286c47eccf6bb029e_Out_0_Vector4, _Multiply_9b815cedb3114644a0a83a5233668176_Out_2_Vector4);
            float4 _Add_c158d86a66754802a15e7b297b5de109_Out_2_Vector4;
            Unity_Add_float4(_Multiply_bacd830cc1ce456dbdc81d82d77a33a9_Out_2_Vector4, _Multiply_9b815cedb3114644a0a83a5233668176_Out_2_Vector4, _Add_c158d86a66754802a15e7b297b5de109_Out_2_Vector4);
            float _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            Unity_Add_float(_Clamp_1b865bcbc2494db0adabaf5b734875e0_Out_3_Float, _SampleTexture2D_7aa9fc35d38e4028a2b4db023a29d09b_A_7_Float, _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float);
            surface.BaseColor = (_Add_c158d86a66754802a15e7b297b5de109_Out_2_Vector4.xyz);
            surface.Alpha = _Add_6786d7a24091457e931dd4b8196886a8_Out_2_Float;
            surface.AlphaClipThreshold = float(0);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}