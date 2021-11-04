shader_type canvas_item;

uniform sampler2D noise_img;
uniform float speed = 1.0;
uniform vec4 smoke_color : hint_color;

uniform float flash_modifier: hint_range(0.0, 1.0)=0.2;

void fragment(){
	vec2 uv1 = vec2(UV.x + TIME*speed, UV.y);
	vec2 uv2 = vec2(UV.x - TIME*speed, UV.y);
	vec2 uv3 = vec2(UV.x, UV.y + TIME*speed);
	//vec2 uv = vec2(UV.x + TIME, UV.y);
	
	float noise_r = texture(noise_img, uv1).r;
	float noise_g = texture(noise_img, uv2).g;
	float noise_b = texture(noise_img, uv3).b;
	
	vec3 new_color = vec3(noise_r, noise_g, noise_b);
	
	float new_alpha = noise_r * noise_g * noise_b;
	
	
	//COLOR.rgb = new_color;
	//COLOR.rgb = new_color;
	COLOR.rgb = texture(TEXTURE, UV).rgb * (new_color/10);
	//COLOR.rgb = mix(color.rgb, vec3(1,1,1).rgb, flash_modifier);
	//COLOR.a = texture(TEXTURE, UV).a;
	COLOR.a = clamp(new_alpha * 50.0 * texture(TEXTURE, UV).a, 0.0, 1.0);
}
