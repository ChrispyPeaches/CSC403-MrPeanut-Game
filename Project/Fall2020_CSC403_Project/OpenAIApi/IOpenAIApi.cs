using Fall2020_CSC403_Project.OpenAIApi;
using Refit;
using System.Threading.Tasks;

namespace Fall2020_CSC403_Project
{
    [Headers("Content-Type: application/json", "Authorization: Bearer")]
    public interface IOpenAIApi
    {
        [Post("/v1/chat/completions")]
        Task<ChatCompletionResponse> GetChatCompletion(ChatCompletionQuery query);
    }
}
