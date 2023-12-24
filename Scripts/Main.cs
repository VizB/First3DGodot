using Godot;

namespace First3DGodot.Scripts;

public partial class Main : Node3D
{
    private Player _player;
    private Label _debugLabel;
    
    public override void _Ready()
    {
        _player = GetNode<Player>("Player");
        _debugLabel = GetNode<Label>("Control/Debug");
    }

    public override void _Process(double delta)
    {
        _debugLabel.Text = $"""
                            FPS: {Engine.GetFramesPerSecond()}
                            isSprinting: {_player.Speed == _player.RunningSpeed}
                            playerVelocity: {_player.Velocity}
                            xPosition: {_player.Position.X}
                            yPosition: {_player.Position.Y}
                            zPosition: {_player.Position.Z}
                            camMode: {(_player.IsFirstPerson ? "First Person" : "Third Person")}
                            """;
    }
}
