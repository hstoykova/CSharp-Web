namespace CinemaApp.Web.ViewModels.Cinema
{
    using Movie;

    public class CinemaDetailsViewModel
    {
        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public IEnumerable<MovieIndexViewModel> Movies { get; set; }
            = new HashSet<MovieIndexViewModel>();
    }
}