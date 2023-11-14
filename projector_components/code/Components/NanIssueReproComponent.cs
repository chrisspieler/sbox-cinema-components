namespace Sandbox;

public class NanIssueReproComponent : BaseComponent
{
	public override void OnStart()
	{
		base.OnStart();

		//var defaultRotation = default( Rotation );
		//var identityRotation = Rotation.Identity;
		//Log.Info( $"default rotation: {defaultRotation}" );
		//Log.Info( $"inverse default rotation: {defaultRotation.Inverse}" );
		//Log.Info( $"identity rotation: {identityRotation}" );
		//Log.Info( $"inverse identity rotation: {identityRotation.Inverse}" );
		var exampleTransform = new Transform()
			.WithPosition( Vector3.Zero )
			.WithRotation( Rotation.Identity )
			.WithScale( 1f );

		Log.Info( $"example transform: {exampleTransform}" );
		var newTransform = exampleTransform.ToLocal( exampleTransform );
		Log.Info( $"new transform: {newTransform}" );
	}
}
