//=============================================================================
// Phong.fx by Wallace Brown <2010>
//
// Impliments PerPixel Lighting w/ Texturing.
//	Notes:
//		A Monolithic effect. Performs other non PostProcess effects.
//=============================================================================

//==========
// Globals
//==========

//globals
#define PI 3.1415926535f
#define planetScale 1.0f
#define planetSize 8.0f
#define starSize 8.0f
#define maxLights 3
#define attenMax 500.0f
#define attenFactor 0.002f
#define distanceScaleMainLight 0.00000002f;
#define distanceScale 0.00002f;

//basic values
uniform extern float4x4 gWorld;
uniform extern float4x4 gWIT;
uniform extern float4x4 gWInv;
uniform extern float4x4 gWVP;

//lighting values
uniform extern float4 gDiffuseMtrl;
uniform extern float4 gDiffuseLight;
uniform extern float4 gAmbientMtrl;
uniform extern float4 gAmbientLight;
uniform extern float4 gSpecularMtrl;
uniform extern float4 gSpecularLight;
uniform extern float  gSpecularPower;

//multiple light system
uniform extern int gNumLights; 
float3 gLightPos_multiple[maxLights];

float4 gDiffuseMtrl_multiple[maxLights];
float4 gDiffuseLight_multiple[maxLights];

float4 gSpecularMtrl_multiple[maxLights];
float4 gSpecularLight_multiple[maxLights];

uniform extern float3 gLightPos_multiple_1;
uniform extern float4 gDiffuseMtrl_multiple_1;
uniform extern float4 gDiffuseLight_multiple_1;
uniform extern float4 gSpecularMtrl_multiple_1;
uniform extern float4 gSpecularLight_multiple_1;

uniform extern float3 gLightPos_multiple_2;
uniform extern float4 gDiffuseMtrl_multiple_2;
uniform extern float4 gDiffuseLight_multiple_2;
uniform extern float4 gSpecularMtrl_multiple_2;
uniform extern float4 gSpecularLight_multiple_2;

uniform extern float3 gLightPos_multiple_3;
uniform extern float4 gDiffuseMtrl_multiple_3;
uniform extern float4 gDiffuseLight_multiple_3;
uniform extern float4 gSpecularMtrl_multiple_3;
uniform extern float4 gSpecularLight_multiple_3;

//texture
uniform extern texture gTex0;
uniform extern texture gTex1;
uniform extern texture gTex2;
uniform extern texture gTexN;
uniform extern texture gTexAnimated;

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
uniform extern float3 gGlowColor;

//rotate values
uniform extern bool	  gRotate;
uniform extern float  gRotTorque;
uniform extern int	  gRotAxis;
uniform extern float  gRotSpeed;

//GreyMap values
uniform extern bool	  gGreyMap;
uniform extern float3 gGreyMapColorA;
uniform extern float3 gGreyMapColorB;
uniform extern float3 gGreyMapColorC;

//alpha testing values
uniform extern bool	  gStratosphere;

//sprite particle values
uniform extern int	  gViewportHeight;
uniform extern float4 gSpriteAlpha;
uniform extern float3 gAccel;
uniform extern float  gAngle;
uniform extern float3 gVelocity;
uniform extern float  gGrav;
uniform extern float  gAge;
uniform extern float  gLifetime;
uniform extern float  gRandX;
uniform extern float  gRandZ;
uniform extern float  gOrbitSpeed;
uniform extern float3 gCenter;
uniform extern float3 gPlayerPos;
uniform extern float3 gDisplacement;
uniform extern float4x4 gPlayerRot;


//==================
//	Texture Samples
//==================
sampler TexS = sampler_state
{
	Texture = <gTex0>;
	MinFilter = anisotropic;
	MagFilter = linear;
	MaxAnisotropy = 8;
	MipFilter = Linear;
	AddressU  = mirror;
    AddressV  = clamp;
};

sampler TexNmap = sampler_state
{
	Texture = <gTexN>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = wrap;
    AddressV  = wrap;
};

sampler TexStratosphere = sampler_state
{
	Texture = <gTexAnimated>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
	AddressU  = wrap;
    AddressV  = wrap;
};

sampler ParticleTex = sampler_state
{
    Texture = <gTex1>;
	MinFilter = point;
	MagFilter = point;
	MipFilter = none;
	AddressU  = clamp;
    AddressV  = clamp;
}; 

sampler StarTex_Low = sampler_state
{
	Texture = <gTex0>;
	MinFilter = anisotropic;
	MagFilter = linear;
	MaxAnisotropy = 8;
	MipFilter = Linear;
	AddressU  = mirror;
    AddressV  = clamp;
};

sampler StarTex_Mid = sampler_state
{
	Texture = <gTex1>;
	MinFilter = anisotropic;
	MagFilter = linear;
	MaxAnisotropy = 8;
	MipFilter = Linear;
	AddressU  = mirror;
    AddressV  = clamp;
};

sampler StarTex_Top = sampler_state
{
	Texture = <gTex2>;
	MinFilter = anisotropic;
	MagFilter = linear;
	MaxAnisotropy = 8;
	MipFilter = Linear;
	AddressU  = wrap;
    AddressV  = wrap;
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
    float3 toEye   : TEXCOORD4;
    float3 lightDir   : TEXCOORD5;
};

/* data passed from vertex shader to pixel shader */
struct vertOut_MultipleLights{
    float4 posH    : POSITION0;
    float2 tex0    : TEXCOORD0;
    float3 normalW : TEXCOORD1;
    float3 posW    : TEXCOORD2;
    float3 toEye    : TEXCOORD3;
    float3 intensity   : TEXCOORD4;
    float3 lightDir[maxLights]   : TEXCOORD5;
};

/* data passed from vertex shader to pixel shader */
struct stratVertOut{
    float4 posH    : POSITION0;
    float2 tex0    : TEXCOORD0;
    float3 normalW : TEXCOORD1;
};

/* data passed from vertex shader to pixel shader for */
/* normal mapping. */
struct vertNormOut{
    float4 posH    : POSITION0;
    float3 tex0    : TEXCOORD0;
    float3 toEye   : TEXCOORD1;
    float3 lightDir   : TEXCOORD2;
};

struct particleVertInOut{
		float4 pos		: POSITION0;
		float4 color : COLOR0;
		float size   : PSIZE0;
};

//============
// Utility Methods
//============
float ClampedSin(float ceil, float floor, float inNum, float scale){
	float range = sin(gTime) * scale;
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

float4 Rotate(float4 inPos,float time, int axis){
	float4 outPos = inPos;
	time *= gRotSpeed;
	float sin,cos;
	
	sincos(time,sin,cos);
	
	if(axis == 'x'){
		float3x3 xRot;
		xRot[0] = float3(1.0f,0.0f,0.0f);
		xRot[1] = float3(0.0f,cos,sin);
		xRot[2] = float3(0.0f,-sin,cos);
		outPos.x = (xRot[0][0]*inPos.x)+(xRot[1][0]*inPos.y)+(xRot[2][0]*inPos.z);
		outPos.y = (xRot[0][1]*inPos.x)+(xRot[1][1]*inPos.y)+(xRot[2][1]*inPos.z);
		outPos.z = (xRot[0][2]*inPos.x)+(xRot[1][2]*inPos.y)+(xRot[2][2]*inPos.z);	
		return outPos;
	}
	else if(axis == 'y'){
		float3x3 yRot;
		yRot[0] = float3(cos,0.0f,-sin);
		yRot[1] = float3(0.0f,1.0f,0.0f);
		yRot[2] = float3(sin,0.0f,cos);
		outPos.x = (yRot[0][0]*inPos.x)+(yRot[1][0]*inPos.y)+(yRot[2][0]*inPos.z);
		outPos.y = (yRot[0][1]*inPos.x)+(yRot[1][1]*inPos.y)+(yRot[2][1]*inPos.z);
		outPos.z = (yRot[0][2]*inPos.x)+(yRot[1][2]*inPos.y)+(yRot[2][2]*inPos.z);	
		return outPos;
	}
	else if(axis == 'z'){
		float3x3 zRot;
		zRot[0] = float3(cos,sin,0.0f);
		zRot[1] = float3(-sin,cos,0.0f);
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
	vOut.posW  = mul(float4(IN.posL, 1.0f), gWorld).xyz;
	vOut.posH = mul(float4(IN.posL, 1.0f), gWVP);
	vOut.tex0 = IN.tex0;
	
	if(gBlob){
		vOut.posH.xyz = Blob(vOut.posH,vOut.normalW,20.0f);
	}
	if(gRotate){
		vOut.posH = mul(Rotate(float4(IN.posL,1.0f),gTime,gRotAxis), gWVP);
		vOut.normalW = mul(Rotate(float4(IN.normalL,1.0f),gTime,gRotAxis), gWIT).xyz ;
	}
	
    return vOut;
}

vertOut Phong_Player_VS(appdata IN)
{
	vertOut vOut = (vertOut)0;
	vOut.normalW = mul(float4(IN.normalL,1.0f), gWIT).xyz;
	vOut.posW  = mul(float4(IN.posL, 1.0f), gWorld).xyz;
	vOut.posH = mul(float4(IN.posL,1.0f), gWVP);
	vOut.tex0 = IN.tex0;
	
    return vOut;
}

vertOut_MultipleLights Phong_MultipleLights_VS(appdata IN)
{
	vertOut_MultipleLights vOut = (vertOut_MultipleLights)0;
	vOut.normalW = mul(float4(IN.normalL,1.0f), gWIT).xyz;
	vOut.posW  = mul(float4(IN.posL, 1.0f), gWorld).xyz;
	vOut.posH = mul(float4(IN.posL,1.0f), gWVP);
	vOut.tex0 = IN.tex0;
	
	if(gBlob){
		vOut.posH.xyz = Blob(vOut.posH,vOut.normalW,20.0f);
	}
	if(gRotate){
		vOut.posH = mul(Rotate(float4(IN.posL,1.0f),gTime,gRotAxis), gWVP);
		vOut.normalW = mul(Rotate(float4(IN.normalL,1.0f),gTime,gRotAxis), gWIT).xyz ;
	}
	
	float3 LightPos_multiple[maxLights];
	LightPos_multiple[0] = gLightPos_multiple_1;
	LightPos_multiple[1] = gLightPos_multiple_2;
	LightPos_multiple[2] = gLightPos_multiple_3;
	
	float distance = 0.0f;
	float x = 0.0f;
	float y = 0.0f;
	float z = 0.0f;
	
	float LightIntensity_multiple[maxLights] = {0.0f,0.0f,0.0f};

	for(int v = 0; v < maxLights; v++)
	{
		x = (LightPos_multiple[v].x - vOut.posW.x);
		y = (LightPos_multiple[v].y - vOut.posW.y);
		z = (LightPos_multiple[v].z - vOut.posW.z);
		distance = sqrt(x*x + y*y + z*z);
		distance *= distance;
		if(v == 0){
			distance *= distanceScaleMainLight;
		}
		else{
			distance *= distanceScale;
		}
		
		if(distance <= 1)
		{
			LightIntensity_multiple[v] = 1.0f;
		}
		else if(distance > 1 && distance < attenMax)
		{
			LightIntensity_multiple[v] = (attenMax - distance)*attenFactor;
		}
		else if(distance >= attenMax)
		{
			LightIntensity_multiple[v] = 0.0f;
		}
		
		//find direction of light to vertex
		x = (LightPos_multiple[v].x - vOut.posW.x);
		y = (LightPos_multiple[v].y - vOut.posW.y);
		z = (LightPos_multiple[v].z - vOut.posW.z);
		vOut.lightDir[v] = normalize(float3(x,y,z));
	}
	
	vOut.intensity.x = saturate(LightIntensity_multiple[0]);
	vOut.intensity.y = saturate(LightIntensity_multiple[1]);
	vOut.intensity.z = saturate(LightIntensity_multiple[2]);
	
    return vOut;
}

vertOut GlowVS(appdata IN){
	vertOut vOut = (vertOut)0;

	vOut.normalW = mul(float4(IN.normalL, 0.0f), gWIT).xyz;
	vOut.normalW = normalize(vOut.normalW);
	
	
	//---------------------
	float4 Po = float4(IN.posL.xyz,1.0f);
    Po += (ClampedSin(4.0f,2.0f,0.0f,gInflation)*normalize(float4(IN.normalL.xyz,0.0f)));
    
	if(gRotate){
		Po = Rotate(Po,gTime,gRotAxis);
	}
    float4 Pw = mul(Po,gWorld);
    vOut.viewW = normalize(gViewInv[3].xyz - Pw.xyz);
    vOut.posH = mul(Po,gWVP);
	vOut.tex0 = IN.tex0 * 0.1f;
	//---------------------
   
	if(gBlob){
		vOut.posH.xyz = Blob(vOut.posH.xyz,vOut.normalW,20.0f);
	}

	return vOut;	
}

vertNormOut normalMap_Planet_VS(float3 pos	: POSITION0,
					float3 tangent	: TANGENT0,
					float3 binormal	: BINORMAL0,
					float3 normal	: NORMAL0,
					float3 tex0	: TEXCOORD0){
	vertNormOut vOut = (vertNormOut)0;
	
	/* Make Tangent-Binormal-Normal Matrix */
	float3x3 TBN;
	TBN[0] = mul(normalize(tangent),gWorld);
	TBN[1] = mul(normalize(binormal),gWorld);
	TBN[2] = mul(normalize(normal),gWorld);
	
	/* transform toEye vector to tan space */
	float3 eyePos = mul(gWInv,float4(gEyePosW,1.0f));
	vOut.toEye = normalize(mul(eyePos - pos,TBN));
	
	/* transform lightDir vector to tan space */
	//vOut.lightDir = normalize(mul(gLightVecW, TBN));
	
	
	float x,y,z;
	//find direction of light to vertex
	x = (gLightPos_multiple_1.x - pos.x);
	y = (gLightPos_multiple_1.y - pos.y);
	z = (gLightPos_multiple_1.z - pos.z);
	vOut.lightDir = normalize(mul(float3(x,y,z), TBN));
	
	vOut.tex0 = pos;

	if(gRotate){
		if(gStratosphere){
			vOut.posH = mul(Rotate(float4(pos*(planetSize + 0.4f),1.0f),gTime,gRotAxis), gWVP);
		}
		else{
			//vOut.posH = mul(Rotate(float4(pos*planetSize, 1.0f),gTime,gRotAxis), gWVP);
			vOut.posH = mul(float4(pos*planetSize, 1.0f), gWVP);
		}
	}
	else{
		if(gStratosphere){
			vOut.posH = mul(float4(pos*(planetSize + 0.4f), 1.0f), gWVP);
		}
			vOut.posH = mul(float4(pos*planetSize, 1.0f), gWVP);
	}
	
	float sine = sin(gTime);
	vOut.posH.y += 50*sine;
	
	
	return vOut;
}

vertOut_MultipleLights normalMap_Planet_MultiLight_VS(float3 pos	: POSITION0,
					float3 tangent	: TANGENT0,
					float3 binormal	: BINORMAL0,
					float3 normal	: NORMAL0,
					float3 tex0	: TEXCOORD0,
					float3 intensity   : TEXCOORD1,
					float3 lightDir[maxLights]   : TEXCOORD2){
	vertOut_MultipleLights vOut = (vertOut_MultipleLights)0;
	
	/* Make Tangent-Binormal-Normal Matrix */
	float3x3 TBN;
	TBN[0] = mul(normalize(tangent),gWorld);
	TBN[1] = mul(normalize(binormal),gWorld);
	TBN[2] = mul(normalize(normal),gWorld);
	
	/* transform toEye vector to tan space */
	float3 eyePos = mul(gWInv,float4(gEyePosW,1.0f));
	vOut.toEye = normalize(mul(eyePos - pos,TBN));
	
	vOut.tex0 = pos;

	if(gRotate){
		if(gStratosphere){
			vOut.posH = mul(Rotate(float4(pos*(planetSize + 0.4f),1.0f),gTime,gRotAxis), gWVP);
		}
		else{
			//vOut.posH = mul(Rotate(float4(pos*planetSize, 1.0f),gTime,gRotAxis), gWVP);
			vOut.posH = mul(float4(pos*planetSize, 1.0f), gWVP);
		}
	}
	else{
		if(gStratosphere){
			vOut.posH = mul(float4(pos*(planetSize + 0.4f), 1.0f), gWVP);
		}
			vOut.posH = mul(float4(pos*planetSize, 1.0f), gWVP);
	}
	
	float sine = sin(gTime);
	vOut.posH.y += 50*sine;
	
	/////////////////////////////
	float3 LightPos_multiple[maxLights];
	LightPos_multiple[0] = gLightPos_multiple_1;
	LightPos_multiple[1] = gLightPos_multiple_2;
	LightPos_multiple[2] = gLightPos_multiple_3;
	
	float distance = 0.0f;
	float x = 0.0f;
	float y = 0.0f;
	float z = 0.0f;
	
	float LightIntensity_multiple[maxLights] = {0.0f,0.0f,0.0f};

	for(int v = 0; v < maxLights; v++)
	{
		x = (LightPos_multiple[v].x - vOut.posW.x);
		y = (LightPos_multiple[v].y - vOut.posW.y);
		z = (LightPos_multiple[v].z - vOut.posW.z);
		distance = sqrt(x*x + y*y + z*z);
		distance *= distance;
		if(v == 0){
			distance *= distanceScaleMainLight;
		}
		else{
			distance *= distanceScale;
		}
		
		if(distance <= 1)
		{
			LightIntensity_multiple[v] = 1.0f;
		}
		else if(distance > 1 && distance < attenMax)
		{
			LightIntensity_multiple[v] = (attenMax - distance)*attenFactor;
		}
		else if(distance >= attenMax)
		{
			LightIntensity_multiple[v] = 0.0f;
		}
		
		//find direction of light to vertex
		x = (LightPos_multiple[v].x - vOut.posW.x);
		y = (LightPos_multiple[v].y - vOut.posW.y);
		z = (LightPos_multiple[v].z - vOut.posW.z);
		vOut.lightDir[v] = normalize(mul(float3(x,y,z),TBN));
	}
	
	vOut.intensity.x = saturate(LightIntensity_multiple[0]);
	vOut.intensity.y = saturate(LightIntensity_multiple[1]);
	vOut.intensity.z = saturate(LightIntensity_multiple[2]);
	/////////////////////////////
	
	return vOut;
}


stratVertOut StratosphereVS(appdata IN)
{
	stratVertOut vOut = (stratVertOut)0;
	vOut.normalW = mul(float4(IN.normalL, 0.0f), gWIT).xyz;
	vOut.normalW = normalize(vOut.normalW);
	vOut.posH = mul(float4(IN.posL, 1.0f), gWVP);
	vOut.tex0 = IN.tex0;
	
	if(gBlob){
		vOut.posH.xyz = Blob(vOut.posH,vOut.normalW,20.0f);
	}
	
	vOut.tex0 = IN.tex0 *= planetScale + 0.5f;
		
	if(gRotate){
		if(gStratosphere){
			vOut.posH = mul(Rotate(float4(IN.posL*(planetSize + 0.4f),1.0f),gTime,gRotAxis), gWVP);
		}
		else{
			vOut.posH = mul(Rotate(float4(IN.posL*planetSize, 1.0f),gTime,gRotAxis), gWVP);
		}
	}
	else{
		if(gStratosphere){
			vOut.posH = mul(float4(IN.posL*(planetSize + 0.7f), 1.0f), gWVP);
		}
		vOut.posH = mul(float4(IN.posL*planetSize, 1.0f), gWVP);
	}
	
	float sine = sin(gTime);
	vOut.posH.y += 50*sine;
	
    return vOut;
}

particleVertInOut SpriteParticle_Orbit_VS(float3 pos    : POSITION0, 
									float3 velocityInitial	: TEXCOORD0,
									float4 color			: COLOR0,
									float size				: PSIZE0,
									float angle				: TEXCOORD1)
{
	particleVertInOut outgoing = (particleVertInOut)0;
	
	float t = gTime/gOrbitSpeed;
	t *= gVelocity.x;
	float r = 200.0f;
	
	// Rotate the particles about local space.
	float sine, cosine;
	sincos(t, sine, cosine);
	
	float x = 0.0f;
	float y = 0.0f;
	float z = 0.0f;
	
	if(gRotAxis == 'x')
	{
		x = r*cosine + r*-sine;
		y = r*sine + r*cosine;
	}
	else if(gRotAxis == 'y')
	{
		x = r*cosine + r*sine;
		z = r*-sine + r*cosine;
	}
	else
	{
		y = r*cosine + r*-sine;
		z = r*sine + r*cosine;
	}
	
	
	pos.x += gCenter.x + x;
	pos.y += gCenter.y + y;
	pos.z += gCenter.z + z;
	
	// Transform to homogeneous clip space.
	outgoing.pos = mul(float4(pos, 1.0f), gWVP);
	
	// Also compute size as a function of the distance from the camera,
	// and the viewport heights.  The constants were found by 
	// experimenting.
	float d = distance(pos, gEyePosW);
	outgoing.size = gViewportHeight*size/(1.0f + 3.0f*d);
	
	// Fade color from white to black over time to fade particles out.
	outgoing.color = float4(0,0,0,1);
	
	// Done--return the output.
    return outgoing;
}

particleVertInOut SpriteParticle_Fall_VS(float3 pos    : POSITION0, 
									float3 velocityInitial	: TEXCOORD0,
									float4 color			: COLOR0,
									float size				: PSIZE0)
{
	particleVertInOut vOut = (particleVertInOut)0;
	
	pos.x += gDisplacement.x;
	pos.y += gDisplacement.y;
	pos.z += gDisplacement.z;
	
	// Transform to homogeneous clip space.
	//vOut.pos = mul(float4(pos, 1.0f), gWVP);
	//vOut.pos = mul(mul(float4(pos, 1.0f), gPlayerRot), gWVP);
	vOut.pos = mul(float4(pos, 1.0f), mul(gPlayerRot, gWVP));
	
	// Also compute size as a function of the distance from the camera,
	// and the viewport heights.  The constants were found by 
	// experimenting.
	float d = distance(pos, gEyePosW);
	vOut.size = gViewportHeight*size/(1.0f + 3.0f*d);
	
    return vOut;
}

particleVertInOut SpriteParticle_Spray_VS(float3 pos    : POSITION0, 
									float2 texIn  : TEXCOORD0)
{
	particleVertInOut outgoing = (particleVertInOut)0;
	
	// Sprinkler--------------
	float sine,cosine;
	sincos(gAngle,sine,cosine);
	pos.x += gVelocity.x;
	pos.y += gVelocity.y;
	//---------------------
	
	// Transform to homogeneous clip space.
	outgoing.pos = mul(float4(pos, 1.0f), gWVP);
	
	// Also compute size as a function of the distance from the camera,
	// and the viewport heights.  The constants were found by 
	// experimenting.
	float d = distance(pos, gEyePosW);
	outgoing.size = gViewportHeight/(1.0f + 3.0f*d);
	
	// Fade color from white to black over time to fade particles out.
	outgoing.color = float4(0,0,0,1);
	
	// Done--return the output.
    return outgoing;
}

vertNormOut normalMap_Star_VS(float3 pos	: POSITION0,
					float3 tangent	: TANGENT0,
					float3 binormal	: BINORMAL0,
					float3 normal	: NORMAL0,
					float3 tex0	: TEXCOORD0){
	vertNormOut vOut = (vertNormOut)0;
	
	/* Make Tangent-Binormal-Normal Matrix */
	float3x3 TBN;
	TBN[0] = mul(normalize(tangent),gWorld);
	TBN[1] = mul(normalize(binormal),gWorld);
	TBN[2] = mul(normalize(normal),gWorld);
	
	/* transform toEye vector to tan space */
	float3 eyePos = mul(gWInv,float4(gEyePosW,1.0f));
	vOut.toEye = normalize(mul(eyePos - pos,TBN));
	
	/* transform lightDir vector to tan space */
	vOut.lightDir = normalize(mul(gLightVecW, TBN));
	
	vOut.tex0 = pos;

	if(gRotate){
		if(gStratosphere){
			vOut.posH = mul(Rotate(float4(pos*(planetSize + 0.4f),1.0f),gTime,gRotAxis), gWVP);
		}
		else{
			vOut.posH = mul(Rotate(float4(pos*planetSize, 1.0f),gTime,gRotAxis), gWVP);
		}
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
	else if(gGreyMap){
		if(diffuseTex.r + diffuseTex.g + diffuseTex.b != 0){
			diffuseTex.r = gGreyMapColorA.r;
			diffuseTex.g = gGreyMapColorA.b;
			diffuseTex.b = gGreyMapColorA.g;
		}
		if(diffuseTex.r + diffuseTex.g + diffuseTex.b == 0){
			diffuseTex.r = gGreyMapColorB.r;
			diffuseTex.g = gGreyMapColorB.b;
			diffuseTex.b = gGreyMapColorB.g;
		}
		return float4(diffuseTex + ambient + spec, gDiffuseMtrl.a);
	}
	else{
		return float4(diffuseTex + ambient + spec, gDiffuseMtrl.a);
	}
	
}

float4 Phong_Player_PS(vertOut IN) : COLOR
{
	IN.normalW = normalize(IN.normalW);
	//=======================================================
	// Lighting

	float3 toEye = normalize(gEyePosW - IN.posW);
	float3 r = reflect(-gLightVecW, IN.normalW);
	float t  = pow(max(dot(r, toEye), 0.0f), gSpecularPower);
	float s = max(dot(gLightVecW, IN.normalW), 0.0f);

	float3 spec = t*(gSpecularMtrl*gSpecularLight).rgb;
	float4 diffuse = s*(gDiffuseMtrl*gDiffuseLight);
	float3 ambient = gAmbientMtrl*gAmbientLight;

	//=======================================================
	float4 texColor = tex2D(TexS,IN.tex0);
	
	float clamp = 0.35f;
	diffuse.r = Clamp(clamp,diffuse.r);
	diffuse.g = Clamp(clamp,diffuse.g);
	diffuse.b = Clamp(clamp,diffuse.b);
	
	float4 diffuseTex = diffuse * texColor;
	
	if(gColorShift){
		return float4((ColorShift().rgb) + ambient + spec, diffuseTex.a);
	}
	else{
		return float4(diffuseTex.rgb + ambient + spec, texColor.a);
	}
}

float4 Phong_MultipleLights_PS(vertOut_MultipleLights IN) : COLOR
{
	IN.normalW = normalize(IN.normalW);
	//=======================================================
	// Lighting

	float3 ambient = gAmbientMtrl*gAmbientLight;
	float3 toEye = normalize(gEyePosW - IN.posW);
	
	float3 r = float3(0.0f,0.0f,0.0f);
	float t = 0.0f;
	float s = 0.0f;

	float3 spec = float3(0.0f,0.0f,0.0f);
	float4 diffuse = float4(0.0f,0.0f,0.0f,0.0f);
	
	//=======================================================
	// Multiple Lights!
	
	float LightIntensity_multiple[maxLights];
	LightIntensity_multiple[0] = IN.intensity.x;
	LightIntensity_multiple[1] = IN.intensity.y;
	LightIntensity_multiple[2] = IN.intensity.z;
	
	gSpecularMtrl_multiple[0] = gSpecularMtrl_multiple_1;
	gSpecularMtrl_multiple[1] = gSpecularMtrl_multiple_2;
	gSpecularMtrl_multiple[2] = gSpecularMtrl_multiple_3;
	
	gSpecularLight_multiple[0] = gSpecularLight_multiple_1;
	gSpecularLight_multiple[1] = gSpecularLight_multiple_2;
	gSpecularLight_multiple[2] = gSpecularLight_multiple_3;
	
	gDiffuseMtrl_multiple[0] = gDiffuseMtrl_multiple_1;
	gDiffuseMtrl_multiple[1] = gDiffuseMtrl_multiple_2;
	gDiffuseMtrl_multiple[2] = gDiffuseMtrl_multiple_3;
	
	gDiffuseLight_multiple[0] = gDiffuseLight_multiple_1;
	gDiffuseLight_multiple[1] = gDiffuseLight_multiple_2;
	gDiffuseLight_multiple[2] = gDiffuseLight_multiple_3;
	
	for(int v = 0; v < maxLights; v++)
	{
		r = reflect(-IN.lightDir[v], IN.normalW);
		t  = pow(max(dot(r, toEye), 0.0f), gSpecularPower);
		spec += t*(gSpecularMtrl_multiple[v]*gSpecularLight_multiple[v]).rgb*LightIntensity_multiple[v];
		
		s = max(dot(IN.lightDir[v], IN.normalW), 0.0f);
		diffuse += s*(gDiffuseMtrl_multiple[v]+gDiffuseLight_multiple[v])*LightIntensity_multiple[v];
	}

	//=======================================================
	float4 texColor = tex2D(TexS,IN.tex0);
	
	float clamp = 0.35f;
	
	if(diffuse.r < clamp)
	{
		diffuse.r = clamp;
	}
	if(diffuse.g < clamp)
	{
		diffuse.g = clamp;
	}
	if(diffuse.b < clamp)
	{
		diffuse.b = clamp;
	}
	
	float4 diffuseTex = diffuse * texColor;
	
	
	return float4(diffuseTex.rgb + ambient + spec, texColor.a);
}


float4 GlowPS(vertOut IN) : COLOR
{
	float3 normal = normalize(IN.normalW);
    float3 view = normalize(IN.viewW);
    float edge = 1.0 - dot(normal,view);
    edge = pow(edge,gGlowExp);
    float3 result = edge * gGlowColor.rgb;
    return float4(result,edge);
}

float4 NormalMap_Planet_PS(	vertNormOut IN) : COLOR
{
	float3 eye = normalize(IN.toEye);
	float3 lightDir = normalize(IN.lightDir);
	float4 texCol = texCUBE(TexS,IN.tex0);
	
	//=======================================================
	// Normal Mapping
	float3 normalTex = texCUBE(TexNmap,IN.tex0);
	
	normalTex = normalize((normalTex*2.0f) - 1.0f);
	//=======================================================
	
	//=======================================================
	// Lighting
	float s = max(dot(lightDir, normalTex),0.0f);

	float3 diffuse = s*(gDiffuseMtrl*gDiffuseLight);
	float3 ambient = (gAmbientMtrl*gAmbientLight);
	//=======================================================
	
	if(gGreyMap){
		if(	(texCol.r >= 0.4f && texCol.r < 0.6f) && 
			(texCol.g >= 0.4f && texCol.g < 0.6f) &&
			(texCol.b >= 0.4f && texCol.b < 0.6f)){
			texCol.r = gGreyMapColorA.r;
			texCol.g = gGreyMapColorA.g;
			texCol.b = gGreyMapColorA.b;
			texCol.a = 1.0f;
		}
		else if(texCol.r == 0.0f && texCol.g == 0.0f && texCol.b == 0.0f){
			texCol.r = gGreyMapColorB.r;
			texCol.g = gGreyMapColorB.g;
			texCol.b = gGreyMapColorB.b;
			texCol.a = 1.0f;
		}
		else{
			texCol.r = 0.0f;
			texCol.g = 0.0f;
			texCol.b = 0.0f;
			texCol.a = 1.0f;
		}
		float3 color = (ambient + diffuse)*texCol.rgb;
		return float4(color,texCol.a);
	}
	else{
		float3 color = (ambient + diffuse)*texCol.rgb;
		return float4(color,texCol.a);
	}
}

/* shades a texture w/ no lighting */
float4 StratospherePS(stratVertOut IN) : COLOR
{
	IN.normalW = normalize(IN.normalW);
	//=======================================================
	// Lighting
	
	float s = max(dot(-gLightVecW, IN.normalW), 0.0f);

	float3 diffuse = s*(gDiffuseMtrl*gDiffuseLight).rgb;
	float3 ambient = gAmbientMtrl*gAmbientLight;

	//=======================================================

	float4 col = tex2D(TexStratosphere,IN.tex0);
	
	if(col.r == 1.0f && col.g == 1.0f && col.b == 1.0f){
		return float4((ambient + diffuse)*col.rgb,0.3f);
	}
	else{
		return float4(col.rgb,0.0f);
	}
}

float4 SpriteParticle_PS(float4 color : COLOR0, 
						 float2 tex1 : TEXCOORD0) : COLOR
{
	float4 col = tex2D(ParticleTex,tex1);
	return col;
}

float4 StarGreyMap_Body(float4 texIn)
{
	
	if(	(texIn.r < 0.7f) && 
			(texIn.g < 0.7f) &&
			(texIn.b < 0.7f)){
			texIn.r = gGreyMapColorB.r;
			texIn.g = gGreyMapColorB.g;
			texIn.b = gGreyMapColorB.b;
			texIn.a = 1.0f;
		}
		else{
			//texIn.r = gGreyMapColorA.r;
			//texIn.g = gGreyMapColorA.g;
			//texIn.b = gGreyMapColorA.b;
			texIn.r = gGreyMapColorB.r + cos(gTime/4) + 0.98f;
			texIn.g = gGreyMapColorB.g + cos(gTime/4) + 0.98f;
			texIn.b = gGreyMapColorB.b + cos(gTime/4) + 0.98f;
			texIn.a = 1.0f;
		}
		
		return texIn;
}


float4 NormalMap_Star_PS(vertNormOut IN) : COLOR
{
	float3 light = normalize(IN.lightDir);
	float4 tex =  texCUBE(StarTex_Mid,IN.tex0);
	
	//=======================================================
	// Lighting
	float s = max(dot(light, float3(1.0f,1.0f,1.0f)),0.0f);

	float3 diffuse = s*(gDiffuseMtrl*gDiffuseLight);
	//float3 ambient = (gAmbientMtrl*gAmbientLight);
	float3 ambient = float3(1.0f,1.0f,1.0f);
	//=======================================================
	
	if(gGreyMap){
		tex = StarGreyMap_Body(tex);
		float3 color = (ambient + diffuse)*tex.rgb;
		return float4(color,tex.a);
	}
	else{
		float3 color = (ambient + diffuse)*tex.rgb;
		return float4(color,tex.a);
	}
}
float4 StarGreyMap_Surface(float4 texIn)
{
	
	if(	
		(texIn.r < 0.55f) && 
			(texIn.g < 0.55f) &&
			(texIn.b < 0.55f) &&
			texIn.a < 1){
		texIn.r = gGreyMapColorC.r;
		texIn.g = gGreyMapColorC.g;
		texIn.b = gGreyMapColorC.b;
		texIn.a = cos(gTime/2) + 1.2f;
	}
	else{
		texIn.a = 0;
	}
	return texIn;
}
float4 NormalMap_Star_Surface_PS(vertNormOut IN) : COLOR
{
	float3 texCoord = IN.tex0;
	texCoord.x += cos(gTime);
	texCoord.y += sin(gTime);
	float3 light = normalize(IN.lightDir);
	float4 tex =  texCUBE(StarTex_Top,texCoord);
	
	
	//=======================================================
	// Lighting
	float s = max(dot(light, float3(1.0f,1.0f,1.0f)),0.0f);

	float3 diffuse = s*(gDiffuseMtrl*gDiffuseLight);
	//float3 ambient = (gAmbientMtrl*gAmbientLight);
	float3 ambient = float3(1.0f,1.0f,1.0f);
	//=======================================================
	
	if(gGreyMap){
		tex = StarGreyMap_Surface(tex);
		float3 color = (ambient + diffuse)*tex.rgb;
		return float4(color,tex.a);
	}
	else{
		float3 color = (ambient + diffuse)*tex.rgb;
		return float4(color,tex.a);
	}
}

technique Main
{
    pass P0
    {
        vertexShader = compile vs_2_0 PhongVS();
        pixelShader  = compile ps_2_0 PhongPS();
		CullMode = CCW;
    }
}

technique Player
{
    pass P0
    {
        vertexShader = compile vs_2_0 Phong_Player_VS();
        pixelShader  = compile ps_2_0 Phong_Player_PS();
        
        ZEnable = true;
		ZWriteEnable = true;
		
		CullMode = CCW;
    }
}

technique Glow
{
    pass P0
    {	
        vertexShader = compile vs_2_0 PhongVS();
        pixelShader  = compile ps_2_0 PhongPS();
		CullMode = CCW;
    }
    pass P1
    {
        vertexShader = compile vs_2_0 GlowVS();
        pixelShader  = compile ps_2_0 GlowPS();
        ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		CullMode = CW;
    }
}

technique Planet
{
	pass P0
	{
        vertexShader = compile vs_2_0 normalMap_Planet_VS();
        pixelShader  = compile ps_2_0 NormalMap_Planet_PS();
        
        ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
		CullMode = CCW;
	}
	
}

technique Stratosphere
{
	pass P0
	{
		vertexShader = compile vs_2_0 StratosphereVS();
        pixelShader  = compile ps_2_0 StratospherePS();
        
        ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		
		CullMode = CCW;
	}
}

technique Particle_Orbit
{
	pass P0
	{
		vertexShader = compile vs_2_0 SpriteParticle_Orbit_VS();
        pixelShader  = compile ps_2_0 SpriteParticle_PS();
        ZEnable = true;
		ZWriteEnable = true;
	}
}

technique Particle_Fall
{
	pass P0
	{
		vertexShader = compile vs_2_0 SpriteParticle_Fall_VS();
        pixelShader  = compile ps_2_0 SpriteParticle_PS();
        ZEnable = true;
		ZWriteEnable = true;
	}
}

technique Particle_Spray
{
	pass P0
	{
		vertexShader = compile vs_2_0 SpriteParticle_Spray_VS();
        pixelShader  = compile ps_2_0 SpriteParticle_PS();
        ZEnable = true;
		ZWriteEnable = true;
	}
}

technique Star
{
	pass P0
	{
		vertexShader = compile vs_2_0 normalMap_Star_VS();
        pixelShader  = compile ps_2_0 NormalMap_Star_PS();
        
        ZEnable = true;
		ZWriteEnable = true;
		CullMode = CCW;
	}
	pass P1
	{
		vertexShader = compile vs_2_0 normalMap_Star_VS();
        pixelShader  = compile ps_2_0 NormalMap_Star_Surface_PS();
        
        ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		CullMode = CCW;
	}
}

technique Main_MultipleLights
{
    pass P0
    {
        vertexShader = compile vs_2_0 Phong_MultipleLights_VS();
        pixelShader  = compile ps_2_0 Phong_MultipleLights_PS();
        
        ZEnable = true;
		ZWriteEnable = true;
		
		CullMode = CCW;
    }
}

technique Particles_MultipleLights
{
    pass P0
    {
        vertexShader = compile vs_2_0 Phong_MultipleLights_VS();
        pixelShader  = compile ps_2_0 Phong_MultipleLights_PS();
        
        ZEnable = true;
		ZWriteEnable = true;
		
		CullMode = CCW;
    }
    pass P1
    {
        vertexShader = compile vs_2_0 GlowVS();
        pixelShader  = compile ps_2_0 GlowPS();
        ZEnable = true;
		ZWriteEnable = true;
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		CullMode = CW;
    }
}
