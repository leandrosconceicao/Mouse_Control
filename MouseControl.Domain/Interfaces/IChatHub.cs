using MouseControl.Domain.Entities;

namespace MouseControl.Domain.Interfaces
{
    public interface IChatHub
    {
        Task SetPosition(MousePosition position);

        Task LeftClick();

        Task RightClick();
    }
}
