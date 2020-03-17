namespace Intercars.Controller
{
    public class IntercarsProfile
    {
        public string Number { get; private set; } = "";

        public string TovarCode { get; set; } = "";

        public string Description { get; set; } = "";

        public string Image { get; set; } = "";

        public string Mark { get; set; } = "";

        public string Model { get; set; } = "";

        public string Zaminniki { get; set; } = "";

        public string OriginalNumbers { get; set; } = "";

        public string AdditionalInformation { get; set; } = "";

        public string PriceType { get; set; } = "";

        public string PriceRozdrib { get; set; } = "";

        public string PriceOpt { get; set; } = "";

        public string OnlineAvailability { get; set; } = "";

        public string AvailabilityInBranchGroup { get; set; } = "";

        public string AvailabilityInViddelenni { get; set; } = "";

        public IntercarsProfile(string number)
        {
            Number = number;
        }

        public string NomerOe { get; set; } = "";

        public string Gru { get; set; } = "";
    }
}