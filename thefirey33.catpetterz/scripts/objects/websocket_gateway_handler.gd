extends TextureRect


@onready
var connection_nonready: Texture2D = load("res://sprites/connection/connection_nonready.png")

@onready
var connection_ready: Texture2D = load("res://sprites/connection/connection_ready.png")

var socket = WebSocketPeer.new()

func get_websocket_origin():
	var origin = HTTPRequestHandler.origin.replace("http://", "ws://")
	origin.replace("https://", "wss://")
	
	return origin
	
func _init() -> void:
	# Set the initial processing of the Gateway handler to false.
	
	set_process(false)

func start_websocket_connection() -> void:
	var err = socket.connect_to_url(
		HTTPRequestHandler.combine_uri(get_websocket_origin(), "updategateway")
	)
	
	# Checks if the WebSocket Connection was successful.
	if err == OK:
		print("Connecting to the CatPetterz Gateway, Please Wait...")
		set_process(true)
	else:
		print("Failed to connect to the Gateway!")
		set_process(false)
		
func _process(_delta: float) -> void:
	socket.poll()
	
	var state = socket.get_ready_state()
	if state == WebSocketPeer.STATE_CLOSED:
		print("Disconnected from Gateway with code %d" % socket.get_close_code())
		set_process(false)
	
