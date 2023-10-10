using Godot;
using System;
using System.Collections.Generic;
using System.IO;


public partial class DragAndDropImport : Control
{
	private Sprite2D _image;
	private Label _textLabel;
	private TextEdit _output;
	private CheckButton _chekiflightblack;
	private CheckButton _chekifgrayphoto;
	private Label _textcount;
	private bool IsLightBlack;
	private bool IsGrayPhoto;
	private String TextImage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsLightBlack = true;
		IsGrayPhoto = false;
		_textLabel = GetNode<Label>("VBoxContainer/Panel/Text");    // Get the text label node
		_image = GetNode<Sprite2D>("VBoxContainer/Panel/Panel/Image");    // Get the Sprite2D image node
		_output = GetNode<TextEdit>("VBoxContainer/Panel2/TextEdit");
		_chekiflightblack = GetNode<CheckButton>("Panel/VBoxContainer/LightBlack");
		_chekifgrayphoto = GetNode<CheckButton>("Panel/VBoxContainer/GrayPhoto");
		_textcount=GetNode<Label>("Panel/VBoxContainer/TextCount");
		ConnectFilesDroppedSignal();
	}

	private bool FileIsText(string fileName)
	{
		// If the file name ends with .txt
		if (Path.GetExtension(fileName).ToLower().Equals(".txt"))
		{
			return true;    // Return true, it's a text file
		}
		// if not, return false
		return false;
	}

	private bool FileIsImage(string fileName)
	{
		// The list of valid image extensions
		List<string> imageExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".bmp", ".svg", ".svgz", ".tga", ".webp" };

		// Loop through the valid extensions
		foreach (var image in imageExtensions)
		{
			// If the file name ends with one of the extensions
			if (Path.GetExtension(fileName).ToLower().Equals(image))
			{
				return true;    // return true
			}
		}
		// Return false, the file is not an image
		return false;
	}


	private void ConnectFilesDroppedSignal()
	{
		var root = GetTree().Root;              // Get the root node
		root.FilesDropped += OnFilesDropped;    // Run the OnFilesDropped method whenever the signal is triggered
	}

	private void LoadSprite2DTextureFromDisk(string imagePath, Sprite2D sprite)
	{
		Image img = new Image();

		// If the image could be loaded
		if (img.Load(imagePath) == Error.Ok)
		{
			if(IsGrayPhoto)
			{
				TextImageGeneratorGray(imagePath);
			}
			else{TextImageGenerator(imagePath);}
			var texture = ImageTexture.CreateFromImage(img);            // Create a texture from the image
			sprite.Texture = texture;                                   // Set the sprite texture
			sprite.Centered = false;                                    // Make the sprite draw from the top-left corner
		}
		// If the image loading failed
		else
		{
			_textLabel.Text = "Could not load the image:" + imagePath;  // Show the user what went wrong
		}
	}
	
	private void TextImageGeneratorGray(string imagePath)
	{
		Image img = new Image();
		img.Load(imagePath);
		int y = img.GetSize()[0];
		int x = img.GetSize()[1];
		var ColorPixel=new Color();
		string text = new String("");
		for(int i=0; i<x;i++)
		{
			for (int f=0; f<y;f++)
			{
				ColorPixel=img.GetPixel(f,i);
				if ((ColorPixel[0]+ColorPixel[1]+ColorPixel[2])>1.5)
				{
					text+="░░";
				}
				else
				{
					text+="██";
				}
			}
			text+="\n";
		}
		_output.Text = text;
		_textcount.Text = (text.Length+"/6000");
	}
	
	private void TextImageGenerator(string imagePath)
	{
		Image img = new Image();
		img.Load(imagePath);
		int y = img.GetSize()[0];
		int x = img.GetSize()[1];
		int shirina = x-1;
		int count_povtor=1;
		var ColorPixel=new Color();
		var ColorPixelNext=new Color();
		string text = new String("");
		string str = "";
		
		
		//GD.Print(_chekiflightblack.Pressed);
		
		for(int i=0; i<x;i++)
		{
			count_povtor = 1;
			for (int f=0; f<y;f++)
			{
				ColorPixel=img.GetPixel(f,i);
				if(((ColorPixel[0]+ColorPixel[1]+ColorPixel[2])==0)&&(IsLightBlack))
				{
					text+="██";
					continue;
				}
				if(i<shirina)
				{
					ColorPixelNext=img.GetPixel(f,i+1);
					if((ColorPixel[0]==ColorPixelNext[0])&&(ColorPixel[1]==ColorPixelNext[1])&&(ColorPixel[2]==ColorPixelNext[2]))
					{
						count_povtor+=1;
					}
					else
					{
						for(int g=1;g<=count_povtor;g++)
						{
							str+="██";
						}
						text+="[color="+'#'+ColorPixel.ToHtml(false)+']'+str+"[/color]";
						count_povtor=1;
						str="";
					}
				}
				
				else
				{
					for(int g=0;g<count_povtor;g++)
					{
						str+="██";
					}
					text+="[color="+'#'+ColorPixel.ToHtml(false)+']'+str+"[/color]";;
					str="";
				}
			}
			text+="\n";
		}
		//GD.Print(text);
		//GD.Print(text.Length);
		_textcount.Text = (text.Length+"/6000");
		_output.Text = text;
	}
	
	public void OnFilesDropped(String[] files)
	{
		_textLabel.Text = string.Empty;                     // Clear the text whenever a file is dropped
		// If the file is a text file
		if (FileIsText(files[0]))
		{
			string text = File.ReadAllText(files[0]);       // Read all the text and store it as string
			_textLabel.Text = text;                         // Set the label text
		}
		// If the file is an image
		else if (FileIsImage(files[0]))
		{
			LoadSprite2DTextureFromDisk(files[0], _image);   // Load the texture from disk and apply it to the Sprite2D node
		}
		else
		{
			// Tell the user that the file type is not supported
			_textLabel.Text = "\"" + Path.GetFileName(files[0]) + "\"" + " Is not a .txt or supported image file";
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void _on_light_black_toggled(bool button_pressed)
	{
		if(IsLightBlack)
		{
			IsLightBlack=false;
		}
		else
		{
			IsLightBlack=true;
		}
	}
	private void _on_gray_photo_toggled(bool button_pressed)
	{
		if(IsGrayPhoto){IsGrayPhoto=false;}
		else{IsGrayPhoto=true;}
	}
	private void _on_text_edit_text_changed()
	{
		String del = _output.Text;
		_textcount.Text =(del.Length+"/6000");
	}
}
