extends VBoxContainer

func _on_profile_picture_on_auth_ready() -> void:
	# Renable the elements if the authorization was a success.
	
	$continue_account.disabled = false
	$log_out_account.disabled = false
	LoadingSpinner.hide()
	
	# If the authentication was successful, connect to the Gateway.
	WebSocketGatewayHandler.start_websocket_connection()

func _on_profile_picture_on_auth_failure() -> void:
	# Hide the loading spinner element.
	LoadingSpinner.hide()
	pass
