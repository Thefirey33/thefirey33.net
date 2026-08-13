extends TextureRect

## The canvas image
@export
var canvas_image: Image

## Image Texture 
var image_texture = ImageTexture.new()

## The maximum size of the image.
const maximum_image_size = 256.0

## The size of the brush.
var brush_size = 5.0

func _init() -> void:
	self.canvas_image = Image.create_empty(maximum_image_size, maximum_image_size, false, Image.FORMAT_RGBA8)
	self.canvas_image.fill(Color.WHITE)
	
	self.image_texture = ImageTexture.create_from_image(canvas_image)
	self.texture = image_texture


## The before mouse X position of the line that will be created.
var before_mouse_x = 0

## The before mouse Y position of the line that will be created.
var before_mouse_y = 0

func draw_line_on_image(image: Image, p1: Vector2i, p2: Vector2i, color: Color):
	var x = p1.x
	var y = p1.y
	var dx = abs(p2.x - p1.x)
	var dy = abs(p2.y - p1.y)
	var sx = 1 if p1.x < p2.x else -1
	var sy = 1 if p1.y < p2.y else -1
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
	
func _process(_delta: float) -> void:

	var mouse_pos = get_local_mouse_position()
	var centered_brush_size = brush_size / 2
	var image_mouse_x = ((mouse_pos.x / size.x) * maximum_image_size) - centered_brush_size
	var image_mouse_y = ((mouse_pos.y / size.y) * maximum_image_size) - centered_brush_size
	
	if self.is_input_down:
		self.draw_line_on_image(self.canvas_image, Vector2i(image_mouse_x, image_mouse_y), Vector2i(before_mouse_x, before_mouse_y), $"../tools/MarginContainer/tool_container/color_selector".color)
		self.image_texture.update(self.canvas_image)
		
	before_mouse_x = image_mouse_x
	before_mouse_y = image_mouse_y
	
	queue_redraw()

## Is the mouse held down?
var is_input_down = false

var font = ThemeDB.get_default_theme().default_font

func _draw() -> void:
	var mouse_pos = get_local_mouse_position()
	mouse_pos = mouse_pos.clamp(Vector2.ZERO, self.size)
	draw_string(font, Vector2(0, size.y - font.get_height() / 4), "Mouse %dx%d" % [mouse_pos.x, mouse_pos.y], HORIZONTAL_ALIGNMENT_LEFT, -1, 16, Color.BLACK)

func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		is_input_down = event.button_index == MouseButton.MOUSE_BUTTON_LEFT and event.pressed
