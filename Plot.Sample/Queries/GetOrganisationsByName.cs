using Plot.Queries;

namespace Plot.Sample.Queries
{
    public class GetOrganisationsByName : AbstractQuery<Organisation>
    {
        public GetOrganisationsByName()
        {
            OrderBy = new[] {"organisation.Labels"};
        }

        public string Name { get; set; }
    }
}
