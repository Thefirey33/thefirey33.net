extends Button


@export
var redirect_uri: String

func _ready() -> void:
	# Redirect to the Discord Authentication Page.
	LoadingSpinner.show()
	
	# If the origin wasn't specified, this is probably a NON-WEB build.
	if HTTPRequestHandler.origin == null:
		push_warning("Origin wasn't specified, skipping redirect link initialization...")
		return
	
	$redirect_link_request.request(HTTPRequestHandler.combine_uri(
			HTTPRequestHandler.origin, "/api/AuthManager/link?path=%s/api/DiscordLogin" % HTTPRequestHandler.origin
		)
	)
	

func _on_redirect_link_request_request_completed(_result: int, _response_code: int, _headers: PackedStringArray, body: PackedByteArray) -> void:
	# Set the redirection.
	var json_data = HTTPRequestHandler.parse_to_json(body)
	redirect_uri = json_data["url"]

func _on_pressed() -> void:
	# Change the location of the redirect to the specified location that will allow the user to gain the authorization.
	JavaScriptBridge.eval("window.location.href = \"%s\"" % redirect_uri)
