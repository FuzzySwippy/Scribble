shader_type canvas_item;

uniform bool show_grid = true;
uniform vec4 line_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float thickness = 0.1;
uniform float interval = 16.0;

void fragment() {
	vec2 uv = UV / interval;
	float line_tickness = thickness / interval / 2.0;
	if (show_grid && (
		abs(fract(uv.x) - 0.5) > 0.5 - line_tickness || 
		abs(fract(uv.y) - 0.5) > 0.5 - line_tickness)) {
		COLOR = line_color;
	}
}