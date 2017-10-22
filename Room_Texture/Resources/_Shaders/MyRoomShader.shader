Shader "Unlit/MyRoomShader"
{
	Properties
	{
		_MyArr("TextureArray", 2DArray) = "" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma target 3.5 // only Unity 3.5 and up can have texture arrays passed in
			#include "UnityCG.cginc"
			#define MAX_SIZE 30
			#pragma only_renderers d3d11 gles3 // https://docs.unity3d.com/Manual/SL-ShaderPrograms.html

			struct appdata
			{
				float4 vertex : POSITION;
				//float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 worldSpace : TEXCOORD0;
			};

			float4x4 _MyObjectToWorld;
			float4x4 _WorldToCameraMatrixArray[MAX_SIZE];
			float4x4 _CameraProjectionMatrixArray[MAX_SIZE];
			float4 _VertexInProjectionSpaceArray[MAX_SIZE];

			float4 vertexPositionInCameraSpaceArray[MAX_SIZE];

			v2f vert (appdata v)
			{
				v2f o;
				// convert vertex position to projection space
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldSpace = mul(_MyObjectToWorld, float4(v.vertex.xyz, 1.0f));//float4(v.vertex.xyz, 1); // form a vertex as a point, not a direction

				//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			UNITY_DECLARE_TEX2DARRAY(_MyArr);

			fixed4 frag (v2f i) : SV_Target
			{
				// Calculate teh image uv, world coordinate->camera coordinates
				for (int k = 0; k < MAX_SIZE; ++k) {
					vertexPositionInCameraSpaceArray[k] = mul(_WorldToCameraMatrixArray[k], float4(i.worldSpace.xyz, 1));
					_VertexInProjectionSpaceArray[k] = mul(_CameraProjectionMatrixArray[k], float4(vertexPositionInCameraSpaceArray[k].xyz, 1.0));
				}
				for (int j = 0; j < MAX_SIZE; ++j) {
					if (vertexPositionInCameraSpaceArray[j].z > 0)
						continue;

					float2 projectedTex = (_VertexInProjectionSpaceArray[j].xy / _VertexInProjectionSpaceArray[j].w);
					float xCut = 1.0 - ((1280.0 - 1024.0) / 1280.0);
					float yCut = 1.0 - ((720.0 - 512.0) / 720.0);

					if (abs(projectedTex.x) < xCut && abs(projectedTex.y) < yCut) {
						float2 unitTexCoord = ((projectedTex * 0.5) + float4(0.5, 0.5, 0, 0));

						unitTexCoord.x = ((unitTexCoord.x - 0.5) / xCut) + 0.5;
						unitTexCoord.y = ((unitTexCoord.y - 0.5) / yCut) + 0.5;

						float3 zAxis = { unitTexCoord.x, unitTexCoord.y, j };

						return UNITY_SAMPLE_TEX2DARRAY(_MyArr, zAxis);
					}
					else
						continue;
				}

				return fixed4(0, 0, 0, 0);
				//// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				//// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				//return col;
			}
			ENDCG
		}
	}
}
