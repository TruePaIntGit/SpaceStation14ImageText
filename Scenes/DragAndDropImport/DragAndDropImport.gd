extends Control

var image
var _output
var _textCount
var _isGrayPhoto = false
var _erors
var colorpicer
var HeadSize

# Called when the node enters the scene tree for the first time.
func _ready():
	#JavaScriptBridge.eval("ysdk.adv.showFullscreenAdv")
	get_viewport().files_dropped.connect(on_files_dropped)
	_output = get_node("VBoxContainer/Panel2/TextEdit")
	_textCount = get_node("Panel/VBoxContainer/TextCount")
	_erors = get_node("VBoxContainer/Panel/erors")
	colorpicer = get_node("Panel/VBoxContainer/ColorButton/ColorPickerButton")
	HeadSize = get_node("Panel/VBoxContainer/Head/HeadSize")

func on_files_dropped(files):
	if is_file_image(files[0]):
		_erors.text=""
		image = Image.load_from_file(files[0])
		var texture = ImageTexture.create_from_image(image)
		get_node("VBoxContainer/Panel/Panel/Image").texture=texture
		if _isGrayPhoto:
			TextImageGeneratorGray()
		else:
			TextImageGenerator()
	else: _erors.text="ОШИБКА: Расширение не поддерживается"

func TextImageGeneratorGray():
	_erors.text=""
	var y = image.get_width()
	var x = image.get_height()
	#print(y)
	var count_povtor=1
	var colors
	var text=""
	for i in range(x):
		count_povtor=1
		for f in range(y):
			colors = image.get_pixel(f,i)
			if (colors[0]+colors[1]+colors[2])>1.5:
				text+="░░"
			else: text+="██"
		text+='\n'
	#text = text.replace('#000000', '#0')
	#text = text.replace('#ffffff','#fff')
	_output.text=text
	var Count=text.length()
	_textCount.text = str(Count)+"/6000"
	if Count>6000:
		_erors.text="Слишком много символов. Этот текст не поместится на листе"

func TextImageGenerator():
	_erors.text=""
	var y = image.get_width()
	var x = image.get_height()
	#print(y)
	var count_povtor=1
	var colors
	var text=""
	for i in range(x):
		count_povtor=1
		for f in range(y):
			colors = image.get_pixel(f,i).to_html(false)
			if (f<(y-1)):
				if ((f==0)and(i>0)and(colors==image.get_pixel(f,i-1).to_html(false))):
					text+= "██"
				elif (colors==image.get_pixel(f+1,i).to_html(false)):
					#print("yesItsWork")
					count_povtor+=1
				else:
					text+="[color=#"+colors+"]"+repeat_(count_povtor)
					count_povtor=1
			else:
				text+="[color=#"+colors+"]"+repeat_(count_povtor)
				count_povtor=1
		text+='\n'
	text = text.replace('#000000', '#0')
	text = text.replace('#ffffff','#fff')
	text+="[color=#0]"
	_output.text=text
	var Count=text.length()
	_textCount.text = str(Count)+"/6000"
	if Count>6000:
		_erors.text="Слишком много символов. Этот текст не поместится на листе"

func repeat_(count):
	var result=""
	for i in range(count):
		result+="██"
	return result

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func is_file_image(files):
	var formats = [".png",".jpeg", ".jpg"]
	for i in range(5):
		if files.ends_with(formats[i]):
			#print("image")
			return true
	return false


func _on_text_edit_text_changed():
	#print(_output.get_caret_column())
	#get_caret_line
	_textCount.text = str(_output.text.length())+"/6000"





func _on_gray_photo_toggled(button_pressed):
	if _isGrayPhoto:
		_isGrayPhoto=false
	else: _isGrayPhoto=true


func _on_color_button_pressed():
	var pos=_output.get_caret_wrap_index()
	_output.insert_text_at_caret("[color=#"+str(colorpicer.color.to_html(false))+"][/color]",pos)
	#_output.insert_text_at_cursor("[color=#"+str(colorpicer.color.to_html(false))+"][/color]")


func _on_bold_pressed():
	var pos=_output.get_caret_wrap_index()
	_output.insert_text_at_caret("[bold][/bold]",pos)


func _on_head_pressed():
	var pos=_output.get_caret_wrap_index()
	_output.insert_text_at_caret("[head="+HeadSize.text+"][/head]",pos)


func _on_italic_pressed():
	var pos=_output.get_caret_wrap_index()
	_output.insert_text_at_caret("[italic][/italic]",pos)


func _on_bold_italic_pressed():
	var pos=_output.get_caret_wrap_index()
	_output.insert_text_at_caret("[bullet]",pos)


func _on_select_pressed():
	_output.select_all()
	_output.copy()
