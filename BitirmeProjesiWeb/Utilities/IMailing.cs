using System.Threading.Tasks;

namespace BitirmeProjesiWeb.Utilities
{
    interface IMailing
    {
        Task<bool> Send();
    }
}