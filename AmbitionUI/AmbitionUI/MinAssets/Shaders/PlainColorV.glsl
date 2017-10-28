#version 120

precision mediump float;

uniform mat4 uProjection;
uniform vec2 uPosition;
uniform vec2 uSize;

attribute vec2 aPoints;

void main(){
	gl_Position = uProjection * vec4(uPosition + (aPoints * uSize), 0.0, 1.0);
}