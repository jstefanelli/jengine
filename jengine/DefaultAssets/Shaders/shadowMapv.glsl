#version 330

uniform mat4 uMV;
uniform mat4 uModel;

in vec3 aPos;

void main(){
	gl_Position = uMV * uModel * vec4(aPos, 1.0);
}