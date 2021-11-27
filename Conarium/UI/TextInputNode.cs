using System.Net.Mime;
using System;
using System.Collections.Generic;
using Conarium.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Conarium.Extension;

namespace Conarium.UI
{
    public interface IKeyboardListener
    {
        KeyboardState CurrentKeyboardState {get;}
        KeyboardState PrevKeyboardState {get;}
        bool JustPressed(Keys key);
    }

    public interface ITextPlug
    {
        string Text {get;}
    }

    public class TextInputNode : TextNode
    {
        public virtual string DefaultText {get;set;}
        public virtual Color DefaultTextColor {get;set;}

        // still used for base-class rendering, so make sure to point to inputbuf
        public override string Text {
            get
            {
                if (Focused)
                    return OutputBuffer;
                else if (InputBuffer != "")
                    return InputBuffer;
                else
                    return DefaultText;
                
            }
        } 

        public delegate void InputMessageEvent(string message); 
        public event InputMessageEvent OnInputMessageSent;

        public void OnTextInput(object sender, TextInputEventArgs args)
        {
            
            if (!Focused)
                return;

            if (BlacklistedCharacters.Contains(args.Character))
                return;

            if (args.Key == Keys.Tab)
                return;

            if (args.Key == Keys.Enter)
            {
                OnInputMessageSent?.Invoke(InputBuffer);
                if (ClearOnReturn)
                {
                    ScissorEnabled = false;
                    InputBuffer = "";
                    CursorPosition = 0;
                }
                return;
            }

            if (args.Key == Keys.Back)
            {
                if (ScissorEnabled)
                {
                    InputBuffer = InputBuffer.Remove(ScissorLower, ScissorUpper - ScissorLower);
                    CursorPosition = ScissorLower;
                    ScissorEnabled = false;
                }
                else
                {
                    if (CursorPosition > 0)
                    {
                        InputBuffer = InputBuffer.Remove(CursorPosition-1, 1);
                        CursorPosition--;
                    }
                }
                return;
            }
            InputBuffer = InputBuffer.Insert(CursorPosition, args.Character.ToString());
            CursorPosition++;
        }

        public bool ClearOnReturn {get;set;}

        public override bool TextWrap => false;

        public float CursorBlinkFrequency;
        public float CursorBlinkTimer;

        public string InputBuffer;

        public int CursorPosition;

        public Color TextScissorColor {get;set;}
        public Color BackgroundScissorColor {get;set;}

        public bool Focused;

        public int ScissorLower    {get; set;}
        public int ScissorUpper    {get; set;}
        public bool ScissorEnabled {get; set;}

        public string OutputBuffer => GetDisplayText();

        private string GetDisplayText() {
            if (Math.Floor(CursorBlinkTimer *4)% 2 == 0)
                return InputBuffer.Insert(CursorPosition, "|");
            return InputBuffer;
        }

        public string GetPreScissorText() => InputBuffer.Substring(0, ScissorLower);
        public string GetScissorText() =>   InputBuffer.Substring(ScissorLower, ScissorUpper - ScissorLower);
        public string GetPostScissorText() => InputBuffer.Substring(ScissorUpper);

        public List<char> BlacklistedCharacters;

        public bool ClearInputBufferOnReturn {get;set;}

        public KeyboardState CurrentKeyboardState {get;set;}

        public KeyboardState PrevKeyboardState {get;set;}

        public TextInputNode(string name) : base(name) {
            BlacklistedCharacters = new List<char>();
            Focused = true;
            Font = GraphicsService.Get().Fonts.Arial10;
            InputBuffer = "";
            InputService.Get().OnTextInput += OnTextInput;
            DefaultText = "Write a command...";
        }

        public void BlacklistCharacter(char disallow) {
            BlacklistedCharacters.Add(disallow);
        }

        public bool JustPressed(Keys key) => CurrentKeyboardState.IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key);

        public override void Update(GameTime gt)
        {
            var Input = InputService.Get();

            CursorBlinkTimer += gt.GetDelta();

            // arrow keys to navigate text
            if (Focused)
            {
                // ctrl+right => start scissor selection from current point, moving right

                if (Input.GetKey(Keys.LeftShift) && Input.KeyPressed(Keys.Right))
                {

                    if (!ScissorEnabled)
                        ScissorLower = CursorPosition;

                    ScissorEnabled = true;
                    CursorPosition = Math.Clamp(0, CursorPosition + 1, InputBuffer.Length);
                    ScissorUpper = CursorPosition;
                }
                // ditto, but for leftward
                if (Input.GetKey(Keys.LeftShift) && Input.KeyPressed(Keys.Left))
                {
                    if (!ScissorEnabled)
                        ScissorUpper = CursorPosition;

                    ScissorEnabled = true;
                    CursorPosition = Math.Clamp(0, CursorPosition - 1, InputBuffer.Length);
                    ScissorLower = CursorPosition;
                }

                if (Input.GetKey(Keys.LeftShift) && Input.GetKey(Keys.LeftControl))
                {

                    

                    // 
                    if (ScissorLower == ScissorUpper)
                        ScissorEnabled = false;
                        
                    
                        
                    if (Input.KeyPressed(Keys.A))
                    {
                        ScissorEnabled = true;
                        ScissorLower = 0;
                        ScissorUpper = InputBuffer.Length;
                    }

                    if (Input.KeyPressed(Keys.C))
                    {
                        if (ScissorEnabled)
                            Input.SetClipboard(GetScissorText());
                        else
                            Input.SetClipboard(InputBuffer);
                    }

                    if (Input.KeyPressed(Keys.V))
                    {
                        if (Input.Clipboard != null)
                        {
                            InputBuffer = InputBuffer.Insert(CursorPosition, Input.Clipboard);
                            CursorPosition += Input.Clipboard.Length;
                        }
                            
                        else
                            Input.SetClipboard(InputBuffer);
                    }

                    if (Input.KeyPressed(Keys.X))
                    {
                        if (ScissorEnabled)
                        {
                            InputBuffer = InputBuffer.Remove(ScissorLower, ScissorUpper - ScissorLower);
                            Input.SetClipboard(GetScissorText());
                        }
                        else
                        {
                            Input.SetClipboard(InputBuffer);
                            InputBuffer = "";
                            CursorPosition = 0;
                        }  
                    }  
                    return;
                }
                if (Input.KeyPressed(Keys.Right))
                    CursorPosition = Math.Clamp(0, CursorPosition+1, InputBuffer.Length);
                if (Input.KeyPressed(Keys.Left))
                    CursorPosition = Math.Clamp(0, CursorPosition-1, InputBuffer.Length);
            }
            
            CursorPosition = Math.Max(CursorPosition, 0);
			CursorPosition = Math.Min(CursorPosition, InputBuffer.Length);
            base.Update(gt);
        }
        public override void Draw()
        {
            
            
            var gfx = GraphicsService.Get();

            var textBounds = Font.MeasureString(OutputBuffer);
            var start = Font.MeasureString(GetPreScissorText());
            var end = Font.MeasureString(GetScissorText());

            if (ScissorEnabled)
            {
                gfx.Rect(Color.Blue, TextDrawPosition + new Vector2(start.X, 0), end);

				// first section
				gfx.Text(Font, GetPreScissorText(), TextDrawPosition, TextColor);
				gfx.Text(Font, GetScissorText(), TextDrawPosition + new Vector2(start.X, 0),  Color.Black);
				gfx.Text(Font, GetPostScissorText() , TextDrawPosition + new Vector2(start.X + end.X, 0), TextColor);
            } 
            else
            {
                base.Draw();
            }
        }
    }
}