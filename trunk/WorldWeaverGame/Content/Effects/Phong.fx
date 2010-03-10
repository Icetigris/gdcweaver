//=============================================================================
// Phong.fx by Wallace Brown (C) 2009 All Rights Reserved.
//
// Impliments PerPixel Lighting w/ Texturing.
//	Notes:
//		A Monolithic effect. Performs other non PostProcess effects through booleans
//			and additional parameters.
//=============================================================================

//==========
// Globals
//==========

//basic values
uniform extern float4x4 gWorld;
uniform extern float4x4 gWIT;
uniform extern float4x4 gWVP;

//lighting values
uniform extern float4 gDiffuseMtrl;
uniform extern float4 gDiffuseLight;
uniform extern float4 gAmbientMtrl;
uniform extern float4 gAmbientLight;
uniform extern float4 gSpecularMtrl;
uniform extern float4 gSpecularLight;
uniform extern float  gSpecularPower;

//texture
uniform extern texture gTex0;

//additional lighting values
uniform extern float3 gLightVecW;
uniform extern float3 gEyePosW;

//color shift values
	//uses gDiffuseMtrl for color base
uniform extern bool	  gColorShift;
uniform extern float  gTime;

//blob values
uniform extern bool	  gBlob;
uniform extern float  gBlobStrecth;

//glow values
uniform extern bool   gGlow;
uniform extern float  gInflation;
uniform extern float  gGlowExp;
uniform extern float4x4 gViewInv;

//rotate values
uniform extern bool	  gRotate;
uniform extern float  gRotTorque;
uniform extern int	  gRotAxis;

//==================
//	Texture Sampler
//==================
sampler TexS = sampler_state
{
	Texture = <gTex0>;
	MinFilter = Anisotropic;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	MaxAnisotropy = 8;
	AddressU  = WRAP;
    AddressV  = WRAP;
};

//========================
// VS/PS structures
//========================

/* Data from application vertex buffer */
struct appdata{
	float3 posL		: POSITION0;
	float3 normalL	: NORMAL0;
	float2 tex0		: TEXCOORD0;
};

/* data passed from vertex shader to pixel shader */
struct vertOut{
    float4 posH    : POSITION0;
    float2 tex0    : TEXCOORD0;
    float3 normalW : TEXCOORD1;
    float3 posW    : TEXCOORD2;
    float3 viewW   : TEXCOORD3;
};

//============
// Utility Methods
//============
float ClampedSin(float ceil, float floor, float inNum, float offset){
	float range = sin(gTime) * offset;
	if(range >= ceil){
		inNum = ceil;
	}
	else if(range <= floor){
		inNum = floor;
	}
	else{
		inNum = range;
	}
	return inNum;
}

float Clamp(float clamp, float num){
	if(num <= clamp){
		num = clamp;
	}
	return num;
}

//============================
//	Additional Effect Methods
//============================
float3 ColorShift(){
	float3 outColor;
	outColor.r = ClampedSin(1.0f,0.1f,gDiffuseMtrl.r,0.0f);
    outColor.g = ClampedSin(1.0f,0.1f,gDiffuseMtrl.g,0.0f);
    outColor.b = ClampedSin(1.0f,0.1f,gDiffuseMtrl.b,0.0f);
    return outColor;
}

float3 Blob(float3 position, float3 normal, float stretch){
	float angle=(gTime%360)*2;
    float freqx = 1.0f+sin(gTime)*4.0f;
    float freqy = 1.0f+sin(gTime*1.3f)*4.0f;
    float freqz = 1.0f+sin(gTime*1.1f)*4.0f;
    float amp = 1.0f+sin(gTime*1.4)*stretch;
   
    float f = sin(normal.x*freqx + gTime) * sin(normal.y*freqy + gTime) * sin(normal.z*freqz + gTime);
    position.z += normal.z * amp * f;
    position.x += normal.x * amp * f;
    position.y += normal.y * amp * f;
	
	return position;
}

float4 Rotate(float4 inPos,float torque,int axis){
	float4 outPos = inPos;
	torque *= 0.05f;
	
	if(axis == 'x'){
		float3x3 xRot;
		xRot[0] = float3(1.0f,0.0f,0.0f);
		xRot[1] = float3(0.0f,cos(torque),sin(torque));
		xRot[2] = float3(0.0f,-sin(torque),cos(torque));
		outPos.x = (xRot[0][0]*inPos.x)+(xRot[1][0]*inPos.y)+(xRot[2][0]*inPos.z);
		outPos.y = (xRot[0][1]*inPos.x)+(xRot[1][1]*inPos.y)+(xRot[2][1]*inPos.z);
		outPos.z = (xRot[0][2]*inPos.x)+(xRot[1][2]*inPos.y)+(xRot[2][2]*inPos.z);
		return outPos;
	}
	else if(axis == 'y'){
		float3x3 yRot;
		yRot[0] = float3(cos(torque),0.0f,-sin(torque));
		yRot[1] = float3(0.0f,1.0f,0.0f);
		yRot[2] = float3(sin(torque),0.0f,cos(torque));
		outPos.x = (yRot[0][0]*inPos.x)+(yRot[1][0]*inPos.y)+(yRot[2][0]*inPos.z);
		outPos.y = (yRot[0][1]*inPos.x)+(yRot[1][1]*inPos.y)+(yRot[2][1]*inPos.z);
		outPos.z = (yRot[0][2]*inPos.x)+(yRot[1][2]*inPos.y)+(yRot[2][2]*inPos.z);
		return outPos;
	}
	else if(axis == 'z'){
		float3x3 zRot;
		zRot[0] = float3(cos(torque),sin(torque),0.0f);
		zRot[1] = float3(-sin(torque),cos(torque),0.0f);
		zRot[2] = float3(0.0f,0.0f,1.0f);
		outPos.x = (zRot[0][0]*inPos.x)+(zRot[1][0]*inPos.y)+(zRot[2][0]*inPos.z);
		outPos.y = (zRot[0][1]*inPos.x)+(zRot[1][1]*inPos.y)+(zRot[2][1]*inPos.z);
		outPos.z = (zRot[0][2]*inPos.x)+(zRot[1][2]*inPos.y)+(zRot[2][2]*inPos.z);
		return outPos;
	}
	else{
		return inPos;
	}
	
}

//========================
// VertexShaders
//========================
vertOut PhongVS(appdata IN)
{
	vertOut vOut = (vertOut)0;
	vOut.normalW = mul(float4(IN.normalL, 0.0f), gWIT).xyz;
	vOut.normalW = normalize(vOut.normalW);
	vOut.posW  = mul(float4(IN.posL, 1.0f), gWorld).xyz;
	vOut.posH = mul(float4(IN.posL, 1.0f), gWVP);
	vOut.tex0 = IN.tex0 * 0.1f;
	
	if(gBlob){
		vOut.posH.xyz = Blob(vOut.posH,vOut.normalW,20.0f);
	}
	if(gRotate){
		vOut.posH = mul(Rotate(float4(IN.posL,1.0f),gTime,gRotAxis), gWVP);
		vOut.normalW = mul(Rotate(float4(IN.normalL,1.0f),gTime,gRotAxis), gWIT).xyz;
	}
	
    return vOut;
}

vertOut GlowVS(appdata IN){
	vertOut vOut = (vertOut)0;

	vOut.normalW = mul(float4(IN.normalL, 0.0f), gWIT).xyz;
	vOut.normalW = normalize(vOut.normalW);
	
	
	//---------------------
	float4 Po = float4(IN.posL.xyz,1.0f);
	if(gRotate){
		Po = Rotate(float4(IN.posL,1.0f),gTime,gRotAxis);
	}
    Po += (ClampedSin(9.0f,0.5f,0.0f,gInflation)*normalize(float4(IN.normalL.xyz,0.0f)));
    float4 Pw = mul(Po,gWorld);
    vOut.viewW = normalize(gViewInv[3].xyz - Pw.xyz);
    vOut.posH = mul(Po,gWVP);
	vOut.tex0 = IN.tex0 * 0.1f;
	//---------------------
   
	
	if(gBlob){
		vOut.posH.xyz = Blob(vOut.posH,vOut.normalW,20.0f);
	}

	return vOut;	
}

//========================
// PixelShaders
//========================
float4 PhongPS(vertOut IN) : COLOR
{
	IN.normalW = normalize(IN.normalW);
	//=======================================================
	// Lighting

	float3 toEye = normalize(gEyePosW - IN.posW);
	float3 r = reflect(-gLightVecW, IN.normalW);
	float t  = pow(max(dot(r, toEye), 0.0f), gSpecularPower);
	float s = max(dot(gLightVecW, IN.normalW), 0.0f);

	float3 spec = t*(gSpecularMtrl*gSpecularLight).rgb;
	float3 diffuse = s*(gDiffuseMtrl*gDiffuseLight).rgb;
	float3 ambient = gAmbientMtrl*gAmbientLight;

	//=======================================================

	float3 texColor = tex2D(TexS,IN.tex0).rgb;
	
	float clamp = 0.35f;
	diffuse.r = Clamp(clamp,diffuse.r);
	diffuse.g = Clamp(clamp,diffuse.g);
	diffuse.b = Clamp(clamp,diffuse.b);
	
	float3 diffuseTex = diffuse.rgb * texColor;
	
	if(gColorShift){
		return float4((ColorShift().rgb) + ambient + spec, gDiffuseMtrl.a);
	}
	else{
		return float4(diffuseTex + ambient + spec, gDiffuseMtrl.a);
	}
	
}

float4 GlowPS(vertOut IN) : COLOR
{
	float3 normal = normalize(IN.normalW);
    float3 view = normalize(IN.viewW);
    float edge = 1.0 - dot(normal,view);
    edge = pow(edge,gGlowExp);
    float3 result = edge * gDiffuseMtrl.rgb;
    return float4(result,edge);
}

technique Main
{
    pass P0
    {
        vertexShader = compile vs_3_0 PhongVS();
        pixelShader  = compile ps_3_0 PhongPS();
		CullMode = CCW;
    }
}

technique Glow
{
    pass P0
    {	
        vertexShader = compile vs_3_0 PhongVS();
        pixelShader  = compile ps_3_0 PhongPS();
		CullMode = CCW;
    }
    pass P1
    {
        vertexShader = compile vs_3_0 GlowVS();
        pixelShader  = compile ps_3_0 GlowPS();
        ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		CullMode = CW;
    }
}
