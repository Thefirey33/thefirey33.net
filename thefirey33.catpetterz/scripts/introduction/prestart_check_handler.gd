extends Button


func _on_pressed() -> void:
	# When the user attempts to continue with their account,
	# The game will check if they have any cats currently that they own.
	# If they do not, the cat creation page will be opened.
	LoadingSpinner.show()
	$create_first_cat_check.request(
		HTTPRequestHandler.combine_uri(HTTPRequestHandler.origin, "/api/Cat/any")
	)


func _on_create_first_cat_check_request_completed(_result: int, response_code: int, _headers: PackedStringArray, _body: PackedByteArray) -> void:
	# If the user has no cats that are found, then redirect them to the "Create Your First Cat!" page.
	if response_code != 200:
		get_tree().change_scene_to_file("res://scenes/introduction/onboarding_create_first_cat.tscn")
	else:
		get_tree().change_scene_to_file("res://scenes/main_menu.tscn")
	LoadingSpinner.hide()
