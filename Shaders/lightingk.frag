#version 330 core

struct Light {
    vec3  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;
    vec3 ambient;
};

uniform Light light;
uniform sampler2D texture0;

in vec2 texCord;
in vec3 FragPos;

out vec4 FragColor;

void main() {
    vec3 result = texture(texture0, texCord).rgb;
    vec3 lightDir = normalize(light.position - FragPos);
    
    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    result *= intensity;

    if (intensity < 0.1) {
        result = light.ambient * texture(texture0, texCord).rgb;
    }
    FragColor = vec4(result, 1);
}