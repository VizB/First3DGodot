extends TextureRect

var screen_size

func _process(delta):
	screen_size = get_viewport_rect().size
	
	position = Vector2(screen_size.x / 2 - texture.get_width() / 2, screen_size.y / 2 - texture.get_height() / 2)
