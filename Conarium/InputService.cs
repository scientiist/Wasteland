using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Conarium.Services
{
    public class InputService : Service
    {
        
        public static InputService Get() => Instance;

        public static InputService Instance {get; private set;}



        public delegate void TextInputEvent(object sender, TextInputEventArgs args);        
        public event TextInputEvent OnTextInput;

        // relays text input events from window
        public void OnTextInputEventRelay(object sender, TextInputEventArgs args) 
            => OnTextInput?.Invoke(sender, args);
        
        public string Clipboard {get; private set;}


        public void SetClipboard(string contents)
        {
            
        }


        KeyboardState CurrentKBState {get;set;}
        KeyboardState PreviousKBState {get;set;}

        MouseState CurrentMouseState {get;set;}
        MouseState PreviousMouseState {get;set;}

        GamePadState CurrentControllerState {get;set;}
        GamePadState PrreviousControllerState {get;set;}
        public InputService(Game game) : base(game)
        {
            Instance = this;
        }

        public override void Initialize()
        {

            Console.WriteLine(GamePad.GetCapabilities(1));
            CurrentMouseState = Mouse.GetState();
            CurrentKBState = Keyboard.GetState();
            CurrentControllerState = GamePad.GetState(1); // TODO: allow handling multiple controllers?

            PreviousMouseState = Mouse.GetState();
            PreviousKBState = Keyboard.GetState();
            PrreviousControllerState = GamePad.GetState(1);


            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            PreviousKBState = CurrentKBState;
            CurrentKBState = Keyboard.GetState();

            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            base.Update(gameTime);
        }

        public bool KeyPressed(Keys key) => CurrentKBState.IsKeyDown(key) && !PreviousKBState.IsKeyDown(key);

        public bool KeyReleased(Keys key) => !CurrentKBState.IsKeyDown(key) && PreviousKBState.IsKeyDown(key);

        public bool GetKey(Keys key) => CurrentKBState.IsKeyDown(key);

        bool GetLMB(MouseState s) => s.LeftButton == ButtonState.Pressed;
        bool GetRMB(MouseState s) => s.RightButton == ButtonState.Pressed;
        bool GetMMB(MouseState s) => s.MiddleButton == ButtonState.Pressed;

        int GetMouseWheel(MouseState s) => s.ScrollWheelValue;

        public bool GetLMB() => GetLMB(CurrentMouseState);
        public bool GetRMB() => GetRMB(CurrentMouseState);
        public bool GetMMB() => GetMMB(CurrentMouseState);

        public bool LMBPressed() => GetLMB(CurrentMouseState) && !GetLMB(PreviousMouseState);
        public bool RMBPressed() => GetRMB(CurrentMouseState) && !GetRMB(PreviousMouseState);
        public bool MMBPressed() => GetMMB(CurrentMouseState) && !GetMMB(PreviousMouseState);

        public bool LMBReleased() => !GetLMB(CurrentMouseState) && GetLMB(PreviousMouseState);
        public bool RMBReleased() => !GetRMB(CurrentMouseState) && GetRMB(PreviousMouseState);
        public bool MMBReleased() => !GetMMB(CurrentMouseState) && GetMMB(PreviousMouseState);


        public bool ButtonPressed(Buttons btn) => CurrentControllerState.IsButtonDown(btn) && !PrreviousControllerState.IsButtonDown(btn);
        public bool ButtonReleased(Buttons btn) => !CurrentControllerState.IsButtonDown(btn) && PrreviousControllerState.IsButtonDown(btn);
        public bool GetButton(Buttons btn) => CurrentControllerState.IsButtonDown(btn);
    }
}