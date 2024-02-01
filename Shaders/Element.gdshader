shader_type canvas_item;

uniform sampler2D mask;

void fragment() {
	
	vec4 UVColor = texture(TEXTURE, UV);
  	vec4 maskColor = texture(mask, SCREEN_UV);
	
	if (maskColor.r == 1.0f)
	{
		COLOR = UVColor;	
	}
	else
	{
		COLOR = vec4(0,0,0,0);
	}
	//COLOR = vec4(SCREEN_UV, 0, 1);
}
