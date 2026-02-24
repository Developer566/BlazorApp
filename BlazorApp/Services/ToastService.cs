namespace BlazorApp.Services
{
    public class ToastService
    {
        public event Func<string, string, Task>? OnShow;

        public string? PendingMessage { get; set; }
        public string PendingType { get; set; } = "success";

        public async Task ShowSuccess(string message)
        {
            PendingMessage = null;
            if (OnShow != null)
                await OnShow.Invoke(message, "success");
        }

        public void QueueSuccess(string message)
        {
            PendingMessage = message;
            PendingType = "success";
        }
        
    }
}