shader_type canvas_item;

uniform float blackness;
uniform float whiteness;
uniform float destroying;
uniform float alphaModulate;
uniform sampler2D mask;

void fragment() {
	
	vec4 UVColor = texture(TEXTURE, UV);
  	vec4 maskColor = texture(mask, SCREEN_UV);
	
	if (maskColor.r == 1.0f)
	{
		COLOR = mix(
			mix(
				UVColor, 
				vec4(1,1,1,UVColor.a), 
				whiteness), 
			vec4(0,0,0,UVColor.a), 
			blackness);
		COLOR.a *= alphaModulate;
		float randX = (UV.x * 775216.414617f);
		float randY = (UV.y * 454617.135216f);
		float rand = randX + randY;
		if (rand - floor(rand) < destroying)	
		{
			COLOR.a = 0.0f;
		}
	}
	else
	{
		COLOR = vec4(0,0,0,0);
	}
	//COLOR = vec4(SCREEN_UV, 0, 1);
}
