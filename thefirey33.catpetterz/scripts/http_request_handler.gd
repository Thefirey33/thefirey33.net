extends Node
class_name HTTPRequestHandler


## The origin of the current browser window.
static var origin = JavaScriptBridge.eval("window.location.origin")

## This combines two URLs together.
##
## @param absolute: The absolute of the URI.
## @param relative: The path to be added to the URI.
static func combine_uri(absolute: String, relative: String):
	if absolute.ends_with("/") and relative.begins_with("/"):
		return absolute + relative.trim_prefix("/")
	elif not relative.ends_with("/") and not relative.begins_with("/"):
		return absolute + "/" + relative

	return absolute + relative
	
## This parses the body of a request to JSON data.
## 
## @param body: The body of the request.
static func parse_to_json(body: PackedByteArray):
	var data = body.get_string_from_utf8()
	return JSON.parse_string(data)
