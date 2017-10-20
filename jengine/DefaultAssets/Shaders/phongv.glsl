#version 330

precision mediump float;
precision lowp int;

uniform mat4 uProj;
uniform mat4 uView;
uniform mat4 uCamera;
uniform mat4 uModel;
uniform mat4 uNormal;
uniform mat4 uLightMatrix0;
uniform mat4 uLightMatrix1;
uniform mat4 uLightMatrix2;

in vec3 aPos;
in vec2 aTexCoord;
in vec4 aNorm;

out vec2 vTexCoord;
out vec4 vNormal;
out vec4 vPosition;
out float vClipSpacePos;
out vec4 vPosInShadow0;
out vec4 vPosInShadow1;
out vec4 vPosInShadow2;

void main(){
	vTexCoord = aTexCoord;
	vec4 pos = uCamera * uModel * vec4(aPos, 1.0);
	gl_Position = uProj * uView * pos;
	vPosition = pos; 
	vClipSpacePos = gl_Position.z;
	vPosInShadow0 = uLightMatrix0 * uModel * vec4(aPos, 1.0);
	vPosInShadow1 = uLightMatrix1 * uModel * vec4(aPos, 1.0);
	vPosInShadow2 = uLightMatrix2 * uModel * vec4(aPos, 1.0);
	vNormal = uNormal * aNorm;
}