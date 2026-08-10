extends VBoxContainer



func _on_profile_picture_on_auth_ready() -> void:
	# Renable the elements if the authorization was a success.
	
	$continue_account.disabled = false
	$log_out_account.disabled = false
	$"../loading_spinner".hide()
