namespace Sandbox;

public class LightCookieCopierComponent : BaseComponent
{
	[Property] public SpotLightComponent FromSpotLight { get; set; }
	[Property] public SpotLightComponent ToSpotLight { get; set; }
	[Property] public bool ShouldCopyData { get; set; } = false;
	[Property] public float DataCopyInterval { get; set; } = 1f;

	private RealTimeSince _lastDataCopyTime { get; set; }

	public override void Update()
	{
		// There's nothing to update.
		if ( FromSpotLight.Cookie is null && ToSpotLight.Cookie is null )
			return;

		// ToSpotLight doesn't have a cookie, or it's the wrong size.
		if ( ToSpotLight.Cookie is null || ToSpotLight.Cookie.Size != FromSpotLight.Cookie.Size )
		{
			Log.Info( $"({GameObject.Name}) Copying light cookie from ({FromSpotLight.GameObject.Name}) to ({ToSpotLight.GameObject.Name})" );
			var fromSize = FromSpotLight.Cookie.Size;
			ToSpotLight.Cookie = Texture.Create( (int)fromSize.x, (int)fromSize.y )
				.WithUAVBinding()
				.Finish();
		}

		if ( !ShouldCopyData || _lastDataCopyTime < DataCopyInterval )
			return;

		Log.Info( $"({GameObject.Name}) Copying light cookie texture data from ({FromSpotLight.GameObject.Name}) to ({ToSpotLight.GameObject.Name})" );
		_lastDataCopyTime = 0f;
		ToSpotLight.Cookie.Update( FromSpotLight.Cookie.GetPixels() );
	}
}
