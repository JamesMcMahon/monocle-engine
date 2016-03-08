﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
	public class PixelText : Component
	{

		private struct Char
		{
			public Vector2 Offset;
			public Rectangle Bounds;
		}

		private List<Char> characters = new List<Char>();
		private PixelFont font;
		private string text;
		private bool dirty;

		public PixelFont Font
		{
			get { return font; }
			set
			{
				if (value != font)
					dirty = true;
				font = value;
			}
		}

		public string Text
		{
			get { return text; }
			set
			{
				if (value != text)
					dirty = true;
				text = value;
			}
		}

		public Vector2 Position = new Vector2();
		public Color Color = Color.White;
		public Vector2 Scale = Vector2.One;
		public int Width { get; private set; }
		public int Height { get; private set; }

		public PixelText(PixelFont font, string text, Color color) 
			: base(false, true)
		{
			Font = font;
			Text = text;
			Color = color;
			Text = text;
			Refresh();
		}

		public void Refresh()
		{
			dirty = false;
			characters.Clear();

			var widest = 0;
			var lines = 1;
			var offset = Vector2.Zero;

			for (int i = 0; i < text.Length; i++)
			{
				// new line
				if (text[i] == '\n')
				{
					offset.X = 0;
					offset.Y += font.LineHeight;
					lines++;
				}

				// add char
				var fontChar = font.Get(text[i]);
				if (fontChar != null)
				{
					characters.Add(new Char()
					{
						Offset = offset,
						Bounds = new Rectangle(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height)
					});

					if (offset.X > widest)
						widest = (int)offset.X;
					offset.X += fontChar.XAdvance;
				}
			}

			Width = widest;
			Height = lines * font.LineHeight;
		}

		public override void Render()
		{
			if (dirty)
				Refresh();

			for (var i = 0; i < characters.Count; i++)
				Draw.Texture(Font.Texture, characters[i].Bounds, Position + characters[i].Offset, Color);
		}

	}
}
