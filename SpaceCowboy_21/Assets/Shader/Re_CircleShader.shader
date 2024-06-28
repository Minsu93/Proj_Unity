Shader "Unlit/Re_CircleShader"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _Radius("Radius", Float) = 0.4
        _LineWidth("LineWidth", Float) = 4
        [HDR]_Color("Color", Color) = (4, 0.5490196, 0.5490196, 0)
        _Segment_Spacing("Segment Spacing", Float) = 0.001
        _SegmentCount("SegmentCount", Float) = 2
        _Percent("Percent", Range(0, 1)) = 1
        _AngleMin("AngleMin", Float) = 0.5
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
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
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
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
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float3 interp3 : INTERP3;
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
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyz =  input.viewDirectionWS;
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
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.texCoord0 = input.interp2.xyzw;
            output.viewDirectionWS = input.interp3.xyz;
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
        float _Radius;
        float _LineWidth;
        float4 _Color;
        float _Segment_Spacing;
        float _SegmentCount;
        float _Percent;
        float _AngleMin;
        CBUFFER_END
        
        // Object and Global properties
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
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_DDXY_0b58f6cba00e4f1d80e1ad975904fb3b_float(float In, out float Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING)
                    #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = abs(ddx(In)) + abs(ddy(In));
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
        }
        
        void Unity_Arctangent2_float(float A, float B, out float Out)
        {
            Out = atan2(A, B);
        }
        
        void Unity_DDXY_77f022a62f4c46a8b7c37eb2524f5551_float(float In, out float Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING)
                    #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = abs(ddx(In)) + abs(ddy(In));
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_DDXY_00f9921fbca8456fa2dede7ec591c5f5_float(float In, out float Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING)
                    #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = abs(ddx(In)) + abs(ddy(In));
        }
        
        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_DDXY_808906106b144ebf940c396e8e2ab27d_float(float In, out float Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING)
                    #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = abs(ddx(In)) + abs(ddy(In));
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_b85b1d12d6774ebb90eb36b2d214d8d3_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
            float2 _TilingAndOffset_6da8d5888dc54fad865685ad921486b8_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (-0.5, -0.5), _TilingAndOffset_6da8d5888dc54fad865685ad921486b8_Out_3);
            float _Length_52b2b7e4fa364580809b078ec5e8dbc6_Out_1;
            Unity_Length_float2(_TilingAndOffset_6da8d5888dc54fad865685ad921486b8_Out_3, _Length_52b2b7e4fa364580809b078ec5e8dbc6_Out_1);
            float _Property_90bdfe2e843a43c3bfbb3dcac22825b3_Out_0 = _Radius;
            float _Subtract_297bf831576a46c6b797916973300e89_Out_2;
            Unity_Subtract_float(_Length_52b2b7e4fa364580809b078ec5e8dbc6_Out_1, _Property_90bdfe2e843a43c3bfbb3dcac22825b3_Out_0, _Subtract_297bf831576a46c6b797916973300e89_Out_2);
            float _Absolute_eacc0481fc0e4537b435402a0167b446_Out_1;
            Unity_Absolute_float(_Subtract_297bf831576a46c6b797916973300e89_Out_2, _Absolute_eacc0481fc0e4537b435402a0167b446_Out_1);
            float _Property_49dc806338a3494f8bdca6b71f64ab25_Out_0 = _LineWidth;
            float _Divide_06e43c792161461a9261fc4eced05172_Out_2;
            Unity_Divide_float(_Property_49dc806338a3494f8bdca6b71f64ab25_Out_0, 100, _Divide_06e43c792161461a9261fc4eced05172_Out_2);
            float _Subtract_13e0aa5378434192abfd4c9ea2158df8_Out_2;
            Unity_Subtract_float(_Absolute_eacc0481fc0e4537b435402a0167b446_Out_1, _Divide_06e43c792161461a9261fc4eced05172_Out_2, _Subtract_13e0aa5378434192abfd4c9ea2158df8_Out_2);
            float _DDXY_0b58f6cba00e4f1d80e1ad975904fb3b_Out_1;
            Unity_DDXY_0b58f6cba00e4f1d80e1ad975904fb3b_float(_Subtract_13e0aa5378434192abfd4c9ea2158df8_Out_2, _DDXY_0b58f6cba00e4f1d80e1ad975904fb3b_Out_1);
            float _Divide_a794623583814709965d493065f0c1af_Out_2;
            Unity_Divide_float(_Subtract_13e0aa5378434192abfd4c9ea2158df8_Out_2, _DDXY_0b58f6cba00e4f1d80e1ad975904fb3b_Out_1, _Divide_a794623583814709965d493065f0c1af_Out_2);
            float _OneMinus_212ace3b62f044a1b644a8e043a285f5_Out_1;
            Unity_OneMinus_float(_Divide_a794623583814709965d493065f0c1af_Out_2, _OneMinus_212ace3b62f044a1b644a8e043a285f5_Out_1);
            float _Clamp_e17382b3d61a4d669a202f27e6af98f0_Out_3;
            Unity_Clamp_float(_OneMinus_212ace3b62f044a1b644a8e043a285f5_Out_1, 0, 1, _Clamp_e17382b3d61a4d669a202f27e6af98f0_Out_3);
            float _Property_0e700b648c3547d2902cc6f376f0edc2_Out_0 = _AngleMin;
            float _Add_b2579047581d4cb78c1dff64415e1316_Out_2;
            Unity_Add_float(_Property_0e700b648c3547d2902cc6f376f0edc2_Out_0, 0.01, _Add_b2579047581d4cb78c1dff64415e1316_Out_2);
            float _Multiply_46b2c34f6362478fba2db118a1a99432_Out_2;
            Unity_Multiply_float_float(_Add_b2579047581d4cb78c1dff64415e1316_Out_2, 6.283185, _Multiply_46b2c34f6362478fba2db118a1a99432_Out_2);
            float _Float_3ca26f18db83411a9386c863fe736ee7_Out_0 = 0.25;
            float _Multiply_3e571002d38741c3ae20dde29f66c00a_Out_2;
            Unity_Multiply_float_float(3.141593, _Float_3ca26f18db83411a9386c863fe736ee7_Out_0, _Multiply_3e571002d38741c3ae20dde29f66c00a_Out_2);
            float2 _Rotate_130d65d828b64b058caaff773b76b002_Out_3;
            Unity_Rotate_Radians_float(_TilingAndOffset_6da8d5888dc54fad865685ad921486b8_Out_3, float2 (0, 0), _Multiply_3e571002d38741c3ae20dde29f66c00a_Out_2, _Rotate_130d65d828b64b058caaff773b76b002_Out_3);
            float _Split_b849b2293e0c48369b3ff63884f727d5_R_1 = _Rotate_130d65d828b64b058caaff773b76b002_Out_3[0];
            float _Split_b849b2293e0c48369b3ff63884f727d5_G_2 = _Rotate_130d65d828b64b058caaff773b76b002_Out_3[1];
            float _Split_b849b2293e0c48369b3ff63884f727d5_B_3 = 0;
            float _Split_b849b2293e0c48369b3ff63884f727d5_A_4 = 0;
            float _Arctangent2_a6cc6dfeeec44d5592a74aa0f822957b_Out_2;
            Unity_Arctangent2_float(_Split_b849b2293e0c48369b3ff63884f727d5_R_1, _Split_b849b2293e0c48369b3ff63884f727d5_G_2, _Arctangent2_a6cc6dfeeec44d5592a74aa0f822957b_Out_2);
            float _Add_ff9c0225df034c3bb51d7c703ca8118e_Out_2;
            Unity_Add_float(_Arctangent2_a6cc6dfeeec44d5592a74aa0f822957b_Out_2, 3.141593, _Add_ff9c0225df034c3bb51d7c703ca8118e_Out_2);
            float _Subtract_01a4960987f24fd08c665893938c7584_Out_2;
            Unity_Subtract_float(_Multiply_46b2c34f6362478fba2db118a1a99432_Out_2, _Add_ff9c0225df034c3bb51d7c703ca8118e_Out_2, _Subtract_01a4960987f24fd08c665893938c7584_Out_2);
            float _DDXY_77f022a62f4c46a8b7c37eb2524f5551_Out_1;
            Unity_DDXY_77f022a62f4c46a8b7c37eb2524f5551_float(_Subtract_01a4960987f24fd08c665893938c7584_Out_2, _DDXY_77f022a62f4c46a8b7c37eb2524f5551_Out_1);
            float _Divide_b32af70c108443f684b294da93cf5cc2_Out_2;
            Unity_Divide_float(_Subtract_01a4960987f24fd08c665893938c7584_Out_2, _DDXY_77f022a62f4c46a8b7c37eb2524f5551_Out_1, _Divide_b32af70c108443f684b294da93cf5cc2_Out_2);
            float _Clamp_8f2909a7917947cca71a2b8183d6fe1f_Out_3;
            Unity_Clamp_float(_Divide_b32af70c108443f684b294da93cf5cc2_Out_2, 0, 1, _Clamp_8f2909a7917947cca71a2b8183d6fe1f_Out_3);
            float _Add_b92b64dfe86f4ea2ad0f7fe07b8919f7_Out_2;
            Unity_Add_float(_Property_0e700b648c3547d2902cc6f376f0edc2_Out_0, 0.25, _Add_b92b64dfe86f4ea2ad0f7fe07b8919f7_Out_2);
            float _Subtract_9227f0849811417da7db40a5fc1c2c4b_Out_2;
            Unity_Subtract_float(_Add_b92b64dfe86f4ea2ad0f7fe07b8919f7_Out_2, 0.01, _Subtract_9227f0849811417da7db40a5fc1c2c4b_Out_2);
            float _Property_b2b30b7610d149e599c57b24109ac4c9_Out_0 = _Percent;
            float _Lerp_df3d3da7a62a488f80138f0a1fcb084e_Out_3;
            Unity_Lerp_float(_Add_b2579047581d4cb78c1dff64415e1316_Out_2, _Subtract_9227f0849811417da7db40a5fc1c2c4b_Out_2, _Property_b2b30b7610d149e599c57b24109ac4c9_Out_0, _Lerp_df3d3da7a62a488f80138f0a1fcb084e_Out_3);
            float _Multiply_eae5e241cd3a4944830fcedd7750b8a6_Out_2;
            Unity_Multiply_float_float(_Lerp_df3d3da7a62a488f80138f0a1fcb084e_Out_3, 6.283185, _Multiply_eae5e241cd3a4944830fcedd7750b8a6_Out_2);
            float _Subtract_13f72e58efcb4a8398a78ef2ce7f9188_Out_2;
            Unity_Subtract_float(_Multiply_eae5e241cd3a4944830fcedd7750b8a6_Out_2, _Add_ff9c0225df034c3bb51d7c703ca8118e_Out_2, _Subtract_13f72e58efcb4a8398a78ef2ce7f9188_Out_2);
            float _DDXY_00f9921fbca8456fa2dede7ec591c5f5_Out_1;
            Unity_DDXY_00f9921fbca8456fa2dede7ec591c5f5_float(_Subtract_13f72e58efcb4a8398a78ef2ce7f9188_Out_2, _DDXY_00f9921fbca8456fa2dede7ec591c5f5_Out_1);
            float _Divide_1b1df37db3564a84bc3dfea9e1969047_Out_2;
            Unity_Divide_float(_Subtract_13f72e58efcb4a8398a78ef2ce7f9188_Out_2, _DDXY_00f9921fbca8456fa2dede7ec591c5f5_Out_1, _Divide_1b1df37db3564a84bc3dfea9e1969047_Out_2);
            float _Clamp_dde8ac035de14722a741701f52b74e08_Out_3;
            Unity_Clamp_float(_Divide_1b1df37db3564a84bc3dfea9e1969047_Out_2, 0, 1, _Clamp_dde8ac035de14722a741701f52b74e08_Out_3);
            float _OneMinus_56dfdc16ad3b4bfcac480b891c49f62e_Out_1;
            Unity_OneMinus_float(_Clamp_dde8ac035de14722a741701f52b74e08_Out_3, _OneMinus_56dfdc16ad3b4bfcac480b891c49f62e_Out_1);
            float _Clamp_ca1fe5d305554e1cb1c6a05d1aba4673_Out_3;
            Unity_Clamp_float(_OneMinus_56dfdc16ad3b4bfcac480b891c49f62e_Out_1, 0, 1, _Clamp_ca1fe5d305554e1cb1c6a05d1aba4673_Out_3);
            float _Add_9d97fab2b4f04ff6b38e8f48ba83cfab_Out_2;
            Unity_Add_float(_Clamp_8f2909a7917947cca71a2b8183d6fe1f_Out_3, _Clamp_ca1fe5d305554e1cb1c6a05d1aba4673_Out_3, _Add_9d97fab2b4f04ff6b38e8f48ba83cfab_Out_2);
            float _Property_f2b0e8e229b94ff3bd423bbab27afc14_Out_0 = _SegmentCount;
            float _Multiply_1b9768374b8e433890edbb6e7736a5e6_Out_2;
            Unity_Multiply_float_float(_Property_f2b0e8e229b94ff3bd423bbab27afc14_Out_0, 4, _Multiply_1b9768374b8e433890edbb6e7736a5e6_Out_2);
            float _Divide_b71e4770efd344088fd0c4b4cf4e6722_Out_2;
            Unity_Divide_float(6.283185, _Multiply_1b9768374b8e433890edbb6e7736a5e6_Out_2, _Divide_b71e4770efd344088fd0c4b4cf4e6722_Out_2);
            float _Divide_c7047e6fa5b34f64b7c0fa5bb753cda2_Out_2;
            Unity_Divide_float(_Divide_b71e4770efd344088fd0c4b4cf4e6722_Out_2, 2, _Divide_c7047e6fa5b34f64b7c0fa5bb753cda2_Out_2);
            float _Add_a06bae32af994642987c19d9aa5a06bb_Out_2;
            Unity_Add_float(_Add_ff9c0225df034c3bb51d7c703ca8118e_Out_2, _Divide_c7047e6fa5b34f64b7c0fa5bb753cda2_Out_2, _Add_a06bae32af994642987c19d9aa5a06bb_Out_2);
            float _Modulo_406bf86f45f94f928ce29bad064ec18e_Out_2;
            Unity_Modulo_float(_Add_a06bae32af994642987c19d9aa5a06bb_Out_2, _Divide_b71e4770efd344088fd0c4b4cf4e6722_Out_2, _Modulo_406bf86f45f94f928ce29bad064ec18e_Out_2);
            float _Subtract_6a14a1c97c8c43dca4900f9f339e9736_Out_2;
            Unity_Subtract_float(_Modulo_406bf86f45f94f928ce29bad064ec18e_Out_2, _Divide_c7047e6fa5b34f64b7c0fa5bb753cda2_Out_2, _Subtract_6a14a1c97c8c43dca4900f9f339e9736_Out_2);
            float _Sine_2401efeff2004fe3ba83f6daca132057_Out_1;
            Unity_Sine_float(_Subtract_6a14a1c97c8c43dca4900f9f339e9736_Out_2, _Sine_2401efeff2004fe3ba83f6daca132057_Out_1);
            float _Absolute_b29801d262c24e93b1bfef9e64890af1_Out_1;
            Unity_Absolute_float(_Sine_2401efeff2004fe3ba83f6daca132057_Out_1, _Absolute_b29801d262c24e93b1bfef9e64890af1_Out_1);
            float2 _TilingAndOffset_a41310c625644f9cb0a03ca00d127054_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (-0.5, -0.5), _TilingAndOffset_a41310c625644f9cb0a03ca00d127054_Out_3);
            float _Length_a36b1ff5a2fb42a8ad0c69d0a7de104e_Out_1;
            Unity_Length_float2(_TilingAndOffset_a41310c625644f9cb0a03ca00d127054_Out_3, _Length_a36b1ff5a2fb42a8ad0c69d0a7de104e_Out_1);
            float _Multiply_f8ac80c7f4674f5396de839d429a61a6_Out_2;
            Unity_Multiply_float_float(_Absolute_b29801d262c24e93b1bfef9e64890af1_Out_1, _Length_a36b1ff5a2fb42a8ad0c69d0a7de104e_Out_1, _Multiply_f8ac80c7f4674f5396de839d429a61a6_Out_2);
            float _Property_65e840039abe470ca3580b7513dd2061_Out_0 = _Segment_Spacing;
            float _Subtract_273314d094dd4de695b223027077035f_Out_2;
            Unity_Subtract_float(_Multiply_f8ac80c7f4674f5396de839d429a61a6_Out_2, _Property_65e840039abe470ca3580b7513dd2061_Out_0, _Subtract_273314d094dd4de695b223027077035f_Out_2);
            float _DDXY_808906106b144ebf940c396e8e2ab27d_Out_1;
            Unity_DDXY_808906106b144ebf940c396e8e2ab27d_float(_Subtract_273314d094dd4de695b223027077035f_Out_2, _DDXY_808906106b144ebf940c396e8e2ab27d_Out_1);
            float _Divide_9e15fe3be2ae4dd5bfb72c3dfd4f0a28_Out_2;
            Unity_Divide_float(_Subtract_273314d094dd4de695b223027077035f_Out_2, _DDXY_808906106b144ebf940c396e8e2ab27d_Out_1, _Divide_9e15fe3be2ae4dd5bfb72c3dfd4f0a28_Out_2);
            float _OneMinus_b9a9d59238a94505848cd90d9d006042_Out_1;
            Unity_OneMinus_float(_Divide_9e15fe3be2ae4dd5bfb72c3dfd4f0a28_Out_2, _OneMinus_b9a9d59238a94505848cd90d9d006042_Out_1);
            float _Clamp_3131875833d0441ebb91a63f0db4c827_Out_3;
            Unity_Clamp_float(_OneMinus_b9a9d59238a94505848cd90d9d006042_Out_1, 0, 1, _Clamp_3131875833d0441ebb91a63f0db4c827_Out_3);
            float _Add_c40339f8fa0f48c8a8eb88987acf5e9f_Out_2;
            Unity_Add_float(_Add_9d97fab2b4f04ff6b38e8f48ba83cfab_Out_2, _Clamp_3131875833d0441ebb91a63f0db4c827_Out_3, _Add_c40339f8fa0f48c8a8eb88987acf5e9f_Out_2);
            float _Subtract_2529c44f588245cb88301b5bb7dd84d2_Out_2;
            Unity_Subtract_float(_Clamp_e17382b3d61a4d669a202f27e6af98f0_Out_3, _Add_c40339f8fa0f48c8a8eb88987acf5e9f_Out_2, _Subtract_2529c44f588245cb88301b5bb7dd84d2_Out_2);
            float _Clamp_8a1270b69025429780e8b45149c3e9fa_Out_3;
            Unity_Clamp_float(_Subtract_2529c44f588245cb88301b5bb7dd84d2_Out_2, 0, 1, _Clamp_8a1270b69025429780e8b45149c3e9fa_Out_3);
            float4 _Add_19282934efc249a383e93bd9d0135318_Out_2;
            Unity_Add_float4(_Property_b85b1d12d6774ebb90eb36b2d214d8d3_Out_0, (_Clamp_8a1270b69025429780e8b45149c3e9fa_Out_3.xxxx), _Add_19282934efc249a383e93bd9d0135318_Out_2);
            surface.BaseColor = (_Add_19282934efc249a383e93bd9d0135318_Out_2.xyz);
            surface.Alpha = _Clamp_8a1270b69025429780e8b45149c3e9fa_Out_3;
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
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}
