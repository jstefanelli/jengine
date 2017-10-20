#version 330

precision mediump float;
precision mediump int;

uniform vec3 uLightPos;
uniform sampler2D uShadowMaps[3];
uniform float cascThreshold[3];

struct phongMaterial{
	vec4 diff;
	vec4 amb;
	vec4 spec;

	sampler2D diffTxt;
	sampler2D ambTxt;
	sampler2D specTxt;

	int hasDiff;
	int hasAmb;
	int hasSpec;

	sampler2D normTxt;

	int hasNorm;
};

uniform phongMaterial uMat;

in vec2 vTexCoord;
in vec4 vNormal;
in vec4 vPosition;
in float vClipSpacePos;
in vec4 vPosInShadow0;
in vec4 vPosInShadow1;
in vec4 vPosInShadow2;

const float bias = 0.0003;

float getShadowMultiplier0(vec4 fragPosLightSpace){
	vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
	projCoords = projCoords * 0.5 + 0.5; 
	float closestDepth = texture(uShadowMaps[0], projCoords.xy).r;
	float currentDepth = projCoords.z;
	float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;  
	if(projCoords.z > 1.0)
        shadow = 0.0;
	return shadow;
}

float getShadowMultiplier1(vec4 fragPosLightSpace){
	vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
	projCoords = projCoords * 0.5 + 0.5; 
	float closestDepth = texture(uShadowMaps[1], projCoords.xy).r;
	float currentDepth = projCoords.z;
	float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;  
	if(projCoords.z > 1.0)
        shadow = 0.0;
	return shadow;
}

float getShadowMultiplier2(vec4 fragPosLightSpace){
	vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
	projCoords = projCoords * 0.5 + 0.5; 
	float closestDepth = texture(uShadowMaps[2], projCoords.xy).r;
	float currentDepth = projCoords.z;
	float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;  
	if(projCoords.z > 1.0)
        shadow = 0.0;
	return shadow;
}

vec3 parseAmbient(phongMaterial mat, vec2 texCoords){
	if(mat.hasAmb == 0){
		return mat.amb.xyz;
	}else{
		vec3 ambColor = texture(mat.ambTxt, texCoords).xyz;
		ambColor *= mat.amb.xyz;
		return ambColor;
	}
}

vec3 parseDiffuse(phongMaterial mat, vec2 texCoords){
	if(mat.hasDiff == 0){
		return mat.diff.xyz;
	}else{
		vec3 diffColor = texture(mat.diffTxt, texCoords).xyz;
		diffColor *= mat.diff.xyz;
		return diffColor;
	}
}

vec3 parseSpecular(phongMaterial mat, vec2 texCoords){
	if(mat.hasSpec == 0){
		return mat.spec.xyz;
	}else{
		vec3 specColor = texture(mat.specTxt, texCoords).xyz;
		specColor *= mat.spec.xyz;
		return specColor;
	}
}

out vec4 oColor;

void main(){
	vec3 uEyePos = vec3(0, 0, 0);
	
	vec3 ambientColor = parseAmbient(uMat, vTexCoord);

	vec3 norm = normalize(vNormal.xyz);
	vec3 lightDir = normalize(uLightPos - vPosition.xyz);
	
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuseColor = parseDiffuse(uMat, vTexCoord);
	diffuseColor *= diff;

	vec3 eyeDir = normalize(uEyePos - vPosition.xyz);
	vec3 reflectDir = reflect(-lightDir, norm);

	float spec = pow(max(dot(eyeDir, reflectDir), 0.0), 32);
	vec3 specularColor = parseSpecular(uMat, vTexCoord); 
	specularColor *= vec3(spec, spec, spec);
	float shadow = 0.0;
	if(vClipSpacePos < cascThreshold[0]){
		shadow = getShadowMultiplier0(vPosInShadow0);
	}else if(vClipSpacePos < cascThreshold[1]){
		shadow = getShadowMultiplier1(vPosInShadow1);
	}else{
		shadow = getShadowMultiplier2(vPosInShadow2);
	}

	oColor = vec4(ambientColor + (max((1.0 - shadow), 0.0) * (diffuseColor + specularColor)), 1.0);
}