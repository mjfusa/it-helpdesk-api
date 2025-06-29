using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Threading.Tasks;

namespace ITHelpdeskAPI.Services
{
    public class UserService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public UserService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<User> GetUserDetailsAsync(string userId)
        {
            return await _graphServiceClient.Users[userId]
                .Request()
                .GetAsync();
        }
    }
}