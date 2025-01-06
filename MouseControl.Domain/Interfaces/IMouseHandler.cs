using System.Runtime.InteropServices;

namespace MouseControl.Domain.Interfaces
{
    public interface IMouseHandler
    {
        public bool SetCursor(int x, int y);

        public void MouseLeftClick();

        public void MouseRightClick();
    }
}
