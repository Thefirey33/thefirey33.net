extends ColorRect


## Should the square be rendered?
var render_square = false

## The size of the square.
const square_size = 30

func _draw() -> void:
	for x in range(0, self.size.x, square_size):
		render_square = not render_square
		
		for y in range(0, self.size.y, square_size):
			if render_square:
				draw_rect(Rect2(x, y, square_size, square_size), Color.LIGHT_GRAY)
				
			render_square = not render_square
