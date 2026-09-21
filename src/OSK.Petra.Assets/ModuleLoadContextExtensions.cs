using OSK.Petra.Assets.Events;
using OSK.Petra.Assets.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Assets;

public static class ModuleLoadContextExtensions
{
    extension(IModuleLoadContext context)
    {
        public async Task WaitForLoadCompletedAsync(CancellationToken cancellationToken = default)
        {
            if (context.LoadProgress.State is not ProgressState.InProgress || context.LoadProgress.State is not ProgressState.NotStarted)
            {
                return;
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnLoadEvent(ModuleLoadEvent loadEvent)
            {
                if (loadEvent is ModuleLoadCompleteEvent)
                {
                    tcs.TrySetResult(true);
                }
            }

            context.LoadEvent += OnLoadEvent;

            using var _ = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

            try
            {
                await tcs.Task;
            }
            finally
            {
                // Always unsubscribe to prevent memory leaks, even if canceled or faulted
                context.LoadEvent -= OnLoadEvent;
            }
        }
    }
}
