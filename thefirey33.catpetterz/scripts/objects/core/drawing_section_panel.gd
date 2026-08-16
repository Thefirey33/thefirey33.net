extends TextureRect
class_name CatDrawingPanel

## The canvas image
@export
var canvas_image: Image

## Image Texture 
var image_texture := ImageTexture.new()

## The maximum size of the image.
const maximum_image_size := 256.0

## The size of the brush.
var brush_size := 3.0

func _init() -> void:
	# shut the fuck up linter about float to int conversion
	var i_maximum_image_size := int(maximum_image_size)

	self.canvas_image = Image.create_empty(i_maximum_image_size, i_maximum_image_size, false, Image.FORMAT_RGBA8)
	self.image_texture = ImageTexture.create_from_image(canvas_image)
	self.texture = image_texture


## The before mouse X position of the line that will be created.
var before_mouse_x: float = 0

## The before mouse Y position of the line that will be created.
var before_mouse_y: float = 0

func draw_line_on_image(image: Image, p1: Vector2i, p2: Vector2i, color: Color):
	var x: int = p1.x
	var y: int = p1.y
	var dx = abs(p2.x - p1.x)
	var dy = abs(p2.y - p1.y)
	var sx: int = 1 if p1.x < p2.x else -1
	var sy: int = 1 if p1.y < p2.y else -1
	var err = dx - dy

	while true:
		image.fill_rect(Rect2(x, y, brush_size, brush_size), color)
		if x == p2.x and y == p2.y: break
		var e2 = 2 * err
		if e2 > -dy:
			err -= dy
			x += sx
		if e2 < dx:
			err += dx
			y += sy

## Represents the different brush modes that the user can use.
enum COLOR_MODES {
	BRUSH,
	ERASE,
	LASSO
}

## The current mode of the brush.
static var current_mode := COLOR_MODES.BRUSH

func get_color():
	if current_mode == COLOR_MODES.ERASE:
		return Color.TRANSPARENT
	return $"../tools/MarginContainer/tool_container/color_picker".color

func _process(_delta: float) -> void:

	match self.current_mode:
		COLOR_MODES.BRUSH, COLOR_MODES.ERASE:
			if self.is_input_down:
				self.draw_line_on_image(self.canvas_image, Vector2(image_mouse_x, image_mouse_y),
						Vector2(before_mouse_x, before_mouse_y), get_color())
				self.image_texture.update(self.canvas_image)

	before_mouse_x = image_mouse_x
	before_mouse_y = image_mouse_y

	queue_redraw()

## Is the mouse held down?
var is_input_down := false

## The default font for drawing the Mouse POSxPOS.
var font := ThemeDB.get_default_theme().default_font

## The X position of the brush relative to the image.
var image_mouse_x: float

## The Y position of the brush relative to the image.
var image_mouse_y: float

## The image that renders the outline.
var outline_rendering_image := Image.new()

## The outline for the brushes.
const outline_size: int = 1

func draw_outline_square(origin: Vector2, size_outline: Vector2, start_outline = true):
	if start_outline:
		outline_rendering_image.copy_from(self.canvas_image)
		
	outline_rendering_image.fill_rect(Rect2(origin.x, origin.y, size_outline.x, outline_size), Color.BLACK)
	outline_rendering_image.fill_rect(Rect2(origin.x, origin.y + size_outline.y, size_outline.x, outline_size), Color.BLACK)
	outline_rendering_image.fill_rect(Rect2(origin.x, origin.y + outline_size, outline_size, size_outline.y), Color.BLACK)
	outline_rendering_image.fill_rect(Rect2(origin.x + size_outline.x, origin.y, outline_size, size_outline.y+ outline_size), Color.BLACK)
	self.image_texture.update(outline_rendering_image)

func _draw() -> void:
	var mouse_pos = get_local_mouse_position()
	mouse_pos = mouse_pos.clamp(Vector2.ZERO, self.size)

	var centered_brush_size = brush_size / 2
	image_mouse_x = ((mouse_pos.x / size.x) * maximum_image_size) - centered_brush_size
	image_mouse_y = ((mouse_pos.y / size.y) * maximum_image_size) - centered_brush_size

	var scaled_pos = Rect2(Vector2.ZERO, self.size)
	var is_show = Input.mouse_mode == Input.MOUSE_MODE_HIDDEN
	var brush_size_modified = 1.0 if current_mode == COLOR_MODES.LASSO else brush_size
	
	if is_show:
		draw_outline_square(Vector2(image_mouse_x, image_mouse_y), Vector2(brush_size_modified, brush_size_modified))
		
	match current_mode:
		COLOR_MODES.LASSO:
			if selection_ongoing:
				selection_end = Vector2(image_mouse_x, image_mouse_y)
				
			draw_outline_square(selection_start, selection_end - selection_start, !is_show)

	draw_texture_rect(self.image_texture, scaled_pos, false)
	draw_string(font, Vector2(0, size.y - font.get_height() / 4), "Mouse %dx%d" % [mouse_pos.x, mouse_pos.y],
			HORIZONTAL_ALIGNMENT_LEFT, -1, 16, Color.BLACK)

## The start of the selection with the Lasso tool.
var selection_start := Vector2.ZERO

## The start of the selection with the Lasso tool.
var selection_end := Vector2.ZERO

## If the selection is ongoing.
var selection_ongoing := false

func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		is_input_down = event.button_index == MouseButton.MOUSE_BUTTON_LEFT and event.pressed
		
		if current_mode == COLOR_MODES.LASSO and event.button_index == MouseButton.MOUSE_BUTTON_LEFT:
			if event.pressed:
				selection_start = Vector2(image_mouse_x, image_mouse_y)
				selection_ongoing = true
			else:
				selection_ongoing = false



func _on_mouse_entered() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_HIDDEN


func _on_mouse_exited() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	self.image_texture.update(self.canvas_image)


func _on_h_slider_value_changed(value: float) -> void:
	brush_size = value


func _on_delete_selection_button_down() -> void:
	var size_selection := selection_end - selection_start
	
	# Clear the selection.
	self.canvas_image.fill_rect(Rect2(selection_start.x, selection_start.y, size_selection.x, size_selection.y), Color.TRANSPARENT)
	self.image_texture.update(self.canvas_image)
