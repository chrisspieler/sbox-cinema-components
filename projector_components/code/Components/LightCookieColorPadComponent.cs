using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox;

public class LightCookieColorPadComponent : BaseComponent
{
	[Property] public Texture InputTexture { get; set; }
	[Property] public SpotLightComponent TargetSpotLight { get; set; }
	[Property, Range(0.2f, 1.0f)] 
	public float Margin { get; set; } = 0.93f;
	[Property]
	public Color PadColor { get; set; } = Color.Black;

	public override void Update()
	{
		base.Update();

		if ( InputTexture is null )
			return;

		if ( TargetSpotLight.Cookie is null )
			InitializeOutputTexture();

		var aspectRatio = TargetSpotLight.Cookie.Size.x / TargetSpotLight.Cookie.Size.y;
		ProjectorShaders.DispatchColorPad( InputTexture, TargetSpotLight.Cookie, PadColor, Margin, aspectRatio );
	}

	private void InitializeOutputTexture()
	{
		float largestDimension = Math.Max( InputTexture?.Width ?? 320, InputTexture?.Height ?? 320 );
		var lightCookieSize = new Vector2( largestDimension / Margin );
		TargetSpotLight.Cookie = Texture.Create( (int)lightCookieSize.x, (int)lightCookieSize.y )
			.WithUAVBinding()
			.Finish();
	}
}
