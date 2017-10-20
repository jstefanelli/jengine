#version 330

precision mediump float;

uniform vec4 uCol;

out vec4 oColor;

void main(){
	oColor = uCol;
}