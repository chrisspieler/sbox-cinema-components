namespace Sandbox;

public class LightCookieGeneratorComponent : BaseComponent
{
	[Property] public SpotLightComponent SpotLight { get; set; }
	/// <summary>
	/// Amount of time in seconds between each cookie generation.
	/// </summary>
	[Property] public float GenerationInterval { get; set; } = 1f;
	[Property] public Vector2 CookieSize { get; set; } = new Vector2( 64 );
	private RealTimeSince _lastGenerationTime { get; set; }

	public override void Update()
	{
		base.Update();

		if (_lastGenerationTime > GenerationInterval)
		{
			_lastGenerationTime = 0f;
			SpotLight.Cookie = GenerateCookie();
		}
	}

	private Texture GenerateCookie()
	{
		var cookie = Texture.Create( (int)CookieSize.x, (int)CookieSize.y )
			.WithUAVBinding()
			.Finish();
		var data = new Color32[(int)CookieSize.x * (int)CookieSize.y];
		for(int i = 0; i < data.Length; i++ )
		{
			data[i] = Color.Random;
		}
		cookie.Update( data );
		return cookie;
	}
}
