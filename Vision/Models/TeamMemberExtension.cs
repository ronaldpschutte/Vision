using System.Drawing;

namespace Vision.Models
{
    public partial class TeamMember
    {

        public string GetInitials()
        {
            return ((String.IsNullOrEmpty(FirstName) || String.IsNullOrEmpty(LastName)) ? UserName.First().ToString() : $"{FirstName.First()}{LastName.First()}").ToUpper();
        }
        public string GetFullName()
        {
            return ((String.IsNullOrEmpty(FirstName) || String.IsNullOrEmpty(LastName)) ? UserName : $"{FirstName} {LastName}");
        }

        public string GetPersonalColor()
        {
            uint noA = (uint)UserName.GetHashCode() / 255;
            uint opaque = noA + 0xFF000000;
            var color = Color.FromArgb((int)opaque);
            return $"rgb({color.R}, {color.G}, {color.B})";
        }
    }
}
