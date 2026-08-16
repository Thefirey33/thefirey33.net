extends HSplitContainer


## The minimum size of the dragged width.
const minimum_width := 450

func _on_dragged(offset: int) -> void:
	self.split_offset = min(self.split_offset, minimum_width)
