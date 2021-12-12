
using Conarium;
using Wasteland.Client;

namespace Wasteland.Client.Desktop
{

    public class DesktopClient : WastelandGameClient
    {
        protected override void Initialize()
        {
            base.Initialize();
            Window.TextInput += InputService.OnTextInputEventRelay;
        }
    }
}
