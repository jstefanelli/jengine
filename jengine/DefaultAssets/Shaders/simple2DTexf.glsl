#version 330

precision mediump float;
precision mediump int;

uniform int uMs;
uniform sampler2DMS uTxtMs;
uniform sampler2D uTxt;

in vec2 vTexCoord;

vec4 sampleMS(sampler2DMS texture, ivec2 ipos)
{
	vec4 texel = vec4(0.0);	
    
    for (int i = 0; i < uMs; i++)
    {
    	texel += texelFetch(texture, ipos, i);
    }
    
    texel /= float(uMs);
    
    return texel;
}

out vec4 oColor;

void main(){
	if(uMs != 0){
		//Run MSAA

		vec2 frameSize = vec2(textureSize(uTxtMs));

		ivec2 ivTexCoord = ivec2(frameSize * vTexCoord);

		oColor = sampleMS(uTxtMs, ivTexCoord);
	}else{
		oColor = texture(uTxt, vTexCoord);
	}
}