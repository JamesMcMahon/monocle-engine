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

        #region Drawing

        public void Draw(string text, Vector2 position, Vector2 scale, Color color)
        {
            var offset = Vector2.Zero;
            for (int i = 0; i < text.Length; i++)
            {
                // new line
                if (text[i] == '\n')
                {
                    offset.X = 0;
                    offset.Y += LineHeight;
                }

                // add char
                var fontChar = Get(text[i]);
                if (fontChar != null)
                {
                    var pos = position + (offset + new Vector2(fontChar.XOffset, fontChar.YOffset)) * scale;
                    fontChar.Texture.Draw(Calc.Floor(pos), Vector2.Zero, color, scale);
                    offset.X += fontChar.XAdvance;
                }
            }
        }

        public void Draw(string text, Vector2 position, Color color)
        {
            Draw(text, position, Vector2.One, color);
        }

        public void Draw(string text, Vector2 position)
        {
            Draw(text, position, Vector2.One, Color.White);
        }

        public void DrawCentered(string text, Vector2 position, Vector2 scale, Color color)
        {
            Vector2 size = Measure(text);
            size.X *= scale.X;
            size.Y *= scale.Y;

            Draw(text, position - size * .5f, scale, color);
        }

        public void DrawCentered(string text, Vector2 position, Color color)
        {
            Vector2 size = Measure(text);
            Draw(text, position - size * .5f, Vector2.One, color);
        }

        public void DrawCentered(string text, Vector2 position)
        {
            Vector2 size = Measure(text);
            Draw(text, position - size * .5f, Vector2.One, Color.White);
        }

        public void DrawJustified(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            Vector2 offset = Measure(text);
            offset.X *= scale.X * -justify.X;
            offset.Y *= scale.Y * -justify.Y;

            Draw(text, position + offset, scale, color);
        }

        public void DrawJustified(string text, Vector2 position, Vector2 justify, Color color)
        {
            DrawJustified(text, position, justify, Vector2.One, color);
        }

        public void DrawJustified(string text, Vector2 position, Vector2 justify)
        {
            DrawJustified(text, position, justify, Vector2.One, Color.White);
        }

        #endregion

        #region Draw Outlined

        public void DrawOutlined(string text, Vector2 position, Vector2 scale, Color outline, Color color)
        {
            Draw(text, position + new Vector2(-1, 0), scale, outline);
            Draw(text, position + new Vector2(1, 0), scale, outline);
            Draw(text, position + new Vector2(0, -1), scale, outline);
            Draw(text, position + new Vector2(0, 1), scale, outline);
            Draw(text, position + new Vector2(-1, -1), scale, outline);
            Draw(text, position + new Vector2(1, 1), scale, outline);
            Draw(text, position + new Vector2(1, -1), scale, outline);
            Draw(text, position + new Vector2(-1, 1), scale, outline);
            Draw(text, position, scale, color);
        }

        public void DrawOutlined(string text, Vector2 position, Color outline, Color color)
        {
            DrawOutlined(text, position, Vector2.One, outline, color);
        }

        public void DrawOutlined(string text, Vector2 position, Color color)
        {
            DrawOutlined(text, position, Vector2.One, Color.Black, color);
        }

        public void DrawOutlined(string text, Vector2 position)
        {
            DrawOutlined(text, position, Vector2.One, Color.Black, Color.White);
        }

        public void DrawOutlinedCentered(string text, Vector2 position, Vector2 scale, Color outline, Color color)
        {
            DrawCentered(text, position + new Vector2(-1, 0), scale, outline);
            DrawCentered(text, position + new Vector2(1, 0), scale, outline);
            DrawCentered(text, position + new Vector2(0, -1), scale, outline);
            DrawCentered(text, position + new Vector2(0, 1), scale, outline);
            DrawCentered(text, position + new Vector2(-1, -1), scale, outline);
            DrawCentered(text, position + new Vector2(1, 1), scale, outline);
            DrawCentered(text, position + new Vector2(1, -1), scale, outline);
            DrawCentered(text, position + new Vector2(-1, 1), scale, outline);
            DrawCentered(text, position, scale, color);
        }

        public void DrawOutlinedCentered(string text, Vector2 position, Color outline, Color color)
        {
            DrawOutlinedCentered(text, position, Vector2.One, outline, color);
        }

        public void DrawOutlinedCentered(string text, Vector2 position, Color color)
        {
            DrawOutlinedCentered(text, position, Vector2.One, Color.Black, color);
        }

        public void DrawOutlinedCentered(string text, Vector2 position)
        {
            DrawOutlinedCentered(text, position, Vector2.One, Color.Black, Color.White);
        }

        public void DrawOutlinedJustified(string text, Vector2 position, Vector2 justify, Vector2 scale, Color outline, Color color)
        {
            DrawJustified(text, position + new Vector2(-1, 0), justify, scale, outline);
            DrawJustified(text, position + new Vector2(1, 0), justify, scale, outline);
            DrawJustified(text, position + new Vector2(0, -1), justify, scale, outline);
            DrawJustified(text, position + new Vector2(0, 1), justify, scale, outline);
            DrawJustified(text, position + new Vector2(-1, -1), justify, scale, outline);
            DrawJustified(text, position + new Vector2(1, 1), justify, scale, outline);
            DrawJustified(text, position + new Vector2(1, -1), justify, scale, outline);
            DrawJustified(text, position + new Vector2(-1, 1), justify, scale, outline);
            DrawJustified(text, position, justify, scale, color);
        }

        public void DrawOutlinedJustified(string text, Vector2 position, Vector2 justify, Color outline, Color color)
        {
            DrawOutlinedJustified(text, position, justify, Vector2.One, outline, color);
        }

        public void DrawOutlinedJustified(string text, Vector2 position, Vector2 justify, Color color)
        {
            DrawOutlinedJustified(text, position, justify, Vector2.One, Color.Black, color);
        }

        public void DrawOutlinedJustified(string text, Vector2 position, Vector2 justify)
        {
            DrawOutlinedJustified(text, position, justify, Vector2.One, Color.Black, Color.White);
        }

        #endregion
    }
}
