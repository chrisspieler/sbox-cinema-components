namespace Sandbox;

public class LightCookieGeneratorComponent : BaseComponent
{
	[Property] public SpotLightComponent SpotLight { get; set; }
	/// <summary>
	/// Amount of time in seconds between each cookie generation.
	/// </summary>
	[Property] public float GenerationInterval { get; set; } = 1f;
	[Property] public Vector2 CookieSize { get; set; } = new Vector2( 64 );
	private RealTimeSince _lastGenerationTime { get; set; } = 0f;

	public override void Update()
	{
		base.Update();

		if (_lastGenerationTime > GenerationInterval)
		{
			_lastGenerationTime = 0f;
			UpdateCookie();
		}
	}

	private void UpdateCookie()
	{
		if ( SpotLight.Cookie == null )
		{
			SpotLight.Cookie = Texture.Create( (int)CookieSize.x, (int)CookieSize.y )
				.WithUAVBinding()
				.Finish();
		}
		// If we didn't create the cookie texture, its size might differ from CookieSize.
		var cookieSize = SpotLight.Cookie.Size;
		var data = new Color32[(int)cookieSize.x * (int)cookieSize.y];
		for(int i = 0; i < data.Length; i++ )
		{
			data[i] = Color.Random;
		}
		SpotLight.Cookie.Update( data );
	}
}
