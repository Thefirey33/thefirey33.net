extends VBoxContainer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	LoadingSpinner.hide()
	pass


func _on_test_drawing_utility_pressed() -> void:
	get_tree().change_scene_to_file("res://objects/core/cat_drawing_panel.tscn")
