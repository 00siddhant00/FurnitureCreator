Shader "Unlit/GradientRawImage"
{
	Properties
	{
		_ColorA("Color A", Color) = (1,1,1,1)
		_ColorB("Color B", Color) = (1,1,1,1)
		_ColorC("Color C", Color) = (1,1,1,1)
		_ColorD("Color D", Color) = (1,1,1,1)
		_ColorStart("Color Start", Range(0,1)) = 1
		_ColorEnd("Color End", Range(0,1)) = 0
		_Type("Gradient Type", Range(0,5)) = 1
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 _ColorA;
			float4 _ColorB;
			float _ColorStart;
			float _ColorEnd;
			float _Type;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.texcoord;
				return OUT;
			}

			float InverseLerp(float a, float b, float v)
			{
				return (v - a) / (b - a);
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 outColor;

				if (_Type < 1)
				{
					outColor = _ColorA;
				}
				else if (_Type >= 1 && _Type < 2)
				{
					float t = saturate(InverseLerp(_ColorStart, _ColorEnd, i.uv.x));
					outColor = lerp(_ColorA, _ColorB, t);
				}
				else if (_Type >= 2 && _Type < 3)
				{
					float t = saturate(InverseLerp(_ColorStart, _ColorEnd, i.uv.y));
					outColor = lerp(_ColorA, _ColorB, t);
				}
				else if (_Type >= 3 && _Type < 4)
				{
					outColor = _ColorB;
				}

				return outColor;
			}
			ENDCG
		}
	}
}
