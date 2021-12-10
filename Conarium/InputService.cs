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
            Clipboard = contents;
        }


        KeyboardState CurrentKBState {get;set;}
        KeyboardState PreviousKBState {get;set;}

        MouseState CurrentMouseState {get;set;}
        MouseState PreviousMouseState {get;set;}

        GamePadState[] CurrentControllerStates {get;set;}
        GamePadState[] PreviousControllerStates {get;set;}
        public InputService(Game game) : base(game)
        {
            Instance = this;
        }

        public override void Initialize()
        {

            CurrentMouseState 		= Mouse.GetState();
            CurrentKBState 			= Keyboard.GetState();
            CurrentControllerStates = new GamePadState[4];
			for (int i = 0; i < 4; i++)
				CurrentControllerStates[i] = GamePad.GetState(i);

            PreviousMouseState 	= Mouse.GetState();
            PreviousKBState 	= Keyboard.GetState();
			PreviousControllerStates = new GamePadState[4];
			for (int i = 0; i < 4; i++)
            	PreviousControllerStates[i] = GamePad.GetState(i);


            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            PreviousKBState = CurrentKBState;
            CurrentKBState 	= Keyboard.GetState();

            PreviousMouseState 	= CurrentMouseState;
            CurrentMouseState 	= Mouse.GetState();

			for (int i = 0; i < 4; i++) {
				PreviousControllerStates[i] = CurrentControllerStates[i];
				CurrentControllerStates[i] 	= GamePad.GetState(i);
			}
			
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


        public bool ButtonPressed(Buttons btn, int plr = 0) 
			=> CurrentControllerStates[plr].IsButtonDown(btn) 
			&& !PreviousControllerStates[plr].IsButtonDown(btn);
        public bool ButtonReleased(Buttons btn, int plr = 0) 
			=> !CurrentControllerStates[plr].IsButtonDown(btn) 
			&& PreviousControllerStates[plr].IsButtonDown(btn);
        public bool GetButton(Buttons btn, int plr = 0) => CurrentControllerStates[plr].IsButtonDown(btn);
    }
}