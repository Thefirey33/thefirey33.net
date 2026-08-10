extends PanelContainer


func _ready() -> void:
	# Attempt to get the current authorization state of the user.
	$profile_picture/authentication_request.request(
		HTTPRequestHandler.combine_uri(HTTPRequestHandler.origin, "/api/AuthManager/auth")
	)

## Is the user ready to continue to catpetterz?
signal on_auth_ready

## The profile picture size (width and height) together.
const profile_picture_size = 64

func set_user_text():
	$profile_picture/username_description.text = Global.current_user["username"]
	on_auth_ready.emit()

func _on_authorize_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	Global.current_user = HTTPRequestHandler.parse_to_json(body)
	
	if response_code != 200:
		$profile_picture/username_description.text = "Guest"
		on_auth_ready.emit()
		return
	
	var profile_picture_url: String = Global.current_user["avatar_url"]
	# Set the username and profile picture accordingly.
	
	if not profile_picture_url.is_empty():
		$profile_picture/profile_picture_request.request(profile_picture_url)
	else:
		set_user_text()

func _on_profile_picture_request_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	if result != HTTPRequest.RESULT_SUCCESS:
		push_warning("Failed to fetch profile picture, skipping!")
		return
		
	var image = Image.new()
	
	# Load the profile picture image from the body.
	image.load_png_from_buffer(body)
	image.resize(profile_picture_size, profile_picture_size)
	
	var image_texture = ImageTexture.create_from_image(image)
	$profile_picture.texture = image_texture
	
	set_user_text()
