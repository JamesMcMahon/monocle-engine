using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Monocle
{
	public class PixelFont
	{

		public class Char
		{
			public int X;
			public int Y;
			public int Width;
			public int Height;
			public int XOffset;
			public int YOffset;
			public int XAdvance;

			public Char(int x, int y, int width, int height, int xOffset, int yOffset, int xAdvance)
			{
				X = x;
				Y = y;
				Width = width;
				Height = height;
				XOffset = xOffset;
				YOffset = yOffset;
				XAdvance = xAdvance;
			}
		}

		public MTexture Texture { get; private set; }
		public Dictionary<int, Char> CharData { get; private set; }
		public int LineHeight { get; private set; }

		public PixelFont(XmlElement data, MTexture texture)
		{
			Texture = texture;

			CharData = new Dictionary<int, Char>();
			foreach (XmlElement character in data["chars"])
			{
				CharData.Add(character.AttrInt("id"),
					new Char(character.AttrInt("x"), character.AttrInt("y"),
								character.AttrInt("width"), character.AttrInt("height"),
								character.AttrInt("xoffset"), character.AttrInt("yoffset"), character.AttrInt("xadvance")));
			}
			LineHeight = data["common"].AttrInt("lineHeight");
		}

		public Char Get(char id)
		{
			Char val = null;
			if (CharData.TryGetValue((int)id, out val))
				return val;
			return null;
		}

		public int WidthOf(string str)
		{
			int width = 0;
			for (int i = 0; i < str.Length; i++)
				if (CharData.ContainsKey((int)str[i]))
					width += CharData[(int)str[i]].XAdvance;
			return width;
		}

		public int HeightOf(string str)
		{
			int lines = str.Split('\n').Length;
			return lines * LineHeight;
		}

		public string AutoNewLine(int maxWidth, string str)
		{
			// create the line array
			List<string> lines = new List<string>();
			int line = 0;
			lines.Add("");

			// add each word to the line array
			string[] words = str.Split(' ');
			for (int i = 0; i < words.Length; i++)
			{
				if (WidthOf(lines[line]) + WidthOf(words[i]) > maxWidth)
				{
					lines.Add(words[i] + ' ');
					line++;
				}
				else
					lines[line] += words[i] + ' ';
			}

			// build resulting string with new lines
			string result = "";
			for (int i = 0; i < lines.Count; i++)
				result += lines[i] + (i < lines.Count - 1 ? "\n" : "");

			// return
			return result;
		}

	}
}
