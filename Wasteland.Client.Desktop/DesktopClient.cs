
using Conarium.Services;

namespace Wasteland.Client.Desktop
{

    public class DesktopClient : WastelandClient
    {
        protected override void Initialize()
        {
            base.Initialize();
            Window.TextInput += InputService.OnTextInputEventRelay;
        }
    }
}
