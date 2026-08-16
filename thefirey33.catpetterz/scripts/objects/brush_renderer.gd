extends TextureButton

## The mode that this will be selected in.
@export
var mode_outline: int = CatDrawingPanel.COLOR_MODES.BRUSH

## The width of the selection outline.
@export
var select_outline: int = 3

## The target canvas that this object will target.
@export
var target_canvas: CatDrawingPanel

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	self.button_down.connect(func on_button_down():
		target_canvas.current_mode = mode_outline
	)


func _draw() -> void:
	if CatDrawingPanel.current_mode == mode_outline:
		draw_rect(Rect2(Vector2.ZERO, self.size), Color.BLACK, false, select_outline)

func _process(_delta: float) -> void:
	queue_redraw()
