extends Area2D

var enabled = true;

func _input(event):
	if(event.is_action_pressed("interaction") and len(get_overlapping_bodies()) > 0 and enabled):
		enabled = false
		
		if get_tree().get_root().get_node("Miasto/Player").getFabular()==0:
			setLines(0, 9)
			find_and_use_dialogue()
			get_tree().get_root().get_node("Miasto").onPortal("DesertPortal")
			get_tree().get_root().get_node("Miasto/Player").updateFabular(1)
			return
		elif get_tree().get_root().get_node("Miasto/Player").getFabular()==1:
			if not get_tree().get_root().get_node("Miasto/Player").isKilledDesertBoss():
				setLines(10, 10)
			else:
				setLines(11, 18)
				get_tree().get_root().get_node("Miasto").onPortal("CementaryPortal")
				get_tree().get_root().get_node("Miasto/Player").updateFabular(2)
				
			find_and_use_dialogue()
			return
		elif get_tree().get_root().get_node("Miasto/Player").getFabular()==2:
			if not get_tree().get_root().get_node("Miasto/Player").isKilledCemetaryBoss():
				setLines(19, 19)
			else:
				setLines(20, 25)
				get_tree().get_root().get_node("Miasto").onPortal("JunglePortal")
				get_tree().get_root().get_node("Miasto/Player").updateFabular(3)
				
			find_and_use_dialogue()
			return
		elif get_tree().get_root().get_node("Miasto/Player").getFabular()==3:
			if not get_tree().get_root().get_node("Miasto/Player").isKilledJungleBoss():
				setLines(26, 26)
			else:
				setLines(27, 29)
				get_tree().get_root().get_node("Miasto").offStone()
				get_tree().get_root().get_node("Miasto/Player").updateFabular(4)
				
			find_and_use_dialogue()
			return
		elif get_tree().get_root().get_node("Miasto/Player").getFabular()==4:
			setLines(30, 30)
				
			find_and_use_dialogue()
			return
		else:
			enabled = true

func _ready():
	setLines(0, 0)
	pass
	
func enable():
	enabled = true
	
func setLines(start, end):
	var dialogue_player = get_node_or_null("DialoguePlayer")

	if(dialogue_player):
		dialogue_player.change_lines(start, end)
	
	
func find_and_use_dialogue():
	var dialogue_player = get_node_or_null("DialoguePlayer")

	if(dialogue_player):
		dialogue_player.play("King")
		
