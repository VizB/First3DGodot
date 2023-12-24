using Godot;

namespace First3DGodot.Scripts;
    
public partial class Player : CharacterBody3D 
{ 
    private float _speed = 5.0f;
    [Export] private float _walkingSpeed = 5.0f;
    [Export] private float _runningSpeed = 8.0f;
    [Export] private float _crouchingSpeed = 3.0f;
    [Export] private float _mouseSensitivity = 0.4f;
    [Export] private float _lerpSpeed = 10.0f;
    private const float JumpVelocity = 4.5f;
    private float _gravity;
    private Vector3 _direction = Vector3.Zero;
    private Node3D _head;
    private Marker3D _marker;
    // Cameras
    private Camera3D _firstPersonCam;
    private Camera3D _thirdPersonCam;
    [Export] private bool _isFirstPerson = true;
     
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _head = GetNode<Node3D>("Head");
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        
        //Set the camera to the first person view
        _firstPersonCam = GetNode<Camera3D>("Head/FirstPersonCam");
        _thirdPersonCam = GetNode<Camera3D>("Head/ThirdPersonCam");
        
        // Marker for the third person camera
        _marker = GetNode<Marker3D>("Head/ThirdPersonCamPos");
    }

    public override void _Input(InputEvent @event)
    {
        // Early returns for non-mouse events and when the mouse is visible
        if (@event is not InputEventMouseMotion mouseMotion) return;
        if(Input.MouseMode == Input.MouseModeEnum.Visible) return;
        
        // Rotate Head
        RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * _mouseSensitivity));
        _head.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * _mouseSensitivity));
        
        // Clamp Head Rotation
        var headRotation = _head.Rotation;
        headRotation.X = Mathf.Clamp(headRotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
        _head.Rotation = headRotation;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("escape"))
        {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible
                ? Input.MouseModeEnum.Captured
                : Input.MouseModeEnum.Visible;
        }

        if (Input.IsActionJustPressed("changeCam"))
        {
            if (_firstPersonCam.Current)
            {
                _thirdPersonCam.Current = true;
            }
            else
            {
                _firstPersonCam.Current = true;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        _speed = Input.IsActionPressed("sprint") ? _runningSpeed : _walkingSpeed;
        
        if (!IsOnFloor())
        {
            velocity.Y -= _gravity * (float) delta;
        }
        
        if(Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            GD.Print($"Jumping: {count++}");
            velocity.Y = JumpVelocity;
            GD.Print(JumpVelocity + " " + velocity.Y);
        }

        var inputDir = Input.GetVector("left", "right", "forward", "backward");
        /*
        _direction = Mathf.Lerp(_direction, Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y).Normalized(),
            (float)delta * _lerpSpeed);
        */
        _direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        _direction.Lerp(Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y).Normalized(), (float)delta * _lerpSpeed);
        
        if (_direction != Vector3.Zero)
        {
            velocity.X = _direction.X * _speed;
            velocity.Z = _direction.Z * _speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, _speed);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, _speed);
        }
        
        Position += velocity * (float) delta;
        MoveAndSlide();
    }
}