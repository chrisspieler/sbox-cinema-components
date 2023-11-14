using System.Threading.Tasks;

namespace Sandbox;

[Title( "Video Player" )]
[Category( "Light" )]
[Icon( "videocam", "red", "white" )]
public class VideoPlayerComponent : BaseComponent
{
	[Property]
	public SpotLightComponent ProjectorLight { get; set; }
	[Property, ResourceType( "mp4" )]
	public string VideoPath { get; set; }
	protected bool IsInitializing { get; set; }
	public virtual bool VideoLoaded { get; protected set; }
	public virtual bool AudioLoaded { get; protected set; }
	protected VideoPlayer VideoPlayer;
	protected Texture VideoTexture;
	protected TimeSince VideoLastUpdated { get; set; }

	public override void OnStart()
	{
		base.OnStart();

		IsInitializing = true;

		PlayFile( VideoPath );
		WaitUntilReady();
	}

	protected virtual void OnTextureData( ReadOnlySpan<byte> span, Vector2 size )
	{
		if ( !VideoLoaded )
			Log.Info( $"Video is now loaded: {size.x}x{size.y}" );

		if ( VideoTexture == null || VideoTexture.Size != size )
		{
			InitializeTexture( size );
		}
		VideoTexture.Update( span, 0, 0, (int)size.x, (int)size.y );
		VideoLoaded = true;
		VideoLastUpdated = 0;
	}

	protected virtual void InitializeTexture( Vector2 size )
	{
		Log.Info( "Initializing texture." );
		VideoTexture?.Dispose();
		VideoTexture = Texture.Create( (int)size.x, (int)size.y, ImageFormat.RGBA8888 )
									.WithName( "direct-video-player-texture" )
									.WithDynamicUsage()
									.WithUAVBinding()
									.Finish();
		ProjectorLight.Cookie = VideoTexture;
	}

	protected virtual async Task WaitUntilReady()
	{
		if ( !IsInitializing )
			return;

		while ( !(VideoLoaded && AudioLoaded) )
		{
			await GameTask.DelaySeconds( Time.Delta );
		}
		Log.Info( "Finished initializing" );
		IsInitializing = false;
	}

	public override void Update()
	{
		base.Update();

		VideoPlayer?.Present();
	}

	public virtual void Stop()
	{
		if ( VideoPlayer == null )
			return;

		// CurrentlyPlayingAudio?.Stop( true );
		AudioLoaded = false;
		VideoPlayer.Stop();
		VideoPlayer.Dispose();
		VideoLoaded = false;
		VideoPlayer = null;
	}

	protected virtual void PlayFile( string filePath )
	{
		VideoPlayer = new VideoPlayer();
		VideoPlayer.OnAudioReady += () =>
		{
			Log.Info( "Audio loaded." );
			AudioLoaded = true;
		};
		VideoPlayer.OnTextureData += OnTextureData;
		VideoPlayer.Play( FileSystem.Mounted, filePath );
	}
}
