shader_type canvas_item;

uniform float flash_modifier: hint_range(0.0, 1.0)=0.3;

void fragment(){
	vec4 color = texture(TEXTURE, UV);
	color.rgb = mix(color.rgb, vec3(1,1,1).rgb, flash_modifier);
	COLOR = color;
}