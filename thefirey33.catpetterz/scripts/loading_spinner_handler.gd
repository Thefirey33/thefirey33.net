extends TextureRect

## The speed of the rotation of the loading spinner.
@export
var rotation_speed = 10.0

func _process(delta: float) -> void:
	# Rotate the loading spinner.
	self.rotation -= delta * rotation_speed
