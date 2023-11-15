namespace Sandbox;

public class LightCookieDebugComponent : BaseComponent
{
	[Property] public SpotLightComponent SpotLight { get; set; }

	public override void Update()
	{
		base.Update();

		Gizmo.Draw.Sprite( Transform.Position, new Vector2( 30 ), SpotLight.Cookie, true );
	}
}
