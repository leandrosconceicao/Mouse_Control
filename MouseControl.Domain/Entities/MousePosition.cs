namespace MouseControl.Domain.Entities
{
    public sealed class MousePosition()
    {
        public double X { get; set; }
        public double Y { get; set; }

        public override string? ToString()
        {
            return $"X => {X}, Y => {Y}";
        }
    }
}
