extends CanvasLayer

export(String, FILE, "*.json") var dialogue_file

var dialogues = []
var current_dialogue_id = -1
var start_id = -1
var end_id = 999
var is_active = false
var OwnerName = "none"

func _ready():
	$NinePatchRect.visible = false
	
func change_lines(start, end):
	if is_active == true:
		return
		
	start_id = start;
	end_id = end;

func play(text):
	
	OwnerName = text
	
	if is_active == true:
		return
	is_active = true
	dialogues = load_dialogue()
	
	turn_off_the_player()
	
	
	$NinePatchRect.visible = true
	current_dialogue_id = start_id-1
	next_line()
	
func _input(event):
	if not is_active:
		return
		
	if(event.is_action_pressed("interaction")):
		next_line()
		
func next_line():
	current_dialogue_id += 1
	
	if(current_dialogue_id >= len(dialogues) || current_dialogue_id>end_id):
		turn_on_the_player()
		$Timer.start()
		$NinePatchRect.visible = false;
		return
	
	$NinePatchRect/Name.text = dialogues[current_dialogue_id]['name']
	$NinePatchRect/Message.text = dialogues[current_dialogue_id]['text']
	
func load_dialogue():
	var file = File.new()
	if(file.file_exists(dialogue_file)):
		file.open(dialogue_file, file.READ)
		return parse_json(file.get_as_text())


func _on_Timer_timeout():
	get_tree().get_root().get_node("Miasto/"+OwnerName).enable()
	is_active = false

func turn_on_the_player():
	var player = get_tree().get_root().find_node("Player", true, false)
	if(player):
		player.set_active(true)

func turn_off_the_player():
	var player = get_tree().get_root().find_node("Player", true, false)
	if(player):
		player.set_active(false)
