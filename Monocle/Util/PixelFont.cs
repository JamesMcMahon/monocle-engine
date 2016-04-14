using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Monocle
{
	public class PixelFont
	{
		public class CharData
		{
            public MTexture Texture;
			public int XOffset;
			public int YOffset;
			public int XAdvance;

			public CharData(MTexture texture, XmlElement xml)
			{
                Texture = texture.GetSubtexture(xml.AttrInt("x"), xml.AttrInt("y"), xml.AttrInt("width"), xml.AttrInt("height"));
                XOffset = xml.AttrInt("xoffset");
                YOffset = xml.AttrInt("yoffset");
                XAdvance = xml.AttrInt("xadvance");
			}
		}

		public MTexture Texture { get; private set; }
		public Dictionary<int, CharData> CharDatas { get; private set; }
		public int LineHeight { get; private set; }

		public PixelFont(XmlElement data, MTexture texture)
		{
			Texture = texture;

            CharDatas = new Dictionary<int, CharData>();
			foreach (XmlElement character in data["chars"])
                CharDatas.Add(character.AttrInt("id"), new CharData(texture, character));
			LineHeight = data["common"].AttrInt("lineHeight");
		}

		public CharData Get(char id)
		{
			CharData val = null;
			if (CharDatas.TryGetValue((int)id, out val))
				return val;
			return null;
		}

		public int WidthOf(string str)
		{
			int width = 0;
			for (int i = 0; i < str.Length; i++)
				if (CharDatas.ContainsKey((int)str[i]))
					width += CharDatas[(int)str[i]].XAdvance;
			return width;
		}

		public int HeightOf(string str)
		{
			int lines = str.Split('\n').Length;
			return lines * LineHeight;
		}

        public Vector2 Measure(string str)
        {
            Vector2 size = new Vector2(0, LineHeight);
            int lineWidth = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n')
                {
                    size.Y += LineHeight;

                    size.X = Math.Max(lineWidth, size.X);
                    lineWidth = 0;
                }
                else if (CharDatas.ContainsKey((int)str[i]))
                    lineWidth += CharDatas[(int)str[i]].XAdvance;
            }
            size.X = Math.Max(lineWidth, size.X);

            return size;
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
