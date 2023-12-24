using Godot;

namespace First3DGodot.Scripts;
    
public partial class Player : CharacterBody3D 
{ 
    public float Speed = 5.0f;
    [Export] public float WalkingSpeed = 5.0f;
    [Export] public float RunningSpeed = 8.0f;
    [Export] public float CrouchingSpeed = 3.0f;
    [Export] public float MouseSensitivity = 0.4f;
    [Export] public float LerpSpeed = 10.0f;
    private const float JumpVelocity = 4.5f;
    private float _gravity;
    private Vector3 _direction = Vector3.Zero;
    private Node3D _head;
    private Marker3D _thirdPersonCamPos;
    private Marker3D _firstPersonCamPos;
    // Cameras
    private Camera3D _camera;
    [Export] public bool IsFirstPerson = true;
     
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        
        _head = GetNode<Node3D>("Head");
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        
        //Set the camera to the first person view
        _camera = GetNode<Camera3D>("Head/Camera");
        
        // Marker for the third person camera
        _thirdPersonCamPos = GetNode<Marker3D>("Head/ThirdPersonCamPos");
        
        // Marker for the first person camera
        _firstPersonCamPos = GetNode<Marker3D>("Head/FirstPersonCamPos");
    }

    public override void _Input(InputEvent @event)
    {
        // Early returns for non-mouse events and when the mouse is visible
        if (@event is not InputEventMouseMotion mouseMotion) return;
        if(Input.MouseMode == Input.MouseModeEnum.Visible) return;
        
        // Rotate Head
        RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * MouseSensitivity));
        _head.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * MouseSensitivity));
        
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
            if (IsFirstPerson)
            {
                _camera.Position = _thirdPersonCamPos.Position;
            } else
            {
                _camera.Position = _firstPersonCamPos.Position;
            }
            IsFirstPerson = !IsFirstPerson;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        Speed = Input.IsActionPressed("sprint") ? RunningSpeed : WalkingSpeed;
        
        if (!IsOnFloor())
        {
            velocity.Y -= _gravity * (float) delta;
        }
        
        if(Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            velocity.Y += JumpVelocity;
        }

        var inputDir = Input.GetVector("left", "right", "forward", "backward");
        /*
        _direction = Mathf.Lerp(_direction, Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y).Normalized(),
            (float)delta * _lerpSpeed);
        */
        _direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        _direction.Lerp(Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y).Normalized(), (float)delta * LerpSpeed);
        
        if (_direction != Vector3.Zero)
        {
            velocity.X = _direction.X * Speed;
            velocity.Z = _direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, Speed);
        }
        
        Velocity = velocity;
        
        /*
            Position += velocity * (float) delta;
            IDK WHY THIS DOESNT WORK????? ^^^^
        */
        MoveAndSlide();
    }
}