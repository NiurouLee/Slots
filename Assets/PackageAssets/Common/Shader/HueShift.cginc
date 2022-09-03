// Writen by Martin Nerurkar ( www.playful.systems). MIT license (see license.txt)
// Based on Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Inspired by HSV Shader for Unity from Gregg Tavares (https://github.com/greggman/hsva-unity). MIT License (see license.txt)

// Shifts from rgb to hsv color space
half3 rgb2hsv(half3 c) {
	half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
	float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

	half d = q.x - min(q.w, q.y);
	half e = 1.0e-10;
	return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Shifts from hsv back to rgb color space
half3 hsv2rgb(half3 c) {
	c = half3(c.x, clamp(c.yz, 0.0, 1.0));
	half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Used for straightforward Hue Shift
half3 shiftHSV(half3 rgb, half3 hsv) {
	return hsv2rgb(rgb2hsv(rgb) + ( - 1.0 + hsv * 2.0 )); // this is because the RGB values range from 0 to 1 in the color but we expect -1 to +1 here.
}

// The core function which uses multi-compiling to get us the appropriate behavior
half4 applyHSV(half4 rgba, half4 hsva, half minRange, half maxRange) {

	// Do we do any masking on the shift?
	#if PS_HSV_ALPHAMASK_ON || PS_HSV_HUERANGE_ON
		float affectMult;
        half3 rgbShifted = rgb2hsv(rgba.rgb);

		#if PS_HSV_HUERANGE_ON 
			affectMult = (rgbShifted.r >= minRange && rgbShifted.r <= maxRange ? 1.0 : 0.0);
	        //affectMult = step(minRange, rgbShifted.r) * step(rgbShifted.r, maxRange);
			#if PS_HSV_ALPHAMASK_ON
			affectMult = min(affectMult, rgba.a);
			#endif
		#else
			affectMult = rgba.a;
		#endif

		rgbShifted = hsv2rgb(rgbShifted + (- 1.0 + hsva.rgb * 2.0)); // this is because the RGB values range from 0 to 1 in the color but we expect -1 to +1 here.
		rgbShifted = rgba.rgb * (1.0 - affectMult) + rgbShifted * affectMult;

		#if PS_HSV_ALPHAMASK_ON 
			return half4(rgbShifted, hsva.a);
		#else
        	return half4(rgbShifted, rgba.a * hsva.a);
		#endif

	// No masking? Then just shift it
	#else
		return half4(shiftHSV(rgba.rgb, hsva.rgb), rgba.a * hsva.a);
	#endif
}