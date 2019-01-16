// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TipToeThiefShader" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Contrast("Contrast", float) = 1
	}
	SubShader {
		Pass {
			ZTest Always Cull Off Zwrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			half _Contrast;
			sampler2D _MainTex;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			// vertex shader
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return saturate(col * 1 / _Contrast);
			}
			ENDCG
		}
	}
	FallBack Off
}
