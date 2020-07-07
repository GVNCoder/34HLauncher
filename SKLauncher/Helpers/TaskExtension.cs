using System.Threading.Tasks;

namespace Launcher.Helpers
{
    public static class TaskExtension
    {
        public static async void FireAndForget(this Task task) => await task;
    }
}