using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApp.Web.ViewModels.Movie
{
    public class MovieDetailsViewModel
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public IEnumerable<MovieIndexViewModel> Movies { get; set; }
        = new HashSet<MovieIndexViewModel>();
    }
}
