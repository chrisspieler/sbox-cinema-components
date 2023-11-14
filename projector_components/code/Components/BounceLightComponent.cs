namespace Sandbox;

[Title( "Bounce Light" )]
[Category( "Light" )]
[Icon( "call_missed_outgoing", "red", "white" )]
[EditorHandle( "materials/gizmo/spotlight.png" )]
public class BounceLightComponent : BaseComponent
{
	[Property] public SpotLightComponent ProjectorLight { get; set; }
	[Property] public float MaxDistance { get; set; } = 1024f;
	[Property] public float AttenuationFactor { get; set; } = 1f;
	/// <summary>
	/// Determines the size of the texture used as the light cookie of <c>BounceSpotlight</c>.
	/// A higher resolution may be more temporally stable, preventing some light flickering, but 
	/// also being less realistic.
	/// </summary>
	[Property] public static int BounceLightCookieSize { get; set; } = 32;
	[Property] public bool EnableDebugVis { get; set; }

	private SpotLightComponent BounceLight { get; set; }
	private float _screenDistanceFromProjector;
	private Vector3 _screenPosition;
	private Texture _downscaledTex { get; set; }
	private Texture _multiplicandTex { get; set; }
	private Texture _productTex { get; set; }


	public override void OnEnabled()
	{
		if ( BounceLight == null )
		{
			var bounceLightGo = new GameObject( true, "Bounce Light" );
			bounceLightGo.Parent = GameObject;
			BounceLight = bounceLightGo.AddComponent<SpotLightComponent>();
			BounceLight.Radius = 1024f;
			BounceLight.ConeOuter = 80f;
			BounceLight.ConeInner = 0f;
			BounceLight.Cookie = ProjectorLight.Cookie;
		}
		BounceLight.Enabled = true;

		InitGraphics();

		BounceLight.Enabled = false;
		BounceLight.Enabled = true;
	}

	public override void OnDisabled()
	{
		BounceLight.Enabled = false;
	}

	public override void Update()
	{
		var traceStart = ProjectorLight.Transform.Position;
		var traceEnd = traceStart + ProjectorLight.Transform.Rotation.Forward * MaxDistance;
		var tr = Scene.PhysicsWorld.Trace
			.Ray( traceStart, traceEnd )
			.Run();

		if ( !tr.Hit )
		{
			BounceLight.Enabled = false;
			return;
		}

		BounceLight.Enabled = true;
		BounceLight.Transform.Position = tr.HitPosition;
		BounceLight.Transform.Rotation = tr.Normal.EulerAngles.ToRotation();
		_screenDistanceFromProjector = tr.Distance;
		_screenPosition = tr.HitPosition;

		var attenuation = MathX.Remap(_screenDistanceFromProjector, 0f, MaxDistance, 0f, 1f);
		attenuation *= AttenuationFactor;
		BounceLight.Attenuation = attenuation;

		/* 
		*  Here we daisy-chain three compute shaders to downscale, multiply, and blur the
		*  main projector texture in order to create a fake bounce light effect.
		*  I am CERTAIN that this is not the most efficient way to do this, but it works
		*  for now, and someone with more HLSL knowledge could probably do it all in one shader.
		*/
		ProjectorShaders.DispatchDownscale( ProjectorLight.Cookie, _downscaledTex );
		ProjectorShaders.DispatchMultiply( _downscaledTex, _multiplicandTex, _productTex );
		ProjectorShaders.DispatchGaussianBlur( _productTex, BounceLight.Cookie );

		if ( EnableDebugVis )
			DebugDrawShaderTextures();
	}

	private void InitGraphics()
	{
		Texture createTexture() => Texture.Create( BounceLightCookieSize, BounceLightCookieSize)
			.WithUAVBinding()
			.WithFormat( ImageFormat.RGBA8888 )
			.WithDynamicUsage()
			.Finish();

		_downscaledTex = createTexture();
		_multiplicandTex = createTexture();
		_productTex = createTexture();
		BounceLight.Cookie = createTexture();

		// Downscale the mask texture to the size of the bounce light cookie
		// and write it to the multiplicand texture.
		var largeMaskTex = Texture.Load( FileSystem.Mounted, "materials/cookies/box_soft.vtex" );
		ProjectorShaders.DispatchDownscale( largeMaskTex, _multiplicandTex );
	}



	private void DebugDrawShaderTextures()
	{
		void DrawTexture(Texture tex, Vector3 pos )
		{
			Gizmo.Draw.Sprite( pos, new Vector2( 30 ), tex, true );
		}
		var x = _screenPosition.x.Approach(ProjectorLight.Transform.Position.x, 30f);
		var yBase = _screenPosition.y;
		var ySpacing = -30f;
		var z = _screenPosition.z - 50f;
		DrawTexture( ProjectorLight.Cookie, new Vector3( x, yBase, z ) );
		DrawTexture( _downscaledTex, new Vector3( x, yBase + ySpacing, z ) );
		DrawTexture( _productTex, new Vector3( x, yBase + ySpacing * 2, z) );
		DrawTexture( BounceLight.Cookie, new Vector3( x, yBase + ySpacing * 3, z ) );
	}
}
