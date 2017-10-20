#version 330

precision mediump float;

uniform mat4 uMVP;

in vec3 aPos;

void main(){
	gl_Position = uMVP * vec4(aPos, 1.0);
}